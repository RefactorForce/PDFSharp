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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.IO;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.Security;
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop.IO
{
    /// <summary>
    /// Represents a writer for generation of PDF streams. 
    /// </summary>
    internal class PDFWriter
    {
        public PDFWriter(Stream pdfStream, PDFStandardSecurityHandler securityHandler)
        {
            Stream = pdfStream;
            SecurityHandler = securityHandler;
            //System.Xml.XmlTextWriter
#if DEBUG
            Layout = PDFWriterLayout.Verbose;
#endif
        }

        public void Close(bool closeUnderlyingStream)
        {
            if (Stream != null && closeUnderlyingStream)
            {
#if UWP
                _stream.Dispose();
#else
                Stream.Close();
#endif
            }
            Stream = null;
        }

        public void Close() => Close(true);

        public int Position => (int)Stream.Position;

        /// <summary>
        /// Gets or sets the kind of layout.
        /// </summary>
        public PDFWriterLayout Layout { get; set; }

        public PDFWriterOptions Options { get; set; }

        // -----------------------------------------------------------

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(bool value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value ? Boolean.TrueString : Boolean.FalseString);
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFBoolean value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value ? "true" : "false");
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(int value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(uint value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFInteger value)
        {
            WriteSeparator(CharCat.Character);
            _lastCat = CharCat.Character;
            WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFUInteger value)
        {
            WriteSeparator(CharCat.Character);
            _lastCat = CharCat.Character;
            WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(double value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(Config.SignificantFigures7, CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFReal value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value.ToString(Config.SignificantFigures7, CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFString value)
        {
            WriteSeparator(CharCat.Delimiter);
#if true
            PDFStringEncoding encoding = (PDFStringEncoding)(value.Flags & PDFStringFlags.EncodingMask);
            string pdf = (value.Flags & PDFStringFlags.HexLiteral) == 0 ?
                PDFEncoders.ToStringLiteral(value.Value, encoding, SecurityHandler) :
                PDFEncoders.ToHexStringLiteral(value.Value, encoding, SecurityHandler);
            WriteRaw(pdf);
#else
            switch (value.Flags & PDFStringFlags.EncodingMask)
            {
                case PDFStringFlags.Undefined:
                case PDFStringFlags.PDFDocEncoding:
                    if ((value.Flags & PDFStringFlags.HexLiteral) == 0)
                        WriteRaw(PDFEncoders.DocEncode(value.Value, false));
                    else
                        WriteRaw(PDFEncoders.DocEncodeHex(value.Value, false));
                    break;

                case PDFStringFlags.WinAnsiEncoding:
                    throw new NotImplementedException("Unexpected encoding: WinAnsiEncoding");

                case PDFStringFlags.Unicode:
                    if ((value.Flags & PDFStringFlags.HexLiteral) == 0)
                        WriteRaw(PDFEncoders.DocEncode(value.Value, true));
                    else
                        WriteRaw(PDFEncoders.DocEncodeHex(value.Value, true));
                    break;

                case PDFStringFlags.StandardEncoding:
                case PDFStringFlags.MacRomanEncoding:
                case PDFStringFlags.MacExpertEncoding:
                default:
                    throw new NotImplementedException("Unexpected encoding");
            }
#endif
            _lastCat = CharCat.Delimiter;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PDFName value)
        {
            WriteSeparator(CharCat.Delimiter, '/');
            string name = value.Value;

            StringBuilder pdf = new StringBuilder("/");
            for (int idx = 1; idx < name.Length; idx++)
            {
                char ch = name[idx];
                Debug.Assert(ch < 256);
                if (ch > ' ')
                    switch (ch)
                    {
                        // TODO: is this all?
                        case '%':
                        case '/':
                        case '<':
                        case '>':
                        case '(':
                        case ')':
                        case '[':
                        case ']':
                        case '#':
                            break;

                        default:
                            pdf.Append(name[idx]);
                            continue;
                    }
                pdf.AppendFormat("#{0:X2}", (int)name[idx]);
            }
            WriteRaw(pdf.ToString());
            _lastCat = CharCat.Character;
        }

        public void Write(PDFLiteral value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value);
            _lastCat = CharCat.Character;
        }

        public void Write(PDFRectangle rect)
        {
            const string format = Config.SignificantFigures3;
            WriteSeparator(CharCat.Delimiter, '/');
            WriteRaw(PDFEncoders.Format("[{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}]", rect.X1, rect.Y1, rect.X2, rect.Y2));
            _lastCat = CharCat.Delimiter;
        }

        public void Write(PDFReference iref)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(iref.ToString());
            _lastCat = CharCat.Character;
        }

        public void WriteDocString(string text, bool unicode)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PDFEncoders.DocEncode(text, unicode));
            byte[] bytes = !unicode ? PDFEncoders.DocEncoding.GetBytes(text) : PDFEncoders.UnicodeEncoding.GetBytes(text);
            bytes = PDFEncoders.FormatStringLiteral(bytes, unicode, true, false, SecurityHandler);
            Write(bytes);
            _lastCat = CharCat.Delimiter;
        }

        public void WriteDocString(string text)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PDFEncoders.DocEncode(text, false));
            byte[] bytes = PDFEncoders.DocEncoding.GetBytes(text);
            bytes = PDFEncoders.FormatStringLiteral(bytes, false, false, false, SecurityHandler);
            Write(bytes);
            _lastCat = CharCat.Delimiter;
        }

        public void WriteDocStringHex(string text)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PDFEncoders.DocEncodeHex(text));
            byte[] bytes = PDFEncoders.DocEncoding.GetBytes(text);
            bytes = PDFEncoders.FormatStringLiteral(bytes, false, false, true, SecurityHandler);
            Stream.Write(bytes, 0, bytes.Length);
            _lastCat = CharCat.Delimiter;
        }

        /// <summary>
        /// Begins a direct or indirect dictionary or array.
        /// </summary>
        public void WriteBeginObject(PDFObject obj)
        {
            bool indirect = obj.IsIndirect;
            if (indirect)
            {
                WriteObjectAddress(obj);
                if (SecurityHandler != null)
                    SecurityHandler.SetHashKey(obj.ObjectID);
            }
            _stack.Add(new StackItem(obj));
            if (indirect)
            {
                if (obj is PDFArray)
                    WriteRaw("[\n");
                else if (obj is PDFDictionary)
                    WriteRaw("<<\n");
                _lastCat = CharCat.NewLine;
            }
            else
            {
                if (obj is PDFArray)
                {
                    WriteSeparator(CharCat.Delimiter);
                    WriteRaw('[');
                    _lastCat = CharCat.Delimiter;
                }
                else if (obj is PDFDictionary)
                {
                    NewLine();
                    WriteSeparator(CharCat.Delimiter);
                    WriteRaw("<<\n");
                    _lastCat = CharCat.NewLine;
                }
            }
            if (Layout == PDFWriterLayout.Verbose)
                IncreaseIndent();
        }

        /// <summary>
        /// Ends a direct or indirect dictionary or array.
        /// </summary>
        public void WriteEndObject()
        {
            int count = _stack.Count;
            Debug.Assert(count > 0, "PDFWriter stack underflow.");

            StackItem stackItem = _stack[count - 1];
            _stack.RemoveAt(count - 1);

            PDFObject value = stackItem.Object;
            bool indirect = value.IsIndirect;
            if (Layout == PDFWriterLayout.Verbose)
                DecreaseIndent();
            if (value is PDFArray)
            {
                if (indirect)
                {
                    WriteRaw("\n]\n");
                    _lastCat = CharCat.NewLine;
                }
                else
                {
                    WriteRaw("]");
                    _lastCat = CharCat.Delimiter;
                }
            }
            else if (value is PDFDictionary)
            {
                if (indirect)
                {
                    if (!stackItem.HasStream)
                        WriteRaw(_lastCat == CharCat.NewLine ? ">>\n" : " >>\n");
                }
                else
                {
                    Debug.Assert(!stackItem.HasStream, "Direct object with stream??");
                    WriteSeparator(CharCat.NewLine);
                    WriteRaw(">>\n");
                    _lastCat = CharCat.NewLine;
                }
            }
            if (indirect)
            {
                NewLine();
                WriteRaw("endobj\n");
                if (Layout == PDFWriterLayout.Verbose)
                    WriteRaw("%--------------------------------------------------------------------------------------------------\n");
            }
        }

        /// <summary>
        /// Writes the stream of the specified dictionary.
        /// </summary>
        public void WriteStream(PDFDictionary value, bool omitStream)
        {
            StackItem stackItem = _stack[_stack.Count - 1];
            Debug.Assert(stackItem.Object is PDFDictionary);
            Debug.Assert(stackItem.Object.IsIndirect);
            stackItem.HasStream = true;

            WriteRaw(_lastCat == CharCat.NewLine ? ">>\nstream\n" : " >>\nstream\n");

            if (omitStream)
            {
                WriteRaw("  «...stream content omitted...»\n");  // useful for debugging only
            }
            else
            {
                byte[] bytes = value.Stream.Value;
                if (bytes.Length != 0)
                {
                    if (SecurityHandler != null)
                    {
                        bytes = (byte[])bytes.Clone();
                        bytes = SecurityHandler.EncryptBytes(bytes);
                    }
                    Write(bytes);
                    if (_lastCat != CharCat.NewLine)
                        WriteRaw('\n');
                }
            }
            WriteRaw("endstream\n");
        }

        public void WriteRaw(string rawString)
        {
            if (String.IsNullOrEmpty(rawString))
                return;

            byte[] bytes = PDFEncoders.RawEncoding.GetBytes(rawString);
            Stream.Write(bytes, 0, bytes.Length);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        public void WriteRaw(char ch)
        {
            Debug.Assert(ch < 256, "Raw character greater than 255 detected.");

            Stream.WriteByte((byte)ch);
            _lastCat = GetCategory(ch);
        }

        public void Write(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            Stream.Write(bytes, 0, bytes.Length);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        void WriteObjectAddress(PDFObject value)
        {
            if (Layout == PDFWriterLayout.Verbose)
                WriteRaw(String.Format("{0} {1} obj   % {2}\n",
                    value.ObjectID.ObjectNumber, value.ObjectID.GenerationNumber,
                    value.GetType().FullName));
            else
                WriteRaw(String.Format("{0} {1} obj\n", value.ObjectID.ObjectNumber, value.ObjectID.GenerationNumber));
        }

        public void WriteFileHeader(PDFDocument document)
        {
            StringBuilder header = new StringBuilder("%PDF-");
            int version = document._version;
            header.Append((version / 10).ToString(CultureInfo.InvariantCulture) + "." +
              (version % 10).ToString(CultureInfo.InvariantCulture) + "\n%\xD3\xF4\xCC\xE1\n");
            WriteRaw(header.ToString());

            if (Layout == PDFWriterLayout.Verbose)
            {
                WriteRaw(String.Format("% PDFSharp Version {0} (verbose mode)\n", VersionMetadata.Version));
                // Keep some space for later fix-up.
                _commentPosition = (int)Stream.Position + 2;
                WriteRaw("%                                                \n");
                WriteRaw("%                                                \n");
                WriteRaw("%                                                \n");
                WriteRaw("%                                                \n");
                WriteRaw("%                                                \n");
                WriteRaw("%--------------------------------------------------------------------------------------------------\n");
            }
        }

        public void WriteEof(PDFDocument document, int startxref)
        {
            WriteRaw("startxref\n");
            WriteRaw(startxref.ToString(CultureInfo.InvariantCulture));
            WriteRaw("\n%%EOF\n");
            int fileSize = (int)Stream.Position;
            if (Layout == PDFWriterLayout.Verbose)
            {
                TimeSpan duration = DateTime.Now - document.Creation;

                Stream.Position = _commentPosition;
                // Without InvariantCulture parameter the following line fails if the current culture is e.g.
                // a Far East culture, because the date string contains non-ASCII characters.
                // So never never never never use ToString without a culture info.
                WriteRaw("Creation date: " + document.Creation.ToString("G", CultureInfo.InvariantCulture));
                Stream.Position = _commentPosition + 50;
                WriteRaw("Creation time: " + duration.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) + " seconds");
                Stream.Position = _commentPosition + 100;
                WriteRaw("File size: " + fileSize.ToString(CultureInfo.InvariantCulture) + " bytes");
                Stream.Position = _commentPosition + 150;
                WriteRaw("Pages: " + document.Pages.Count.ToString(CultureInfo.InvariantCulture));
                Stream.Position = _commentPosition + 200;
                WriteRaw("Objects: " + document.IrefTable.ObjectTable.Count.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets or sets the indentation for a new indentation level.
        /// </summary>
        internal int Indent { get; set; } = 2;

        int _writeIndent;

        /// <summary>
        /// Increases indent level.
        /// </summary>
        void IncreaseIndent() => _writeIndent += Indent;

        /// <summary>
        /// Decreases indent level.
        /// </summary>
        void DecreaseIndent() => _writeIndent -= Indent;

        ///// <summary>
        ///// Returns an indent string of blanks.
        ///// </summary>
        //static string Ind(int _indent)
        //{
        //  return new String(' ', _indent);
        //}

        /// <summary>
        /// Gets an indent string of current indent.
        /// </summary>
        string IndentBlanks => new string(' ', _writeIndent);

        void WriteIndent() => WriteRaw(IndentBlanks);

        void WriteSeparator(CharCat cat, char ch)
        {
            switch (_lastCat)
            {
                case CharCat.NewLine:
                    if (Layout == PDFWriterLayout.Verbose)
                        WriteIndent();
                    break;

                case CharCat.Delimiter:
                    break;

                case CharCat.Character:
                    if (Layout == PDFWriterLayout.Verbose)
                    {
                        //if (cat == CharCat.Character || ch == '/')
                        Stream.WriteByte((byte)' ');
                    }
                    else
                    {
                        if (cat == CharCat.Character)
                            Stream.WriteByte((byte)' ');
                    }
                    break;
            }
        }

        void WriteSeparator(CharCat cat) => WriteSeparator(cat, '\0');

        public void NewLine()
        {
            if (_lastCat != CharCat.NewLine)
                WriteRaw('\n');
        }

        CharCat GetCategory(char ch) => Lexer.IsDelimiter(ch) ? CharCat.Delimiter : ch == Chars.LF ? CharCat.NewLine : CharCat.Character;

        enum CharCat
        {
            NewLine,
            Character,
            Delimiter,
        }
        CharCat _lastCat;

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        internal Stream Stream { get; private set; }

        internal PDFStandardSecurityHandler SecurityHandler { get; set; }

        class StackItem
        {
            public StackItem(PDFObject value) => Object = value;

            public readonly PDFObject Object;
            public bool HasStream;
        }

        readonly List<StackItem> _stack = new List<StackItem>();
        int _commentPosition;
    }
}
