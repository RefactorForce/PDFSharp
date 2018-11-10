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

using System;
using System.Diagnostics;
using PDFSharp.Drawing.PDF;
using PDFSharp.Interop.Filters;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents the content of a page. PDFSharp supports only one content stream per page.
    /// If an imported page has an array of content streams, the streams are concatenated to
    /// one single stream.
    /// </summary>
    public sealed class PDFContent : PDFDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFContent"/> class.
        /// </summary>
        public PDFContent(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFContent"/> class.
        /// </summary>
        internal PDFContent(PDFPage page)
            : base(page?.Owner)
        {
            //_pageContent = new PageContent(page);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFContent"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public PDFContent(PDFDictionary dict) // HACK PDFContent
            : base(dict) =>
            // A PDFContent dictionary is always unfiltered.
            Decode();

        /// <summary>
        /// Sets a value indicating whether the content is compressed with the ZIP algorithm.
        /// </summary>
        public bool Compressed
        {
            set
            {
                if (value)
                {
                    PDFItem filter = Elements["/Filter"];
                    if (filter == null)
                    {
                        byte[] bytes = Filtering.FlateDecode.Encode(Stream.Value, _document.Options.FlateEncodeMode);
                        Stream.Value = bytes;
                        Elements.SetInteger("/Length", Stream.Length);
                        Elements.SetName("/Filter", "/FlateDecode");
                    }
                }
            }
        }

        /// <summary>
        /// Unfilters the stream.
        /// </summary>
        void Decode()
        {
            if (Stream != null && Stream.Value != null)
            {
                PDFItem item = Elements["/Filter"];
                if (item != null)
                {
                    byte[] bytes = Filtering.Decode(Stream.Value, item);
                    if (bytes != null)
                    {
                        Stream.Value = bytes;
                        Elements.Remove("/Filter");
                        Elements.SetInteger("/Length", Stream.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Surround content with q/Q operations if necessary.
        /// </summary>
        internal void PreserveGraphicsState()
        {
            // If a content stream is touched by PDFSharp it is typically because graphical operations are
            // prepended or appended. Some nasty PDF tools does not preserve the graphical state correctly.
            // Therefore we try to relieve the problem by surrounding the content stream with push/restore 
            // graphic state operation.
            if (Stream != null)
            {
                byte[] value = Stream.Value;
                int length = value.Length;
                if (length != 0 && (value[0] != (byte)'q' || value[1] != (byte)'\n'))
                {
                    byte[] newValue = new byte[length + 2 + 3];
                    newValue[0] = (byte)'q';
                    newValue[1] = (byte)'\n';
                    Array.Copy(value, 0, newValue, 2, length);
                    newValue[length + 2] = (byte)' ';
                    newValue[length + 3] = (byte)'Q';
                    newValue[length + 4] = (byte)'\n';
                    Stream.Value = newValue;
                    Elements.SetInteger("/Length", Stream.Length);
                }
            }
        }

        internal override void WriteObject(PDFWriter writer)
        {
            if (_pdfRenderer != null)
            {
                // GetContent also disposes the underlying XGraphics object, if one exists
                //Stream = new PDFStream(PDFEncoders.RawEncoding.GetBytes(pdfRenderer.GetContent()), this);
                _pdfRenderer.Close();
                Debug.Assert(_pdfRenderer == null);
            }

            if (Stream != null)
            {
                //if (Owner.Options.CompressContentStreams)
                if (Owner.Options.CompressContentStreams && Elements.GetName("/Filter").Length == 0)
                {
                    Stream.Value = Filtering.FlateDecode.Encode(Stream.Value, _document.Options.FlateEncodeMode);
                    //Elements["/Filter"] = new PDFName("/FlateDecode");
                    Elements.SetName("/Filter", "/FlateDecode");
                }
                Elements.SetInteger("/Length", Stream.Length);
            }

            base.WriteObject(writer);
        }

        internal XGraphicsPDFRenderer _pdfRenderer;

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : PDFStream.Keys
        {
            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta => _meta ?? (_meta = CreateMeta(typeof(Keys)));
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta => Keys.Meta;
    }
}
