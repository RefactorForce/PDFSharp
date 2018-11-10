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
using System.Diagnostics;
using System.Globalization;
using PDFSharp.Pdf.IO;

namespace PDFSharp.Pdf
{
    /// <summary>
    /// Represents a direct integer value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfInteger : PdfNumber, IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfInteger"/> class.
        /// </summary>
        public PdfInteger()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfInteger"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfInteger(int value) => Value = value;

        /// <summary>
        /// Gets the value as integer.
        /// </summary>
        public int Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; }

        /// <summary>
        /// Returns the integer as string.
        /// </summary>
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Writes the integer as string.
        /// </summary>
        internal override void WriteObject(PdfWriter writer) => writer.Write(this);

        #region IConvertible Members

        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => throw new InvalidCastException();

        double IConvertible.ToDouble(IFormatProvider provider) => Value;

        DateTime IConvertible.ToDateTime(IFormatProvider provider) =>
            // TODO:  Add PdfInteger.ToDateTime implementation
            new DateTime();

        float IConvertible.ToSingle(IFormatProvider provider) => Value;

        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Value);

        int IConvertible.ToInt32(IFormatProvider provider) => Value;

        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value);

        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(Value);

        string IConvertible.ToString(IFormatProvider provider) => Value.ToString(provider);

        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(Value);

        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(Value);

        long IConvertible.ToInt64(IFormatProvider provider) => Value;

        /// <summary>
        /// Returns TypeCode for 32-bit integers.
        /// </summary>
        public TypeCode GetTypeCode() => TypeCode.Int32;

        decimal IConvertible.ToDecimal(IFormatProvider provider) => Value;

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) =>
            // TODO:  Add PdfInteger.ToType implementation
            null;

        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value);

        #endregion
    }
}