﻿#region PDFSharp - A .NET library for processing PDF
//
// Authors:
//   Thomas Hövel
//
// Copyright (c) 2005-2017 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.IO;
using PDFSharp.Interop;

namespace PDFSharp.Drawing
{
    /// <summary>
    /// This interface will be implemented by specialized classes, one for JPEG, one for BMP, one for PNG, one for GIF. Maybe more.
    /// </summary>
    internal interface IImageImporter
    {
        /// <summary>
        /// Imports the image. Returns null if the image importer does not support the format.
        /// </summary>
        ImportedImage ImportImage(StreamReaderHelper stream, PDFDocument document);

        /// <summary>
        /// Prepares the image data needed for the PDF file.
        /// </summary>
        ImageData PrepareImage(ImagePrivateData data);
    }

    // $THHO Add IDispose?.
    /// <summary>
    /// Helper for dealing with Stream data.
    /// </summary>
    internal class StreamReaderHelper
    {
        internal StreamReaderHelper(Stream stream)
        {
#if GDI || WPF
            _stream = stream;
            MemoryStream ms = stream as MemoryStream;
            if (ms == null)
            {
                // THHO4STLA byte[] or MemoryStream?
                _ownedMemoryStream = ms = new MemoryStream();
                CopyStream(stream, ms);
                // For .NET 4: stream.CopyTo(ms);
            }
            _data = ms.GetBuffer();
            _length = (int)ms.Length;
#else
            // For WinRT there is no GetBuffer() => alternative implementation for WinRT.
            // TODO: Are there advantages of GetBuffer()? It should reduce LOH fragmentation.
            OriginalStream = stream;
            OriginalStream.Position = 0;
            if (OriginalStream.Length > Int32.MaxValue)
                throw new ArgumentException("Stream is too large.", "stream");
            Length = (int)OriginalStream.Length;
            Data = new byte[Length];
            OriginalStream.Read(Data, 0, Length);
#endif
        }

        internal byte GetByte(int offset)
        {
            if (CurrentOffset + offset >= Length)
            {
                Debug.Assert(false);
                return 0;
            }
            return Data[CurrentOffset + offset];
        }

        internal ushort GetWord(int offset, bool bigEndian) => (ushort)(bigEndian ?
                GetByte(offset) * 256 + GetByte(offset + 1) :
                GetByte(offset) + GetByte(offset + 1) * 256);

        internal uint GetDWord(int offset, bool bigEndian) => (uint)(bigEndian ?
                GetWord(offset, true) * 65536 + GetWord(offset + 2, true) :
                GetWord(offset, false) + GetWord(offset + 2, false) * 65536);

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[65536];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() => CurrentOffset = 0;

        /// <summary>
        /// Gets the original stream.
        /// </summary>
        public Stream OriginalStream { get; }

        internal int CurrentOffset { get; set; }

        /// <summary>
        /// Gets the data as byte[].
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Gets the length of Data.
        /// </summary>
        public int Length { get; }

#if GDI || WPF
        /// <summary>
        /// Gets the owned memory stream. Can be null if no MemoryStream was created.
        /// </summary>
        public MemoryStream OwnedMemoryStream
        {
            get { return _ownedMemoryStream; }
        }
        private readonly MemoryStream _ownedMemoryStream;
#endif
    }

    /// <summary>
    /// The imported image.
    /// </summary>
    internal abstract class ImportedImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedImage"/> class.
        /// </summary>
        protected ImportedImage(IImageImporter importer, ImagePrivateData data, PDFDocument document)
        {
            Data = data;
            _document = document;
            data.Image = this;
            _importer = importer;
        }


        /// <summary>
        /// Gets information about the image.
        /// </summary>
        public ImageInformation Information { get; private set; } = new ImageInformation();

        /// <summary>
        /// Gets a value indicating whether image data for the PDF file was already prepared.
        /// </summary>
        public bool HasImageData => _imageData != null;

        /// <summary>
        /// Gets the image data needed for the PDF file.
        /// </summary>
        public ImageData ImageData
        {
            get { if (!HasImageData) _imageData = PrepareImageData(); return _imageData; }
            private set => _imageData = value;
        }
        private ImageData _imageData;

        internal virtual ImageData PrepareImageData() => throw new NotImplementedException();

        private readonly IImageImporter _importer;
        internal ImagePrivateData Data;
        internal readonly PDFDocument _document;
    }

    /// <summary>
    /// Public information about the image, filled immediately.
    /// Note: The stream will be read and decoded on the first call to PrepareImageData().
    /// ImageInformation can be filled for corrupted images that will throw an expection on PrepareImageData().
    /// </summary>
    internal class ImageInformation
    {
        internal enum ImageFormats
        {
            /// <summary>
            /// Standard JPEG format (RGB).
            /// </summary>
            JPEG,
            /// <summary>
            /// Grayscale JPEG format.
            /// </summary>
            JPEGGRAY,
            /// <summary>
            /// JPEG file with inverted CMYK, thus RGBW.
            /// </summary>
            JPEGRGBW,
            /// <summary>
            /// JPEG file with CMYK.
            /// </summary>
            JPEGCMYK,
            Palette1,
            Palette4,
            Palette8,
            RGB24,
            ARGB32
        }

        internal ImageFormats ImageFormat;

        internal uint Width;
        internal uint Height;

        /// <summary>
        /// The horizontal DPI (dots per inch). Can be 0 if not supported by the image format.
        /// Note: JFIF (JPEG) files may contain either DPI or DPM or just the aspect ratio. Windows BMP files will contain DPM. Other formats may support any combination, including none at all.
        /// </summary>
        internal decimal HorizontalDPI;
        /// <summary>
        /// The vertical DPI (dots per inch). Can be 0 if not supported by the image format.
        /// </summary>
        internal decimal VerticalDPI;

        /// <summary>
        /// The horizontal DPM (dots per meter). Can be 0 if not supported by the image format.
        /// </summary>
        internal decimal HorizontalDPM;
        /// <summary>
        /// The vertical DPM (dots per meter). Can be 0 if not supported by the image format.
        /// </summary>
        internal decimal VerticalDPM;

        /// <summary>
        /// The horizontal component of the aspect ratio. Can be 0 if not supported by the image format.
        /// Note: Aspect ratio will be set if either DPI or DPM was set, but may also be available in the absence of both DPI and DPM.
        /// </summary>
        internal decimal HorizontalAspectRatio;
        /// <summary>
        /// The vertical component of the aspect ratio. Can be 0 if not supported by the image format.
        /// </summary>
        internal decimal VerticalAspectRatio;

        /// <summary>
        /// The colors used. Only valid for images with palettes, will be 0 otherwise.
        /// </summary>
        internal uint ColorsUsed;
    }

    /// <summary>
    /// Contains internal data. This includes a reference to the Stream if data for PDF was not yet prepared.
    /// </summary>
    internal abstract class ImagePrivateData
    {
        internal ImagePrivateData()
        {
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public ImportedImage Image { get; internal set; }
    }

    /// <summary>
    /// Contains data needed for PDF. Will be prepared when needed.
    /// </summary>
    internal abstract class ImageData
    {
    }
}
