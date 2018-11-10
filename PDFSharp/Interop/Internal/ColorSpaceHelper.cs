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
using PDFSharp.Drawing;

namespace PDFSharp.Interop.Internal
{
    /// <summary>
    /// Helper functions for RGB and CMYK colors.
    /// </summary>
    static class ColorSpaceHelper
    {
        /// <summary>
        /// Checks whether a color mode and a color match.
        /// </summary>
        public static XColor EnsureColorMode(PDFColorMode colorMode, XColor color) =>
#if true
            colorMode == PDFColorMode.Rgb && color.ColorSpace != XColorSpace.Rgb
                ? XColor.FromArgb((int)(color.A * 255), color.R, color.G, color.B)
                : colorMode == PDFColorMode.Cmyk && color.ColorSpace != XColorSpace.Cmyk
                ? XColor.FromCmyk(color.A, color.C, color.M, color.Y, color.K)
                : color;
#else
      if (colorMode == PDFColorMode.Rgb && color.ColorSpace != XColorSpace.Rgb)
        throw new InvalidOperationException(PSSR.InappropriateColorSpace(colorMode, color.ColorSpace));

      if (colorMode == PDFColorMode.Cmyk && color.ColorSpace != XColorSpace.Cmyk)
        throw new InvalidOperationException(PSSR.InappropriateColorSpace(colorMode, color.ColorSpace));
#endif


        /// <summary>
        /// Checks whether the color mode of a document and a color match.
        /// </summary>
        public static XColor EnsureColorMode(PDFDocument document, XColor color)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            return EnsureColorMode(document.Options.ColorMode, color);
        }

        /// <summary>
        /// Determines whether two colors are equal referring to their CMYK color values.
        /// </summary>
        public static bool IsEqualCmyk(XColor x, XColor y) => x.ColorSpace != XColorSpace.Cmyk || y.ColorSpace != XColorSpace.Cmyk
                ? false
                : x.C == y.C && x.M == y.M && x.Y == y.Y && x.K == y.K;
    }
}