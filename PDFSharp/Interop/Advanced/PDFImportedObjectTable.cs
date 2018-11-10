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

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents the imported objects of an external document. Used to cache objects that are
    /// already imported when a PDFFormXObject is added to a page.
    /// </summary>
    internal sealed class PDFImportedObjectTable
    {
        /// <summary>
        /// Initializes a new instance of this class with the document the objects are imported from.
        /// </summary>
        public PDFImportedObjectTable(PDFDocument owner, PDFDocument externalDocument)
        {
            if (externalDocument == null)
                throw new ArgumentNullException("externalDocument");
            Owner = owner ?? throw new ArgumentNullException("owner");
            _externalDocumentHandle = externalDocument.Handle;
            _xObjects = new PDFFormXObject[externalDocument.PageCount];
        }
        readonly PDFFormXObject[] _xObjects;

        /// <summary>
        /// Gets the document this table belongs to.
        /// </summary>
        public PDFDocument Owner { get; }

        /// <summary>
        /// Gets the external document, or null, if the external document is garbage collected.
        /// </summary>
        public PDFDocument ExternalDocument => _externalDocumentHandle.IsAlive ? _externalDocumentHandle.Target : null;
        readonly PDFDocument.DocumentHandle _externalDocumentHandle;

        public PDFFormXObject GetXObject(int pageNumber) => _xObjects[pageNumber - 1];

        public void SetXObject(int pageNumber, PDFFormXObject xObject) => _xObjects[pageNumber - 1] = xObject;

        /// <summary>
        /// Indicates whether the specified object is already imported.
        /// </summary>
        public bool Contains(PDFObjectID externalID) => _externalIDs.ContainsKey(externalID.ToString());

        /// <summary>
        /// Adds a cloned object to this table.
        /// </summary>
        /// <param name="externalID">The object identifier in the foreign object.</param>
        /// <param name="iref">The cross reference to the clone of the foreign object, which belongs to
        /// this document. In general the clone has a different object identifier.</param>
        public void Add(PDFObjectID externalID, PDFReference iref) => _externalIDs[externalID.ToString()] = iref;

        /// <summary>
        /// Gets the cloned object that corresponds to the specified external identifier.
        /// </summary>
        public PDFReference this[PDFObjectID externalID] => _externalIDs[externalID.ToString()];

        /// <summary>
        /// Maps external object identifiers to cross reference entries of the importing document
        /// {PDFObjectID -> PDFReference}.
        /// </summary>
        readonly Dictionary<string, PDFReference> _externalIDs = new Dictionary<string, PDFReference>();
    }
}
