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

using System.Collections.Generic;

namespace PDFSharp.Interop.Advanced
{
    /// <summary>
    /// Represents a PDF resource object.
    /// </summary>
    public sealed class PDFResources : PDFDictionary
    {
        // Resource management works roughly like this:
        // When the user creates an XFont and uses it in the XGraphics of a PDFPage, then at the first time
        // a PDFFont is created and cached in the document global font table. If the user creates a new
        // XFont object for an exisisting PDFFont, the PDFFont object is reused. When the PDFFont is added
        // to the resources of a PDFPage for the first time, it is added to the page local PDFResourceMap for 
        // fonts and automatically associated with a local resource name.

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFResources"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PDFResources(PDFDocument document)
            : base(document) => Elements[Keys.ProcSet] = new PDFLiteral("[/PDF/Text/ImageB/ImageC/ImageI]");

        internal PDFResources(PDFDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Adds the specified font to this resource dictionary and returns its local resource name.
        /// </summary>
        public string AddFont(PDFFont font)
        {
            if (!_resources.TryGetValue(font, out string name))
            {
                name = NextFontName;
                _resources[font] = name;
                if (font.Reference == null)
                    Owner.IrefTable.Add(font);
                Fonts.Elements[name] = font.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified image to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddImage(PDFImage image)
        {
            if (!_resources.TryGetValue(image, out string name))
            {
                name = NextImageName;
                _resources[image] = name;
                if (image.Reference == null)
                    Owner.IrefTable.Add(image);
                XObjects.Elements[name] = image.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified form object to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddForm(PDFFormXObject form)
        {
            if (!_resources.TryGetValue(form, out string name))
            {
                name = NextFormName;
                _resources[form] = name;
                if (form.Reference == null)
                    Owner.IrefTable.Add(form);
                XObjects.Elements[name] = form.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified graphics state to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddExtGState(PDFExtGState extGState)
        {
            if (!_resources.TryGetValue(extGState, out string name))
            {
                name = NextExtGStateName;
                _resources[extGState] = name;
                if (extGState.Reference == null)
                    Owner.IrefTable.Add(extGState);
                ExtGStates.Elements[name] = extGState.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified pattern to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddPattern(PDFShadingPattern pattern)
        {
            if (!_resources.TryGetValue(pattern, out string name))
            {
                name = NextPatternName;
                _resources[pattern] = name;
                if (pattern.Reference == null)
                    Owner.IrefTable.Add(pattern);
                Patterns.Elements[name] = pattern.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified pattern to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddPattern(PDFTilingPattern pattern)
        {
            if (!_resources.TryGetValue(pattern, out string name))
            {
                name = NextPatternName;
                _resources[pattern] = name;
                if (pattern.Reference == null)
                    Owner.IrefTable.Add(pattern);
                Patterns.Elements[name] = pattern.Reference;
            }
            return name;
        }

        /// <summary>
        /// Adds the specified shading to this resource dictionary
        /// and returns its local resource name.
        /// </summary>
        public string AddShading(PDFShading shading)
        {
            if (!_resources.TryGetValue(shading, out string name))
            {
                name = NextShadingName;
                _resources[shading] = name;
                if (shading.Reference == null)
                    Owner.IrefTable.Add(shading);
                Shadings.Elements[name] = shading.Reference;
            }
            return name;
        }

        /// <summary>
        /// Gets the fonts map.
        /// </summary>
        internal PDFResourceMap Fonts => _fonts ?? (_fonts = (PDFResourceMap)Elements.GetValue(Keys.Font, VCF.Create));
        PDFResourceMap _fonts;

        /// <summary>
        /// Gets the external objects map.
        /// </summary>
        internal PDFResourceMap XObjects => _xObjects ?? (_xObjects = (PDFResourceMap)Elements.GetValue(Keys.XObject, VCF.Create));
        PDFResourceMap _xObjects;

        // TODO: make own class
        internal PDFResourceMap ExtGStates => _extGStates ?? (_extGStates = (PDFResourceMap)Elements.GetValue(Keys.ExtGState, VCF.Create));
        PDFResourceMap _extGStates;

        // TODO: make own class
        internal PDFResourceMap ColorSpaces => _colorSpaces ?? (_colorSpaces = (PDFResourceMap)Elements.GetValue(Keys.ColorSpace, VCF.Create));
        PDFResourceMap _colorSpaces;

        // TODO: make own class
        internal PDFResourceMap Patterns => _patterns ?? (_patterns = (PDFResourceMap)Elements.GetValue(Keys.Pattern, VCF.Create));
        PDFResourceMap _patterns;

        // TODO: make own class
        internal PDFResourceMap Shadings => _shadings ?? (_shadings = (PDFResourceMap)Elements.GetValue(Keys.Shading, VCF.Create));
        PDFResourceMap _shadings;

        // TODO: make own class
        internal PDFResourceMap Properties => _properties ?? (_properties = (PDFResourceMap)Elements.GetValue(Keys.Properties, VCF.Create));
        PDFResourceMap _properties;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextFontName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/F{0}", _fontNumber++))) { }
                return name;
            }
        }
        int _fontNumber;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextImageName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/I{0}", _imageNumber++))) { }
                return name;
            }
        }
        int _imageNumber;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextFormName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/Fm{0}", _formNumber++))) { }
                return name;
            }
        }
        int _formNumber;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextExtGStateName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/GS{0}", _extGStateNumber++))) { }
                return name;
            }
        }
        int _extGStateNumber;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextPatternName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/Pa{0}", _patternNumber++))) ;
                return name;
            }
        }
        int _patternNumber;

        /// <summary>
        /// Gets a new local name for this resource.
        /// </summary>
        string NextShadingName
        {
            get
            {
                string name;
                while (ExistsResourceNames(name = System.String.Format("/Sh{0}", _shadingNumber++))) ;
                return name;
            }
        }
        int _shadingNumber;

        /// <summary>
        /// Check whether a resource name is already used in the context of this resource dictionary.
        /// PDF4NET uses GUIDs as resource names, but I think this weapon is to heavy.
        /// </summary>
        internal bool ExistsResourceNames(string name)
        {
            // TODO: more precise: is this page imported and is PageOptions != Replace
            // BUG: 
            //if (!Owner.IsImported)
            //  return false;

            // Collect all resouce names of all imported resources.
            if (_importedResourceNames == null)
            {
                _importedResourceNames = new Dictionary<string, object>();

                if (Elements[Keys.Font] != null)
                    Fonts.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.XObject] != null)
                    XObjects.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.ExtGState] != null)
                    ExtGStates.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.ColorSpace] != null)
                    ColorSpaces.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.Pattern] != null)
                    Patterns.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.Shading] != null)
                    Shadings.CollectResourceNames(_importedResourceNames);

                if (Elements[Keys.Properties] != null)
                    Properties.CollectResourceNames(_importedResourceNames);
            }
            return _importedResourceNames.ContainsKey(name);
            // This is superfluous because PDFSharp resource names cannot be double.
            // importedResourceNames.Add(name, null);
        }

        /// <summary>
        /// All the names of imported resources.
        /// </summary>
        Dictionary<string, object> _importedResourceNames;

        /// <summary>
        /// Maps all PDFSharp resources to their local resource names.
        /// </summary>
        readonly Dictionary<PDFObject, string> _resources = new Dictionary<PDFObject, string>();

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public sealed class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) A dictionary that maps resource names to graphics state 
            /// parameter dictionaries.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string ExtGState = "/ExtGState";

            /// <summary>
            /// (Optional) A dictionary that maps each resource name to either the name of a
            /// device-dependent color space or an array describing a color space.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string ColorSpace = "/ColorSpace";

            /// <summary>
            /// (Optional) A dictionary that maps each resource name to either the name of a
            /// device-dependent color space or an array describing a color space.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string Pattern = "/Pattern";

            /// <summary>
            /// (Optional; PDF 1.3) A dictionary that maps resource names to shading dictionaries.
            /// </summary>
            [KeyInfo("1.3", KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string Shading = "/Shading";

            /// <summary>
            /// (Optional) A dictionary that maps resource names to external objects.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string XObject = "/XObject";

            /// <summary>
            /// (Optional) A dictionary that maps resource names to font dictionaries.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string Font = "/Font";

            /// <summary>
            /// (Optional) An array of predefined procedure set names.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string ProcSet = "/ProcSet";

            /// <summary>
            /// (Optional; PDF 1.2) A dictionary that maps resource names to property list
            /// dictionaries for marked content.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PDFResourceMap))]
            public const string Properties = "/Properties";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta => _meta ?? (_meta = CreateMeta(typeof(Keys)));
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta => Keys.Meta;
    }
}
