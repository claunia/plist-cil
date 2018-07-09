using System;
using System.Collections.Generic;

namespace Claunia.PropertyList
{
    public partial class BinaryPropertyListWriter
    {
        /// <summary>
        ///     The equality comparer which is used when adding an object to the <see cref="BinaryPropertyListWriter.idMap" />. In
        ///     most cases,
        ///     objects are always added. The only exception are very specific strings, which are only added once.
        /// </summary>
        class AddObjectEqualityComparer : EqualityComparer<NSObject>
        {
            public override bool Equals(NSObject x, NSObject y)
            {
                NSString a = x as NSString;
                NSString b = y as NSString;

                if(a == null || b == null) return ReferenceEquals(x, y);

                if(!IsSerializationPrimitive(a) || !IsSerializationPrimitive(b)) return ReferenceEquals(x, y);

                return string.Equals(a.Content, b.Content, StringComparison.Ordinal);
            }

            public override int GetHashCode(NSObject obj)
            {
                if(obj == null) return 0;

                NSString s = obj as NSString;
                if(s != null && IsSerializationPrimitive(s)) return s.Content.GetHashCode();

                return obj.GetHashCode();
            }
        }
    }
}