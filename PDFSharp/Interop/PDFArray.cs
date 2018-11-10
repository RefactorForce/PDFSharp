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
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Text;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a PDF array object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class PDFArray : PDFObject, IEnumerable<PDFItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFArray"/> class.
        /// </summary>
        public PDFArray()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFArray"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PDFArray(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFArray"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="items">The items.</param>
        public PDFArray(PDFDocument document, params PDFItem[] items)
            : base(document)
        {
            foreach (PDFItem item in items)
                Elements.Add(item);
        }

        /// <summary>
        /// Initializes a new instance from an existing dictionary. Used for object type transformation.
        /// </summary>
        /// <param name="array">The array.</param>
        protected PDFArray(PDFArray array)
            : base(array)
        {
            if (array._elements != null)
                array._elements.ChangeOwner(this);
        }

        /// <summary>
        /// Creates a copy of this array. Direct elements are deep copied.
        /// Indirect references are not modified.
        /// </summary>
        public new PDFArray Clone() => (PDFArray)Copy();

        /// <summary>
        /// Implements the copy mechanism.
        /// </summary>
        protected override object Copy()
        {
            PDFArray array = (PDFArray)base.Copy();
            if (array._elements != null)
            {
                array._elements = array._elements.Clone();
                int count = array._elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PDFItem item = array._elements[idx];
                    if (item is PDFObject)
                        array._elements[idx] = item.Clone();
                }
            }
            return array;
        }

        /// <summary>
        /// Gets the collection containing the elements of this object.
        /// </summary>
        public ArrayElements Elements => _elements ?? (_elements = new ArrayElements(this));

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        public virtual IEnumerator<PDFItem> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns a string with the content of this object in a readable form. Useful for debugging purposes only.
        /// </summary>
        public override string ToString()
        {
            StringBuilder pdf = new StringBuilder();
            pdf.Append("[ ");
            int count = Elements.Count;
            for (int idx = 0; idx < count; idx++)
                pdf.Append(Elements[idx] + " ");
            pdf.Append("]");
            return pdf.ToString();
        }

        internal override void WriteObject(PDFWriter writer)
        {
            writer.WriteBeginObject(this);
            int count = Elements.Count;
            for (int idx = 0; idx < count; idx++)
            {
                PDFItem value = Elements[idx];
                value.WriteObject(writer);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Represents the elements of an PDFArray.
        /// </summary>
        public sealed class ArrayElements : IList<PDFItem>, ICloneable
        {
            internal ArrayElements(PDFArray array)
            {
                _elements = new List<PDFItem>();
                _ownerArray = array;
            }

            object ICloneable.Clone()
            {
                ArrayElements elements = (ArrayElements)MemberwiseClone();
                elements._elements = new List<PDFItem>(elements._elements);
                elements._ownerArray = null;
                return elements;
            }

            /// <summary>
            /// Creates a shallow copy of this object.
            /// </summary>
            public ArrayElements Clone() => (ArrayElements)((ICloneable)this).Clone();

            /// <summary>
            /// Moves this instance to another array during object type transformation.
            /// </summary>
            internal void ChangeOwner(PDFArray array)
            {
                if (_ownerArray != null)
                {
                    // ???
                }

                // Set new owner.
                _ownerArray = array;

                // Set owners elements to this.
                array._elements = this;
            }

            /// <summary>
            /// Converts the specified value to boolean.
            /// If the value does not exist, the function returns false.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public bool GetBoolean(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return false;

                if (obj is PDFBoolean boolean)
                    return boolean.Value;

                if (obj is PDFBooleanObject booleanObject)
                    return booleanObject.Value;

                throw new InvalidCastException("GetBoolean: Object is not a boolean.");
            }

            /// <summary>
            /// Converts the specified value to integer.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public int GetInteger(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return 0;

                if (obj is PDFInteger integer)
                    return integer.Value;

                if (obj is PDFIntegerObject integerObject)
                    return integerObject.Value;

                throw new InvalidCastException("GetInteger: Object is not an integer.");
            }

            /// <summary>
            /// Converts the specified value to double.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public double GetReal(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return 0;

                if (obj is PDFReal real)
                    return real.Value;

                if (obj is PDFRealObject realObject)
                    return realObject.Value;

                if (obj is PDFInteger integer)
                    return integer.Value;

                if (obj is PDFIntegerObject integerObject)
                    return integerObject.Value;

                throw new InvalidCastException("GetReal: Object is not a number.");
            }

            /// <summary>
            /// Converts the specified value to double?.
            /// If the value does not exist, the function returns null.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public double? GetNullableReal(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return null;

                if (obj is PDFNull @null)
                    return null;

                if (obj is PDFNullObject nullObject)
                    return null;

                if (obj is PDFReal real)
                    return real.Value;

                if (obj is PDFRealObject realObject)
                    return realObject.Value;

                if (obj is PDFInteger integer)
                    return integer.Value;

                if (obj is PDFIntegerObject integerObject)
                    return integerObject.Value;

                throw new InvalidCastException("GetReal: Object is not a number.");
            }

            /// <summary>
            /// Converts the specified value to string.
            /// If the value does not exist, the function returns the empty string.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public string GetString(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return String.Empty;

                if (obj is PDFString str)
                    return str.Value;

                if (obj is PDFStringObject strObject)
                    return strObject.Value;

                throw new InvalidCastException("GetString: Object is not a string.");
            }

            /// <summary>
            /// Converts the specified value to a name.
            /// If the value does not exist, the function returns the empty string.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// If the index is out of range, the function throws an ArgumentOutOfRangeException.
            /// </summary>
            public string GetName(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                object obj = this[index];
                if (obj == null)
                    return String.Empty;

                PDFName name = obj as PDFName;
                if (name != null)
                    return name.Value;

                PDFNameObject nameObject = obj as PDFNameObject;
                if (nameObject != null)
                    return nameObject.Value;

                throw new InvalidCastException("GetName: Object is not a name.");
            }

            /// <summary>
            /// Returns the indirect object if the value at the specified index is a PDFReference.
            /// </summary>
            [Obsolete("Use GetObject, GetDictionary, GetArray, or GetReference")]
            public PDFObject GetIndirectObject(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                PDFReference reference = this[index] as PDFReference;
                return reference?.Value;
            }

            /// <summary>
            /// Gets the PDFObject with the specified index, or null, if no such object exists. If the index refers to
            /// a reference, the referenced PDFObject is returned.
            /// </summary>
            public PDFObject GetObject(int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);

                PDFItem item = this[index];
                return item is PDFReference reference ? reference.Value : item as PDFObject;
            }

            /// <summary>
            /// Gets the PDFArray with the specified index, or null, if no such object exists. If the index refers to
            /// a reference, the referenced PDFArray is returned.
            /// </summary>
            public PDFDictionary GetDictionary(int index) => GetObject(index) as PDFDictionary;

            /// <summary>
            /// Gets the PDFArray with the specified index, or null, if no such object exists. If the index refers to
            /// a reference, the referenced PDFArray is returned.
            /// </summary>
            public PDFArray GetArray(int index) => GetObject(index) as PDFArray;

            /// <summary>
            /// Gets the PDFReference with the specified index, or null, if no such object exists.
            /// </summary>
            public PDFReference GetReference(int index)
            {
                PDFItem item = this[index];
                return item as PDFReference;
            }

            /// <summary>
            /// Gets all items of this array.
            /// </summary>
            public PDFItem[] Items => _elements.ToArray();

            #region IList Members

            /// <summary>
            /// Returns false.
            /// </summary>
            public bool IsReadOnly => false;

            /// <summary>
            /// Gets or sets an item at the specified index.
            /// </summary>
            /// <value></value>
            public PDFItem this[int index]
            {
                get => _elements[index];
                set => _elements[index] = value ?? throw new ArgumentNullException("value");
            }

            /// <summary>
            /// Removes the item at the specified index.
            /// </summary>
            public void RemoveAt(int index) => _elements.RemoveAt(index);

            /// <summary>
            /// Removes the first occurrence of a specific object from the array/>.
            /// </summary>
            public bool Remove(PDFItem item) => _elements.Remove(item);

            /// <summary>
            /// Inserts the item the specified index.
            /// </summary>
            public void Insert(int index, PDFItem value) => _elements.Insert(index, value);

            /// <summary>
            /// Determines whether the specified value is in the array.
            /// </summary>
            public bool Contains(PDFItem value) => _elements.Contains(value);

            /// <summary>
            /// Removes all items from the array.
            /// </summary>
            public void Clear() => _elements.Clear();

            /// <summary>
            /// Gets the index of the specified item.
            /// </summary>
            public int IndexOf(PDFItem value) => _elements.IndexOf(value);

            /// <summary>
            /// Appends the specified object to the array.
            /// </summary>
            public void Add(PDFItem value)
            {
                // TODO: ??? 
                //Debug.Assert((value is PDFObject && ((PDFObject)value).Reference == null) | !(value is PDFObject),
                //  "You try to set an indirect object directly into an array.");

                if (value is PDFObject obj && obj.IsIndirect)
                    _elements.Add(obj.Reference);
                else
                    _elements.Add(value);
            }

            /// <summary>
            /// Returns false.
            /// </summary>
            public bool IsFixedSize => false;

            #endregion

            #region ICollection Members

            /// <summary>
            /// Returns false.
            /// </summary>
            public bool IsSynchronized => false;

            /// <summary>
            /// Gets the number of elements in the array.
            /// </summary>
            public int Count => _elements.Count;

            /// <summary>
            /// Copies the elements of the array to the specified array.
            /// </summary>
            public void CopyTo(PDFItem[] array, int index) => _elements.CopyTo(array, index);

            /// <summary>
            /// The current implementation return null.
            /// </summary>
            public object SyncRoot => null;

            #endregion

            /// <summary>
            /// Returns an enumerator that iterates through the array.
            /// </summary>
            public IEnumerator<PDFItem> GetEnumerator() => _elements.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();

            /// <summary>
            /// The elements of the array.
            /// </summary>
            List<PDFItem> _elements;

            /// <summary>
            /// The array this objects belongs to.
            /// </summary>
            PDFArray _ownerArray;
        }

        ArrayElements _elements;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
#if true
                return String.Format(CultureInfo.InvariantCulture, "array({0},[{1}])", ObjectID.DebuggerDisplay, _elements == null ? 0 : _elements.Count);
#else
                return String.Format(CultureInfo.InvariantCulture, "array({0},[{1}])", ObjectID.DebuggerDisplay, _elements == null ? 0 : _elements.Count);
#endif
            }
        }
    }
}
