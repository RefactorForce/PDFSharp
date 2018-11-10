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
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;
using PDFSharp.Drawing;
using PDFSharp.Internal;
using PDFSharp.Interop.IO;
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents text that is written 'as it is' into the PDF stream. This class can lead to invalid PDF files.
    /// E.g. strings in a literal are not encrypted when the document is saved with a password.
    /// </summary>
    public sealed class PDFLiteral : PDFItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFLiteral"/> class.
        /// </summary>
        public PDFLiteral()
        { }

        /// <summary>
        /// Initializes a new instance with the specified string.
        /// </summary>
        public PDFLiteral(string value) => Value = value;

        /// <summary>
        /// Initializes a new instance with the culture invariant formatted specified arguments.
        /// </summary>
        public PDFLiteral(string format, params object[] args) => Value = PDFEncoders.Format(format, args);

        /// <summary>
        /// Creates a literal from an XMatrix
        /// </summary>
        public static PDFLiteral FromMatrix(XMatrix matrix) => new PDFLiteral("[" + PDFEncoders.ToString(matrix) + "]");

        /// <summary>
        /// Gets the value as litaral string.
        /// </summary>
        public string Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; } = String.Empty;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString() => Value;

        internal override void WriteObject(PDFWriter writer) => writer.Write(this);
    }
}
