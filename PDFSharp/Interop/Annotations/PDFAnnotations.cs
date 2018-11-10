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
using System.Text;
using System.IO;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.IO;
using System.Collections.Generic;

namespace PDFSharp.Interop.Annotations
{
    /// <summary>
    /// Represents the annotations array of a page.
    /// </summary>
    public sealed class PDFAnnotations : PDFArray
    {
        internal PDFAnnotations(PDFDocument document)
            : base(document)
        { }

        internal PDFAnnotations(PDFArray array)
            : base(array)
        { }

        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Add(PDFAnnotation annotation)
        {
            annotation.Document = Owner;
            Owner.IrefTable.Add(annotation);
            Elements.Add(annotation.Reference);
        }

        /// <summary>
        /// Removes an annotation from the document.
        /// </summary>
        public void Remove(PDFAnnotation annotation)
        {
            if (annotation.Owner != Owner)
                throw new InvalidOperationException("The annotation does not belong to this document.");

            Owner.Internals.RemoveObject(annotation);
            Elements.Remove(annotation.Reference);
        }

        /// <summary>
        /// Removes all the annotations from the current page.
        /// </summary>
        public void Clear()
        {
            for (int idx = Count - 1; idx >= 0; idx--)
                Page.Annotations.Remove(Page.Annotations[idx]);
        }

        //public void Insert(int index, PDFAnnotation annotation)
        //{
        //  annotation.Document = Document;
        //  annotations.Insert(index, annotation);
        //}

        /// <summary>
        /// Gets the number of annotations in this collection.
        /// </summary>
        public int Count => Elements.Count;

        /// <summary>
        /// Gets the <see cref="PDFAnnotation"/> at the specified index.
        /// </summary>
        public PDFAnnotation this[int index]
        {
            get
            {
                PDFReference iref;
                PDFDictionary dict;
                PDFItem item = Elements[index];
                if ((iref = item as PDFReference) != null)
                {
                    Debug.Assert(iref.Value is PDFDictionary, "Reference to dictionary expected.");
                    dict = (PDFDictionary)iref.Value;
                }
                else
                {
                    Debug.Assert(item is PDFDictionary, "Dictionary expected.");
                    dict = (PDFDictionary)item;
                }
                if (!(dict is PDFAnnotation annotation))
                {
                    annotation = new PDFGenericAnnotation(dict);
                    if (iref == null)
                        Elements[index] = annotation;
                }
                return annotation;
            }
        }

        //public PDFAnnotation this[int index]
        //{
        //  get 
        //  {
        //      //DMH 6/7/06
        //      //Broke this out to simplfy debugging
        //      //Use a generic annotation to access the Meta data
        //      //Assign this as the parent of the annotation
        //      PDFReference r = Elements[index] as PDFReference;
        //      PDFDictionary d = r.Value as PDFDictionary;
        //      PDFGenericAnnotation a = new PDFGenericAnnotation(d);
        //      a.Collection = this;
        //      return a;
        //  }
        //}

        /// <summary>
        /// Gets the page the annotations belongs to.
        /// </summary>
        internal PDFPage Page { get; set; }

        /// <summary>
        /// Fixes the /P element in imported annotation.
        /// </summary>
        internal static void FixImportedAnnotation(PDFPage page)
        {
            PDFArray annots = page.Elements.GetArray(PDFPage.Keys.Annots);
            if (annots != null)
            {
                int count = annots.Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PDFDictionary annot = annots.Elements.GetDictionary(idx);
                    if (annot != null && annot.Elements.ContainsKey("/P"))
                        annot.Elements["/P"] = page.Reference;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        public override IEnumerator<PDFItem> GetEnumerator() => new AnnotationsIterator(this);
        // THHO4STLA: AnnotationsIterator: Implementation does not work http://forum.pdfsharp.net/viewtopic.php?p=3285#p3285
        // Code using the enumerator like this will crash:
        //foreach (var annotation in page.Annotations)
        //{
        //    annotation.GetType();
        //}

        //!!!new 2015-10-15: use PDFItem instead of PDFAnnotation. 
        // TODO Should we change this to "public new IEnumerator<PDFAnnotation> GetEnumerator()"?

        class AnnotationsIterator : IEnumerator<PDFItem/*PDFAnnotation*/>
        {
            public AnnotationsIterator(PDFAnnotations annotations)
            {
                _annotations = annotations;
                _index = -1;
            }

            public PDFItem/*PDFAnnotation*/ Current => _annotations[_index];

            object IEnumerator.Current => Current;

            public bool MoveNext() => ++_index < _annotations.Count;

            public void Reset() => _index = -1;

            public void Dispose()
            {
                //throw new NotImplementedException();
            }

            readonly PDFAnnotations _annotations;
            int _index;
        }
    }
}
