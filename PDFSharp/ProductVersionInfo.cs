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

namespace PDFSharp
{
    /// <summary>
    /// Version info base for all PDFSharp related assemblies.
    /// </summary>
    public static class VersionMetadata
    {
        /// <summary>
        /// The title of the product.
        /// </summary>
        public static string Title { get; } = nameof(PDFSharp);

        /// <summary>
        /// The URI to the product's homepage.
        /// </summary>
        public static string ProjectURI { get; } = "https://github.com/RefactorForce/PDFSharp";
        /// <summary>
        /// The PDF producer information string including VersionPatch.
        /// </summary>
        public static string Header { get; } = $"{Title} {Version} ({ProjectURI})";

        /// <summary>
        /// The full version number.
        /// </summary>
        public static string Version { get; } = $"{VersionMajor}.{VersionMinor}.{VersionPatch}-{VersionBuild}-{VersionPrerelease}";

        /// <summary>
        /// The major version number of the product.
        /// </summary>
        public static string VersionMajor { get; } = "2";

        /// <summary>
        /// The minor version number of the product.
        /// </summary>
        public static string VersionMinor { get; } = "0";

        /// <summary>
        /// The build number of the product.
        /// </summary>
        public static string VersionBuild { get; } = "20181110";

        /// <summary>
        /// The patch number of the product.
        /// </summary>
        public static string VersionPatch { get; } = "0";

        /// <summary>
        /// The Version Prerelease String for NuGet.
        /// </summary>
        public static string VersionPrerelease { get; } = "beta1";
    }
}
