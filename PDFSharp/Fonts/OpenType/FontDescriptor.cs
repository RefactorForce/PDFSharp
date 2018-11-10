﻿#region PDFsharp - A .NET library for processing PDF
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

#if GDI
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif
using PdfSharp.Pdf.Internal;
using PdfSharp.Fonts;
#if !EDF_CORE
using PdfSharp.Drawing;
#endif

#pragma warning disable 0649

namespace PdfSharp.Fonts.OpenType
{
    // TODO: Needs to be refactored #???
    /// <summary>
    /// Base class for all font descriptors.
    /// Currently only OpenTypeDescriptor is derived from this base class.
    /// </summary>
    internal class FontDescriptor
    {
        protected FontDescriptor(string key) => Key = key;

        public string Key { get; }







        ///// <summary>
        ///// 
        ///// </summary>
        //public string FontFile
        //{
        //  get { return _fontFile; }
        //  private set { _fontFile = value; }  // BUG: never set
        //}
        //string _fontFile;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FontType
        //{
        //  get { return _fontType; }
        //  private set { _fontType = value; }  // BUG: never set
        //}
        //string _fontType;

        /// <summary>
        /// 
        /// </summary>
        public string FontName { get; protected set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FullName
        //{
        //    get { return _fullName; }
        //    private set { _fullName = value; }  // BUG: never set
        //}
        //string _fullName;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FamilyName
        //{
        //    get { return _familyName; }
        //    private set { _familyName = value; }  // BUG: never set
        //}
        //string _familyName;

        /// <summary>
        /// 
        /// </summary>
        public string Weight { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance belongs to a bold font.
        /// </summary>
        public virtual bool IsBoldFace => false;

        /// <summary>
        /// 
        /// </summary>
        public float ItalicAngle { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance belongs to an italic font.
        /// </summary>
        public virtual bool IsItalicFace => false;

        /// <summary>
        /// 
        /// </summary>
        public int XMin { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int YMin { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int XMax { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int YMax { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFixedPitch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int UnderlinePosition { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int UnderlineThickness { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrikeoutPosition { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrikeoutSize { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string Notice
        //{
        //  get { return Notice; }
        //}
        //protected string notice;

        /// <summary>
        /// 
        /// </summary>
        public string EncodingScheme { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int UnitsPerEm { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int CapHeight { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int XHeight { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int Ascender { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int Descender { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int Leading { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int Flags { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int StemV { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int LineSpacing { get; protected set; }

        internal static string ComputeKey(XFont font) => font.GlyphTypeface.Key;//return ComputeKey(font.GlyphTypeface.Fontface.FullFaceName, font.Style);//XGlyphTypeface glyphTypeface = font.GlyphTypeface;//string key = glyphTypeface.Fontface.FullFaceName.ToLowerInvariant() +//    (glyphTypeface.IsBold ? "/b" : "") + (glyphTypeface.IsItalic ? "/i" : "");//return key;

        internal static string ComputeKey(string name, XFontStyle style) => ComputeKey(name,
                (style & XFontStyle.Bold) == XFontStyle.Bold,
                (style & XFontStyle.Italic) == XFontStyle.Italic);

        internal static string ComputeKey(string name, bool isBold, bool isItalic)
        {
            string key = name.ToLowerInvariant() + '/'
                + (isBold ? "b" : "") + (isItalic ? "i" : "");
            return key;
        }

        internal static string ComputeKey(string name)
        {
            string key = name.ToLowerInvariant();
            return key;
        }
    }
}