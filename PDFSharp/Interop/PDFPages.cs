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
using PDFSharp.Interop.IO;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.Annotations;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents the pages of the document.
    /// </summary>
    [DebuggerDisplay("(PageCount={Count})")]
    public sealed class PDFPages : PDFDictionary, IEnumerable<PDFPage>
    {
        internal PDFPages(PDFDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Pages");
            Elements[Keys.Count] = new PDFInteger(0);
        }

        internal PDFPages(PDFDictionary dictionary)
            : base(dictionary)
        { }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        public int Count => PagesArray.Elements.Count;

        /// <summary>
        /// Gets the page with the specified index.
        /// </summary>
        public PDFPage this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.PageIndexOutOfRange);

                PDFDictionary dict = (PDFDictionary)((PDFReference)PagesArray.Elements[index]).Value;
                if (!(dict is PDFPage))
                    dict = new PDFPage(dict);
                return (PDFPage)dict;
            }
        }

        /// <summary>
        /// Finds a page by its id. Transforms it to PDFPage if necessary.
        /// </summary>
        internal PDFPage FindPage(PDFObjectID id)  // TODO: public?
        {
            PDFPage page = null;
            foreach (PDFItem item in PagesArray)
            {
                if (item is PDFReference reference)
                {
                    if (reference.Value is PDFDictionary dictionary && dictionary.ObjectID == id)
                    {
                        page = dictionary as PDFPage ?? new PDFPage(dictionary);
                        break;
                    }
                }
            }
            return page;
        }

        /// <summary>
        /// Creates a new PDFPage, adds it to the end of this document, and returns it.
        /// </summary>
        public PDFPage Add()
        {
            PDFPage page = new PDFPage();
            Insert(Count, page);
            return page;
        }

        /// <summary>
        /// Adds the specified PDFPage to the end of this document and maybe returns a new PDFPage object.
        /// The value returned is a new object if the added page comes from a foreign document.
        /// </summary>
        public PDFPage Add(PDFPage page) => Insert(Count, page);

        /// <summary>
        /// Creates a new PDFPage, inserts it at the specified position into this document, and returns it.
        /// </summary>
        public PDFPage Insert(int index)
        {
            PDFPage page = new PDFPage();
            Insert(index, page);
            return page;
        }

        /// <summary>
        /// Inserts the specified PDFPage at the specified position to this document and maybe returns a new PDFPage object.
        /// The value returned is a new object if the inserted page comes from a foreign document.
        /// </summary>
        public PDFPage Insert(int index, PDFPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            // Is the page already owned by this document?
            if (page.Owner == Owner)
            {
                // Case: Page is first removed and than inserted again, maybe at another position.
                int count = Count;
                // Check if page is not already part of the document.
                for (int idx = 0; idx < count; idx++)
                {
                    if (ReferenceEquals(this[idx], page))
                        throw new InvalidOperationException(PSSR.MultiplePageInsert);
                }

                // TODO: check this case
                // Because the owner of the inserted page is this document we assume that the page was former part of it 
                // and it is therefore well-defined.
                Owner._irefTable.Add(page);
                Debug.Assert(page.Owner == Owner);

                // Insert page in array.
                PagesArray.Elements.Insert(index, page.Reference);

                // Update page count.
                Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);

                return page;
            }

            // All new page insertions come here.
            if (page.Owner == null)
            {
                // Case: New page was newly created and inserted now.
                page.Document = Owner;

                Owner._irefTable.Add(page);
                Debug.Assert(page.Owner == Owner);
                PagesArray.Elements.Insert(index, page.Reference);
                Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);
            }
            else
            {
                // Case: Page is from an external document -> import it.
                PDFPage importPage = page;
                page = ImportExternalPage(importPage);
                Owner._irefTable.Add(page);

                // Add page substitute to importedObjectTable.
                PDFImportedObjectTable importedObjectTable = Owner.FormTable.GetImportedObjectTable(importPage);
                importedObjectTable.Add(importPage.ObjectID, page.Reference);

                PagesArray.Elements.Insert(index, page.Reference);
                Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);
                PDFAnnotations.FixImportedAnnotation(page);
            }
            if (Owner.Settings.TrimMargins.AreSet)
                page.TrimMargins = Owner.Settings.TrimMargins;
            
            return page;
        }

        /// <summary>
        /// Inserts  pages of the specified document into this document.
        /// </summary>
        /// <param name="index">The index in this document where to insert the page .</param>
        /// <param name="document">The document to be inserted.</param>
        /// <param name="startIndex">The index of the first page to be inserted.</param>
        /// <param name="pageCount">The number of pages to be inserted.</param>
        public void InsertRange(int index, PDFDocument document, int startIndex, int pageCount)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index", "Argument 'index' out of range.");

            int importDocumentPageCount = document.PageCount;

            if (startIndex < 0 || startIndex + pageCount > importDocumentPageCount)
                throw new ArgumentOutOfRangeException("startIndex", "Argument 'startIndex' out of range.");

            if (pageCount > importDocumentPageCount)
                throw new ArgumentOutOfRangeException("pageCount", "Argument 'pageCount' out of range.");

            PDFPage[] insertPages = new PDFPage[pageCount];
            PDFPage[] importPages = new PDFPage[pageCount];

            // 1st create all new pages.
            for (int idx = 0, insertIndex = index, importIndex = startIndex;
                importIndex < startIndex + pageCount;
                idx++, insertIndex++, importIndex++)
            {
                PDFPage importPage = document.Pages[importIndex];
                PDFPage page = ImportExternalPage(importPage);
                insertPages[idx] = page;
                importPages[idx] = importPage;

                Owner._irefTable.Add(page);

                // Add page substitute to importedObjectTable.
                PDFImportedObjectTable importedObjectTable = Owner.FormTable.GetImportedObjectTable(importPage);
                importedObjectTable.Add(importPage.ObjectID, page.Reference);

                PagesArray.Elements.Insert(insertIndex, page.Reference);

                if (Owner.Settings.TrimMargins.AreSet)
                    page.TrimMargins = Owner.Settings.TrimMargins;
            }
            Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);

            // 2nd copy link annotations that are in the range of the imported pages.
            for (int idx = 0, importIndex = startIndex;
                importIndex < startIndex + pageCount;
                idx++, importIndex++)
            {
                PDFPage importPage = document.Pages[importIndex];
                PDFPage page = insertPages[idx];

                // Get annotations.
                PDFArray annots = importPage.Elements.GetArray(PDFPage.Keys.Annots);
                if (annots != null)
                {
                    PDFAnnotations annotations = new PDFAnnotations(Owner);

                    // Loop through annotations.
                    int count = annots.Elements.Count;
                    for (int idxAnnotation = 0; idxAnnotation < count; idxAnnotation++)
                    {
                        PDFDictionary annot = annots.Elements.GetDictionary(idxAnnotation);
                        if (annot != null)
                        {
                            string subtype = annot.Elements.GetString(PDFAnnotation.Keys.Subtype);
                            if (subtype == "/Link")
                            {
                                bool addAnnotation = false;
                                PDFLinkAnnotation newAnnotation = new PDFLinkAnnotation(Owner);

                                PDFName[] importAnnotationKeyNames = annot.Elements.KeyNames;
                                foreach (PDFName pdfItem in importAnnotationKeyNames)
                                {
                                    PDFItem impItem;
                                    switch (pdfItem.Value)
                                    {
                                        case "/BS":
                                            newAnnotation.Elements.Add("/BS", new PDFLiteral("<</W 0>>"));
                                            break;

                                        case "/F":  // /F 4
                                            impItem = annot.Elements.GetValue("/F");
                                            Debug.Assert(impItem is PDFInteger);
                                            newAnnotation.Elements.Add("/F", impItem.Clone());
                                            break;

                                        case "/Rect":  // /Rect [68.6 681.08 145.71 702.53]
                                            impItem = annot.Elements.GetValue("/Rect");
                                            Debug.Assert(impItem is PDFArray);
                                            newAnnotation.Elements.Add("/Rect", impItem.Clone());
                                            break;

                                        case "/StructParent":  // /StructParent 3
                                            impItem = annot.Elements.GetValue("/StructParent");
                                            Debug.Assert(impItem is PDFInteger);
                                            newAnnotation.Elements.Add("/StructParent", impItem.Clone());
                                            break;

                                        case "/Subtype":  // Already set.
                                            break;

                                        case "/Dest":  // /Dest [30 0 R /XYZ 68 771 0]
                                            impItem = annot.Elements.GetValue("/Dest");
                                            impItem = impItem.Clone();

                                            // Is value an array with 5 elements where the first one is an iref?
                                            PDFArray destArray = impItem as PDFArray;
                                            if (destArray != null && destArray.Elements.Count == 5)
                                            {
                                                if (destArray.Elements[0] is PDFReference iref)
                                                {
                                                    iref = RemapReference(insertPages, importPages, iref);
                                                    if (iref != null)
                                                    {
                                                        destArray.Elements[0] = iref;
                                                        newAnnotation.Elements.Add("/Dest", destArray);
                                                        addAnnotation = true;
                                                    }
                                                }
                                            }
                                            break;

                                        default:
#if DEBUG_
                                            Debug-Break.Break(true);
#endif
                                            break;

                                    }
                                }
                                // Add newAnnotations only it points to an imported page.
                                if (addAnnotation)
                                    annotations.Add(newAnnotation);
                            }
                        }
                    }

                    // At least one link annotation found?
                    if (annotations.Count > 0)
                    {
                        //Owner._irefTable.Add(annotations);
                        page.Elements.Add(PDFPage.Keys.Annots, annotations);
                    }
                }

            }
        }

        /// <summary>
        /// Inserts all pages of the specified document into this document.
        /// </summary>
        /// <param name="index">The index in this document where to insert the page .</param>
        /// <param name="document">The document to be inserted.</param>
        public void InsertRange(int index, PDFDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            InsertRange(index, document, 0, document.PageCount);
        }

        /// <summary>
        /// Inserts all pages of the specified document into this document.
        /// </summary>
        /// <param name="index">The index in this document where to insert the page .</param>
        /// <param name="document">The document to be inserted.</param>
        /// <param name="startIndex">The index of the first page to be inserted.</param>
        public void InsertRange(int index, PDFDocument document, int startIndex)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            InsertRange(index, document, startIndex, document.PageCount - startIndex);
        }

        /// <summary>
        /// Removes the specified page from the document.
        /// </summary>
        public void Remove(PDFPage page)
        {
            PagesArray.Elements.Remove(page.Reference);
            Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);
        }

        /// <summary>
        /// Removes the specified page from the document.
        /// </summary>
        public void RemoveAt(int index)
        {
            PagesArray.Elements.RemoveAt(index);
            Elements.SetInteger(Keys.Count, PagesArray.Elements.Count);
        }

        /// <summary>
        /// Moves a page within the page sequence.
        /// </summary>
        /// <param name="oldIndex">The page index before this operation.</param>
        /// <param name="newIndex">The page index after this operation.</param>
        public void MovePage(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= Count)
                throw new ArgumentOutOfRangeException("oldIndex");
            if (newIndex < 0 || newIndex >= Count)
                throw new ArgumentOutOfRangeException("newIndex");
            if (oldIndex == newIndex)
                return;

            //PDFPage page = (PDFPage)pagesArray.Elements[oldIndex];
            PDFReference page = (PDFReference)_pagesArray.Elements[oldIndex];
            _pagesArray.Elements.RemoveAt(oldIndex);
            _pagesArray.Elements.Insert(newIndex, page);
        }

        /// <summary>
        /// Imports an external page. The elements of the imported page are cloned and added to this document.
        /// Important: In contrast to PDFFormXObject adding an external page always make a deep copy
        /// of their transitive closure. Any reuse of already imported objects is not intended because
        /// any modification of an imported page must not change another page.
        /// </summary>
        PDFPage ImportExternalPage(PDFPage importPage)
        {
            if (importPage.Owner._openMode != PDFDocumentOpenMode.Import)
                throw new InvalidOperationException("A PDF document must be opened with PDFDocumentOpenMode.Import to import pages from it.");

            PDFPage page = new PDFPage(_document);

            // ReSharper disable AccessToStaticMemberViaDerivedType for a better code readability.
            CloneElement(page, importPage, PDFPage.InheritablePageKeys.Resources, false);
            CloneElement(page, importPage, PDFPage.Keys.Contents, false);
            CloneElement(page, importPage, PDFPage.InheritablePageKeys.MediaBox, true);
            CloneElement(page, importPage, PDFPage.InheritablePageKeys.CropBox, true);
            CloneElement(page, importPage, PDFPage.InheritablePageKeys.Rotate, true);
            CloneElement(page, importPage, PDFPage.Keys.BleedBox, true);
            CloneElement(page, importPage, PDFPage.Keys.TrimBox, true);
            CloneElement(page, importPage, PDFPage.Keys.ArtBox, true);
#if true
            // Do not deep copy annotations.
            CloneElement(page, importPage, PDFPage.Keys.Annots, false);
#else
            // Deep copy annotations.
            CloneElement(page, importPage, PDFPage.Keys.Annots, true);
#endif
            // ReSharper restore AccessToStaticMemberViaDerivedType
            // TODO more elements?
            return page;
        }

        /// <summary>
        /// Helper function for ImportExternalPage.
        /// </summary>
        void CloneElement(PDFPage page, PDFPage importPage, string key, bool deepcopy)
        {
            Debug.Assert(page != null);
            Debug.Assert(page.Owner == _document);
            Debug.Assert(importPage.Owner != null);
            Debug.Assert(importPage.Owner != _document);

            PDFItem item = importPage.Elements[key];
            if (item != null)
            {
                PDFImportedObjectTable importedObjectTable = null;
                if (!deepcopy)
                    importedObjectTable = Owner.FormTable.GetImportedObjectTable(importPage);

                // The item can be indirect. If so, replace it by its value.
                if (item is PDFReference)
                    item = ((PDFReference)item).Value;
                if (item is PDFObject root)
                {
                    if (deepcopy)
                    {
                        Debug.Assert(root.Owner != null, "See 'else' case for details");
                        root = DeepCopyClosure(_document, root);
                    }
                    else
                    {
                        // The owner can be null if the item is not a reference.
                        if (root.Owner == null)
                            root.Document = importPage.Owner;
                        root = ImportClosure(importedObjectTable, page.Owner, root);
                    }

                    page.Elements[key] = root.Reference == null ? root : (PDFItem)root.Reference;
                }
                else
                {
                    // Simple items are just cloned.
                    page.Elements[key] = item.Clone();
                }
            }
        }

        static PDFReference RemapReference(PDFPage[] newPages, PDFPage[] impPages, PDFReference iref)
        {
            // Directs the iref to a one of the imported pages?
            for (int idx = 0; idx < newPages.Length; idx++)
            {
                if (impPages[idx].Reference == iref)
                    return newPages[idx].Reference;
            }
            return null;
        }

        /// <summary>
        /// Gets a PDFArray containing all pages of this document. The array must not be modified.
        /// </summary>
        public PDFArray PagesArray
        {
            get
            {
                if (_pagesArray == null)
                    _pagesArray = (PDFArray)Elements.GetValue(Keys.Kids, VCF.Create);
                return _pagesArray;
            }
        }
        PDFArray _pagesArray;

        /// <summary>
        /// Replaces the page tree by a flat array of indirect references to the pages objects.
        /// </summary>
        internal void FlattenPageTree()
        {
            // Acrobat creates a balanced tree if the number of pages is roughly more than ten. This is
            // not difficult but obviously also not necessary. I created a document with 50000 pages with
            // PDF4NET and Acrobat opened it in less than 2 seconds.

            //PDFReference xrefRoot = Document.Catalog.Elements[PDFCatalog.Keys.Pages] as PDFReference;
            //PDFDictionary[] pages = GetKids(xrefRoot, null);

            // Promote inheritable values down the page tree
            PDFPage.InheritedValues values = new PDFPage.InheritedValues();
            PDFPage.InheritValues(this, ref values);
            PDFDictionary[] pages = GetKids(Reference, values, null);

            // Replace /Pages in catalog by this object
            // xrefRoot.Value = this;

            PDFArray array = new PDFArray(Owner);
            foreach (PDFDictionary page in pages)
            {
                // Fix the parent
                page.Elements[PDFPage.Keys.Parent] = Reference;
                array.Elements.Add(page.Reference);
            }

            Elements.SetName(Keys.Type, "/Pages");
#if true
            // Direct array.
            Elements.SetValue(Keys.Kids, array);
#else
            // Indirect array.
            Document.xrefTable.Add(array);
            Elements.SetValue(Keys.Kids, array.XRef);
#endif
            Elements.SetInteger(Keys.Count, array.Elements.Count);
        }

        /// <summary>
        /// Recursively converts the page tree into a flat array.
        /// </summary>
        PDFDictionary[] GetKids(PDFReference iref, PDFPage.InheritedValues values, PDFDictionary parent)
        {
            // TODO: inherit inheritable keys...
            PDFDictionary kid = (PDFDictionary)iref.Value;

#if true
            string type = kid.Elements.GetName(Keys.Type);
            if (type == "/Page")
            {
                PDFPage.InheritValues(kid, values);
                return new PDFDictionary[] { kid };
            }

            if (String.IsNullOrEmpty(type))
            {
                // Type is required. If type is missing, assume it is "/Page" and hope it will work.
                // TODO Implement a "Strict" mode in PDFsharp and don't do this in "Strict" mode.
                PDFPage.InheritValues(kid, values);
                return new PDFDictionary[] { kid };
            }

#else
            if (kid.Elements.GetName(Keys.Type) == "/Page")
            {
                PDFPage.InheritValues(kid, values);
                return new PDFDictionary[] { kid };
            }
#endif

            Debug.Assert(kid.Elements.GetName(Keys.Type) == "/Pages");
            PDFPage.InheritValues(kid, ref values);
            List<PDFDictionary> list = new List<PDFDictionary>();
            PDFArray kids = kid.Elements["/Kids"] as PDFArray;

            if (kids == null)
            {
                if (kid.Elements["/Kids"] is PDFReference xref3)
                    kids = xref3.Value as PDFArray;
            }

            foreach (PDFReference xref2 in kids)
                list.AddRange(GetKids(xref2, values, kid));
            int count = list.Count;
            Debug.Assert(count == kid.Elements.GetInteger("/Count"));
            return list.ToArray();
        }

        /// <summary>
        /// Prepares the document for saving.
        /// </summary>
        internal override void PrepareForSave()
        {
            // TODO: Close all open content streams

            // TODO: Create the page tree.
            // Arrays have a limit of 8192 entries, but I successfully tested documents
            // with 50000 pages and no page tree.
            // ==> wait for bug report.
            int count = _pagesArray.Elements.Count;
            for (int idx = 0; idx < count; idx++)
            {
                PDFPage page = this[idx];
                page.PrepareForSave();
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public new IEnumerator<PDFPage> GetEnumerator() => new PDFPagesEnumerator(this);

        class PDFPagesEnumerator : IEnumerator<PDFPage>
        {
            internal PDFPagesEnumerator(PDFPages list)
            {
                _list = list;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_index < _list.Count - 1)
                {
                    _index++;
                    _currentElement = _list[_index];
                    return true;
                }
                _index = _list.Count;
                return false;
            }

            public void Reset()
            {
                _currentElement = null;
                _index = -1;
            }

            object IEnumerator.Current => Current;

            public PDFPage Current
            {
                get
                {
                    if (_index == -1 || _index >= _list.Count)
                        throw new InvalidOperationException(PSSR.ListEnumCurrentOutOfRange);
                    return _currentElement;
                }
            }

            public void Dispose()
            {
                // Nothing to do.
            }

            PDFPage _currentElement;
            int _index;
            readonly PDFPages _list;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : PDFPage.InheritablePageKeys
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes; 
            /// must be Pages for a page tree node.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "Pages")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required except in root node; must be an indirect reference)
            /// The page tree node that is the immediate parent of this one.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Parent = "/Parent";

            /// <summary>
            /// (Required) An array of indirect references to the immediate children of this node.
            /// The children may be page objects or other page tree nodes.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Required)]
            public const string Kids = "/Kids";

            /// <summary>
            /// (Required) The number of leaf nodes (page objects) that are descendants of this node 
            /// within the page tree.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Count = "/Count";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta => _meta ?? (_meta = CreateMeta(typeof(Keys)));
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta => Keys.Meta;
    }
}
