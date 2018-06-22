using System;
using System.Collections.Generic;

namespace Claunia.PropertyList
{
    public partial class BinaryPropertyListWriter
    {
        /// <summary>
        /// The equality comparer which is used when adding an object to the <see cref="BinaryPropertyListWriter.idMap" />. In most cases,
        /// objects are always added. The only exception are very specific strings, which are only added once.
        /// </summary>
        private class AddObjectEqualityComparer : EqualityComparer<NSObject>
        {
            public override bool Equals(NSObject x, NSObject y)
            {
                var a = x as NSString;
                var b = y as NSString;

                if (a == null || b == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                if (!BinaryPropertyListWriter.IsSerializationPrimitive(a) || !BinaryPropertyListWriter.IsSerializationPrimitive(b))
                {
                    return object.ReferenceEquals(x, y);
                }

                return string.Equals(a.Content, b.Content, StringComparison.Ordinal);
            }

            public override int GetHashCode(NSObject obj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}