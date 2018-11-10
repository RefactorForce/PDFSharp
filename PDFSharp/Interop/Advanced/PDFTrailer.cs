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
using PDFSharp.Interop.IO;
using PDFSharp.Interop.Security;
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents a PDF trailer dictionary. Even though trailers are dictionaries they never have a cross
    /// reference entry in PDFReferenceTable.
    /// </summary>
    internal class PDFTrailer : PDFDictionary  // Reference: 3.4.4  File Trailer / Page 96
    {
        /// <summary>
        /// Initializes a new instance of PDFTrailer.
        /// </summary>
        public PDFTrailer(PDFDocument document)
            : base(document) => _document = document;

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFTrailer"/> class from a <see cref="PDFCrossReferenceStream"/>.
        /// </summary>
        public PDFTrailer(PDFCrossReferenceStream trailer)
            : base(trailer._document)
        {
            _document = trailer._document;

            // /ID [<09F877EBF282E9408ED1882A9A21D9F2><2A4938E896006F499AC1C2EA7BFB08E4>]
            // /Info 7 0 R
            // /Root 1 0 R
            // /Size 10

            PDFReference iref = trailer.Elements.GetReference(Keys.Info);
            if (iref != null)
                Elements.SetReference(Keys.Info, iref);

            Elements.SetReference(Keys.Root, trailer.Elements.GetReference(Keys.Root));

            Elements.SetInteger(Keys.Size, trailer.Elements.GetInteger(Keys.Size));

            PDFArray id = trailer.Elements.GetArray(Keys.ID);
            if (id != null)
                Elements.SetValue(Keys.ID, id);
        }

        public int Size
        {
            get => Elements.GetInteger(Keys.Size);
            set => Elements.SetInteger(Keys.Size, value);
        }

        // TODO: needed when linearized...
        //public int Prev
        //{
        //  get {return Elements.GetInteger(Keys.Prev);}
        //}

        public PDFDocumentInformation Info => (PDFDocumentInformation)Elements.GetValue(Keys.Info, VCF.CreateIndirect);

        /// <summary>
        /// (Required; must be an indirect reference)
        /// The catalog dictionary for the PDF document contained in the file.
        /// </summary>
        public PDFCatalog Root => (PDFCatalog)Elements.GetValue(Keys.Root, VCF.CreateIndirect);

        /// <summary>
        /// Gets the first or second document identifier.
        /// </summary>
        public string GetDocumentID(int index)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");

            if (!(Elements[Keys.ID] is PDFArray array) || array.Elements.Count < 2)
                return "";
            PDFItem item = array.Elements[index];
            return item is PDFString ? ((PDFString)item).Value : "";
        }

        /// <summary>
        /// Sets the first or second document identifier.
        /// </summary>
        public void SetDocumentID(int index, string value)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");

            if (!(Elements[Keys.ID] is PDFArray array) || array.Elements.Count < 2)
                array = CreateNewDocumentIDs();
            array.Elements[index] = new PDFString(value, PDFStringFlags.HexLiteral);
        }

        /// <summary>
        /// Creates and sets two identical new document IDs.
        /// </summary>
        internal PDFArray CreateNewDocumentIDs()
        {
            PDFArray array = new PDFArray(_document);
            byte[] docID = Guid.NewGuid().ToByteArray();
            string id = PDFEncoders.RawEncoding.GetString(docID, 0, docID.Length);
            array.Elements.Add(new PDFString(id, PDFStringFlags.HexLiteral));
            array.Elements.Add(new PDFString(id, PDFStringFlags.HexLiteral));
            Elements[Keys.ID] = array;
            return array;
        }

        /// <summary>
        /// Gets the standard security handler.
        /// </summary>
        public PDFStandardSecurityHandler SecurityHandler
        {
            get
            {
                if (_securityHandler == null)
                    _securityHandler = (PDFStandardSecurityHandler)Elements.GetValue(Keys.Encrypt, VCF.CreateIndirect);
                return _securityHandler;
            }
        }
        internal PDFStandardSecurityHandler _securityHandler;

        internal override void WriteObject(PDFWriter writer)
        {
            // Delete /XRefStm entry, if any.
            // HACK: 
            _elements.Remove(Keys.XRefStm);

            // Don't encrypt myself
            PDFStandardSecurityHandler securityHandler = writer.SecurityHandler;
            writer.SecurityHandler = null;
            base.WriteObject(writer);
            writer.SecurityHandler = securityHandler;
        }

        /// <summary>
        /// Replace temporary irefs by their correct counterparts from the iref table.
        /// </summary>
        internal void Finish()
        {
            // /Root
            if (_document.Trailer.Elements[Keys.Root] is PDFReference iref && iref.Value == null)
            {
                iref = _document.IrefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document.Trailer.Elements[Keys.Root] = iref;
            }

            // /Info
            iref = _document.Trailer.Elements[Keys.Info] as PDFReference;
            if (iref != null && iref.Value == null)
            {
                iref = _document.IrefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document.Trailer.Elements[Keys.Info] = iref;
            }

            // /Encrypt
            iref = _document.Trailer.Elements[Keys.Encrypt] as PDFReference;
            if (iref != null)
            {
                iref = _document.IrefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document.Trailer.Elements[Keys.Encrypt] = iref;

                // The encryption dictionary (security handler) was read in before the XRefTable construction 
                // was completed. The next lines fix that state (it took several hours to find these bugs...).
                iref.Value = _document.Trailer._securityHandler;
                _document.Trailer._securityHandler.Reference = iref;
                iref.Value.Reference = iref;
            }

            Elements.Remove(Keys.Prev);

            Debug.Assert(_document.IrefTable.IsUnderConstruction == false);
            _document.IrefTable.IsUnderConstruction = false;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase  // Reference: TABLE 3.13  Entries in the file trailer dictionary / Page 97
        {
            /// <summary>
            /// (Required; must not be an indirect reference) The total number of entries in the file’s 
            /// cross-reference table, as defined by the combination of the original section and all
            /// update sections. Equivalently, this value is 1 greater than the highest object number
            /// used in the file.
            /// Note: Any object in a cross-reference section whose number is greater than this value is
            /// ignored and considered missing.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Size = "/Size";

            /// <summary>
            /// (Present only if the file has more than one cross-reference section; must not be an indirect
            /// reference) The byte offset from the beginning of the file to the beginning of the previous 
            /// cross-reference section.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Prev = "/Prev";

            /// <summary>
            /// (Required; must be an indirect reference) The catalog dictionary for the PDF document
            /// contained in the file.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required, typeof(PDFCatalog))]
            public const string Root = "/Root";

            /// <summary>
            /// (Required if document is encrypted; PDF 1.1) The document’s encryption dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFStandardSecurityHandler))]
            public const string Encrypt = "/Encrypt";

            /// <summary>
            /// (Optional; must be an indirect reference) The document’s information dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFDocumentInformation))]
            public const string Info = "/Info";

            /// <summary>
            /// (Optional, but strongly recommended; PDF 1.1) An array of two strings constituting
            /// a file identifier for the file. Although this entry is optional, 
            /// its absence might prevent the file from functioning in some workflows
            /// that depend on files being uniquely identified.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional) The byte offset from the beginning of the file of a cross-reference stream.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string XRefStm = "/XRefStm";

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
