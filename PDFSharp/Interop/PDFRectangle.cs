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
using System.Globalization;
#if GDI
using System.DrawingCore;
#endif
#if WPF
using System.Windows.Media;
#endif
using PDFSharp.Drawing;
using PDFSharp.Interop.Advanced;
using PDFSharp.Interop.IO;
using PDFSharp.Interop.Internal;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Represents a PDF rectangle value, that is internally an array with 4 real values.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed class PDFRectangle : PDFItem
    {
        // This class must behave like a value type. Therefore it cannot be changed (like System.String).

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class.
        /// </summary>
        public PDFRectangle()
        { }

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with two points specifying
        /// two diagonally opposite corners. Notice that in contrast to GDI+ convention the 
        /// 3rd and the 4th parameter specify a point and not a width. This is so much confusing
        /// that this function is for internal use only.
        /// </summary>
        internal PDFRectangle(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with two points specifying
        /// two diagonally opposite corners.
        /// </summary>
        public PDFRectangle(PointF pt1, PointF pt2)
        {
            _x1 = pt1.X;
            _y1 = pt1.Y;
            _x2 = pt2.X;
            _y2 = pt2.Y;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with two points specifying
        /// two diagonally opposite corners.
        /// </summary>
        public PDFRectangle(XPoint pt1, XPoint pt2)
        {
            X1 = pt1.X;
            Y1 = pt1.Y;
            X2 = pt2.X;
            Y2 = pt2.Y;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with the specified location and size.
        /// </summary>
        public PDFRectangle(PointF pt, SizeF size)
        {
            _x1 = pt.X;
            _y1 = pt.Y;
            _x2 = pt.X + size.Width;
            _y2 = pt.Y + size.Height;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with the specified location and size.
        /// </summary>
        public PDFRectangle(XPoint pt, XSize size)
        {
            X1 = pt.X;
            Y1 = pt.Y;
            X2 = pt.X + size.Width;
            Y2 = pt.Y + size.Height;
        }

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with the specified XRect.
        /// </summary>
        public PDFRectangle(XRect rect)
        {
            X1 = rect.X;
            Y1 = rect.Y;
            X2 = rect.X + rect.Width;
            Y2 = rect.Y + rect.Height;
        }

        /// <summary>
        /// Initializes a new instance of the PDFRectangle class with the specified PDFArray.
        /// </summary>
        internal PDFRectangle(PDFItem item)
        {
            if (item == null || item is PDFNull)
                return;

            if (item is PDFReference)
                item = ((PDFReference)item).Value;

            if (!(item is PDFArray array))
                throw new InvalidOperationException(PSSR.UnexpectedTokenInPDFFile);

            X1 = array.Elements.GetReal(0);
            Y1 = array.Elements.GetReal(1);
            X2 = array.Elements.GetReal(2);
            Y2 = array.Elements.GetReal(3);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        public new PDFRectangle Clone() => (PDFRectangle)Copy();

        /// <summary>
        /// Implements cloning this instance.
        /// </summary>
        protected override object Copy()
        {
            PDFRectangle rect = (PDFRectangle)base.Copy();
            return rect;
        }

        /// <summary>
        /// Tests whether all coordinate are zero.
        /// </summary>
        public bool IsEmpty => X1 == 0 && Y1 == 0 && X2 == 0 && Y2 == 0;

        /// <summary>
        /// Tests whether the specified object is a PDFRectangle and has equal coordinates.
        /// </summary>
        public override bool Equals(object obj)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            PDFRectangle rectangle = obj as PDFRectangle;
            if (rectangle != null)
            {
                PDFRectangle rect = rectangle;
                return rect.X1 == X1 && rect.Y1 == Y1 && rect.X2 == X2 && rect.Y2 == Y2;
            }
            return false;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode() =>
            // This code is from System.DrawingCore...
            (int)(((uint)X1) ^ ((((uint)Y1) << 13) |
              (((uint)Y1) >> 0x13)) ^ ((((uint)X2) << 0x1a) |
              (((uint)X2) >> 6)) ^ ((((uint)Y2) << 7) |
              (((uint)Y2) >> 0x19)));

        /// <summary>
        /// Tests whether two structures have equal coordinates.
        /// </summary>
        public static bool operator ==(PDFRectangle left, PDFRectangle right) =>
            // ReSharper disable CompareOfFloatsByEqualityOperator
            // use: if (Object.ReferenceEquals(left, null))
            !(left is null)
                ? !(right is null) ? left.X1 == right.X1 && left.Y1 == right.Y1 && left.X2 == right.X2 && left.Y2 == right.Y2 : false
                : right is null;// ReSharper restore CompareOfFloatsByEqualityOperator

        /// <summary>
        /// Tests whether two structures differ in one or more coordinates.
        /// </summary>
        public static bool operator !=(PDFRectangle left, PDFRectangle right) => !(left == right);

        /// <summary>
        /// Gets or sets the x-coordinate of the first corner of this PDFRectangle.
        /// </summary>
        public double X1 { get; }

        /// <summary>
        /// Gets or sets the y-coordinate of the first corner of this PDFRectangle.
        /// </summary>
        public double Y1 { get; }

        /// <summary>
        /// Gets or sets the x-coordinate of the second corner of this PDFRectangle.
        /// </summary>
        public double X2 { get; }

        /// <summary>
        /// Gets or sets the y-coordinate of the second corner of this PDFRectangle.
        /// </summary>
        public double Y2 { get; }

        /// <summary>
        /// Gets X2 - X1.
        /// </summary>
        public double Width => X2 - X1;

        /// <summary>
        /// Gets Y2 - Y1.
        /// </summary>
        public double Height => Y2 - Y1;

        /// <summary>
        /// Gets or sets the coordinates of the first point of this PDFRectangle.
        /// </summary>
        public XPoint Location => new XPoint(X1, Y1);

        /// <summary>
        /// Gets or sets the size of this PDFRectangle.
        /// </summary>
        public XSize Size => new XSize(X2 - X1, Y2 - Y1);

#if GDI
        /// <summary>
        /// Determines if the specified point is contained within this PDFRectangle.
        /// </summary>
        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }
#endif

        /// <summary>
        /// Determines if the specified point is contained within this PDFRectangle.
        /// </summary>
        public bool Contains(XPoint pt) => Contains(pt.X, pt.Y);

        /// <summary>
        /// Determines if the specified point is contained within this PDFRectangle.
        /// </summary>
        public bool Contains(double x, double y) =>
            // Treat rectangle inclusive/inclusive.
            X1 <= x && x <= X2 && Y1 <= y && y <= Y2;

#if GDI
        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PDFRectangle.
        /// </summary>
        public bool Contains(RectangleF rect)
        {
            return _x1 <= rect.X && (rect.X + rect.Width) <= _x2 &&
              _y1 <= rect.Y && (rect.Y + rect.Height) <= _y2;
        }
#endif

        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PDFRectangle.
        /// </summary>
        public bool Contains(XRect rect) => X1 <= rect.X && rect.X + rect.Width <= X2 &&
              Y1 <= rect.Y && rect.Y + rect.Height <= Y2;

        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PDFRectangle.
        /// </summary>
        public bool Contains(PDFRectangle rect) => X1 <= rect.X1 && rect.X2 <= X2 &&
              Y1 <= rect.Y1 && rect.Y2 <= Y2;

        /// <summary>
        /// Returns the rectangle as an XRect object.
        /// </summary>
        public XRect ToXRect() => new XRect(X1, Y1, Width, Height);

        /// <summary>
        /// Returns the rectangle as a string in the form «[x1 y1 x2 y2]».
        /// </summary>
        public override string ToString()
        {
            const string format = Config.SignificantFigures3;
            return PDFEncoders.Format("[{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}]", X1, Y1, X2, Y2);
        }

        /// <summary>
        /// Writes the rectangle.
        /// </summary>
        internal override void WriteObject(PDFWriter writer) => writer.Write(this);

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                const string format = Config.SignificantFigures10;
                return String.Format(CultureInfo.InvariantCulture,
                    "X1={0:" + format + "}, Y1={1:" + format + "}, X2={2:" + format + "}, Y2={3:" + format + "}", X1, Y1, X2, Y2);
            }
        }

#if false // This object is considered as immutable.
    //    /// <summary>
    //    /// Adjusts the location of this PDFRectangle by the specified amount.
    //    /// </summary>
    //    public void Offset(PointF pos)
    //    {
    //      Offset(pos.X, pos.Y);
    //    }
    //
    //    /// <summary>
    //    /// Adjusts the location of this PDFRectangle by the specified amount.
    //    /// </summary>
    //    public void Offset(double x, double y)
    //    {
    //      _x1 += x;
    //      _y1 += y;
    //      _x2 += x;
    //      _y2 += y;
    //    }
    //
    //    /// <summary>
    //    /// Inflates this PDFRectangle by the specified amount.
    //    /// </summary>
    //    public void Inflate(double x, double y)
    //    {
    //      _x1 -= x;
    //      _y1 -= y;
    //      _x2 += x;
    //      _y2 += y;
    //    }
    //
    //    /// <summary>
    //    /// Inflates this PDFRectangle by the specified amount.
    //    /// </summary>
    //    public void Inflate(SizeF size)
    //    {
    //      Inflate(size.Width, size.Height);
    //    }
    //
    //    /// <summary>
    //    /// Creates and returns an inflated copy of the specified PDFRectangle.
    //    /// </summary>
    //    public static PDFRectangle Inflate(PDFRectangle rect, double x, double y)
    //    {
    //      rect.Inflate(x, y);
    //      return rect;
    //    }
    //
    //    /// <summary>
    //    /// Replaces this PDFRectangle with the intersection of itself and the specified PDFRectangle.
    //    /// </summary>
    //    public void Intersect(PDFRectangle rect)
    //    {
    //      PDFRectangle rect2 = PDFRectangle.Intersect(rect, this);
    //      _x1 = rect2.x1;
    //      _y1 = rect2.y1;
    //      _x2 = rect2.x2;
    //      _y2 = rect2.y2;
    //    }
    //
    //    /// <summary>
    //    /// Returns a PDFRectangle that represents the intersection of two rectangles. If there is no intersection,
    //    /// an empty PDFRectangle is returned.
    //    /// </summary>
    //    public static PDFRectangle Intersect(PDFRectangle rect1, PDFRectangle rect2)
    //    {
    //      double xx1 = Math.Max(rect1.x1, rect2.x1);
    //      double xx2 = Math.Min(rect1.x2, rect2.x2);
    //      double yy1 = Math.Max(rect1.y1, rect2.y1);
    //      double yy2 = Math.Min(rect1.y2, rect2.y2);
    //      if (xx2 >= xx1 && yy2 >= yy1)
    //        return new PDFRectangle(xx1, yy1, xx2, yy2);
    //      return PDFRectangle.Empty;
    //    }
    //
    //    /// <summary>
    //    /// Determines if this rectangle intersects with the specified PDFRectangle.
    //    /// </summary>
    //    public bool IntersectsWith(PDFRectangle rect)
    //    {
    //      return rect.x1 < _x2 && _x1 < rect.x2 && rect.y1 < _y2 && _y1 < rect.y2;
    //    }
    //
    //    /// <summary>
    //    /// Creates the smallest rectangle that can contain both of two specified rectangles.
    //    /// </summary>
    //    public static PDFRectangle Union(PDFRectangle rect1, PDFRectangle rect2)
    //    {
    //      return new PDFRectangle(
    //        Math.Min(rect1.x1, rect2.x1), Math.Max(rect1.x2, rect2.x2),
    //        Math.Min(rect1.y1, rect2.y1), Math.Max(rect1.y2, rect2.y2));
    //    }
#endif

        /// <summary>
        /// Represents an empty PDFRectangle.
        /// </summary>
        public static readonly PDFRectangle Empty = new PDFRectangle();
    }
}
