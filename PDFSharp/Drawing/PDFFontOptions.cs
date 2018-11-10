#region PDFsharp - A .NET library for processing PDF
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
#if GDI
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
#endif
#if WPF
using System.Windows.Media;
#endif
using PDFSharp.Interop;

namespace PDFSharp.Drawing
{
    /// <summary>
    /// Specifies details about how the font is used in PDF creation.
    /// </summary>
    public class XPDFFontOptions
    {
        internal XPDFFontOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPDFFontOptions"/> class.
        /// </summary>
        [Obsolete("Must not specify an embedding option anymore.")]
        public XPDFFontOptions(PDFFontEncoding encoding, PDFFontEmbedding embedding) => FontEncoding = encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="XPDFFontOptions"/> class.
        /// </summary>
        public XPDFFontOptions(PDFFontEncoding encoding) => FontEncoding = encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="XPDFFontOptions"/> class.
        /// </summary>
        [Obsolete("Must not specify an embedding option anymore.")]
        public XPDFFontOptions(PDFFontEmbedding embedding) => FontEncoding = PDFFontEncoding.WinAnsi;

        /// <summary>
        /// Gets a value indicating the font embedding.
        /// </summary>
        public PDFFontEmbedding FontEmbedding => PDFFontEmbedding.Always;

        /// <summary>
        /// Gets a value indicating how the font is encoded.
        /// </summary>
        public PDFFontEncoding FontEncoding { get; }

        /// <summary>
        /// Gets the default options with WinAnsi encoding and always font embedding.
        /// </summary>
        public static XPDFFontOptions WinAnsiDefault => new XPDFFontOptions(PDFFontEncoding.WinAnsi);

        /// <summary>
        /// Gets the default options with Unicode encoding and always font embedding.
        /// </summary>
        public static XPDFFontOptions UnicodeDefault => new XPDFFontOptions(PDFFontEncoding.Unicode);
    }
}
