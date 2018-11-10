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

// ReSharper disable UnusedMember.Global

#define USE_CACHE_is_not_thread_safe

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Pens for all the pre-defined colors.
    /// </summary>
    public static class XPens
    {
        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen AliceBlue => new XPen(XColors.AliceBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen AntiqueWhite => new XPen(XColors.AntiqueWhite, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Aqua => new XPen(XColors.Aqua, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Aquamarine => new XPen(XColors.Aquamarine, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Azure => new XPen(XColors.Azure, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Beige => new XPen(XColors.Beige, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Bisque => new XPen(XColors.Bisque, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Black => new XPen(XColors.Black, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BlanchedAlmond => new XPen(XColors.BlanchedAlmond, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Blue => new XPen(XColors.Blue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BlueViolet => new XPen(XColors.BlueViolet, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Brown => new XPen(XColors.Brown, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BurlyWood => new XPen(XColors.BurlyWood, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen CadetBlue => new XPen(XColors.CadetBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Chartreuse => new XPen(XColors.Chartreuse, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Chocolate => new XPen(XColors.Chocolate, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Coral => new XPen(XColors.Coral, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen CornflowerBlue => new XPen(XColors.CornflowerBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Cornsilk => new XPen(XColors.Cornsilk, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Crimson => new XPen(XColors.Crimson, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Cyan => new XPen(XColors.Cyan, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkBlue => new XPen(XColors.DarkBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkCyan => new XPen(XColors.DarkCyan, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGoldenrod => new XPen(XColors.DarkGoldenrod, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGray => new XPen(XColors.DarkGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGreen => new XPen(XColors.DarkGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkKhaki => new XPen(XColors.DarkKhaki, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkMagenta => new XPen(XColors.DarkMagenta, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOliveGreen => new XPen(XColors.DarkOliveGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOrange => new XPen(XColors.DarkOrange, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOrchid => new XPen(XColors.DarkOrchid, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkRed => new XPen(XColors.DarkRed, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSalmon => new XPen(XColors.DarkSalmon, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSeaGreen => new XPen(XColors.DarkSeaGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSlateBlue => new XPen(XColors.DarkSlateBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSlateGray => new XPen(XColors.DarkSlateGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkTurquoise => new XPen(XColors.DarkTurquoise, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkViolet => new XPen(XColors.DarkViolet, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DeepPink => new XPen(XColors.DeepPink, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DeepSkyBlue => new XPen(XColors.DeepSkyBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DimGray => new XPen(XColors.DimGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DodgerBlue => new XPen(XColors.DodgerBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Firebrick => new XPen(XColors.Firebrick, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen FloralWhite => new XPen(XColors.FloralWhite, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen ForestGreen => new XPen(XColors.ForestGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Fuchsia => new XPen(XColors.Fuchsia, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gainsboro => new XPen(XColors.Gainsboro, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen GhostWhite => new XPen(XColors.GhostWhite, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gold => new XPen(XColors.Gold, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Goldenrod => new XPen(XColors.Goldenrod, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gray => new XPen(XColors.Gray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Green => new XPen(XColors.Green, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen GreenYellow => new XPen(XColors.GreenYellow, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Honeydew => new XPen(XColors.Honeydew, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen HotPink => new XPen(XColors.HotPink, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen IndianRed => new XPen(XColors.IndianRed, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Indigo => new XPen(XColors.Indigo, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Ivory => new XPen(XColors.Ivory, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Khaki => new XPen(XColors.Khaki, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Lavender => new XPen(XColors.Lavender, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LavenderBlush => new XPen(XColors.LavenderBlush, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LawnGreen => new XPen(XColors.LawnGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LemonChiffon => new XPen(XColors.LemonChiffon, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightBlue => new XPen(XColors.LightBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightCoral => new XPen(XColors.LightCoral, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightCyan => new XPen(XColors.LightCyan, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGoldenrodYellow => new XPen(XColors.LightGoldenrodYellow, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGray => new XPen(XColors.LightGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGreen => new XPen(XColors.LightGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightPink => new XPen(XColors.LightPink, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSalmon => new XPen(XColors.LightSalmon, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSeaGreen => new XPen(XColors.LightSeaGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSkyBlue => new XPen(XColors.LightSkyBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSlateGray => new XPen(XColors.LightSlateGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSteelBlue => new XPen(XColors.LightSteelBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightYellow => new XPen(XColors.LightYellow, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Lime => new XPen(XColors.Lime, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LimeGreen => new XPen(XColors.LimeGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Linen => new XPen(XColors.Linen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Magenta => new XPen(XColors.Magenta, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Maroon => new XPen(XColors.Maroon, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumAquamarine => new XPen(XColors.MediumAquamarine, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumBlue => new XPen(XColors.MediumBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumOrchid => new XPen(XColors.MediumOrchid, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumPurple => new XPen(XColors.MediumPurple, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSeaGreen => new XPen(XColors.MediumSeaGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSlateBlue => new XPen(XColors.MediumSlateBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSpringGreen => new XPen(XColors.MediumSpringGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumTurquoise => new XPen(XColors.MediumTurquoise, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumVioletRed => new XPen(XColors.MediumVioletRed, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MidnightBlue => new XPen(XColors.MidnightBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MintCream => new XPen(XColors.MintCream, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MistyRose => new XPen(XColors.MistyRose, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Moccasin => new XPen(XColors.Moccasin, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen NavajoWhite => new XPen(XColors.NavajoWhite, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Navy => new XPen(XColors.Navy, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OldLace => new XPen(XColors.OldLace, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Olive => new XPen(XColors.Olive, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OliveDrab => new XPen(XColors.OliveDrab, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Orange => new XPen(XColors.Orange, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OrangeRed => new XPen(XColors.OrangeRed, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Orchid => new XPen(XColors.Orchid, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleGoldenrod => new XPen(XColors.PaleGoldenrod, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleGreen => new XPen(XColors.PaleGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleTurquoise => new XPen(XColors.PaleTurquoise, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleVioletRed => new XPen(XColors.PaleVioletRed, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PapayaWhip => new XPen(XColors.PapayaWhip, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PeachPuff => new XPen(XColors.PeachPuff, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Peru => new XPen(XColors.Peru, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Pink => new XPen(XColors.Pink, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Plum => new XPen(XColors.Plum, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PowderBlue => new XPen(XColors.PowderBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Purple => new XPen(XColors.Purple, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Red => new XPen(XColors.Red, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen RosyBrown => new XPen(XColors.RosyBrown, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen RoyalBlue => new XPen(XColors.RoyalBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SaddleBrown => new XPen(XColors.SaddleBrown, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Salmon => new XPen(XColors.Salmon, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SandyBrown => new XPen(XColors.SandyBrown, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SeaGreen => new XPen(XColors.SeaGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SeaShell => new XPen(XColors.SeaShell, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Sienna => new XPen(XColors.Sienna, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Silver => new XPen(XColors.Silver, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SkyBlue => new XPen(XColors.SkyBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SlateBlue => new XPen(XColors.SlateBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SlateGray => new XPen(XColors.SlateGray, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Snow => new XPen(XColors.Snow, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SpringGreen => new XPen(XColors.SpringGreen, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SteelBlue => new XPen(XColors.SteelBlue, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Tan => new XPen(XColors.Tan, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Teal => new XPen(XColors.Teal, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Thistle => new XPen(XColors.Thistle, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Tomato => new XPen(XColors.Tomato, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Transparent => new XPen(XColors.Transparent, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Turquoise => new XPen(XColors.Turquoise, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Violet => new XPen(XColors.Violet, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Wheat => new XPen(XColors.Wheat, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen White => new XPen(XColors.White, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen WhiteSmoke => new XPen(XColors.WhiteSmoke, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Yellow => new XPen(XColors.Yellow, 1, true);

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen YellowGreen => new XPen(XColors.YellowGreen, 1, true);

#if USE_CACHE
        static XPen _aliceBlue;
        static XPen _antiqueWhite;
        static XPen _aqua;
        static XPen _aquamarine;
        static XPen _azure;
        static XPen _beige;
        static XPen _bisque;
        static XPen _black;
        static XPen _blanchedAlmond;
        static XPen _blue;
        static XPen _blueViolet;
        static XPen _brown;
        static XPen _burlyWood;
        static XPen _cadetBlue;
        static XPen _chartreuse;
        static XPen _chocolate;
        static XPen _coral;
        static XPen _cornflowerBlue;
        static XPen _cornsilk;
        static XPen _crimson;
        static XPen _cyan;
        static XPen _darkBlue;
        static XPen _darkCyan;
        static XPen _darkGoldenrod;
        static XPen _darkGray;
        static XPen _darkGreen;
        static XPen _darkKhaki;
        static XPen _darkMagenta;
        static XPen _darkOliveGreen;
        static XPen _darkOrange;
        static XPen _darkOrchid;
        static XPen _darkRed;
        static XPen _darkSalmon;
        static XPen _darkSeaGreen;
        static XPen _darkSlateBlue;
        static XPen _darkSlateGray;
        static XPen _darkTurquoise;
        static XPen _darkViolet;
        static XPen _deepPink;
        static XPen _deepSkyBlue;
        static XPen _dimGray;
        static XPen _dodgerBlue;
        static XPen _firebrick;
        static XPen _floralWhite;
        static XPen _forestGreen;
        static XPen _fuchsia;
        static XPen _gainsboro;
        static XPen _ghostWhite;
        static XPen _gold;
        static XPen _goldenrod;
        static XPen _gray;
        static XPen _green;
        static XPen _greenYellow;
        static XPen _honeydew;
        static XPen _hotPink;
        static XPen _indianRed;
        static XPen _indigo;
        static XPen _ivory;
        static XPen _khaki;
        static XPen _lavender;
        static XPen _lavenderBlush;
        static XPen _lawnGreen;
        static XPen _lemonChiffon;
        static XPen _lightBlue;
        static XPen _lightCoral;
        static XPen _lightCyan;
        static XPen _lightGoldenrodYellow;
        static XPen _lightGray;
        static XPen _lightGreen;
        static XPen _lightPink;
        static XPen _lightSalmon;
        static XPen _lightSeaGreen;
        static XPen _lightSkyBlue;
        static XPen _lightSlateGray;
        static XPen _lightSteelBlue;
        static XPen _lightYellow;
        static XPen _lime;
        static XPen _limeGreen;
        static XPen _linen;
        static XPen _magenta;
        static XPen _maroon;
        static XPen _mediumAquamarine;
        static XPen _mediumBlue;
        static XPen _mediumOrchid;
        static XPen _mediumPurple;
        static XPen _mediumSeaGreen;
        static XPen _mediumSlateBlue;
        static XPen _mediumSpringGreen;
        static XPen _mediumTurquoise;
        static XPen _mediumVioletRed;
        static XPen _midnightBlue;
        static XPen _mintCream;
        static XPen _mistyRose;
        static XPen _moccasin;
        static XPen _navajoWhite;
        static XPen _navy;
        static XPen _oldLace;
        static XPen _olive;
        static XPen _oliveDrab;
        static XPen _orange;
        static XPen _orangeRed;
        static XPen _orchid;
        static XPen _paleGoldenrod;
        static XPen _paleGreen;
        static XPen _paleTurquoise;
        static XPen _paleVioletRed;
        static XPen _papayaWhip;
        static XPen _peachPuff;
        static XPen _peru;
        static XPen _pink;
        static XPen _plum;
        static XPen _powderBlue;
        static XPen _purple;
        static XPen _red;
        static XPen _rosyBrown;
        static XPen _royalBlue;
        static XPen _saddleBrown;
        static XPen _salmon;
        static XPen _sandyBrown;
        static XPen _seaGreen;
        static XPen _seaShell;
        static XPen _sienna;
        static XPen _silver;
        static XPen _skyBlue;
        static XPen _slateBlue;
        static XPen _slateGray;
        static XPen _snow;
        static XPen _springGreen;
        static XPen _steelBlue;
        static XPen _tan;
        static XPen _teal;
        static XPen _thistle;
        static XPen _tomato;
        static XPen _transparent;
        static XPen _turquoise;
        static XPen _violet;
        static XPen _wheat;
        static XPen _white;
        static XPen _whiteSmoke;
        static XPen _yellow;
        static XPen _yellowGreen;
#endif
    }
}
