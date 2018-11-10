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

using System.Diagnostics;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a direct boolean value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFBoolean : PDFItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFBoolean"/> class.
        /// </summary>
        public PDFBoolean()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFBoolean"/> class.
        /// </summary>
        public PDFBoolean(bool value) => Value = value;

        /// <summary>
        /// Gets the value of this instance as boolean value.
        /// </summary>
        public bool Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; }

        /// <summary>
        /// A pre-defined value that represents <c>true</c>.
        /// </summary>
        public static readonly PDFBoolean True = new PDFBoolean(true);

        /// <summary>
        /// A pre-defined value that represents <c>false</c>.
        /// </summary>
        public static readonly PDFBoolean False = new PDFBoolean(false);

        /// <summary>
        /// Returns 'false' or 'true'.
        /// </summary>
        public override string ToString() => Value ? System.Boolean.TrueString : System.Boolean.FalseString;

        /// <summary>
        /// Writes 'true' or 'false'.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) => writer.Write(this);
    }
}
