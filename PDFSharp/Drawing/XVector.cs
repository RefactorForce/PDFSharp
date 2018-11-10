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
using System.Globalization;
using System.Runtime.InteropServices;
using PDFSharp.Internal;
#if GDI
using System.DrawingCore;
#endif
#if WPF
using System.Windows.Media;
#endif

#pragma warning disable 1591

#if !EDF_CORE
namespace PDFSharp.Drawing
#else
namespace Edf.Drawing
#endif
{
    /// <summary>
    /// Represents a two-dimensional vector specified by x- and y-coordinates.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XVector : IFormattable
    {
        public XVector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(XVector vector1, XVector vector2) =>
            // ReSharper disable CompareOfFloatsByEqualityOperator
            vector1.X == vector2.X && vector1.Y == vector2.Y;// ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool operator !=(XVector vector1, XVector vector2) =>
            // ReSharper disable CompareOfFloatsByEqualityOperator
            vector1.X != vector2.X || vector1.Y != vector2.Y;// ReSharper restore CompareOfFloatsByEqualityOperator

        public static bool Equals(XVector vector1, XVector vector2) => vector1.X.Equals(vector2.X) ? vector1.Y.Equals(vector2.Y) : false;

        public override bool Equals(object o) => !(o is XVector) ? false : Equals(this, (XVector)o);

        public bool Equals(XVector value) => Equals(this, value);

        public override int GetHashCode() =>
            // ReSharper disable NonReadonlyFieldInGetHashCode
            X.GetHashCode() ^ Y.GetHashCode();// ReSharper restore NonReadonlyFieldInGetHashCode

        public static XVector Parse(string source)
        {
            TokenizerHelper helper = new TokenizerHelper(source, CultureInfo.InvariantCulture);
            string str = helper.NextTokenRequired();
            XVector vector = new XVector(Convert.ToDouble(str, CultureInfo.InvariantCulture), Convert.ToDouble(helper.NextTokenRequired(), CultureInfo.InvariantCulture));
            helper.LastTokenRequired();
            return vector;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public override string ToString() => ConvertToString(null, null);

        public string ToString(IFormatProvider provider) => ConvertToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => ConvertToString(format, provider);

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            const char numericListSeparator = ',';
            provider = provider ?? CultureInfo.InvariantCulture;
            // ReSharper disable once FormatStringProblem
            return String.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", numericListSeparator, X, Y);
        }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public double LengthSquared => X * X + Y * Y;

        public void Normalize()
        {
            this = this / Math.Max(Math.Abs(X), Math.Abs(Y));
            this = this / Length;
        }

        public static double CrossProduct(XVector vector1, XVector vector2) => vector1.X * vector2.Y - vector1.Y * vector2.X;

        public static double AngleBetween(XVector vector1, XVector vector2)
        {
            double y = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double x = vector1.X * vector2.X + vector1.Y * vector2.Y;
            return Math.Atan2(y, x) * 57.295779513082323;
        }

        public static XVector operator -(XVector vector) => new XVector(-vector.X, -vector.Y);

        public void Negate()
        {
            X = -X;
            Y = -Y;
        }

        public static XVector operator +(XVector vector1, XVector vector2) => new XVector(vector1.X + vector2.X, vector1.Y + vector2.Y);

        public static XVector Add(XVector vector1, XVector vector2) => new XVector(vector1.X + vector2.X, vector1.Y + vector2.Y);

        public static XVector operator -(XVector vector1, XVector vector2) => new XVector(vector1.X - vector2.X, vector1.Y - vector2.Y);

        public static XVector Subtract(XVector vector1, XVector vector2) => new XVector(vector1.X - vector2.X, vector1.Y - vector2.Y);

        public static XPoint operator +(XVector vector, XPoint point) => new XPoint(point.X + vector.X, point.Y + vector.Y);

        public static XPoint Add(XVector vector, XPoint point) => new XPoint(point.X + vector.X, point.Y + vector.Y);

        public static XVector operator *(XVector vector, double scalar) => new XVector(vector.X * scalar, vector.Y * scalar);

        public static XVector Multiply(XVector vector, double scalar) => new XVector(vector.X * scalar, vector.Y * scalar);

        public static XVector operator *(double scalar, XVector vector) => new XVector(vector.X * scalar, vector.Y * scalar);

        public static XVector Multiply(double scalar, XVector vector) => new XVector(vector.X * scalar, vector.Y * scalar);

        public static XVector operator /(XVector vector, double scalar) => vector * (1.0 / scalar);

        public static XVector Divide(XVector vector, double scalar) => vector * (1.0 / scalar);

        public static XVector operator *(XVector vector, XMatrix matrix) => matrix.Transform(vector);

        public static XVector Multiply(XVector vector, XMatrix matrix) => matrix.Transform(vector);

        public static double operator *(XVector vector1, XVector vector2) => vector1.X * vector2.X + vector1.Y * vector2.Y;

        public static double Multiply(XVector vector1, XVector vector2) => vector1.X * vector2.X + vector1.Y * vector2.Y;

        public static double Determinant(XVector vector1, XVector vector2) => vector1.X * vector2.Y - vector1.Y * vector2.X;

        public static explicit operator XSize(XVector vector) => new XSize(Math.Abs(vector.X), Math.Abs(vector.Y));

        public static explicit operator XPoint(XVector vector) => new XPoint(vector.X, vector.Y);

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        /// <value>The debugger display.</value>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                const string format = Config.SignificantFigures10;
                return String.Format(CultureInfo.InvariantCulture, "vector=({0:" + format + "}, {1:" + format + "})", X, Y);
            }
        }
    }
}
