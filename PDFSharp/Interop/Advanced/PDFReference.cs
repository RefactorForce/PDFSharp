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

// With this define each iref object gets a unique number (uid) to make them distinguishable in the debugger
#define UNIQUE_IREF_

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDFSharp.Interop.IO;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents an indirect reference to a PDFObject.
    /// </summary>
    [DebuggerDisplay("iref({ObjectNumber}, {GenerationNumber})")]
    public sealed class PDFReference : PDFItem
    {
        // About PDFReference 
        // 
        // * A PDFReference holds either the ObjectID or the PDFObject or both.
        // 
        // * Each PDFObject has a PDFReference if and only if it is an indirect object. Direct objects have
        //   no PDFReference, because they are embedded in a parent objects.
        //
        // * PDFReference objects are used to reference PDFObject instances. A value in a PDF dictionary
        //   or array that is a PDFReference represents an indirect reference. A value in a PDF dictionary or
        //   or array that is a PDFObject represents a direct (or embeddded) object.
        //
        // * When a PDF file is imported, the PDFXRefTable is filled with PDFReference objects keeping the
        //   ObjectsIDs and file positions (offsets) of all indirect objects.
        //
        // * Indirect objects can easily be renumbered because they do not rely on their ObjectsIDs.
        //
        // * During modification of a document the ObjectID of an indirect object has no meaning,
        //   except that they must be different in pairs.

        /// <summary>
        /// Initializes a new PDFReference instance for the specified indirect object.
        /// </summary>
        public PDFReference(PDFObject pdfObject)
        {
            if (pdfObject.Reference != null)
                throw new InvalidOperationException("Must not create iref for an object that already has one.");
            _value = pdfObject;
            pdfObject.Reference = this;
#if UNIQUE_IREF && DEBUG
            _uid = ++s_counter;
#endif
        }

        /// <summary>
        /// Initializes a new PDFReference instance from the specified object identifier and file position.
        /// </summary>
        public PDFReference(PDFObjectID objectID, int position)
        {
            _objectID = objectID;
            Position = position;
#if UNIQUE_IREF && DEBUG
            _uid = ++s_counter;
#endif
        }

        /// <summary>
        /// Writes the object in PDF iref table format.
        /// </summary>
        internal void WriteXRefEnty(PDFWriter writer)
        {
            // PDFSharp does not yet support PDF 1.5 object streams.

            // Each line must be exactly 20 bytes long, otherwise Acrobat repairs the file.
            string text = String.Format("{0:0000000000} {1:00000} n\n",
              Position, _objectID.GenerationNumber); // InUse ? 'n' : 'f');
            writer.WriteRaw(text);
        }

        /// <summary>
        /// Writes an indirect reference.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) => writer.Write(this);

        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        public PDFObjectID ObjectID
        {
            get => _objectID;
            set
            {
                // Ignore redundant invokations.
                if (_objectID == value)
                    return;

                _objectID = value;
                if (Document != null)
                {
                    //PDFXRefTable table = Document.xrefTable;
                    //table.Remove(this);
                    //objectID = value;
                    //table.Add(this);
                }
            }
        }
        PDFObjectID _objectID;

        /// <summary>
        /// Gets the object number of the object identifier.
        /// </summary>
        public int ObjectNumber => _objectID.ObjectNumber;

        /// <summary>
        /// Gets the generation number of the object identifier.
        /// </summary>
        public int GenerationNumber => _objectID.GenerationNumber;

        /// <summary>
        /// Gets or sets the file position of the related PDFObject.
        /// </summary>
        public int Position { get; set; }

        //public bool InUse
        //{
        //  get {return inUse;}
        //  set {inUse = value;}
        //}
        //bool inUse;

        /// <summary>
        /// Gets or sets the referenced PDFObject.
        /// </summary>
        public PDFObject Value
        {
            get => _value;
            set
            {
                Debug.Assert(value != null, "The value of a PDFReference must never be null.");
                Debug.Assert(value.Reference == null || ReferenceEquals(value.Reference, this), "The reference of the value must be null or this.");
                _value = value;
                // value must never be null
                value.Reference = this;
            }
        }
        PDFObject _value;

        /// <summary>
        /// Hack for dead objects.
        /// </summary>
        internal void SetObject(PDFObject value) => _value = value;

        /// <summary>
        /// Gets or sets the document this object belongs to.
        /// </summary>
        public PDFDocument Document { get; set; }

        /// <summary>
        /// Gets a string representing the object identifier.
        /// </summary>
        public override string ToString() => _objectID + " R";

        internal static PDFReferenceComparer Comparer => new PDFReferenceComparer();

        /// <summary>
        /// Implements a comparer that compares PDFReference objects by their PDFObjectID.
        /// </summary>
        internal class PDFReferenceComparer : IComparer<PDFReference>
        {
            public int Compare(PDFReference x, PDFReference y)
            {
                PDFReference l = x;
                PDFReference r = y;
                return l != null ? r != null ? l._objectID.CompareTo(r._objectID) : -1 : r != null ? 1 : 0;
            }
        }

#if UNIQUE_IREF && DEBUG
        static int s_counter = 0;
        int _uid;
#endif
    }
}
