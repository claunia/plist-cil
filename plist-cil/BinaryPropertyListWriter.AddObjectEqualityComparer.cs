// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2015-2025 Natalia Portillo
//
// This code is based on:
// plist - An open source library to parse and generate property lists
// Copyright (C) 2014 Daniel Dreibrodt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;

namespace Claunia.PropertyList;

public partial class BinaryPropertyListWriter
{
    /// <summary>
    ///     The equality comparer which is used when adding an object to the <see cref="BinaryPropertyListWriter.idMap" />
    ///     . In most cases, objects are always added. The only exception are very specific strings, which are only added once.
    /// </summary>
    class AddObjectEqualityComparer : EqualityComparer<NSObject>
    {
        public override bool Equals(NSObject x, NSObject y)
        {
            if(x is not NSString a || y is not NSString b) return ReferenceEquals(x, y);

            if(!IsSerializationPrimitive(a) || !IsSerializationPrimitive(b)) return ReferenceEquals(x, y);

            return string.Equals(a.Content, b.Content, StringComparison.Ordinal);
        }

        public override int GetHashCode(NSObject obj)
        {
            if(obj is NSString s && IsSerializationPrimitive(s)) return s.Content.GetHashCode();

            return obj.GetHashCode();
        }
    }
}