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

using System.Collections.Generic;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Contains all used ExtGState objects of a document.
    /// </summary>
    public sealed class PDFExtGStateTable : PDFResourceTable
    {
        /// <summary>
        /// Initializes a new instance of this class, which is a singleton for each document.
        /// </summary>
        public PDFExtGStateTable(PDFDocument document)
            : base(document)
        { }


        /// <summary>
        /// Gets a PDFExtGState with the key 'CA' set to the specified alpha value.
        /// </summary>
        public PDFExtGState GetExtGStateStroke(double alpha, bool overprint)
        {
            string key = PDFExtGState.MakeKey(alpha, overprint);
            if (!_strokeAlphaValues.TryGetValue(key, out PDFExtGState extGState))
            {
                extGState = new PDFExtGState(Owner)
                {
                    //extGState.Elements[PDFExtGState.Keys.CA] = new PDFReal(alpha);
                    StrokeAlpha = alpha
                };
                if (overprint)
                {
                    extGState.StrokeOverprint = true;
                    extGState.Elements.SetInteger(PDFExtGState.Keys.OPM, 1);
                }
                _strokeAlphaValues[key] = extGState;
            }
            return extGState;
        }

        /// <summary>
        /// Gets a PDFExtGState with the key 'ca' set to the specified alpha value.
        /// </summary>
        public PDFExtGState GetExtGStateNonStroke(double alpha, bool overprint)
        {
            string key = PDFExtGState.MakeKey(alpha, overprint);
            if (!_nonStrokeStates.TryGetValue(key, out PDFExtGState extGState))
            {
                extGState = new PDFExtGState(Owner)
                {
                    //extGState.Elements[PDFExtGState.Keys.ca] = new PDFReal(alpha);
                    NonStrokeAlpha = alpha
                };
                if (overprint)
                {
                    extGState.NonStrokeOverprint = true;
                    extGState.Elements.SetInteger(PDFExtGState.Keys.OPM, 1);
                }

                _nonStrokeStates[key] = extGState;
            }
            return extGState;
        }

        readonly Dictionary<string, PDFExtGState> _strokeAlphaValues = new Dictionary<string, PDFExtGState>();
        readonly Dictionary<string, PDFExtGState> _nonStrokeStates = new Dictionary<string, PDFExtGState>();
    }
}