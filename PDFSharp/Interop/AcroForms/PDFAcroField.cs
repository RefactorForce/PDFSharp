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
using System.Collections.Generic;
using System.Diagnostics;
using PDFSharp.Interop.Advanced;

namespace PDFSharp.Interop.AcroForms
{
    /// <summary>
    /// Represents the base class for all interactive field dictionaries.
    /// </summary>
    public abstract class PDFAcroField : PDFDictionary
    {
        /// <summary>
        /// Initializes a new instance of PDFAcroField.
        /// </summary>
        internal PDFAcroField(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFAcroField"/> class. Used for type transformation.
        /// </summary>
        protected PDFAcroField(PDFDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets the name of this field.
        /// </summary>
        public string Name
        {
            get
            {
                string name = Elements.GetString(Keys.T);
                return name;
            }
        }

        /// <summary>
        /// Gets the field flags of this instance.
        /// </summary>
        public PDFAcroFieldFlags Flags => (PDFAcroFieldFlags)Elements.GetInteger(Keys.Ff);

        internal PDFAcroFieldFlags SetFlags
        {
            get => (PDFAcroFieldFlags)Elements.GetInteger(Keys.Ff);
            set => Elements.SetInteger(Keys.Ff, (int)value);
        }

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public virtual PDFItem Value
        {
            get => Elements[Keys.V];
            set
            {
                if (ReadOnly)
                    throw new InvalidOperationException("The field is read only.");
                if (value is PDFString || value is PDFName)
                    Elements[Keys.V] = value;
                else
                    throw new NotImplementedException("Values other than string cannot be set.");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field is read only.
        /// </summary>
        public bool ReadOnly
        {
            get => (Flags & PDFAcroFieldFlags.ReadOnly) != 0;
            set
            {
                if (value)
                    SetFlags |= PDFAcroFieldFlags.ReadOnly;
                else
                    SetFlags &= ~PDFAcroFieldFlags.ReadOnly;
            }
        }

        /// <summary>
        /// Gets the field with the specified name.
        /// </summary>
        public PDFAcroField this[string name] => GetValue(name);

        /// <summary>
        /// Gets a child field by name.
        /// </summary>
        protected virtual PDFAcroField GetValue(string name) => String.IsNullOrEmpty(name) ? (this) : HasKids ? Fields.GetValue(name) : null;

        /// <summary>
        /// Indicates whether the field has child fields.
        /// </summary>
        public bool HasKids
        {
            get
            {
                PDFItem item = Elements[Keys.Kids];
                return item == null ? false : item is PDFArray ? ((PDFArray)item).Elements.Count > 0 : false;
            }
        }

        /// <summary>
        /// Gets the names of all descendants of this field.
        /// </summary>
        [Obsolete("Use GetDescendantNames")]
        public string[] DescendantNames  // Properties should not return arrays.
=> GetDescendantNames();

        /// <summary>
        /// Gets the names of all descendants of this field.
        /// </summary>
        public string[] GetDescendantNames()
        {
            List<string> names = new List<string>();
            if (HasKids)
            {
                PDFAcroFieldCollection fields = Fields;
                fields.GetDescendantNames(ref names, null);
            }
            List<string> temp = new List<string>();
            foreach (string name in names)
                temp.Add(name);
            return temp.ToArray();
        }

        /// <summary>
        /// Gets the names of all appearance dictionaries of this AcroField.
        /// </summary>
        public string[] GetAppearanceNames()
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            if (Elements["/AP"] is PDFDictionary dict)
            {
                AppDict(dict, names);

                if (HasKids)
                {
                    PDFItem[] kids = Fields.Elements.Items;
                    foreach (PDFItem pdfItem in kids)
                    {
                        if (pdfItem is PDFReference)
                        {
                            if (((PDFReference)pdfItem).Value is PDFDictionary xxx)
                                AppDict(xxx, names);
                        }
                    }
                    //((PDFDictionary)(((PDFReference)(Fields.Elements.Items[1])).Value)).Elements.SetName(Keys.V, name1);

                }
            }
            string[] array = new string[names.Count];
            names.Keys.CopyTo(array, 0);
            return array;
        }

        //static string[] AppearanceNames(PDFDictionary dictIn)
        //{
        //  Dictionary<string, object> names = new Dictionary<string, object>();
        //  PDFDictionary dict = dictIn["/AP"] as PDFDictionary;
        //  if (dict != null)
        //  {
        //    AppDict(dict, names);

        //    if (HasKids)
        //    {
        //      PDFItem[] kids = Fields.Elements.Items;
        //      foreach (PDFItem pdfItem in kids)
        //      {
        //        if (pdfItem is PDFReference)
        //        {
        //          PDFDictionary xxx = ((PDFReference)pdfItem).Value as PDFDictionary;
        //          if (xxx != null)
        //            AppDict(xxx, names);
        //        }
        //      }
        //      //((PDFDictionary)(((PDFReference)(Fields.Elements.Items[1])).Value)).Elements.SetName(Keys.V, name1);

        //    }
        //  }
        //  string[] array = new string[names.Count];
        //  names.Keys.CopyTo(array, 0);
        //  return array;
        //}

        static void AppDict(PDFDictionary dict, Dictionary<string, object> names)
        {
            if (dict.Elements["/D"] is PDFDictionary sub)
                AppDict2(sub, names);
            if ((sub = dict.Elements["/N"] as PDFDictionary) != null)
                AppDict2(sub, names);
        }

        static void AppDict2(PDFDictionary dict, Dictionary<string, object> names)
        {
            foreach (string key in dict.Elements.Keys)
            {
                if (!names.ContainsKey(key))
                    names.Add(key, null);
            }
        }

        internal virtual void GetDescendantNames(ref List<string> names, string partialName)
        {
            if (HasKids)
            {
                PDFAcroFieldCollection fields = Fields;
                string t = Elements.GetString(Keys.T);
                Debug.Assert(t != "");
                if (t.Length > 0)
                {
                    if (!String.IsNullOrEmpty(partialName))
                        partialName += "." + t;
                    else
                        partialName = t;
                    fields.GetDescendantNames(ref names, partialName);
                }
            }
            else
            {
                string t = Elements.GetString(Keys.T);
                Debug.Assert(t != "");
                if (t.Length > 0)
                {
                    if (!String.IsNullOrEmpty(partialName))
                        names.Add(partialName + "." + t);
                    else
                        names.Add(t);
                }
            }
        }

        /// <summary>
        /// Gets the collection of fields within this field.
        /// </summary>
        public PDFAcroFieldCollection Fields
        {
            get
            {
                if (_fields == null)
                {
                    object o = Elements.GetValue(Keys.Kids, VCF.CreateIndirect);
                    _fields = (PDFAcroFieldCollection)o;
                }
                return _fields;
            }
        }
        PDFAcroFieldCollection _fields;

        /// <summary>
        /// Holds a collection of interactive fields.
        /// </summary>
        public sealed class PDFAcroFieldCollection : PDFArray
        {
            PDFAcroFieldCollection(PDFArray array)
                : base(array)
            { }

            /// <summary>  
            /// Gets the number of elements in the array.  
            /// </summary>  
            public int Count => Elements.Count;

            /// <summary>
            /// Gets the names of all fields in the collection.
            /// </summary>
            public string[] Names
            {
                get
                {
                    int count = Elements.Count;
                    string[] names = new string[count];
                    for (int idx = 0; idx < count; idx++)
                        names[idx] = ((PDFDictionary)((PDFReference)Elements[idx]).Value).Elements.GetString(Keys.T);
                    return names;
                }
            }

            /// <summary>
            /// Gets an array of all descendant names.
            /// </summary>
            public string[] DescendantNames
            {
                get
                {
                    List<string> names = new List<string>();
                    GetDescendantNames(ref names, null);
                    //List<string> temp = new List<string>();
                    //foreach (PDFName name in names)
                    //  temp.Add(name.ToString());
                    return names.ToArray();
                }
            }

            internal void GetDescendantNames(ref List<string> names, string partialName)
            {
                int count = Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PDFAcroField field = this[idx];
                    if (field != null)
                        field.GetDescendantNames(ref names, partialName);
                }
            }

            /// <summary>
            /// Gets a field from the collection. For your convenience an instance of a derived class like
            /// PDFTextField or PDFCheckBox is returned if PDFSharp can guess the actual type of the dictionary.
            /// If the actual type cannot be guessed by PDFSharp the function returns an instance
            /// of PDFGenericField.
            /// </summary>
            public PDFAcroField this[int index]
            {
                get
                {
                    PDFItem item = Elements[index];
                    Debug.Assert(item is PDFReference);
                    PDFDictionary dict = ((PDFReference)item).Value as PDFDictionary;
                    Debug.Assert(dict != null);
                    PDFAcroField field = dict as PDFAcroField;
                    if (field == null && dict != null)
                    {
                        // Do type transformation
                        field = CreateAcroField(dict);
                        //Elements[index] = field.XRef;
                    }
                    return field;
                }
            }

            /// <summary>
            /// Gets the field with the specified name.
            /// </summary>
            public PDFAcroField this[string name] => GetValue(name);

            internal PDFAcroField GetValue(string name)
            {
                if (String.IsNullOrEmpty(name))
                    return null;

                int dot = name.IndexOf('.');
                string prefix = dot == -1 ? name : name.Substring(0, dot);
                string suffix = dot == -1 ? "" : name.Substring(dot + 1);

                int count = Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PDFAcroField field = this[idx];
                    if (field.Name == prefix)
                        return field.GetValue(suffix);
                }
                return null;
            }

            /// <summary>
            /// Create a derived type like PDFTextField or PDFCheckBox if possible.
            /// If the actual cannot be guessed by PDFSharp the function returns an instance
            /// of PDFGenericField.
            /// </summary>
            PDFAcroField CreateAcroField(PDFDictionary dict)
            {
                string ft = dict.Elements.GetName(Keys.FT);
                PDFAcroFieldFlags flags = (PDFAcroFieldFlags)dict.Elements.GetInteger(Keys.Ff);
                switch (ft)
                {
                    case "/Btn":
                        if ((flags & PDFAcroFieldFlags.Pushbutton) != 0)
                            return new PDFPushButtonField(dict);

                        if ((flags & PDFAcroFieldFlags.Radio) != 0)
                            return new PDFRadioButtonField(dict);

                        return new PDFCheckBoxField(dict);

                    case "/Tx":
                        return new PDFTextField(dict);

                    case "/Ch":
                        return (flags & PDFAcroFieldFlags.Combo) != 0 ? new PDFComboBoxField(dict) : (PDFAcroField)new PDFListBoxField(dict);

                    case "/Sig":
                        return new PDFSignatureField(dict);

                    default:
                        return new PDFGenericField(dict);
                }
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required for terminal fields; inheritable) The type of field that this dictionary
            /// describes:
            ///   Btn           Button
            ///   Tx            Text
            ///   Ch            Choice
            ///   Sig (PDF 1.3) Signature
            /// Note: This entry may be present in a nonterminal field (one whose descendants
            /// are themselves fields) in order to provide an inheritable FT value. However, a
            /// nonterminal field does not logically have a type of its own; it is merely a container
            /// for inheritable attributes that are intended for descendant terminal fields of
            /// any type.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string FT = "/FT";

            /// <summary>
            /// (Required if this field is the child of another in the field hierarchy; absent otherwise)
            /// The field that is the immediate parent of this one (the field, if any, whose Kids array
            /// includes this field). A field can have at most one parent; that is, it can be included
            /// in the Kids array of at most one other field.
            /// </summary>
            [KeyInfo(KeyType.Dictionary)]
            public const string Parent = "/Parent";

            /// <summary>
            /// (Optional) An array of indirect references to the immediate children of this field.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional, typeof(PDFAcroFieldCollection))]
            public const string Kids = "/Kids";

            /// <summary>
            /// (Optional) The partial field name.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string T = "/T";

            /// <summary>
            /// (Optional; PDF 1.3) An alternate field name, to be used in place of the actual
            /// field name wherever the field must be identified in the user interface (such as
            /// in error or status messages referring to the field). This text is also useful
            /// when extracting the document�s contents in support of accessibility to disabled
            /// users or for other purposes.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string TU = "/TU";

            /// <summary>
            /// (Optional; PDF 1.3) The mapping name to be used when exporting interactive form field 
            /// data from the document.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string TM = "/TM";

            /// <summary>
            /// (Optional; inheritable) A set of flags specifying various characteristics of the field.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Ff = "/Ff";

            /// <summary>
            /// (Optional; inheritable) The field�s value, whose format varies depending on
            /// the field type; see the descriptions of individual field types for further information.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string V = "/V";

            /// <summary>
            /// (Optional; inheritable) The default value to which the field reverts when a
            /// reset-form action is executed. The format of this value is the same as that of V.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string DV = "/DV";

            /// <summary>
            /// (Optional; PDF 1.2) An additional-actions dictionary defining the field�s behavior
            /// in response to various trigger events. This entry has exactly the same meaning as
            /// the AA entry in an annotation dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string AA = "/AA";

            // ----- Additional entries to all fields containing variable text --------------------------

            /// <summary>
            /// (Required; inheritable) A resource dictionary containing default resources
            /// (such as fonts, patterns, or color spaces) to be used by the appearance stream.
            /// At a minimum, this dictionary must contain a Font entry specifying the resource
            /// name and font dictionary of the default font for displaying the field�s text.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string DR = "/DR";

            /// <summary>
            /// (Required; inheritable) The default appearance string, containing a sequence of
            /// valid page-content graphics or text state operators defining such properties as
            /// the field�s text size and color.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Required)]
            public const string DA = "/DA";

            /// <summary>
            /// (Optional; inheritable) A code specifying the form of quadding (justification)
            /// to be used in displaying the text:
            ///   0 Left-justified
            ///   1 Centered
            ///   2 Right-justified
            /// Default value: 0 (left-justified).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Q = "/Q";

            // ReSharper restore InconsistentNaming
        }
    }
}
