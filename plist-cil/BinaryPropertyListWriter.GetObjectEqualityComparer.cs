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

using System.Collections.Generic;

namespace Claunia.PropertyList;

public partial class BinaryPropertyListWriter
{
    /// <summary>
    ///     The equality comparer which is used when retrieving objects in the
    ///     <see cref="BinaryPropertyListWriter.idMap" />. The logic is slightly different from
    ///     <see cref="AddObjectEqualityComparer" />, results in two equivalent objects (UIDs mainly) being added to the
    ///     <see cref="BinaryPropertyListWriter.idMap" />. Whenever the ID for one of those equivalent objects is requested,
    ///     the first ID is always returned. This means that there are "orphan" objects in binary property lists - duplicate
    ///     objects which are never referenced -; this logic exists purely to maintain binary compatibility with Apple's
    ///     format.
    /// </summary>
    class GetObjectEqualityComparer : EqualityComparer<NSObject>
    {
        public override bool Equals(NSObject x, NSObject y) => x switch
                                                               {
                                                                   // By default, use reference equality. Even if there are two objects - say a NSString - with the same
                                                                   // value, do not consider them equal unless they are the same instance of NSString.
                                                                   // The exceptions are UIDs, where we always compare by value, and "primitive" strings (a list of well-known
                                                                   // strings), which are treaded specially and "recycled".
                                                                   UID => x.Equals(y),
                                                                   NSNumber number when IsSerializationPrimitive(number)
                                                                       => number.Equals(y),
                                                                   NSString nsString when
                                                                       IsSerializationPrimitive(nsString) => nsString
                                                                          .Equals(y),
                                                                   _ => ReferenceEquals(x, y)
                                                               };

        public override int GetHashCode(NSObject obj) => obj switch
                                                         {
                                                             UID u => u.GetHashCode(),
                                                             NSNumber n when IsSerializationPrimitive(n) => n.ToObject()
                                                                .GetHashCode(),
                                                             NSString s when IsSerializationPrimitive(s) => s.Content
                                                                .GetHashCode(),
                                                             _ => obj.GetHashCode()
                                                         };
    }
}