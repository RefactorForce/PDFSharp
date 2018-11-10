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

namespace PDFSharp.Interop
{
    /// <summary>
    /// This class is intended for empira internal use only and may change or drop in future releases.
    /// </summary>
    public class PDFCustomValues : PDFDictionary
    {
        internal PDFCustomValues()
        { }

        internal PDFCustomValues(PDFDocument document)
            : base(document)
        { }

        internal PDFCustomValues(PDFDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public PDFCustomValueCompressionMode CompressionMode
        {
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public bool Contains(string key) => Elements.ContainsKey(key);

        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public PDFCustomValue this[string key]
        {
            get
            {
                PDFDictionary dict = Elements.GetDictionary(key);
                if (dict == null)
                    return null;
                if (!(dict is PDFCustomValue cust))
                    cust = new PDFCustomValue(dict);
                return cust;
            }
            set
            {
                if (value == null)
                {
                    Elements.Remove(key);
                }
                else
                {
                    Owner.Internals.AddObject(value);
                    Elements.SetReference(key, value);
                }
            }
#if old
            get
            {
                PDFDictionary dict = Elements.GetDictionary(key);
                if (dict == null)
                    return null;
                if (!(dict is PDFCustomValue))
                    dict = new PDFCustomValue(dict);
                return dict.Stream.Value;
            }
            set
            {
                PDFCustomValue cust;
                PDFDictionary dict = Elements.GetDictionary(key);
                if (dict == null)
                {
                    cust = new PDFCustomValue();
                    Owner.Internals.AddObject(cust);
                    Elements.Add(key, cust);
                }
                else
                {
                    cust = dict as PDFCustomValue;
                    if (cust == null)
                        cust = new PDFCustomValue(dict);
                }
                cust.Value = value;
            }
#endif
        }

        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public static void ClearAllCustomValues(PDFDocument document)
        {
            document.CustomValues = null;
            foreach (PDFPage page in document.Pages)
                page.CustomValues = null;
        }

        //public static string Key = "/PDFSharp.CustomValue";

        internal static PDFCustomValues Get(DictionaryElements elem)
        {
            string key = elem.Owner.Owner.Internals.CustomValueKey;
            PDFCustomValues customValues;
            PDFDictionary dict = elem.GetDictionary(key);
            if (dict == null)
            {
                customValues = new PDFCustomValues();
                elem.Owner.Owner.Internals.AddObject(customValues);
                elem.Add(key, customValues);
            }
            else
            {
                customValues = dict as PDFCustomValues;
                if (customValues == null)
                    customValues = new PDFCustomValues(dict);
            }
            return customValues;
        }

        internal static void Remove(DictionaryElements elem) => elem.Remove(elem.Owner.Owner.Internals.CustomValueKey);
    }
}
