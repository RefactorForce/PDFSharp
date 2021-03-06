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
using System.Globalization;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a PDF object identifier, a pair of object and generation number.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct PDFObjectID : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFObjectID"/> class.
        /// </summary>
        /// <param name="objectNumber">The object number.</param>
        public PDFObjectID(int objectNumber)
        {
            Debug.Assert(objectNumber >= 1, "Object number out of range.");
            ObjectNumber = objectNumber;
            _generationNumber = 0;
#if DEBUG_
            // Just a place for a breakpoint during debugging.
            if (objectNumber == 5894)
                GetType();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFObjectID"/> class.
        /// </summary>
        /// <param name="objectNumber">The object number.</param>
        /// <param name="generationNumber">The generation number.</param>
        public PDFObjectID(int objectNumber, int generationNumber)
        {
            Debug.Assert(objectNumber >= 1, "Object number out of range.");
            //Debug.Assert(generationNumber >= 0 && generationNumber <= 65535, "Generation number out of range.");
#if DEBUG_
            // iText creates generation numbers with a value of 65536... 
            if (generationNumber > 65535)
                Debug.WriteLine(String.Format("Generation number: {0}", generationNumber));
#endif
            ObjectNumber = objectNumber;
            _generationNumber = (ushort)generationNumber;
        }

        /// <summary>
        /// Gets or sets the object number.
        /// </summary>
        public int ObjectNumber { get; }

        /// <summary>
        /// Gets or sets the generation number.
        /// </summary>
        public int GenerationNumber => _generationNumber;
        readonly ushort _generationNumber;

        /// <summary>
        /// Indicates whether this object is an empty object identifier.
        /// </summary>
        public bool IsEmpty => ObjectNumber == 0;

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is PDFObjectID id)
            {
                if (ObjectNumber == id.ObjectNumber)
                    return _generationNumber == id._generationNumber;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => ObjectNumber ^ _generationNumber;

        /// <summary>
        /// Determines whether the two objects are equal.
        /// </summary>
        public static bool operator ==(PDFObjectID left, PDFObjectID right) => left.Equals(right);

        /// <summary>
        /// Determines whether the tow objects not are equal.
        /// </summary>
        public static bool operator !=(PDFObjectID left, PDFObjectID right) => !left.Equals(right);

        /// <summary>
        /// Returns the object and generation numbers as a string.
        /// </summary>
        public override string ToString() => ObjectNumber.ToString(CultureInfo.InvariantCulture) + " " + _generationNumber.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Creates an empty object identifier.
        /// </summary>
        public static PDFObjectID Empty => new PDFObjectID();

        /// <summary>
        /// Compares the current object id with another object.
        /// </summary>
        public int CompareTo(object obj) => obj is PDFObjectID id
                ? ObjectNumber == id.ObjectNumber ? _generationNumber - id._generationNumber : ObjectNumber - id.ObjectNumber
                : 1;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        internal string DebuggerDisplay => String.Format("id=({0})", ToString());
    }
}
