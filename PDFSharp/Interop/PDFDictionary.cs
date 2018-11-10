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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using PDFSharp.Drawing;
using PDFSharp.Interop.IO;
using PDFSharp.Interop.Filters;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Value creation flags. Specifies whether and how a value that does not exist is created.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public enum VCF
    // ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// Don't create the value.
        /// </summary>
        None,

        /// <summary>
        /// Create the value as direct object.
        /// </summary>
        Create,

        /// <summary>
        /// Create the value as indirect object.
        /// </summary>
        CreateIndirect,
    }

    /// <summary>
    /// Represents a PDF dictionary object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class PDFDictionary : PDFObject, IEnumerable<KeyValuePair<string, PDFItem>>
    {
        // Reference: 3.2.6  Dictionary Objects / Page 59

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDictionary"/> class.
        /// </summary>
        public PDFDictionary()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDictionary"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PDFDictionary(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance from an existing dictionary. Used for object type transformation.
        /// </summary>
        protected PDFDictionary(PDFDictionary dict)
            : base(dict)
        {
            if (dict._elements != null)
                dict._elements.ChangeOwner(this);
            if (dict.Stream != null)
                dict.Stream.ChangeOwner(this);
        }

        /// <summary>
        /// Creates a copy of this dictionary. Direct values are deep copied. Indirect references are not
        /// modified.
        /// </summary>
        public new PDFDictionary Clone() => (PDFDictionary)Copy();

        /// <summary>
        /// This function is useful for importing objects from external documents. The returned object is not
        /// yet complete. irefs refer to external objects and directed objects are cloned but their document
        /// property is null. A cloned dictionary or array needs a 'fix-up' to be a valid object.
        /// </summary>
        protected override object Copy()
        {
            PDFDictionary dict = (PDFDictionary)base.Copy();
            if (dict._elements != null)
            {
                dict._elements = dict._elements.Clone();
                dict._elements.ChangeOwner(dict);
                PDFName[] names = dict._elements.KeyNames;
                foreach (PDFName name in names)
                {
                    if (dict._elements[name] is PDFObject obj)
                    {
                        obj = obj.Clone();
                        // Recall that obj.Document is now null.
                        dict._elements[name] = obj;
                    }
                }
            }
            if (dict.Stream != null)
            {
                dict.Stream = dict.Stream.Clone();
                dict.Stream.ChangeOwner(dict);
            }
            return dict;
        }

        /// <summary>
        /// Gets the dictionary containing the elements of this dictionary.
        /// </summary>
        public DictionaryElements Elements => _elements ?? (_elements = new DictionaryElements(this));

        /// <summary>
        /// The elements of the dictionary.
        /// </summary>
        internal DictionaryElements _elements;

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary elements.
        /// </summary>
        public IEnumerator<KeyValuePair<string, PDFItem>> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns a string with the content of this object in a readable form. Useful for debugging purposes only.
        /// </summary>
        public override string ToString()
        {
            // Get keys and sort.
            PDFName[] keys = Elements.KeyNames;
            List<PDFName> list = new List<PDFName>(keys);
            list.Sort(PDFName.Comparer);
            list.CopyTo(keys, 0);

            StringBuilder pdf = new StringBuilder();
            pdf.Append("<< ");
            foreach (PDFName key in keys)
                pdf.Append(key + " " + Elements[key] + " ");
            pdf.Append(">>");

            return pdf.ToString();
        }

        internal override void WriteObject(PDFWriter writer)
        {
            writer.WriteBeginObject(this);
            //int count = Elements.Count;
            PDFName[] keys = Elements.KeyNames;

#if DEBUG
            // TODO: automatically set length
            if (Stream != null)
                Debug.Assert(Elements.ContainsKey(PDFStream.Keys.Length), "Dictionary has a stream but no length is set.");
#endif

#if DEBUG
            // Sort keys for debugging purposes. Comparing PDF files with for example programs like
            // Araxis Merge is easier with sorted keys.
            if (writer.Layout == PDFWriterLayout.Verbose)
            {
                List<PDFName> list = new List<PDFName>(keys);
                list.Sort(PDFName.Comparer);
                list.CopyTo(keys, 0);
            }
#endif

            foreach (PDFName key in keys)
                WriteDictionaryElement(writer, key);
            if (Stream != null)
                WriteDictionaryStream(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a key/value pair of this dictionary. This function is intended to be overridden
        /// in derived classes.
        /// </summary>
        internal virtual void WriteDictionaryElement(PDFWriter writer, PDFName key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            PDFItem item = Elements[key];
#if DEBUG
            // TODO: simplify PDFSharp
            if (item is PDFObject && ((PDFObject)item).IsIndirect)
            {
                // Replace an indirect object by its Reference.
                item = ((PDFObject)item).Reference;
                Debug.Assert(false, "Check when we come here.");
            }
#endif
            key.WriteObject(writer);
            item.WriteObject(writer);
            writer.NewLine();
        }

        /// <summary>
        /// Writes the stream of this dictionary. This function is intended to be overridden
        /// in a derived class.
        /// </summary>
        internal virtual void WriteDictionaryStream(PDFWriter writer) => writer.WriteStream(this, (writer.Options & PDFWriterOptions.OmitStream) == PDFWriterOptions.OmitStream);

        /// <summary>
        /// Gets or sets the PDF stream belonging to this dictionary. Returns null if the dictionary has
        /// no stream. To create the stream, call the CreateStream function.
        /// </summary>
        public PDFStream Stream { get; set; }

        /// <summary>
        /// Creates the stream of this dictionary and initializes it with the specified byte array.
        /// The function must not be called if the dictionary already has a stream.
        /// </summary>
        public PDFStream CreateStream(byte[] value)
        {
            if (Stream != null)
                throw new InvalidOperationException("The dictionary already has a stream.");

            Stream = new PDFStream(value, this);
            // Always set the length.
            Elements[PDFStream.Keys.Length] = new PDFInteger(Stream.Length);
            return Stream;
        }

        /// <summary>
        /// When overridden in a derived class, gets the KeysMeta of this dictionary type.
        /// </summary>
        internal virtual DictionaryMeta Meta => null;

        /// <summary>
        /// Represents the interface to the elements of a PDF dictionary.
        /// </summary>
        [DebuggerDisplay("{DebuggerDisplay}")]
        public sealed class DictionaryElements : IDictionary<string, PDFItem>, ICloneable
        {
            internal DictionaryElements(PDFDictionary ownerDictionary)
            {
                _elements = new Dictionary<string, PDFItem>();
                Owner = ownerDictionary;
            }

            object ICloneable.Clone()
            {
                DictionaryElements dictionaryElements = (DictionaryElements)MemberwiseClone();
                dictionaryElements._elements = new Dictionary<string, PDFItem>(dictionaryElements._elements);
                dictionaryElements.Owner = null;
                return dictionaryElements;
            }

            /// <summary>
            /// Creates a shallow copy of this object. The clone is not owned by a dictionary anymore.
            /// </summary>
            public DictionaryElements Clone() => (DictionaryElements)((ICloneable)this).Clone();

            /// <summary>
            /// Moves this instance to another dictionary during object type transformation.
            /// </summary>
            internal void ChangeOwner(PDFDictionary ownerDictionary)
            {
                if (Owner != null)
                {
                    // ???
                }

                // Set new owner.
                Owner = ownerDictionary;

                // Set owners elements to this.
                ownerDictionary._elements = this;
            }

            /// <summary>
            /// Gets the dictionary to which this elements object belongs to.
            /// </summary>
            internal PDFDictionary Owner { get; private set; }

            /// <summary>
            /// Converts the specified value to boolean.
            /// If the value does not exist, the function returns false.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public bool GetBoolean(string key, bool create)
            {
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFBoolean();
                    return false;
                }

                if (obj is PDFReference)
                    obj = ((PDFReference)obj).Value;

                if (obj is PDFBoolean boolean)
                    return boolean.Value;

                if (obj is PDFBooleanObject booleanObject)
                    return booleanObject.Value;
                throw new InvalidCastException("GetBoolean: Object is not a boolean.");
            }

            /// <summary>
            /// Converts the specified value to boolean.
            /// If the value does not exist, the function returns false.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public bool GetBoolean(string key) => GetBoolean(key, false);

            /// <summary>
            /// Sets the entry to a direct boolean value.
            /// </summary>
            public void SetBoolean(string key, bool value) => this[key] = new PDFBoolean(value);

            /// <summary>
            /// Converts the specified value to integer.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public int GetInteger(string key, bool create)
            {
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFInteger();
                    return 0;
                }
                if (obj is PDFReference reference)
                    obj = reference.Value;

                if (obj is PDFInteger integer)
                    return integer.Value;

                if (obj is PDFIntegerObject integerObject)
                    return integerObject.Value;

                throw new InvalidCastException("GetInteger: Object is not an integer.");
            }

            /// <summary>
            /// Converts the specified value to integer.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public int GetInteger(string key) => GetInteger(key, false);

            /// <summary>
            /// Sets the entry to a direct integer value.
            /// </summary>
            public void SetInteger(string key, int value) => this[key] = new PDFInteger(value);

            /// <summary>
            /// Converts the specified value to double.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public double GetReal(string key, bool create)
            {
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFReal();
                    return 0;
                }

                if (obj is PDFReference reference)
                    obj = reference.Value;

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
            /// Converts the specified value to double.
            /// If the value does not exist, the function returns 0.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public double GetReal(string key) => GetReal(key, false);

            /// <summary>
            /// Sets the entry to a direct double value.
            /// </summary>
            public void SetReal(string key, double value) => this[key] = new PDFReal(value);

            /// <summary>
            /// Converts the specified value to String.
            /// If the value does not exist, the function returns the empty string.
            /// </summary>
            public string GetString(string key, bool create)
            {
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFString();
                    return "";
                }

                if (obj is PDFReference reference)
                    obj = reference.Value;

                if (obj is PDFString str)
                    return str.Value;

                if (obj is PDFStringObject strObject)
                    return strObject.Value;

                PDFName name = obj as PDFName;
                if (name != null)
                    return name.Value;

                PDFNameObject nameObject = obj as PDFNameObject;
                if (nameObject != null)
                    return nameObject.Value;

                throw new InvalidCastException("GetString: Object is not a string.");
            }

            /// <summary>
            /// Converts the specified value to String.
            /// If the value does not exist, the function returns the empty string.
            /// </summary>
            public string GetString(string key) => GetString(key, false);

            /// <summary>
            /// Tries to get the string. TODO: more TryGet...
            /// </summary>
            public bool TryGetString(string key, out string value)
            {
                value = null;
                object obj = this[key];
                if (obj == null)
                    return false;

                if (obj is PDFReference reference)
                    obj = reference.Value;

                if (obj is PDFString str)
                {
                    value = str.Value;
                    return true;
                }

                if (obj is PDFStringObject strObject)
                {
                    value = strObject.Value;
                    return true;
                }

                PDFName name = obj as PDFName;
                if (name != null)
                {
                    value = name.Value;
                    return true;
                }

                PDFNameObject nameObject = obj as PDFNameObject;
                if (nameObject != null)
                {
                    value = nameObject.Value;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Sets the entry to a direct string value.
            /// </summary>
            public void SetString(string key, string value) => this[key] = new PDFString(value);

            /// <summary>
            /// Converts the specified value to a name.
            /// If the value does not exist, the function returns the empty string.
            /// </summary>
            public string GetName(string key)
            {
                object obj = this[key];
                if (obj == null)
                {
                    //if (create)
                    //  this[key] = new PDF();
                    return String.Empty;
                }

                if (obj is PDFReference reference)
                    obj = reference.Value;

                PDFName name = obj as PDFName;
                if (name != null)
                    return name.Value;

                PDFNameObject nameObject = obj as PDFNameObject;
                if (nameObject != null)
                    return nameObject.Value;

                throw new InvalidCastException("GetName: Object is not a name.");
            }

            /// <summary>
            /// Sets the specified name value.
            /// If the value doesn't start with a slash, it is added automatically.
            /// </summary>
            public void SetName(string key, string value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length == 0 || value[0] != '/')
                    value = "/" + value;

                this[key] = new PDFName(value);
            }

            /// <summary>
            /// Converts the specified value to PDFRectangle.
            /// If the value does not exist, the function returns an empty rectangle.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public PDFRectangle GetRectangle(string key, bool create)
            {
                PDFRectangle value = new PDFRectangle();
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = value = new PDFRectangle();
                    return value;
                }
                if (obj is PDFReference)
                    obj = ((PDFReference)obj).Value;

                if (obj is PDFArray array && array.Elements.Count == 4)
                {
                    value = new PDFRectangle(array.Elements.GetReal(0), array.Elements.GetReal(1),
                      array.Elements.GetReal(2), array.Elements.GetReal(3));
                    this[key] = value;
                }
                else
                    value = (PDFRectangle)obj;
                return value;
            }

            /// <summary>
            /// Converts the specified value to PDFRectangle.
            /// If the value does not exist, the function returns an empty rectangle.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public PDFRectangle GetRectangle(string key) => GetRectangle(key, false);

            /// <summary>
            /// Sets the entry to a direct rectangle value, represented by an array with four values.
            /// </summary>
            public void SetRectangle(string key, PDFRectangle rect) => _elements[key] = rect;

            /// Converts the specified value to XMatrix.
            /// If the value does not exist, the function returns an identity matrix.
            /// If the value is not convertible, the function throws an InvalidCastException.
            public XMatrix GetMatrix(string key, bool create)
            {
                XMatrix value = new XMatrix();
                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFLiteral("[1 0 0 1 0 0]");  // cannot be parsed, implement a PDFMatrix...
                    return value;
                }
                if (obj is PDFReference reference)
                    obj = reference.Value;

                if (obj is PDFArray array && array.Elements.Count == 6)
                {
                    value = new XMatrix(array.Elements.GetReal(0), array.Elements.GetReal(1), array.Elements.GetReal(2),
                      array.Elements.GetReal(3), array.Elements.GetReal(4), array.Elements.GetReal(5));
                }
                else if (obj is PDFLiteral)
                {
                    throw new NotImplementedException("Parsing matrix from literal.");
                }
                else
                    throw new InvalidCastException("Element is not an array with 6 values.");
                return value;
            }

            /// Converts the specified value to XMatrix.
            /// If the value does not exist, the function returns an identity matrix.
            /// If the value is not convertible, the function throws an InvalidCastException.
            public XMatrix GetMatrix(string key) => GetMatrix(key, false);

            /// <summary>
            /// Sets the entry to a direct matrix value, represented by an array with six values.
            /// </summary>
            public void SetMatrix(string key, XMatrix matrix) => _elements[key] = PDFLiteral.FromMatrix(matrix);

            /// <summary>
            /// Converts the specified value to DateTime.
            /// If the value does not exist, the function returns the specified default value.
            /// If the value is not convertible, the function throws an InvalidCastException.
            /// </summary>
            public DateTime GetDateTime(string key, DateTime defaultValue)
            {
                object obj = this[key];
                if (obj == null)
                {
                    return defaultValue;
                }

                if (obj is PDFReference reference)
                    obj = reference.Value;

                if (obj is PDFDate date)
                    return date.Value;

                string strDate;
                if (obj is PDFString pdfString)
                    strDate = pdfString.Value;
                else
                {
                    if (obj is PDFStringObject stringObject)
                        strDate = stringObject.Value;
                    else
                        throw new InvalidCastException("GetName: Object is not a name.");
                }

                if (strDate != "")
                {
                    try
                    {
                        defaultValue = Parser.ParseDateTime(strDate, defaultValue);
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch { }
                    // ReSharper restore EmptyGeneralCatchClause
                }
                return defaultValue;
            }

            /// <summary>
            /// Sets the entry to a direct datetime value.
            /// </summary>
            public void SetDateTime(string key, DateTime value) => _elements[key] = new PDFDate(value);

            internal int GetEnumFromName(string key, object defaultValue, bool create)
            {
                if (!(defaultValue is Enum))
                    throw new ArgumentException("defaultValue");

                object obj = this[key];
                if (obj == null)
                {
                    if (create)
                        this[key] = new PDFName(defaultValue.ToString());

                    // ReSharper disable once PossibleInvalidCastException because Enum objects can always be casted to int.
                    return (int)defaultValue;
                }
                Debug.Assert(obj is Enum);
                return (int)Enum.Parse(defaultValue.GetType(), obj.ToString().Substring(1), false);
            }

            internal int GetEnumFromName(string key, object defaultValue) => GetEnumFromName(key, defaultValue, false);

            internal void SetEnumAsName(string key, object value)
            {
                if (!(value is Enum))
                    throw new ArgumentException("value");
                _elements[key] = new PDFName("/" + value);
            }

            /// <summary>
            /// Gets the value for the specified key. If the value does not exist, it is optionally created.
            /// </summary>
            public PDFItem GetValue(string key, VCF options)
            {
                PDFObject obj;
                PDFDictionary dict;
                PDFArray array;
                PDFReference iref;
                PDFItem value = this[key];
                if (value == null ||
                    value is PDFNull ||
                    value is PDFReference && ((PDFReference)value).Value is PDFNullObject)
                {
                    if (options != VCF.None)
                    {
#if NETFX_CORE && DEBUG_
                        if (key == "/Resources")
                            Debug-Break.Break();
#endif
                        Type type = GetValueType(key);
                        if (type != null)
                        {
#if !NETFX_CORE
                            Debug.Assert(typeof(PDFItem).IsAssignableFrom(type), "Type not allowed.");
                            if (typeof(PDFDictionary).IsAssignableFrom(type))
                            {
                                value = obj = CreateDictionary(type, null);
                            }
                            else if (typeof(PDFArray).IsAssignableFrom(type))
                            {
                                value = obj = CreateArray(type, null);
                            }
                            else
                                throw new NotImplementedException("Type other than array or dictionary.");
#else
                            // Rewritten WinRT style.
                            TypeInfo typeInfo = type.GetTypeInfo();
                            Debug.Assert(typeof(PDFItem).GetTypeInfo().IsAssignableFrom(typeInfo), "Type not allowed.");
                            if (typeof(PDFDictionary).GetTypeInfo().IsAssignableFrom(typeInfo))
                            {
                                value = obj = CreateDictionary(type, null);
                            }
                            else if (typeof(PDFArray).GetTypeInfo().IsAssignableFrom(typeInfo))
                            {
                                value = obj = CreateArray(type, null);
                            }
                            else
                                throw new NotImplementedException("Type other than array or dictionary.");
#endif
                            if (options == VCF.CreateIndirect)
                            {
                                Owner.Owner.IrefTable.Add(obj);
                                this[key] = obj.Reference;
                            }
                            else
                                this[key] = obj;
                        }
                        else
                            throw new NotImplementedException("Cannot create value for key: " + key);
                    }
                }
                else
                {
                    // The value exists and can be returned. But for imported documents check for necessary
                    // object type transformation.
                    if ((iref = value as PDFReference) != null)
                    {
                        // Case: value is an indirect reference.
                        value = iref.Value;
                        if (value == null)
                        {
                            // If we come here PDF file is corrupted.
                            throw new InvalidOperationException("Indirect reference without value.");
                        }

                        if (true) // || _owner.Document.IsImported)
                        {
                            Type type = GetValueType(key);
                            Debug.Assert(type != null, "No value type specified in meta information. Please send this file to PDFSharp support.");

#if !NETFX_CORE
                            if (type != null && type != value.GetType())
                            {
                                if (typeof(PDFDictionary).IsAssignableFrom(type))
                                {
                                    Debug.Assert(value is PDFDictionary, "Bug in PDFSharp. Please send this file to PDFSharp support.");
                                    value = CreateDictionary(type, (PDFDictionary)value);
                                }
                                else if (typeof(PDFArray).IsAssignableFrom(type))
                                {
                                    Debug.Assert(value is PDFArray, "Bug in PDFSharp. Please send this file to PDFSharp support.");
                                    value = CreateArray(type, (PDFArray)value);
                                }
                                else
                                    throw new NotImplementedException("Type other than array or dictionary.");
                            }
#else
                            // Rewritten WinRT style.
                            TypeInfo typeInfo = type.GetTypeInfo();
                            if (type != null && type != value.GetType())
                            {
                                if (typeof(PDFDictionary).GetTypeInfo().IsAssignableFrom(typeInfo))
                                {
                                    Debug.Assert(value is PDFDictionary, "Bug in PDFSharp. Please send this file to PDFSharp support.");
                                    value = CreateDictionary(type, (PDFDictionary)value);
                                }
                                else if (typeof(PDFArray).GetTypeInfo().IsAssignableFrom(typeInfo))
                                {
                                    Debug.Assert(value is PDFArray, "Bug in PDFSharp. Please send this file to PDFSharp support.");
                                    value = CreateArray(type, (PDFArray)value);
                                }
                                else
                                    throw new NotImplementedException("Type other than array or dictionary.");
                            }
#endif
                        }
                        return value;
                    }

                    // Transformation is only possible after PDF import.
                    if (true) // || _owner.Document.IsImported)
                    {
                        // Case: value is a direct object
                        if ((dict = value as PDFDictionary) != null)
                        {
                            Debug.Assert(!dict.IsIndirect);

                            Type type = GetValueType(key);
                            Debug.Assert(type != null, "No value type specified in meta information. Please send this file to PDFSharp support.");
                            if (dict.GetType() != type)
                                dict = CreateDictionary(type, dict);
                            return dict;
                        }

                        if ((array = value as PDFArray) != null)
                        {
                            Debug.Assert(!array.IsIndirect);

                            Type type = GetValueType(key);
                            // This is more complicated. If type is null do nothing
                            //Debug.Assert(type != null, "No value type specified in meta information. Please send this file to PDFSharp support.");
                            if (type != null && type != array.GetType())
                                array = CreateArray(type, array);
                            return array;
                        }
                    }
                }
                return value;
            }

            /// <summary>
            /// Short cut for GetValue(key, VCF.None).
            /// </summary>
            public PDFItem GetValue(string key) => GetValue(key, VCF.None);

            /// <summary>
            /// Returns the type of the object to be created as value of the specified key.
            /// </summary>
            Type GetValueType(string key)  // TODO: move to PDFObject
            {
                Type type = null;
                DictionaryMeta meta = Owner.Meta;
                if (meta != null)
                {
                    KeyDescriptor kd = meta[key];
                    if (kd != null)
                        type = kd.GetValueType();
                    //else
                    //    Debug.WriteLine("Warning: Key not descriptor table: " + key);  // TODO: check what this means...
                }
                //else
                //    Debug.WriteLine("Warning: No meta provided for type: " + _owner.GetType().Name);  // TODO: check what this means...
                return type;
            }

            PDFArray CreateArray(Type type, PDFArray oldArray)
            {
#if !NETFX_CORE && !UWP
                ConstructorInfo ctorInfo;
                PDFArray array;
                if (oldArray == null)
                {
                    // Use constructor with signature 'Ctor(PDFDocument owner)'.
                    ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new Type[] { typeof(PDFDocument) }, null);
                    Debug.Assert(ctorInfo != null, "No appropriate constructor found for type: " + type.Name);
                    array = ctorInfo.Invoke(new object[] { Owner.Owner }) as PDFArray;
                }
                else
                {
                    // Use constructor with signature 'Ctor(PDFDictionary dict)'.
                    ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new Type[] { typeof(PDFArray) }, null);
                    Debug.Assert(ctorInfo != null, "No appropriate constructor found for type: " + type.Name);
                    array = ctorInfo.Invoke(new object[] { oldArray }) as PDFArray;
                }
                return array;
#else
                // Rewritten WinRT style.
                PDFArray array = null;
                if (oldArray == null)
                {
                    // Use constructor with signature 'Ctor(PDFDocument owner)'.
                    var ctorInfos = type.GetTypeInfo().DeclaredConstructors; //.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    //null, new Type[] { typeof(PDFDocument) }, null);
                    foreach (var ctorInfo in ctorInfos)
                    {
                        var parameters = ctorInfo.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PDFDocument))
                        {
                            array = ctorInfo.Invoke(new object[] { _ownerDictionary.Owner }) as PDFArray;
                            break;
                        }
                    }
                    Debug.Assert(array != null, "No appropriate constructor found for type: " + type.Name);
                }
                else
                {
                    // Use constructor with signature 'Ctor(PDFDictionary dict)'.
                    var ctorInfos = type.GetTypeInfo().DeclaredConstructors; // .GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    //null, new Type[] { typeof(PDFArray) }, null);
                    foreach (var ctorInfo in ctorInfos)
                    {
                        var parameters = ctorInfo.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PDFArray))
                        {
                            array = ctorInfo.Invoke(new object[] { oldArray }) as PDFArray;
                            break;
                        }
                    }
                    Debug.Assert(array != null, "No appropriate constructor found for type: " + type.Name);
                }
                return array;
#endif
            }

            PDFDictionary CreateDictionary(Type type, PDFDictionary oldDictionary)
            {
#if !NETFX_CORE && !UWP
                ConstructorInfo ctorInfo;
                PDFDictionary dict;
                if (oldDictionary == null)
                {
                    // Use constructor with signature 'Ctor(PDFDocument owner)'.
                    ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new Type[] { typeof(PDFDocument) }, null);
                    Debug.Assert(ctorInfo != null, "No appropriate constructor found for type: " + type.Name);
                    dict = ctorInfo.Invoke(new object[] { Owner.Owner }) as PDFDictionary;
                }
                else
                {
                    // Use constructor with signature 'Ctor(PDFDictionary dict)'.
                    ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                      null, new Type[] { typeof(PDFDictionary) }, null);
                    Debug.Assert(ctorInfo != null, "No appropriate constructor found for type: " + type.Name);
                    dict = ctorInfo.Invoke(new object[] { oldDictionary }) as PDFDictionary;
                }
                return dict;
#else
                // Rewritten WinRT style.
                PDFDictionary dict = null;
                if (oldDictionary == null)
                {
                    // Use constructor with signature 'Ctor(PDFDocument owner)'.
                    var ctorInfos = type.GetTypeInfo().DeclaredConstructors; //GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    //null, new Type[] { typeof(PDFDocument) }, null);
                    foreach (var ctorInfo in ctorInfos)
                    {
                        var parameters = ctorInfo.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PDFDocument))
                        {
                            dict = ctorInfo.Invoke(new object[] { _ownerDictionary.Owner }) as PDFDictionary;
                            break;
                        }
                    }
                    Debug.Assert(dict != null, "No appropriate constructor found for type: " + type.Name);
                }
                else
                {
                    var ctorInfos = type.GetTypeInfo().DeclaredConstructors; // GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(PDFDictionary) }, null);
                    foreach (var ctorInfo in ctorInfos)
                    {
                        var parameters = ctorInfo.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PDFDictionary))
                        {
                            dict = ctorInfo.Invoke(new object[] { _ownerDictionary.Owner }) as PDFDictionary;
                            break;
                        }
                    }
                    Debug.Assert(dict != null, "No appropriate constructor found for type: " + type.Name);
                }
                return dict;
#endif
            }

            PDFItem CreateValue(Type type, PDFDictionary oldValue)
            {
#if !NETFX_CORE && !UWP
                ConstructorInfo ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new Type[] { typeof(PDFDocument) }, null);
                PDFObject obj = ctorInfo.Invoke(new object[] { Owner.Owner }) as PDFObject;
                if (oldValue != null)
                {
                    obj.Reference = oldValue.Reference;
                    obj.Reference.Value = obj;
                    if (obj is PDFDictionary dict)
                    {
                        dict._elements = oldValue._elements;
                    }
                }
                return obj;
#else
                // Rewritten WinRT style.
                PDFObject obj = null;
                var ctorInfos = type.GetTypeInfo().DeclaredConstructors; // GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(PDFDocument) }, null);
                foreach (var ctorInfo in ctorInfos)
                {
                    var parameters = ctorInfo.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PDFDocument))
                    {
                        obj = ctorInfo.Invoke(new object[] { _ownerDictionary.Owner }) as PDFObject;
                        break;
                    }
                }
                Debug.Assert(obj != null, "No appropriate constructor found for type: " + type.Name);
                if (oldValue != null)
                {
                    obj.Reference = oldValue.Reference;
                    obj.Reference.Value = obj;
                    if (obj is PDFDictionary)
                    {
                        PDFDictionary dict = (PDFDictionary)obj;
                        dict._elements = oldValue._elements;
                    }
                }
                return obj;
#endif
            }

            /// <summary>
            /// Sets the entry with the specified value. DON'T USE THIS FUNCTION - IT MAY BE REMOVED.
            /// </summary>
            public void SetValue(string key, PDFItem value)
            {
                Debug.Assert((value is PDFObject && ((PDFObject)value).Reference == null) | !(value is PDFObject),
                    "You try to set an indirect object directly into a dictionary.");

                // HACK?
                _elements[key] = value;
            }

            ///// <summary>
            ///// Returns the indirect object if the value of the specified key is a PDFReference.
            ///// </summary>
            //[Obsolete("Use GetObject, GetDictionary, GetArray, or GetReference")]
            //public PDFObject GetIndirectObject(string key)
            //{
            //    PDFItem item = this[key];
            //    if (item is PDFReference)
            //        return ((PDFReference)item).Value;
            //    return null;
            //}

            /// <summary>
            /// Gets the PDFObject with the specified key, or null, if no such object exists. If the key refers to
            /// a reference, the referenced PDFObject is returned.
            /// </summary>
            public PDFObject GetObject(string key)
            {
                PDFItem item = this[key];
                return item is PDFReference reference ? reference.Value : item as PDFObject;
            }

            /// <summary>
            /// Gets the PDFDictionary with the specified key, or null, if no such object exists. If the key refers to
            /// a reference, the referenced PDFDictionary is returned.
            /// </summary>
            public PDFDictionary GetDictionary(string key) => GetObject(key) as PDFDictionary;

            /// <summary>
            /// Gets the PDFArray with the specified key, or null, if no such object exists. If the key refers to
            /// a reference, the referenced PDFArray is returned.
            /// </summary>
            public PDFArray GetArray(string key) => GetObject(key) as PDFArray;

            /// <summary>
            /// Gets the PDFReference with the specified key, or null, if no such object exists.
            /// </summary>
            public PDFReference GetReference(string key)
            {
                PDFItem item = this[key];
                return item as PDFReference;
            }

            /// <summary>
            /// Sets the entry to the specified object. The object must not be an indirect object,
            /// otherwise an exception is raised.
            /// </summary>
            public void SetObject(string key, PDFObject obj)
            {
                if (obj.Reference != null)
                    throw new ArgumentException("PDFObject must not be an indirect object.", "obj");
                this[key] = obj;
            }

            /// <summary>
            /// Sets the entry as a reference to the specified object. The object must be an indirect object,
            /// otherwise an exception is raised.
            /// </summary>
            public void SetReference(string key, PDFObject obj)
            {
                if (obj.Reference == null)
                    throw new ArgumentException("PDFObject must be an indirect object.", "obj");
                this[key] = obj.Reference;
            }

            /// <summary>
            /// Sets the entry as a reference to the specified iref.
            /// </summary>
            public void SetReference(string key, PDFReference iref) => this[key] = iref ?? throw new ArgumentNullException("iref");

            #region IDictionary Members

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object is read-only.
            /// </summary>
            public bool IsReadOnly => false;

            /// <summary>
            /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
            /// </summary>
            public IEnumerator<KeyValuePair<string, PDFItem>> GetEnumerator() => _elements.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((ICollection)_elements).GetEnumerator();

            /// <summary>
            /// Gets or sets an entry in the dictionary. The specified key must be a valid PDF name
            /// starting with a slash '/'. This property provides full access to the elements of the
            /// PDF dictionary. Wrong use can lead to errors or corrupt PDF files.
            /// </summary>
            public PDFItem this[string key]
            {
                get
                {
                    _elements.TryGetValue(key, out PDFItem item);
                    return item;
                }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
#if DEBUG_
                    if (key == "/MediaBox")
                        key.GetType();

                    //if (value is PDFObject)
                    //{
                    //  PDFObject obj = (PDFObject)value;
                    //  if (obj.Reference != null)
                    //    throw new ArgumentException("An object with an indirect reference cannot be a direct value. Try to set an indirect reference.");
                    //}
                    if (value is PDFDictionary)
                    {
                        PDFDictionary dict = (PDFDictionary)value;
                        if (dict._stream != null)
                            throw new ArgumentException("A dictionary with stream cannot be a direct value.");
                    }
#endif
                    if (value is PDFObject obj && obj.IsIndirect)
                        value = obj.Reference;
                    _elements[key] = value;
                }
            }

            /// <summary>
            /// Gets or sets an entry in the dictionary identified by a PDFName object.
            /// </summary>
            public PDFItem this[PDFName key]
            {
                get => this[key.Value];
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

#if DEBUG
                    if (value is PDFDictionary dictionary)
                    {
                        PDFDictionary dict = dictionary;
                        if (dict.Stream != null)
                            throw new ArgumentException("A dictionary with stream cannot be a direct value.");
                    }

#endif

                    if (value is PDFObject obj && obj.IsIndirect)
                        value = obj.Reference;
                    _elements[key.Value] = value;
                }
            }

            /// <summary>
            /// Removes the value with the specified key.
            /// </summary>
            public bool Remove(string key) => _elements.Remove(key);

            /// <summary>
            /// Removes the value with the specified key.
            /// </summary>
            public bool Remove(KeyValuePair<string, PDFItem> item) => throw new NotImplementedException();

            ///// <summary>
            ///// Determines whether the dictionary contains the specified name.
            ///// </summary>
            //[Obsolete("Use ContainsKey.")]
            //public bool Contains(string key)
            //{
            //    return _elements.ContainsKey(key);
            //}

            /// <summary>
            /// Determines whether the dictionary contains the specified name.
            /// </summary>
            public bool ContainsKey(string key) => _elements.ContainsKey(key);

            /// <summary>
            /// Determines whether the dictionary contains a specific value.
            /// </summary>
            public bool Contains(KeyValuePair<string, PDFItem> item) => throw new NotImplementedException();

            /// <summary>
            /// Removes all elements from the dictionary.
            /// </summary>
            public void Clear() => _elements.Clear();

            /// <summary>
            /// Adds the specified value to the dictionary.
            /// </summary>
            public void Add(string key, PDFItem value)
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                if (key[0] != '/')
                    throw new ArgumentException("The key must start with a slash '/'.");

                // If object is indirect automatically convert value to reference.
                if (value is PDFObject obj && obj.IsIndirect)
                    value = obj.Reference;

                _elements.Add(key, value);
            }

            /// <summary>
            /// Adds an item to the dictionary.
            /// </summary>
            public void Add(KeyValuePair<string, PDFItem> item) => Add(item.Key, item.Value);

            /// <summary>
            /// Gets all keys currently in use in this dictionary as an array of PDFName objects.
            /// </summary>
            public PDFName[] KeyNames
            {
                get
                {
                    ICollection values = _elements.Keys;
                    int count = values.Count;
                    string[] strings = new string[count];
                    values.CopyTo(strings, 0);
                    PDFName[] names = new PDFName[count];
                    for (int idx = 0; idx < count; idx++)
                        names[idx] = new PDFName(strings[idx]);
                    return names;
                }
            }

            /// <summary>
            /// Get all keys currently in use in this dictionary as an array of string objects.
            /// </summary>
            public ICollection<string> Keys
            {
                // It is by design not to return _elements.Keys, but a copy.
                get
                {
                    ICollection values = _elements.Keys;
                    int count = values.Count;
                    string[] keys = new string[count];
                    values.CopyTo(keys, 0);
                    return keys;
                }
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            public bool TryGetValue(string key, out PDFItem value) => _elements.TryGetValue(key, out value);

            /// <summary>
            /// Gets all values currently in use in this dictionary as an array of PDFItem objects.
            /// </summary>
            //public ICollection<PDFItem> Values
            public ICollection<PDFItem> Values
            {
                // It is by design not to return _elements.Values, but a copy.
                get
                {
                    ICollection values = _elements.Values;
                    PDFItem[] items = new PDFItem[values.Count];
                    values.CopyTo(items, 0);
                    return items;
                }
            }

            /// <summary>
            /// Return false.
            /// </summary>
            public bool IsFixedSize => false;

            #endregion

            #region ICollection Members

            /// <summary>
            /// Return false.
            /// </summary>
            public bool IsSynchronized => false;

            /// <summary>
            /// Gets the number of elements contained in the dictionary.
            /// </summary>
            public int Count => _elements.Count;

            /// <summary>
            /// Copies the elements of the dictionary to an array, starting at a particular index.
            /// </summary>
            public void CopyTo(KeyValuePair<string, PDFItem>[] array, int arrayIndex) => throw new NotImplementedException();

            /// <summary>
            /// The current implementation returns null.
            /// </summary>
            public object SyncRoot => null;

            #endregion

            /// <summary>
            /// Gets the DebuggerDisplayAttribute text.
            /// </summary>
            // ReSharper disable UnusedMember.Local
            internal string DebuggerDisplay
            // ReSharper restore UnusedMember.Local
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat(CultureInfo.InvariantCulture, "key={0}:(", _elements.Count);
                    bool addSpace = false;
                    ICollection<string> keys = _elements.Keys;
                    foreach (string key in keys)
                    {
                        if (addSpace)
                            sb.Append(' ');
                        addSpace = true;
                        sb.Append(key);
                    }
                    sb.Append(")");
                    return sb.ToString();
                }
            }

            /// <summary>
            /// The elements of the dictionary with a string as key.
            /// Because the string is a name it starts always with a '/'.
            /// </summary>
            Dictionary<string, PDFItem> _elements;
        }

        /// <summary>
        /// The PDF stream objects.
        /// </summary>
        public sealed class PDFStream
        {
            internal PDFStream(PDFDictionary ownerDictionary) => _ownerDictionary = ownerDictionary ?? throw new ArgumentNullException("ownerDictionary");

            /// <summary>
            /// A .NET string can contain char(0) as a valid character.
            /// </summary>
            internal PDFStream(byte[] value, PDFDictionary owner)
                : this(owner) => _value = value;

            /// <summary>
            /// Clones this stream by creating a deep copy.
            /// </summary>
            public PDFStream Clone()
            {
                PDFStream stream = (PDFStream)MemberwiseClone();
                stream._ownerDictionary = null;
                if (stream._value != null)
                {
                    stream._value = new byte[stream._value.Length];
                    _value.CopyTo(stream._value, 0);
                }
                return stream;
            }

            /// <summary>
            /// Moves this instance to another dictionary during object type transformation.
            /// </summary>
            internal void ChangeOwner(PDFDictionary dict)
            {
                if (_ownerDictionary != null)
                {
                    // ???
                }

                // Set new owner.
                _ownerDictionary = dict;

                // Set owners stream to this.
                _ownerDictionary.Stream = this;
            }

            /// <summary>
            /// The dictionary the stream belongs to.
            /// </summary>
            PDFDictionary _ownerDictionary;

            /// <summary>
            /// Gets the length of the stream, i.e. the actual number of bytes in the stream.
            /// </summary>
            public int Length => _value != null ? _value.Length : 0;

            /// <summary>
            /// Gets a value indicating whether this stream has decode parameters.
            /// </summary>
            internal bool HasDecodeParams
            {
                //  TODO: Move to Stream.Internals
                get
                {
                    // TODO: DecodeParams can be an array.
                    PDFDictionary dictionary = _ownerDictionary.Elements.GetDictionary(Keys.DecodeParms);
                    if (dictionary != null)
                    {
                        // More to do here?
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Gets the decode predictor for LZW- or FlateDecode.
            /// Returns 0 if no such value exists.
            /// </summary>
            internal int DecodePredictor  // Reference: TABLE 3.8  Predictor values / Page 76
            {
                get
                {
                    PDFDictionary dictionary = _ownerDictionary.Elements.GetDictionary(Keys.DecodeParms);
                    return dictionary != null ? dictionary.Elements.GetInteger("/Predictor") : 0;
                }
            }

            /// <summary>
            /// Gets the decode Columns for LZW- or FlateDecode.
            /// Returns 0 if no such value exists.
            /// </summary>
            internal int DecodeColumns  // Reference: TABLE 3.8  Predictor values / Page 76
            {
                get
                {
                    PDFDictionary dictionary = _ownerDictionary.Elements.GetDictionary(Keys.DecodeParms);
                    return dictionary != null ? dictionary.Elements.GetInteger("/Columns") : 0;
                }
            }

            /// <summary>
            /// Get or sets the bytes of the stream as they are, i.e. if one or more filters exist the bytes are
            /// not unfiltered.
            /// </summary>
            public byte[] Value
            {
                get => _value;
                set
                {
                    _value = value ?? throw new ArgumentNullException("value");
                    _ownerDictionary.Elements.SetInteger(Keys.Length, value.Length);
                }
            }
            byte[] _value;

            /// <summary>
            /// Gets the value of the stream unfiltered. The stream content is not modified by this operation.
            /// </summary>
            public byte[] UnfilteredValue
            {
                get
                {
                    byte[] bytes = null;
                    if (_value != null)
                    {
                        PDFItem filter = _ownerDictionary.Elements["/Filter"];
                        if (filter != null)
                        {
                            bytes = Filtering.Decode(_value, filter);
                            if (bytes == null)
                            {
                                string message = String.Format("«Cannot decode filter '{0}'»", filter);
                                bytes = PDFEncoders.RawEncoding.GetBytes(message);
                            }
                        }
                        else
                        {
                            bytes = new byte[_value.Length];
                            _value.CopyTo(bytes, 0);
                        }
                    }
                    return bytes ?? new byte[0];
                }
            }

            /// <summary>
            /// Tries to unfilter the bytes of the stream. If the stream is filtered and PDFSharp knows the filter
            /// algorithm, the stream content is replaced by its unfiltered value and the function returns true.
            /// Otherwise the content remains untouched and the function returns false.
            /// The function is useful for analyzing existing PDF files.
            /// </summary>
            public bool TryUnfilter()  // TODO: Take DecodeParams into account.
            {
                if (_value != null)
                {
                    PDFItem filter = _ownerDictionary.Elements["/Filter"];
                    if (filter != null)
                    {
                        // PDFSharp can only uncompress streams that are compressed with the ZIP or LZH algorithm.
                        byte[] bytes = Filtering.Decode(_value, filter);
                        if (bytes != null)
                        {
                            _ownerDictionary.Elements.Remove(Keys.Filter);
                            Value = bytes;
                        }
                        else
                            return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Compresses the stream with the FlateDecode filter.
            /// If a filter is already defined, the function has no effect.
            /// </summary>
            public void Zip()
            {
                if (_value == null)
                    return;

                if (!_ownerDictionary.Elements.ContainsKey("/Filter"))
                {
                    _value = Filtering.FlateDecode.Encode(_value, _ownerDictionary._document.Options.FlateEncodeMode);
                    _ownerDictionary.Elements["/Filter"] = new PDFName("/FlateDecode");
                    _ownerDictionary.Elements["/Length"] = new PDFInteger(_value.Length);
                }
            }

            /// <summary>
            /// Returns the stream content as a raw string.
            /// </summary>
            public override string ToString()
            {
                if (_value == null)
                    return "«null»";

                string stream;
                PDFItem filter = _ownerDictionary.Elements["/Filter"];
                if (filter != null)
                {
#if true
                    byte[] bytes = Filtering.Decode(_value, filter);
                    if (bytes != null)
                        stream = PDFEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
#else

                    if (_owner.Elements.GetString("/Filter") == "/FlateDecode")
                    {
                        stream = Filtering.FlateDecode.DecodeToString(_value);
                    }
#endif
                    else
                        throw new NotImplementedException("Unknown filter");
                }
                else
                    stream = PDFEncoders.RawEncoding.GetString(_value, 0, _value.Length);

                return stream;
            }

            //internal void WriteObject_(Stream stream)
            //{
            //    if (_value != null)
            //        stream.Write(_value, 0, value.Length);
            //}

            ///// <summary>
            ///// Converts a raw encoded string into a byte array.
            ///// </summary>
            //public static byte[] RawEncode(string content)
            //{
            //    return PDFEncoders.RawEncoding.GetBytes(content);
            //}

            /// <summary>
            /// Common keys for all streams.
            /// </summary>
            public class Keys : KeysBase
            {
                // ReSharper disable InconsistentNaming

                /// <summary>
                /// (Required) The number of bytes from the beginning of the line following the keyword
                /// stream to the last byte just before the keyword endstream. (There may be an additional
                /// EOL marker, preceding endstream, that is not included in the count and is not logically
                /// part of the stream data.)
                /// </summary>
                [KeyInfo(KeyType.Integer | KeyType.Required)]
                public const string Length = "/Length";

                /// <summary>
                /// (Optional) The name of a filter to be applied in processing the stream data found between
                /// the keywords stream and endstream, or an array of such names. Multiple filters should be
                /// specified in the order in which they are to be applied.
                /// </summary>
                [KeyInfo(KeyType.NameOrArray | KeyType.Optional)]
                public const string Filter = "/Filter";

                /// <summary>
                /// (Optional) A parameter dictionary or an array of such dictionaries, used by the filters
                /// specified by Filter. If there is only one filter and that filter has parameters, DecodeParms
                /// must be set to the filter’s parameter dictionary unless all the filter’s parameters have
                /// their default values, in which case the DecodeParms entry may be omitted. If there are 
                /// multiple filters and any of the filters has parameters set to nondefault values, DecodeParms
                /// must be an array with one entry for each filter: either the parameter dictionary for that
                /// filter, or the null object if that filter has no parameters (or if all of its parameters have
                /// their default values). If none of the filters have parameters, or if all their parameters
                /// have default values, the DecodeParms entry may be omitted.
                /// </summary>
                [KeyInfo(KeyType.ArrayOrDictionary | KeyType.Optional)]
                public const string DecodeParms = "/DecodeParms";

                /// <summary>
                /// (Optional; PDF 1.2) The file containing the stream data. If this entry is present, the bytes
                /// between stream and endstream are ignored, the filters are specified by FFilter rather than
                /// Filter, and the filter parameters are specified by FDecodeParms rather than DecodeParms.
                /// However, the Length entry should still specify the number of those bytes. (Usually, there are
                /// no bytes and Length is 0.)
                /// </summary>
                [KeyInfo("1.2", KeyType.String | KeyType.Optional)]
                public const string F = "/F";

                /// <summary>
                /// (Optional; PDF 1.2) The name of a filter to be applied in processing the data found in the
                /// stream’s external file, or an array of such names. The same rules apply as for Filter.
                /// </summary>
                [KeyInfo("1.2", KeyType.NameOrArray | KeyType.Optional)]
                public const string FFilter = "/FFilter";

                /// <summary>
                /// (Optional; PDF 1.2) A parameter dictionary, or an array of such dictionaries, used by the
                /// filters specified by FFilter. The same rules apply as for DecodeParms.
                /// </summary>
                [KeyInfo("1.2", KeyType.ArrayOrDictionary | KeyType.Optional)]
                public const string FDecodeParms = "/FDecodeParms";

                /// <summary>
                /// Optional; PDF 1.5) A non-negative integer representing the number of bytes in the decoded
                /// (defiltered) stream. It can be used to determine, for example, whether enough disk space is
                /// available to write a stream to a file.
                /// This value should be considered a hint only; for some stream filters, it may not be possible
                /// to determine this value precisely.
                /// </summary>
                [KeyInfo("1.5", KeyType.Integer | KeyType.Optional)]
                public const string DL = "/DL";

                // ReSharper restore InconsistentNaming
            }
        }

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
                return String.Format(CultureInfo.InvariantCulture, "dictionary({0},[{1}])={2}", 
                    ObjectID.DebuggerDisplay, 
                    Elements.Count,
                    _elements.DebuggerDisplay);
#else
                return String.Format(CultureInfo.InvariantCulture, "dictionary({0},[{1}])=", ObjectID.DebuggerDisplay, _elements.DebuggerDisplay);
#endif
            }
        }
    }
}
