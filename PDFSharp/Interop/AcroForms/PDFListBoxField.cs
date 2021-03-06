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

namespace PDFSharp.Interop.AcroForms
{
    /// <summary>
    /// Represents the list box field.
    /// </summary>
    public sealed class PDFListBoxField : PDFChoiceField
    {
        /// <summary>
        /// Initializes a new instance of PDFListBoxField.
        /// </summary>
        internal PDFListBoxField(PDFDocument document)
            : base(document)
        { }

        internal PDFListBoxField(PDFDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the index of the selected item
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                string value = Elements.GetString(PDFAcroField.Keys.V);
                return IndexInOptArray(value);
            }
            set
            {
                string key = ValueInOptArray(value);
                Elements.SetString(PDFAcroField.Keys.V, key);
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public new class Keys : PDFAcroField.Keys
        {
            // List boxes have no additional entries.

            internal static DictionaryMeta Meta => _meta ?? (_meta = CreateMeta(typeof(Keys)));
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta => Keys.Meta;
    }
}
