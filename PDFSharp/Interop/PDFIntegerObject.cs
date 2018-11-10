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
using System.Globalization;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents an indirect integer value. This type is not used by PDFSharp. If it is imported from
    /// an external PDF file, the value is converted into a direct object.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFIntegerObject : PDFNumberObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFIntegerObject"/> class.
        /// </summary>
        public PDFIntegerObject()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFIntegerObject"/> class.
        /// </summary>
        public PDFIntegerObject(int value) => Value = value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFIntegerObject"/> class.
        /// </summary>
        public PDFIntegerObject(PDFDocument document, int value)
            : base(document) => Value = value;

        /// <summary>
        /// Gets the value as integer.
        /// </summary>
        public int Value { get;
        //set {_value = value;}
        }

        /// <summary>
        /// Returns the integer as string.
        /// </summary>
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Writes the integer literal.
        /// </summary>
        internal override void WriteObject(PDFWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(Value);
            writer.WriteEndObject();
        }
    }
}
