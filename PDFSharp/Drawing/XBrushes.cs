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

namespace PDFSharp.Drawing
{
    /// <summary>
    /// Brushes for all the pre-defined colors.
    /// </summary>
    public static class XBrushes
    {
        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush AliceBlue => new XSolidBrush(XColors.AliceBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush AntiqueWhite => new XSolidBrush(XColors.AntiqueWhite, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Aqua => new XSolidBrush(XColors.Aqua, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Aquamarine => new XSolidBrush(XColors.Aquamarine, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Azure => new XSolidBrush(XColors.Azure, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Beige => new XSolidBrush(XColors.Beige, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Bisque => new XSolidBrush(XColors.Bisque, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Black => new XSolidBrush(XColors.Black, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BlanchedAlmond => new XSolidBrush(XColors.BlanchedAlmond, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Blue => new XSolidBrush(XColors.Blue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BlueViolet => new XSolidBrush(XColors.BlueViolet, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Brown => new XSolidBrush(XColors.Brown, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BurlyWood => new XSolidBrush(XColors.BurlyWood, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush CadetBlue => new XSolidBrush(XColors.CadetBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Chartreuse => new XSolidBrush(XColors.Chartreuse, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Chocolate => new XSolidBrush(XColors.Chocolate, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Coral => new XSolidBrush(XColors.Coral, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush CornflowerBlue => new XSolidBrush(XColors.CornflowerBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Cornsilk => new XSolidBrush(XColors.Cornsilk, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Crimson => new XSolidBrush(XColors.Crimson, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Cyan => new XSolidBrush(XColors.Cyan, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkBlue => new XSolidBrush(XColors.DarkBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkCyan => new XSolidBrush(XColors.DarkCyan, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGoldenrod => new XSolidBrush(XColors.DarkGoldenrod, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGray => new XSolidBrush(XColors.DarkGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGreen => new XSolidBrush(XColors.DarkGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkKhaki => new XSolidBrush(XColors.DarkKhaki, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkMagenta => new XSolidBrush(XColors.DarkMagenta, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOliveGreen => new XSolidBrush(XColors.DarkOliveGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOrange => new XSolidBrush(XColors.DarkOrange, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOrchid => new XSolidBrush(XColors.DarkOrchid, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkRed => new XSolidBrush(XColors.DarkRed, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSalmon => new XSolidBrush(XColors.DarkSalmon, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSeaGreen => new XSolidBrush(XColors.DarkSeaGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSlateBlue => new XSolidBrush(XColors.DarkSlateBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSlateGray => new XSolidBrush(XColors.DarkSlateGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkTurquoise => new XSolidBrush(XColors.DarkTurquoise, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkViolet => new XSolidBrush(XColors.DarkViolet, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DeepPink => new XSolidBrush(XColors.DeepPink, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DeepSkyBlue => new XSolidBrush(XColors.DeepSkyBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DimGray => new XSolidBrush(XColors.DimGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DodgerBlue => new XSolidBrush(XColors.DodgerBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Firebrick => new XSolidBrush(XColors.Firebrick, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush FloralWhite => new XSolidBrush(XColors.FloralWhite, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush ForestGreen => new XSolidBrush(XColors.ForestGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Fuchsia => new XSolidBrush(XColors.Fuchsia, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gainsboro => new XSolidBrush(XColors.Gainsboro, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush GhostWhite => new XSolidBrush(XColors.GhostWhite, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gold => new XSolidBrush(XColors.Gold, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Goldenrod => new XSolidBrush(XColors.Goldenrod, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gray => new XSolidBrush(XColors.Gray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Green => new XSolidBrush(XColors.Green, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush GreenYellow => new XSolidBrush(XColors.GreenYellow, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Honeydew => new XSolidBrush(XColors.Honeydew, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush HotPink => new XSolidBrush(XColors.HotPink, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush IndianRed => new XSolidBrush(XColors.IndianRed, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Indigo => new XSolidBrush(XColors.Indigo, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Ivory => new XSolidBrush(XColors.Ivory, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Khaki => new XSolidBrush(XColors.Khaki, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Lavender => new XSolidBrush(XColors.Lavender, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LavenderBlush => new XSolidBrush(XColors.LavenderBlush, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LawnGreen => new XSolidBrush(XColors.LawnGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LemonChiffon => new XSolidBrush(XColors.LemonChiffon, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightBlue => new XSolidBrush(XColors.LightBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightCoral => new XSolidBrush(XColors.LightCoral, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightCyan => new XSolidBrush(XColors.LightCyan, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGoldenrodYellow => new XSolidBrush(XColors.LightGoldenrodYellow, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGray => new XSolidBrush(XColors.LightGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGreen => new XSolidBrush(XColors.LightGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightPink => new XSolidBrush(XColors.LightPink, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSalmon => new XSolidBrush(XColors.LightSalmon, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSeaGreen => new XSolidBrush(XColors.LightSeaGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSkyBlue => new XSolidBrush(XColors.LightSkyBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSlateGray => new XSolidBrush(XColors.LightSlateGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSteelBlue => new XSolidBrush(XColors.LightSteelBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightYellow => new XSolidBrush(XColors.LightYellow, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Lime => new XSolidBrush(XColors.Lime, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LimeGreen => new XSolidBrush(XColors.LimeGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Linen => new XSolidBrush(XColors.Linen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Magenta => new XSolidBrush(XColors.Magenta, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Maroon => new XSolidBrush(XColors.Maroon, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumAquamarine => new XSolidBrush(XColors.MediumAquamarine, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumBlue => new XSolidBrush(XColors.MediumBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumOrchid => new XSolidBrush(XColors.MediumOrchid, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumPurple => new XSolidBrush(XColors.MediumPurple, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSeaGreen => new XSolidBrush(XColors.MediumSeaGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSlateBlue => new XSolidBrush(XColors.MediumSlateBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSpringGreen => new XSolidBrush(XColors.MediumSpringGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumTurquoise => new XSolidBrush(XColors.MediumTurquoise, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumVioletRed => new XSolidBrush(XColors.MediumVioletRed, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MidnightBlue => new XSolidBrush(XColors.MidnightBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MintCream => new XSolidBrush(XColors.MintCream, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MistyRose => new XSolidBrush(XColors.MistyRose, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Moccasin => new XSolidBrush(XColors.Moccasin, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush NavajoWhite => new XSolidBrush(XColors.NavajoWhite, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Navy => new XSolidBrush(XColors.Navy, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OldLace => new XSolidBrush(XColors.OldLace, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Olive => new XSolidBrush(XColors.Olive, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OliveDrab => new XSolidBrush(XColors.OliveDrab, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Orange => new XSolidBrush(XColors.Orange, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OrangeRed => new XSolidBrush(XColors.OrangeRed, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Orchid => new XSolidBrush(XColors.Orchid, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleGoldenrod => new XSolidBrush(XColors.PaleGoldenrod, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleGreen => new XSolidBrush(XColors.PaleGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleTurquoise => new XSolidBrush(XColors.PaleTurquoise, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleVioletRed => new XSolidBrush(XColors.PaleVioletRed, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PapayaWhip => new XSolidBrush(XColors.PapayaWhip, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PeachPuff => new XSolidBrush(XColors.PeachPuff, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Peru => new XSolidBrush(XColors.Peru, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Pink => new XSolidBrush(XColors.Pink, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Plum => new XSolidBrush(XColors.Plum, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PowderBlue => new XSolidBrush(XColors.PowderBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Purple => new XSolidBrush(XColors.Purple, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Red => new XSolidBrush(XColors.Red, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush RosyBrown => new XSolidBrush(XColors.RosyBrown, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush RoyalBlue => new XSolidBrush(XColors.RoyalBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SaddleBrown => new XSolidBrush(XColors.SaddleBrown, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Salmon => new XSolidBrush(XColors.Salmon, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SandyBrown => new XSolidBrush(XColors.SandyBrown, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SeaGreen => new XSolidBrush(XColors.SeaGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SeaShell => new XSolidBrush(XColors.SeaShell, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Sienna => new XSolidBrush(XColors.Sienna, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Silver => new XSolidBrush(XColors.Silver, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SkyBlue => new XSolidBrush(XColors.SkyBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SlateBlue => new XSolidBrush(XColors.SlateBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SlateGray => new XSolidBrush(XColors.SlateGray, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Snow => new XSolidBrush(XColors.Snow, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SpringGreen => new XSolidBrush(XColors.SpringGreen, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SteelBlue => new XSolidBrush(XColors.SteelBlue, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Tan => new XSolidBrush(XColors.Tan, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Teal => new XSolidBrush(XColors.Teal, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Thistle => new XSolidBrush(XColors.Thistle, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Tomato => new XSolidBrush(XColors.Tomato, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Transparent => new XSolidBrush(XColors.Transparent, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Turquoise => new XSolidBrush(XColors.Turquoise, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Violet => new XSolidBrush(XColors.Violet, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Wheat => new XSolidBrush(XColors.Wheat, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush White => new XSolidBrush(XColors.White, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush WhiteSmoke => new XSolidBrush(XColors.WhiteSmoke, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Yellow => new XSolidBrush(XColors.Yellow, true);

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush YellowGreen => new XSolidBrush(XColors.YellowGreen, true);

#if USE_CACHE
        static XSolidBrush _aliceBlue;
        static XSolidBrush _antiqueWhite;
        static XSolidBrush _aqua;
        static XSolidBrush _aquamarine;
        static XSolidBrush _azure;
        static XSolidBrush _beige;
        static XSolidBrush _bisque;
        static XSolidBrush _black;
        static XSolidBrush _blanchedAlmond;
        static XSolidBrush _blue;
        static XSolidBrush _blueViolet;
        static XSolidBrush _brown;
        static XSolidBrush _burlyWood;
        static XSolidBrush _cadetBlue;
        static XSolidBrush _chartreuse;
        static XSolidBrush _chocolate;
        static XSolidBrush _coral;
        static XSolidBrush _cornflowerBlue;
        static XSolidBrush _cornsilk;
        static XSolidBrush _crimson;
        static XSolidBrush _cyan;
        static XSolidBrush _darkBlue;
        static XSolidBrush _darkCyan;
        static XSolidBrush _darkGoldenrod;
        static XSolidBrush _darkGray;
        static XSolidBrush _darkGreen;
        static XSolidBrush _darkKhaki;
        static XSolidBrush _darkMagenta;
        static XSolidBrush _darkOliveGreen;
        static XSolidBrush _darkOrange;
        static XSolidBrush _darkOrchid;
        static XSolidBrush _darkRed;
        static XSolidBrush _darkSalmon;
        static XSolidBrush _darkSeaGreen;
        static XSolidBrush _darkSlateBlue;
        static XSolidBrush _darkSlateGray;
        static XSolidBrush _darkTurquoise;
        static XSolidBrush _darkViolet;
        static XSolidBrush _deepPink;
        static XSolidBrush _deepSkyBlue;
        static XSolidBrush _dimGray;
        static XSolidBrush _dodgerBlue;
        static XSolidBrush _firebrick;
        static XSolidBrush _floralWhite;
        static XSolidBrush _forestGreen;
        static XSolidBrush _fuchsia;
        static XSolidBrush _gainsboro;
        static XSolidBrush _ghostWhite;
        static XSolidBrush _gold;
        static XSolidBrush _goldenrod;
        static XSolidBrush _gray;
        static XSolidBrush _green;
        static XSolidBrush _greenYellow;
        static XSolidBrush _honeydew;
        static XSolidBrush _hotPink;
        static XSolidBrush _indianRed;
        static XSolidBrush _indigo;
        static XSolidBrush _ivory;
        static XSolidBrush _khaki;
        static XSolidBrush _lavender;
        static XSolidBrush _lavenderBlush;
        static XSolidBrush _lawnGreen;
        static XSolidBrush _lemonChiffon;
        static XSolidBrush _lightBlue;
        static XSolidBrush _lightCoral;
        static XSolidBrush _lightCyan;
        static XSolidBrush _lightGoldenrodYellow;
        static XSolidBrush _lightGray;
        static XSolidBrush _lightGreen;
        static XSolidBrush _lightPink;
        static XSolidBrush _lightSalmon;
        static XSolidBrush _lightSeaGreen;
        static XSolidBrush _lightSkyBlue;
        static XSolidBrush _lightSlateGray;
        static XSolidBrush _lightSteelBlue;
        static XSolidBrush _lightYellow;
        static XSolidBrush _lime;
        static XSolidBrush _limeGreen;
        static XSolidBrush _linen;
        static XSolidBrush _magenta;
        static XSolidBrush _maroon;
        static XSolidBrush _mediumAquamarine;
        static XSolidBrush _mediumBlue;
        static XSolidBrush _mediumOrchid;
        static XSolidBrush _mediumPurple;
        static XSolidBrush _mediumSeaGreen;
        static XSolidBrush _mediumSlateBlue;
        static XSolidBrush _mediumSpringGreen;
        static XSolidBrush _mediumTurquoise;
        static XSolidBrush _mediumVioletRed;
        static XSolidBrush _midnightBlue;
        static XSolidBrush _mintCream;
        static XSolidBrush _mistyRose;
        static XSolidBrush _moccasin;
        static XSolidBrush _navajoWhite;
        static XSolidBrush _navy;
        static XSolidBrush _oldLace;
        static XSolidBrush _olive;
        static XSolidBrush _oliveDrab;
        static XSolidBrush _orange;
        static XSolidBrush _orangeRed;
        static XSolidBrush _orchid;
        static XSolidBrush _paleGoldenrod;
        static XSolidBrush _paleGreen;
        static XSolidBrush _paleTurquoise;
        static XSolidBrush _paleVioletRed;
        static XSolidBrush _papayaWhip;
        static XSolidBrush _peachPuff;
        static XSolidBrush _peru;
        static XSolidBrush _pink;
        static XSolidBrush _plum;
        static XSolidBrush _powderBlue;
        static XSolidBrush _purple;
        static XSolidBrush _red;
        static XSolidBrush _rosyBrown;
        static XSolidBrush _royalBlue;
        static XSolidBrush _saddleBrown;
        static XSolidBrush _salmon;
        static XSolidBrush _sandyBrown;
        static XSolidBrush _seaGreen;
        static XSolidBrush _seaShell;
        static XSolidBrush _sienna;
        static XSolidBrush _silver;
        static XSolidBrush _skyBlue;
        static XSolidBrush _slateBlue;
        static XSolidBrush _slateGray;
        static XSolidBrush _snow;
        static XSolidBrush _springGreen;
        static XSolidBrush _steelBlue;
        static XSolidBrush _tan;
        static XSolidBrush _teal;
        static XSolidBrush _thistle;
        static XSolidBrush _tomato;
        static XSolidBrush _transparent;
        static XSolidBrush _turquoise;
        static XSolidBrush _violet;
        static XSolidBrush _wheat;
        static XSolidBrush _white;
        static XSolidBrush _whiteSmoke;
        static XSolidBrush _yellow;
        static XSolidBrush _yellowGreen;
#endif
    }
}
