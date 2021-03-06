#region PDFSharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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

// ReSharper disable ConvertToAutoProperty

namespace PDFSharp.Interop
{
    /// <summary>
    /// Holds information how to handle the document when it is saved as PDF stream.
    /// </summary>
    public sealed class PDFDocumentOptions
    {
        internal PDFDocumentOptions(PDFDocument document)
        {
            //_deflateContents = true;
            //_writeProcedureSets = true;
        }

        /// <summary>
        /// Gets or sets the color mode.
        /// </summary>
        public PDFColorMode ColorMode { get; set; } = PDFColorMode.Rgb;

        /// <summary>
        /// Gets or sets a value indicating whether to compress content streams of PDF pages.
        /// </summary>
        public bool CompressContentStreams { get; set; } = false;

#if DEBUG
#else
        bool _compressContentStreams = true;
#endif

        /// <summary>
        /// Gets or sets a value indicating that all objects are not compressed.
        /// </summary>
        public bool NoCompression { get; set; }

        /// <summary>
        /// Gets or sets the flate encode mode. Besides the balanced default mode you can set modes for best compression (slower) or best speed (larger files).
        /// </summary>
        public PDFFlateEncodeMode FlateEncodeMode { get; set; } = PDFFlateEncodeMode.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to compress bilevel images using CCITT compression.
        /// With true, PDFSharp will try FlateDecode CCITT and will use the smallest one or a combination of both.
        /// With false, PDFSharp will always use FlateDecode only - files may be a few bytes larger, but file creation is faster.
        /// </summary>
        public bool EnableCcittCompressionForBilevelImages { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to compress JPEG images with the FlateDecode filter.
        /// </summary>
        public PDFUseFlateDecoderForJpegImages UseFlateDecoderForJpegImages { get; set; } = PDFUseFlateDecoderForJpegImages.Never;
    }
}