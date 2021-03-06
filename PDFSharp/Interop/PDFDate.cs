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
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a direct date value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFDate : PDFItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDate"/> class.
        /// </summary>
        public PDFDate()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDate"/> class.
        /// </summary>
        public PDFDate(string value) => Value = Parser.ParseDateTime(value, DateTime.MinValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDate"/> class.
        /// </summary>
        public PDFDate(DateTime value) => Value = value;

        /// <summary>
        /// Gets the value as DateTime.
        /// </summary>
        public DateTime Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; }

        /// <summary>
        /// Returns the value in the PDF date format.
        /// </summary>
        public override string ToString()
        {
            string delta = Value.ToString("zzz").Replace(':', '\'');
            return String.Format("D:{0:yyyyMMddHHmmss}{1}'", Value, delta);
        }

        /// <summary>
        /// Writes the value in the PDF date format.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) => writer.WriteDocString(ToString());
    }
}
