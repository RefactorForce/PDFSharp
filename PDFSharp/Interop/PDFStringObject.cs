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
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents an indirect text string value. This type is not used by PDFSharp. If it is imported from
    /// an external PDF file, the value is converted into a direct object.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFStringObject : PDFObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFStringObject"/> class.
        /// </summary>
        public PDFStringObject() => _flags = PDFStringFlags.RawEncoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFStringObject"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="value">The value.</param>
        public PDFStringObject(PDFDocument document, string value)
            : base(document)
        {
            _value = value;
            _flags = PDFStringFlags.RawEncoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFStringObject"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encoding">The encoding.</param>
        public PDFStringObject(string value, PDFStringEncoding encoding)
        {
            _value = value;
            //if ((flags & PDFStringFlags.EncodingMask) == 0)
            //  flags |= PDFStringFlags.PDFDocEncoding;
            _flags = (PDFStringFlags)encoding;
        }

        internal PDFStringObject(string value, PDFStringFlags flags)
        {
            _value = value;
            //if ((flags & PDFStringFlags.EncodingMask) == 0)
            //  flags |= PDFStringFlags.PDFDocEncoding;
            _flags = flags;
        }

        /// <summary>
        /// Gets the number of characters in this string.
        /// </summary>
        public int Length => _value == null ? 0 : _value.Length;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public PDFStringEncoding Encoding
        {
            get => (PDFStringEncoding)(_flags & PDFStringFlags.EncodingMask);
            set => _flags = (_flags & ~PDFStringFlags.EncodingMask) | ((PDFStringFlags)value & PDFStringFlags.EncodingMask);
        }

        /// <summary>
        /// Gets a value indicating whether the string is a hexadecimal literal.
        /// </summary>
        public bool HexLiteral
        {
            get => (_flags & PDFStringFlags.HexLiteral) != 0;
            set => _flags = value ? _flags | PDFStringFlags.HexLiteral : _flags & ~PDFStringFlags.HexLiteral;
        }
        PDFStringFlags _flags;

        /// <summary>
        /// Gets or sets the value as string
        /// </summary>
        public string Value
        {
            get => _value ?? "";
            set => _value = value ?? "";
        }
        string _value;

        /// <summary>
        /// Gets or sets the string value for encryption purposes.
        /// </summary>
        internal byte[] EncryptionValue
        {
            // TODO: Unicode case is not handled!
            get => _value == null ? new byte[0] : PDFEncoders.RawEncoding.GetBytes(_value);
            set => _value = PDFEncoders.RawEncoding.GetString(value, 0, value.Length);
        }

        /// <summary>
        /// Returns the string.
        /// </summary>
        public override string ToString() => _value;

        /// <summary>
        /// Writes the string literal with encoding DOCEncoded.
        /// </summary>
        internal override void WriteObject(PDFWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PDFString(_value, _flags));
            writer.WriteEndObject();
        }
    }
}
