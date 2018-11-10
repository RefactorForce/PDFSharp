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
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents an indirect name value. This type is not used by PDFsharp. If it is imported from
    /// an external PDF file, the value is converted into a direct object. Acrobat sometime uses indirect
    /// names to save space, because an indirect reference to a name may be shorter than a long name.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFNameObject : PDFObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFNameObject"/> class.
        /// </summary>
        public PDFNameObject() => Value = "/";  // Empty name.

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFNameObject"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="value">The value.</param>
        public PDFNameObject(PDFDocument document, string value)
            : base(document)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0 || value[0] != '/')
                throw new ArgumentException(PSSR.NameMustStartWithSlash);

            Value = value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj) => Value.Equals(obj);

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Gets or sets the name value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns the name. The string always begins with a slash.
        /// </summary>
        public override string ToString() =>
            // TODO: Encode characters.
            Value;

        /// <summary>
        /// Determines whether a name is equal to a string.
        /// </summary>
        public static bool operator ==(PDFNameObject name, string str) => name.Value == str;

        /// <summary>
        /// Determines whether a name is not equal to a string.
        /// </summary>
        public static bool operator !=(PDFNameObject name, string str) => name.Value != str;

#if leads_to_ambiguity
        public static bool operator ==(string str, PDFName name)
        {
            return str == name.value;
        }

        public static bool operator !=(string str, PDFName name)
        {
            return str == name.value;
        }

        public static bool operator ==(PDFName name1, PDFName name2)
        {
            return name1.value == name2.value;
        }

        public static bool operator !=(PDFName name1, PDFName name2)
        {
            return name1.value != name2.value;
        }
#endif

        /// <summary>
        /// Writes the name including the leading slash.
        /// </summary>
        internal override void WriteObject(PDFWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PDFName(Value));
            writer.WriteEndObject();
        }
    }
}
