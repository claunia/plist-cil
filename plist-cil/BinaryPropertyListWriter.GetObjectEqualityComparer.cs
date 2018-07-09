using System.Collections.Generic;

namespace Claunia.PropertyList
{
    public partial class BinaryPropertyListWriter
    {
        /// <summary>
        ///     The equality comparer which is used when retrieving objects in the <see cref="BinaryPropertyListWriter.idMap" />.
        ///     The logic is slightly different from <see cref="AddObjectEqualityComparer" />, results in two equivalent objects
        ///     (UIDs mainly) being added to the <see cref="BinaryPropertyListWriter.idMap" />. Whenever the ID for one of
        ///     those equivalent objects is requested, the first ID is always returned.
        ///     This means that there are "orphan" objects in binary property lists - duplicate objects which are never referenced
        ///     -;
        ///     this logic exists purely to maintain binary compatibility with Apple's format.
        /// </summary>
        class GetObjectEqualityComparer : EqualityComparer<NSObject>
        {
            public override bool Equals(NSObject x, NSObject y)
            {
                // By default, use reference equality. Even if there are two objects - say a NSString - with the same
                // value, do not consider them equal unless they are the same instance of NSString.
                // The exceptions are UIDs, where we always compare by value, and "primitive" strings (a list of well-known
                // strings), which are treaded specially and "recycled".
                if(x is UID) return x.Equals(y);

                if(x is NSNumber && IsSerializationPrimitive((NSNumber)x)) return x.Equals(y);
                if(x is NSString && IsSerializationPrimitive((NSString)x)) return x.Equals(y);

                return ReferenceEquals(x, y);
            }

            public override int GetHashCode(NSObject obj)
            {
                if(obj == null) return 0;

                UID u = obj as UID;
                if(u != null) return u.GetHashCode();

                NSNumber n = obj as NSNumber;
                if(n != null && IsSerializationPrimitive(n)) return n.ToObject().GetHashCode();

                NSString s = obj as NSString;
                if(s != null && IsSerializationPrimitive(s)) return s.Content.GetHashCode();

                return obj.GetHashCode();
            }
        }
    }
}