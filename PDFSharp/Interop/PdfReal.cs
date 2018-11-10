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

using System.Diagnostics;
using System.Globalization;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a direct real value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFReal : PDFNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFReal"/> class.
        /// </summary>
        public PDFReal()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFReal"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PDFReal(double value) => Value = value;

        /// <summary>
        /// Gets the value as double.
        /// </summary>
        public double Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; }

        /// <summary>
        /// Returns the real number as string.
        /// </summary>
        public override string ToString() => Value.ToString(Config.SignificantFigures3, CultureInfo.InvariantCulture);

        /// <summary>
        /// Writes the real value with up to three digits.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) => writer.Write(this);
    }
}