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
using System.Diagnostics;
using System.IO;
#if NETFX_CORE
using System.Threading.Tasks;
#endif
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.Internal;
using PDFSharp.Interop.IO;
using PDFSharp.Interop.AcroForms;
using PDFSharp.Interop.Security;

// ReSharper disable ConvertPropertyToExpressionBody

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a PDF document.
    /// </summary>
    [DebuggerDisplay("(Name={Name})")] // A name makes debugging easier
    public sealed class PDFDocument : PDFObject, IDisposable
    {
        internal DocumentState _state;
        internal PDFDocumentOpenMode _openMode;

#if DEBUG_
        static PDFDocument()
        {
            PSSR.TestResourceMessages();
            //string test = PSSR.ResMngr.GetString("SampleMessage1");
            //test.GetType();
        }
#endif

        /// <summary>
        /// Creates a new PDF document in memory.
        /// To open an existing PDF file, use the PDFReader class.
        /// </summary>
        public PDFDocument()
        {
            //PDFDocument.Gob.AttatchDocument(Handle);

            _creation = DateTime.Now;
            _state = DocumentState.Created;
            _version = 14;
            Initialize();
            Info.CreationDate = _creation;
        }

        /// <summary>
        /// Creates a new PDF document with the specified file name. The file is immediately created and keeps
        /// locked until the document is closed, at that time the document is saved automatically.
        /// Do not call Save() for documents created with this constructor, just call Close().
        /// To open an existing PDF file and import it, use the PDFReader class.
        /// </summary>
        public PDFDocument(string filename)
        {
            //PDFDocument.Gob.AttatchDocument(Handle);

            _creation = DateTime.Now;
            _state = DocumentState.Created;
            _version = 14;
            Initialize();
            Info.CreationDate = _creation;

            // TODO 4STLA: encapsulate the whole c'tor with #if !NETFX_CORE?
#if !NETFX_CORE
            _outStream = new FileStream(filename, FileMode.Create);
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Creates a new PDF document using the specified stream.
        /// The stream won't be used until the document is closed, at that time the document is saved automatically.
        /// Do not call Save() for documents created with this constructor, just call Close().
        /// To open an existing PDF file, use the PDFReader class.
        /// </summary>
        public PDFDocument(Stream outputStream)
        {
            //PDFDocument.Gob.AttatchDocument(Handle);

            _creation = DateTime.Now;
            _state = DocumentState.Created;
            Initialize();
            Info.CreationDate = _creation;

            _outStream = outputStream;
        }

        internal PDFDocument(Lexer lexer)
        {
            //PDFDocument.Gob.AttatchDocument(Handle);

            _creation = DateTime.Now;
            _state = DocumentState.Imported;

            //_info = new PDFInfo(this);
            //_pages = new PDFPages(this);
            //_fontTable = new PDFFontTable();
            //_catalog = new PDFCatalog(this);
            ////_font = new PDFFont();
            //_objects = new PDFObjectTable(this);
            //_trailer = new PDFTrailer(this);
            _irefTable = new PDFCrossReferenceTable(this);
            _lexer = lexer;
        }

        void Initialize()
        {
            //_info = new PDFInfo(this);
            _fontTable = new PDFFontTable(this);
            _imageTable = new PDFImageTable(this);
            _trailer = new PDFTrailer(this);
            _irefTable = new PDFCrossReferenceTable(this);
            _trailer.CreateNewDocumentIDs();
        }

        //~PDFDocument()
        //{
        //  Dispose(false);
        //}

        /// <summary>
        /// Disposes all references to this document stored in other documents. This function should be called
        /// for documents you finished importing pages from. Calling Dispose is technically not necessary but
        /// useful for earlier reclaiming memory of documents you do not need anymore.
        /// </summary>
        public void Dispose() => Dispose(true);//GC.SuppressFinalize(this);

        void Dispose(bool disposing)
        {
            if (_state != DocumentState.Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }
                //PDFDocument.Gob.DetatchDocument(Handle);
            }
            _state = DocumentState.Disposed;
        }

        /// <summary>
        /// Gets or sets a user defined object that contains arbitrary information associated with this document.
        /// The tag is not used by PDFsharp.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets a value used to distinguish PDFDocument objects.
        /// The name is not used by PDFsharp.
        /// </summary>
        string Name { get; set; } = NewName();

        /// <summary>
        /// Get a new default name for a new document.
        /// </summary>
        static string NewName() =>
#if DEBUG_
            if (PDFDocument.nameCount == 57)
                PDFDocument.nameCount.GetType();
#endif
            "Document " + _nameCount++;
        static int _nameCount;

        internal bool CanModify => true;

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);

            if (_outStream != null)
            {
                // Get security handler if document gets encrypted
                PDFStandardSecurityHandler securityHandler = null;
                if (SecuritySettings.DocumentSecurityLevel != PDFDocumentSecurityLevel.None)
                    securityHandler = SecuritySettings.SecurityHandler;

                PDFWriter writer = new PDFWriter(_outStream, securityHandler);
                try
                {
                    DoSave(writer);
                }
                finally
                {
                    writer.Close();
                }
            }
        }

#if true //!NETFX_CORE
        /// <summary>
        /// Saves the document to the specified path. If a file already exists, it will be overwritten.
        /// </summary>
        public void Save(string path)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);

#if !NETFX_CORE
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream);
            }
#else
            var task = SaveAsync(path, true);

            ////var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("MyWav.wav", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            ////var stream = file.OpenStreamForWriteAsync();
            ////var writer = new StreamWriter(stream);
            ////Save(stream);

            //var ms = new MemoryStream();
            //Save(ms, false);
            //byte[] pdf = ms.ToArray();
            //ms.Close();
#endif
        }
#endif

#if NETFX_CORE
        /// <summary>
        /// Saves the document to the specified path. If a file already exists, it will be overwritten.
        /// </summary>
        public async Task SaveAsync(string path, bool closeStream)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);

            // Just march through...

            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("My1st.pdf", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenStreamForWriteAsync();
            using (var writer = new StreamWriter(stream))
            {
                Save(stream, false);
            }

            //var ms = new MemoryStream();
            //Save(ms, false);
            //byte[] pdf = ms.ToArray();
            //ms.Close();
            //await stream.WriteAsync(pdf, 0, pdf.Length);
            //stream.Close();
        }
#endif

        /// <summary>
        /// Saves the document to the specified stream.
        /// </summary>
        public void Save(Stream stream, bool closeStream)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);

            // TODO: more diagnostic checks
            string message = "";
            if (!CanSave(ref message))
                throw new PDFSharpException(message);

            // Get security handler if document gets encrypted.
            PDFStandardSecurityHandler securityHandler = null;
            if (SecuritySettings.DocumentSecurityLevel != PDFDocumentSecurityLevel.None)
                securityHandler = SecuritySettings.SecurityHandler;

            PDFWriter writer = null;
            try
            {
                writer = new PDFWriter(stream, securityHandler);
                DoSave(writer);
            }
            finally
            {
                if (stream != null)
                {
                    if (closeStream)
#if UWP
                        stream.Dispose();
#else
                        stream.Close();
#endif
                    else
                    {
                        if (stream.CanRead && stream.CanSeek)
                            stream.Position = 0; // Reset the stream position if the stream is kept open.
                    }
                }
                if (writer != null)
                    writer.Close(closeStream);
            }
        }

        /// <summary>
        /// Saves the document to the specified stream.
        /// The stream is not closed by this function.
        /// (Older versions of PDFsharp closes the stream. That was not very useful.)
        /// </summary>
        public void Save(Stream stream) => Save(stream, false);

        /// <summary>
        /// Implements saving a PDF file.
        /// </summary>
        void DoSave(PDFWriter writer)
        {
            if (_pages == null || _pages.Count == 0)
            {
                if (_outStream != null)
                {
                    // Give feedback if the wrong constructor was used.
                    throw new InvalidOperationException("Cannot save a PDF document with no pages. Do not use \"public PDFDocument(string filename)\" or \"public PDFDocument(Stream outputStream)\" if you want to open an existing PDF document from a file or stream; use PDFReader.Open() for that purpose.");
                }
                throw new InvalidOperationException("Cannot save a PDF document with no pages.");
            }

            try
            {
                // HACK: Remove XRefTrailer
                if (_trailer is PDFCrossReferenceStream)
                {
                    // HACK^2: Preserve the SecurityHandler.
                    PDFStandardSecurityHandler securityHandler = _securitySettings.SecurityHandler;
                    _trailer = new PDFTrailer((PDFCrossReferenceStream)_trailer)
                    {
                        _securityHandler = securityHandler
                    };
                }

                bool encrypt = _securitySettings.DocumentSecurityLevel != PDFDocumentSecurityLevel.None;
                if (encrypt)
                {
                    PDFStandardSecurityHandler securityHandler = _securitySettings.SecurityHandler;
                    if (securityHandler.Reference == null)
                        _irefTable.Add(securityHandler);
                    else
                        Debug.Assert(_irefTable.Contains(securityHandler.ObjectID));
                    _trailer.Elements[PDFTrailer.Keys.Encrypt] = _securitySettings.SecurityHandler.Reference;
                }
                else
                    _trailer.Elements.Remove(PDFTrailer.Keys.Encrypt);

                PrepareForSave();

                if (encrypt)
                    _securitySettings.SecurityHandler.PrepareEncryption();

                writer.WriteFileHeader(this);
                PDFReference[] irefs = _irefTable.AllReferences;
                int count = irefs.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    PDFReference iref = irefs[idx];
#if DEBUG_
                    if (iref.ObjectNumber == 378)
                        GetType();
#endif
                    iref.Position = writer.Position;
                    iref.Value.WriteObject(writer);
                }
                int startxref = writer.Position;
                _irefTable.WriteObject(writer);
                writer.WriteRaw("trailer\n");
                _trailer.Elements.SetInteger("/Size", count + 1);
                _trailer.WriteObject(writer);
                writer.WriteEof(this, startxref);

                //if (encrypt)
                //{
                //  state &= ~DocumentState.SavingEncrypted;
                //  //_securitySettings.SecurityHandler.EncryptDocument();
                //}
            }
            finally
            {
                if (writer != null)
                {
                    writer.Stream.Flush();
                    // DO NOT CLOSE WRITER HERE
                    //writer.Close();
                }
            }
        }

        /// <summary>
        /// Dispatches PrepareForSave to the objects that need it.
        /// </summary>
        internal override void PrepareForSave()
        {
            PDFDocumentInformation info = Info;

            // Add patch level to producer if it is not '0'.
            string pdfSharpProducer = ProductVersionInfo.Producer;
            if (!ProductVersionInfo.VersionPatch.Equals("0"))
                pdfSharpProducer = ProductVersionInfo.Producer2;

            // Set Creator if value is undefined.
            if (info.Elements[PDFDocumentInformation.Keys.Creator] == null)
                info.Creator = pdfSharpProducer;

            // Keep original producer if file was imported.
            string producer = info.Producer;
            if (producer.Length == 0)
                producer = pdfSharpProducer;
            else
            {
                // Prevent endless concatenation if file is edited with PDFsharp more than once.
                if (!producer.StartsWith(ProductVersionInfo.Title))
                    producer = pdfSharpProducer + " (Original: " + producer + ")";
            }
            info.Elements.SetString(PDFDocumentInformation.Keys.Producer, producer);

            // Prepare used fonts.
            if (_fontTable != null)
                _fontTable.PrepareForSave();

            // Let catalog do the rest.
            Catalog.PrepareForSave();

#if true
            // Remove all unreachable objects (e.g. from deleted pages)
            int removed = _irefTable.Compact();
            if (removed != 0)
                Debug.WriteLine("PrepareForSave: Number of deleted unreachable objects: " + removed);
            _irefTable.Renumber();
#endif
        }

        /// <summary>
        /// Determines whether the document can be saved.
        /// </summary>
        public bool CanSave(ref string message) => !SecuritySettings.CanSave(ref message) ? false : true;

        internal bool HasVersion(string version) => String.Compare(Catalog.Version, version) >= 0;

        /// <summary>
        /// Gets the document options used for saving the document.
        /// </summary>
        public PDFDocumentOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new PDFDocumentOptions(this);
                return _options;
            }
        }
        PDFDocumentOptions _options;

        /// <summary>
        /// Gets PDF specific document settings.
        /// </summary>
        public PDFDocumentSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = new PDFDocumentSettings(this);
                return _settings;
            }
        }
        PDFDocumentSettings _settings;

        /// <summary>
        /// NYI Indicates whether large objects are written immediately to the output stream to relieve
        /// memory consumption.
        /// </summary>
        internal bool EarlyWrite => false;

        /// <summary>
        /// Gets or sets the PDF version number. Return value 14 e.g. means PDF 1.4 / Acrobat 5 etc.
        /// </summary>
        public int Version
        {
            get => _version;
            set
            {
                if (!CanModify)
                    throw new InvalidOperationException(PSSR.CannotModify);
                if (value < 12 || value > 17) // TODO not really implemented
                    throw new ArgumentException(PSSR.InvalidVersionNumber, "value");
                _version = value;
            }
        }
        internal int _version;

        /// <summary>
        /// Gets the number of pages in the document.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (CanModify)
                    return Pages.Count;
                // PDFOpenMode is InformationOnly
                PDFDictionary pageTreeRoot = (PDFDictionary)Catalog.Elements.GetObject(PDFCatalog.Keys.Pages);
                return pageTreeRoot.Elements.GetInteger(PDFPages.Keys.Count);
            }
        }

        /// <summary>
        /// Gets the file size of the document.
        /// </summary>
        public long FileSize => _fileSize;
        internal long _fileSize; // TODO: make private

        /// <summary>
        /// Gets the full qualified file name if the document was read form a file, or an empty string otherwise.
        /// </summary>
        public string FullPath => _fullPath;
        internal string _fullPath = String.Empty; // TODO: make private

        /// <summary>
        /// Gets a Guid that uniquely identifies this instance of PDFDocument.
        /// </summary>
        public Guid Guid { get; } = Guid.NewGuid();

        internal DocumentHandle Handle
        {
            get
            {
                if (_handle == null)
                    _handle = new DocumentHandle(this);
                return _handle;
            }
        }
        DocumentHandle _handle;

        /// <summary>
        /// Returns a value indicating whether the document was newly created or opened from an existing document.
        /// Returns true if the document was opened with the PDFReader.Open function, false otherwise.
        /// </summary>
        public bool IsImported => (_state & DocumentState.Imported) != 0;

        /// <summary>
        /// Returns a value indicating whether the document is read only or can be modified.
        /// </summary>
        public bool IsReadOnly => _openMode != PDFDocumentOpenMode.Modify;

        internal Exception DocumentNotImported() => new InvalidOperationException("Document not imported.");

        /// <summary>
        /// Gets information about the document.
        /// </summary>
        public PDFDocumentInformation Info
        {
            get
            {
                if (_info == null)
                    _info = _trailer.Info;
                return _info;
            }
        }
        PDFDocumentInformation _info;  // never changes if once created

        /// <summary>
        /// This function is intended to be undocumented.
        /// </summary>
        public PDFCustomValues CustomValues
        {
            get
            {
                if (_customValues == null)
                    _customValues = PDFCustomValues.Get(Catalog.Elements);
                return _customValues;
            }
            set
            {
                if (value != null)
                    throw new ArgumentException("Only null is allowed to clear all custom values.");
                PDFCustomValues.Remove(Catalog.Elements);
                _customValues = null;
            }
        }
        PDFCustomValues _customValues;

        /// <summary>
        /// Get the pages dictionary.
        /// </summary>
        public PDFPages Pages
        {
            get
            {
                if (_pages == null)
                    _pages = Catalog.Pages;
                return _pages;
            }
        }
        PDFPages _pages;  // never changes if once created

        /// <summary>
        /// Gets or sets a value specifying the page layout to be used when the document is opened.
        /// </summary>
        public PDFPageLayout PageLayout
        {
            get => Catalog.PageLayout;
            set
            {
                if (!CanModify)
                    throw new InvalidOperationException(PSSR.CannotModify);
                Catalog.PageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying how the document should be displayed when opened.
        /// </summary>
        public PDFPageMode PageMode
        {
            get => Catalog.PageMode;
            set
            {
                if (!CanModify)
                    throw new InvalidOperationException(PSSR.CannotModify);
                Catalog.PageMode = value;
            }
        }

        /// <summary>
        /// Gets the viewer preferences of this document.
        /// </summary>
        public PDFViewerPreferences ViewerPreferences => Catalog.ViewerPreferences;

        /// <summary>
        /// Gets the root of the outline (or bookmark) tree.
        /// </summary>
        public PDFOutlineCollection Outlines => Catalog.Outlines;

        /// <summary>
        /// Get the AcroForm dictionary.
        /// </summary>
        public PDFAcroForm AcroForm => Catalog.AcroForm;

        /// <summary>
        /// Gets or sets the default language of the document.
        /// </summary>
        public string Language
        {
            get => Catalog.Language;
            set => Catalog.Language = value;
        }

        /// <summary>
        /// Gets the security settings of this document.
        /// </summary>
        public PDFSecuritySettings SecuritySettings => _securitySettings ?? (_securitySettings = new PDFSecuritySettings(this));
        internal PDFSecuritySettings _securitySettings;

        /// <summary>
        /// Gets the document font table that holds all fonts used in the current document.
        /// </summary>
        internal PDFFontTable FontTable => _fontTable ?? (_fontTable = new PDFFontTable(this));
        PDFFontTable _fontTable;

        /// <summary>
        /// Gets the document image table that holds all images used in the current document.
        /// </summary>
        internal PDFImageTable ImageTable
        {
            get
            {
                if (_imageTable == null)
                    _imageTable = new PDFImageTable(this);
                return _imageTable;
            }
        }
        PDFImageTable _imageTable;

        /// <summary>
        /// Gets the document form table that holds all form external objects used in the current document.
        /// </summary>
        internal PDFFormXObjectTable FormTable  // TODO: Rename to ExternalDocumentTable.
=> _formTable ?? (_formTable = new PDFFormXObjectTable(this));
        PDFFormXObjectTable _formTable;

        /// <summary>
        /// Gets the document ExtGState table that holds all form state objects used in the current document.
        /// </summary>
        internal PDFExtGStateTable ExtGStateTable => _extGStateTable ?? (_extGStateTable = new PDFExtGStateTable(this));
        PDFExtGStateTable _extGStateTable;

        /// <summary>
        /// Gets the PDFCatalog of the current document.
        /// </summary>
        internal PDFCatalog Catalog => _catalog ?? (_catalog = _trailer.Root);
        PDFCatalog _catalog;  // never changes if once created

        /// <summary>
        /// Gets the PDFInternals object of this document, that grants access to some internal structures
        /// which are not part of the public interface of PDFDocument.
        /// </summary>
        public new PDFInternals Internals => _internals ?? (_internals = new PDFInternals(this));
        PDFInternals _internals;

        /// <summary>
        /// Creates a new page and adds it to this document.
        /// Depending of the IsMetric property of the current region the page size is set to 
        /// A4 or Letter respectively. If this size is not appropriate it should be changed before
        /// any drawing operations are performed on the page.
        /// </summary>
        public PDFPage AddPage()
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);
            return Catalog.Pages.Add();
        }

        /// <summary>
        /// Adds the specified page to this document. If the page is from an external document,
        /// it is imported to this document. In this case the returned page is not the same
        /// object as the specified one.
        /// </summary>
        public PDFPage AddPage(PDFPage page)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);
            return Catalog.Pages.Add(page);
        }

        /// <summary>
        /// Creates a new page and inserts it in this document at the specified position.
        /// </summary>
        public PDFPage InsertPage(int index)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);
            return Catalog.Pages.Insert(index);
        }

        /// <summary>
        /// Inserts the specified page in this document. If the page is from an external document,
        /// it is imported to this document. In this case the returned page is not the same
        /// object as the specified one.
        /// </summary>
        public PDFPage InsertPage(int index, PDFPage page)
        {
            if (!CanModify)
                throw new InvalidOperationException(PSSR.CannotModify);
            return Catalog.Pages.Insert(index, page);
        }

        /// <summary>  
        /// Flattens a document (make the fields non-editable).  
        /// </summary>  
        public void Flatten()
        {
            for (int idx = 0; idx < AcroForm.Fields.Count; idx++)
            {
                AcroForm.Fields[idx].ReadOnly = true;
            }
        }

        /// <summary>
        /// Gets the security handler.
        /// </summary>
        public PDFStandardSecurityHandler SecurityHandler => _trailer.SecurityHandler;

        internal PDFTrailer _trailer;
        internal PDFCrossReferenceTable _irefTable;
        internal Stream _outStream;

        // Imported Document
        internal Lexer _lexer;

        internal DateTime _creation;

        /// <summary>
        /// Occurs when the specified document is not used anymore for importing content.
        /// </summary>
        internal void OnExternalDocumentFinalized(DocumentHandle handle)
        {
            if (tls != null)
            {
                //PDFDocument[] documents = tls.Documents;
                tls.DetachDocument(handle);
            }

            if (_formTable != null)
                _formTable.DetachDocument(handle);
        }

        //internal static GlobalObjectTable Gob = new GlobalObjectTable();

        /// <summary>
        /// Gets the ThreadLocalStorage object. It is used for caching objects that should created
        /// only once.
        /// </summary>
        internal static ThreadLocalStorage Tls => tls ?? (tls = new ThreadLocalStorage());
        [ThreadStatic]
        static ThreadLocalStorage tls;

        [DebuggerDisplay("(ID={ID}, alive={IsAlive})")]
        internal class DocumentHandle
        {
            public DocumentHandle(PDFDocument document)
            {
                _weakRef = new WeakReference(document);
                ID = document.Guid.ToString("B").ToUpper();
            }

            public bool IsAlive => _weakRef.IsAlive;

            public PDFDocument Target => _weakRef.Target as PDFDocument;
            readonly WeakReference _weakRef;

            public string ID;

            public override bool Equals(object obj)
            {
                DocumentHandle handle = obj as DocumentHandle;
                return !(handle is null) ? ID == handle.ID : false;
            }

            public override int GetHashCode() => ID.GetHashCode();

            public static bool operator ==(DocumentHandle left, DocumentHandle right) => left is null ? right is null : left.Equals(right);

            public static bool operator !=(DocumentHandle left, DocumentHandle right) => !(left == right);
        }
    }
}