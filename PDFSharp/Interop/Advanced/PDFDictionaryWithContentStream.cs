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
#if GDI
using System.DrawingCore;
using System.DrawingCore.Imaging;
#endif
#if WPF
using System.Windows.Media;
#endif
using PDFSharp.Drawing;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents a base class for dictionaries with a content stream.
    /// Implement IContentStream for use with a content writer.
    /// </summary>
    public abstract class PDFDictionaryWithContentStream : PDFDictionary, IContentStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDictionaryWithContentStream"/> class.
        /// </summary>
        public PDFDictionaryWithContentStream()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFDictionaryWithContentStream"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PDFDictionaryWithContentStream(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance from an existing dictionary. Used for object type transformation.
        /// </summary>
        protected PDFDictionaryWithContentStream(PDFDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets the resources dictionary of this dictionary. If no such dictionary exists, it is created.
        /// </summary>
        internal PDFResources Resources
        {
            get
            {
                if (_resources == null)
                    _resources = (PDFResources)Elements.GetValue(Keys.Resources, VCF.Create);
                return _resources;
            }
        }
        PDFResources _resources;

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        PDFResources IContentStream.Resources => Resources;

        internal string GetFontName(XFont font, out PDFFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(font);
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(XFont font, out PDFFont pdfFont) => GetFontName(font, out pdfFont);

        internal string GetFontName(string idName, byte[] fontData, out PDFFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(idName, fontData);
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(string idName, byte[] fontData, out PDFFont pdfFont) => GetFontName(idName, fontData, out pdfFont);

        /// <summary>
        /// Gets the resource name of the specified image within this dictionary.
        /// </summary>
        internal string GetImageName(XImage image)
        {
            PDFImage pdfImage = _document.ImageTable.GetImage(image);
            Debug.Assert(pdfImage != null);
            string name = Resources.AddImage(pdfImage);
            return name;
        }

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        string IContentStream.GetImageName(XImage image) => throw new NotImplementedException();

        /// <summary>
        /// Gets the resource name of the specified form within this dictionary.
        /// </summary>
        internal string GetFormName(XForm form)
        {
            PDFFormXObject pdfForm = _document.ExternalDocumentTable.GetForm(form);
            Debug.Assert(pdfForm != null);
            string name = Resources.AddForm(pdfForm);
            return name;
        }

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        string IContentStream.GetFormName(XForm form) => throw new NotImplementedException();

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : PDFStream.Keys
        {
            /// <summary>
            /// (Optional but strongly recommended; PDF 1.2) A dictionary specifying any
            /// resources (such as fonts and images) required by the form XObject.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResources))]
            public const string Resources = "/Resources";
        }
    }
}
