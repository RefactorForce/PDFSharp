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
using System.Collections.Generic;
using System.Reflection;

namespace PDFSharp.Interop
{
    /// <summary>
    /// Holds information about the value of a key in a dictionary. This information is used to create
    /// and interpret this value.
    /// </summary>
    internal sealed class KeyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of KeyDescriptor from the specified attribute during a KeysMeta
        /// initializes itself using reflection.
        /// </summary>
        public KeyDescriptor(KeyInfoAttribute attribute)
        {
            Version = attribute.Version;
            KeyType = attribute.KeyType;
            FixedValue = attribute.FixedValue;
            ObjectType = attribute.ObjectType;

            if (Version == "")
                Version = "1.0";
        }

        /// <summary>
        /// Gets or sets the PDF version starting with the availability of the described key.
        /// </summary>
        public string Version { get; set; }

        public KeyType KeyType { get; set; }

        public string KeyValue { get; set; }

        public string FixedValue { get; }

        public Type ObjectType { get; set; }

        public bool CanBeIndirect => (KeyType & KeyType.MustNotBeIndirect) == 0;

        /// <summary>
        /// Returns the type of the object to be created as value for the described key.
        /// </summary>
        public Type GetValueType()
        {
            Type type = ObjectType;
            if (type == null)
            {
                // If we have no ObjectType specified, use the KeyType enumeration.
                switch (KeyType & KeyType.TypeMask)
                {
                    case KeyType.Name:
                        type = typeof(PDFName);
                        break;

                    case KeyType.String:
                        type = typeof(PDFString);
                        break;

                    case KeyType.Boolean:
                        type = typeof(PDFBoolean);
                        break;

                    case KeyType.Integer:
                        type = typeof(PDFInteger);
                        break;

                    case KeyType.Real:
                        type = typeof(PDFReal);
                        break;

                    case KeyType.Date:
                        type = typeof(PDFDate);
                        break;

                    case KeyType.Rectangle:
                        type = typeof(PDFRectangle);
                        break;

                    case KeyType.Array:
                        type = typeof(PDFArray);
                        break;

                    case KeyType.Dictionary:
                        type = typeof(PDFDictionary);
                        break;

                    case KeyType.Stream:
                        type = typeof(PDFDictionary);
                        break;

                    // The following types are not yet used

                    case KeyType.NumberTree:
                        throw new NotImplementedException("KeyType.NumberTree");

                    case KeyType.NameOrArray:
                        throw new NotImplementedException("KeyType.NameOrArray");

                    case KeyType.ArrayOrDictionary:
                        throw new NotImplementedException("KeyType.ArrayOrDictionary");

                    case KeyType.StreamOrArray:
                        throw new NotImplementedException("KeyType.StreamOrArray");

                    case KeyType.ArrayOrNameOrString:
                        return null; // HACK: Make PDFOutline work
                                     //throw new NotImplementedException("KeyType.ArrayOrNameOrString");

                    default:
                        Debug.Assert(false, "Invalid KeyType: " + KeyType);
                        break;
                }
            }
            return type;
        }
    }

    /// <summary>
    /// Contains meta information about all keys of a PDF dictionary.
    /// </summary>
    internal class DictionaryMeta
    {
        public DictionaryMeta(Type type)
        {
            //#if (NETFX_CORE && DEBUG) || CORE
            //            if (type == typeof(PDFPages.Keys))
            //            {
            //                var x = typeof(PDFPages).GetRuntimeFields();
            //                var y = typeof(PDFPages).GetTypeInfo().DeclaredFields;
            //                x.GetType();
            //                y.GetType();
            //                Debug-Break.Break();
            //                Test.It();
            //            }
            //#endif
#if !NETFX_CORE && !UWP
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in fields)
            {
                object[] attributes = field.GetCustomAttributes(typeof(KeyInfoAttribute), false);
                if (attributes.Length == 1)
                {
                    KeyInfoAttribute attribute = (KeyInfoAttribute)attributes[0];
                    KeyDescriptor descriptor = new KeyDescriptor(attribute)
                    {
                        KeyValue = (string)field.GetValue(null)
                    };
                    _keyDescriptors[descriptor.KeyValue] = descriptor;
                }
            }
#else
            // Rewritten for WinRT.
            CollectKeyDescriptors(type);
            //var fields = type.GetRuntimeFields();  // does not work
            //fields2.GetType();
            //foreach (FieldInfo field in fields)
            //{
            //    var attributes = field.GetCustomAttributes(typeof(KeyInfoAttribute), false);
            //    foreach (var attribute in attributes)
            //    {
            //        KeyDescriptor descriptor = new KeyDescriptor((KeyInfoAttribute)attribute);
            //        descriptor.KeyValue = (string)field.GetValue(null);
            //        _keyDescriptors[descriptor.KeyValue] = descriptor;
            //    }
            //}
#endif
        }

#if NETFX_CORE || UWP
        // Background: The function GetRuntimeFields gets constant fields only for the specified type,
        // not for its base types. So we have to walk recursively through base classes.
        // The docmentation says full trust for the immediate caller is required for property BaseClass.
        // TODO: Rewrite this stuff for medium trust.
        void CollectKeyDescriptors(Type type)
        {
            // Get fields of the specified type only.
            var fields = type.GetTypeInfo().DeclaredFields;
            foreach (FieldInfo field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(KeyInfoAttribute), false);
                foreach (var attribute in attributes)
                {
                    KeyDescriptor descriptor = new KeyDescriptor((KeyInfoAttribute)attribute);
                    descriptor.KeyValue = (string)field.GetValue(null);
                    _keyDescriptors[descriptor.KeyValue] = descriptor;
                }
            }
            type = type.GetTypeInfo().BaseType;
            if (type != typeof(object) && type != typeof(PDFObject))
                CollectKeyDescriptors(type);
        }
#endif

#if (NETFX_CORE || CORE) && true_
        public class A
        {
            public string _a;
            public const string _ca = "x";
        }
        public class B : A
        {
            public string _b;
            public const string _cb = "x";

            void Foo()
            {
                var str = A._ca;
            }
        }
        class Test
        {
            public static void It()
            {
                string s = "Runtime fields of B:";
                foreach (var fieldInfo in typeof(B).GetRuntimeFields()) { s += " " + fieldInfo.Name; }
                Debug.WriteLine(s);

                s = "Declared fields of B:";
                foreach (var fieldInfo in typeof(B).GetTypeInfo().DeclaredFields) { s += " " + fieldInfo.Name; }
                Debug.WriteLine(s);

                s = "Runtime fields of PDFPages.Keys:";
                foreach (var fieldInfo in typeof(PDFPages.Keys).GetRuntimeFields()) { s += " " + fieldInfo.Name; }
                Debug.WriteLine(s);
            }
        }
#endif
        /// <summary>
        /// Gets the KeyDescriptor of the specified key, or null if no such descriptor exits.
        /// </summary>
        public KeyDescriptor this[string key]
        {
            get
            {
                _keyDescriptors.TryGetValue(key, out KeyDescriptor keyDescriptor);
                return keyDescriptor;
            }
        }

        readonly Dictionary<string, KeyDescriptor> _keyDescriptors = new Dictionary<string, KeyDescriptor>();
    }
}
