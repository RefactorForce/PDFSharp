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

namespace PDFSharp.Drawing
{
    /// <summary>
    /// Specifies the format of the image.
    /// </summary>
    public sealed class XImageFormat
    {
        XImageFormat(Guid guid) => Guid = guid;

        internal Guid Guid { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj) => !(obj is XImageFormat format) ? false : Guid == format.Guid;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => Guid.GetHashCode();

        /// <summary>
        /// Gets the Portable Network Graphics (PNG) image format. 
        /// </summary>
        public static XImageFormat Png { get; } = new XImageFormat(new Guid("{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}"));

        /// <summary>
        /// Gets the Graphics Interchange Format (GIF) image format.
        /// </summary>
        public static XImageFormat Gif { get; } = new XImageFormat(new Guid("{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}"));

        /// <summary>
        /// Gets the Joint Photographic Experts Group (JPEG) image format.
        /// </summary>
        public static XImageFormat Jpeg { get; } = new XImageFormat(new Guid("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}"));

        /// <summary>
        /// Gets the Tag Image File Format (TIFF) image format.
        /// </summary>
        public static XImageFormat Tiff { get; } = new XImageFormat(new Guid("{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}"));

        /// <summary>
        /// Gets the Portable Document Format (PDF) image format
        /// </summary>
        public static XImageFormat Pdf { get; } = new XImageFormat(new Guid("{84570158-DBF0-4C6B-8368-62D6A3CA76E0}"));

        /// <summary>
        /// Gets the Windows icon image format.
        /// </summary>
        public static XImageFormat Icon { get; } = new XImageFormat(new Guid("{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}"));
    }
}