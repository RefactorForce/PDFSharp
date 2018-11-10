#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-20165 empira Software GmbH, Cologne Area (Germany)
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
using System.Collections.Generic;
using System.Diagnostics;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a PDF name value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PDFName : PDFItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFName"/> class.
        /// </summary>
        public PDFName() => Value = "/";  // Empty name.

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFName"/> class.
        /// Parameter value always must start with a '/'.
        /// </summary>
        public PDFName(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0 || value[0] != '/')
                throw new ArgumentException(PSSR.NameMustStartWithSlash);

            Value = value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to this name.
        /// </summary>
        public override bool Equals(object obj) => Value.Equals(obj);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Gets the name as a string.
        /// </summary>
        public string Value {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get; }

        /// <summary>
        /// Returns the name. The string always begins with a slash.
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Determines whether the specified name and string are equal.
        /// </summary>
        public static bool operator ==(PDFName name, string str) => name is null ? str == null : name.Value == str;

        /// <summary>
        /// Determines whether the specified name and string are not equal.
        /// </summary>
        public static bool operator !=(PDFName name, string str) => name is null ? str != null : name.Value != str;

        /// <summary>
        /// Represents the empty name.
        /// </summary>
        public static readonly PDFName Empty = new PDFName("/");

        /// <summary>
        /// Writes the name including the leading slash.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) =>
            // TODO: what if unicode character are part of the name? 
            writer.Write(this);

        /// <summary>
        /// Gets the comparer for this type.
        /// </summary>
        public static PDFXNameComparer Comparer => new PDFXNameComparer();

        /// <summary>
        /// Implements a comparer that compares PDFName objects.
        /// </summary>
        public class PDFXNameComparer : IComparer<PDFName>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="l">The first object to compare.</param>
            /// <param name="r">The second object to compare.</param>
            public int Compare(PDFName l, PDFName r) =>
#if true_
#else
                l != null ? r != null ? String.Compare(l.Value, r.Value, StringComparison.Ordinal) : -1 : r != null ? 1 : 0;
#endif

        }
    }
}
