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
using System.Collections.Generic;
using System.Globalization;
using PDFSharp.Drawing;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Contains all external PDF files from which PDFFormXObjects are imported into the current document.
    /// </summary>
    internal sealed class PDFFormXObjectTable : PDFResourceTable
    {
        // The name PDFFormXObjectTable is technically not correct, because in contrast to PDFFontTable
        // or PDFImageTable this class holds no PDFFormXObject objects. Actually it holds instances of
        // the class ImportedObjectTable, one for each external document. The PDFFormXObject instances
        // are not cached, because they hold a transformation matrix that make them unique. If the user
        // wants to use a particual page of a PDFFormXObject more than once, he must reuse the object
        // before he changes the PageNumber or the transformation matrix. In other words this class
        // caches the indirect objects of an external form, not the form itself.

        /// <summary>
        /// Initializes a new instance of this class, which is a singleton for each document.
        /// </summary>
        public PDFFormXObjectTable(PDFDocument document)
            : base(document)
        { }

        /// <summary>
        /// Gets a PDFFormXObject from an XPDFForm. Because the returned objects must be unique, always
        /// a new instance of PDFFormXObject is created if none exists for the specified form. 
        /// </summary>
        public PDFFormXObject GetForm(XForm form)
        {
            // If the form already has a PDFFormXObject, return it.
            if (form._pdfForm != null)
            {
                Debug.Assert(form.IsTemplate, "An XPDFForm must not have a PDFFormXObject.");
                if (ReferenceEquals(form._pdfForm.Owner, Owner))
                    return form._pdfForm;
                //throw new InvalidOperationException("Because of a current limitation of PDFSharp an XPDFForm object can be used only within one single PDFDocument.");

                // Dispose PDFFromXObject when document has changed
                form._pdfForm = null;
            }

            if (form is XPDFForm pdfForm)
            {
                // Is the external PDF file from which is imported already known for the current document?
                Selector selector = new Selector(form);
                if (!_forms.TryGetValue(selector, out PDFImportedObjectTable importedObjectTable))
                {
                    // No: Get the external document from the form and create ImportedObjectTable.
                    PDFDocument doc = pdfForm.ExternalDocument;
                    importedObjectTable = new PDFImportedObjectTable(Owner, doc);
                    _forms[selector] = importedObjectTable;
                }

                PDFFormXObject xObject = importedObjectTable.GetXObject(pdfForm.PageNumber);
                if (xObject == null)
                {
                    xObject = new PDFFormXObject(Owner, importedObjectTable, pdfForm);
                    importedObjectTable.SetXObject(pdfForm.PageNumber, xObject);
                }
                return xObject;
            }
            Debug.Assert(form.GetType() == typeof(XForm));
            form._pdfForm = new PDFFormXObject(Owner, form);
            return form._pdfForm;
        }

        /// <summary>
        /// Gets the imported object table.
        /// </summary>
        public PDFImportedObjectTable GetImportedObjectTable(PDFPage page)
        {
            // Is the external PDF file from which is imported already known for the current document?
            Selector selector = new Selector(page);
            if (!_forms.TryGetValue(selector, out PDFImportedObjectTable importedObjectTable))
            {
                importedObjectTable = new PDFImportedObjectTable(Owner, page.Owner);
                _forms[selector] = importedObjectTable;
            }
            return importedObjectTable;
        }

        /// <summary>
        /// Gets the imported object table.
        /// </summary>
        public PDFImportedObjectTable GetImportedObjectTable(PDFDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // Is the external PDF file from which is imported already known for the current document?
            Selector selector = new Selector(document);
            if (!_forms.TryGetValue(selector, out PDFImportedObjectTable importedObjectTable))
            {
                // Create new table for document.
                importedObjectTable = new PDFImportedObjectTable(Owner, document);
                _forms[selector] = importedObjectTable;
            }
            return importedObjectTable;
        }

        public void DetachDocument(PDFDocument.DocumentHandle handle)
        {
            if (handle.IsAlive)
            {
                foreach (Selector selector in _forms.Keys)
                {
                    PDFImportedObjectTable table = _forms[selector];
                    if (table.ExternalDocument != null && table.ExternalDocument.Handle == handle)
                    {
                        _forms.Remove(selector);
                        break;
                    }
                }
            }

            // Clean table
            bool itemRemoved = true;
            while (itemRemoved)
            {
                itemRemoved = false;
                foreach (Selector selector in _forms.Keys)
                {
                    PDFImportedObjectTable table = _forms[selector];
                    if (table.ExternalDocument == null)
                    {
                        _forms.Remove(selector);
                        itemRemoved = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Map from Selector to PDFImportedObjectTable.
        /// </summary>
        readonly Dictionary<Selector, PDFImportedObjectTable> _forms = new Dictionary<Selector, PDFImportedObjectTable>();

        /// <summary>
        /// A collection of information that uniquely identifies a particular ImportedObjectTable.
        /// </summary>
        public class Selector
        {
            /// <summary>
            /// Initializes a new instance of FormSelector from an XPDFForm.
            /// </summary>
            public Selector(XForm form) =>
                // HACK: just use full path to identify
                Path = form._path.ToLowerInvariant();

            /// <summary>
            /// Initializes a new instance of FormSelector from a PDFPage.
            /// </summary>
            public Selector(PDFPage page)
            {
                PDFDocument owner = page.Owner;
                Path = "*" + owner.Guid.ToString("B");
                Path = Path.ToLowerInvariant();
            }

            public Selector(PDFDocument document)
            {
                Path = "*" + document.Guid.ToString("B");
                Path = Path.ToLowerInvariant();
            }

            public string Path { get; set; }

            public override bool Equals(object obj) => !(obj is Selector selector) ? false : Path == selector.Path;

            public override int GetHashCode() => Path.GetHashCode();
        }
    }
}
