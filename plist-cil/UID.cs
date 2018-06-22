// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2015 Natalia Portillo
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
using System.Buffers.Binary;
using System.Text;

namespace Claunia.PropertyList
{
    /// <summary>
    /// An UID. Only found in binary property lists that are keyed archives.
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public class UID : NSObject
    {
        readonly ulong value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bytes">Bytes.</param>
        [Obsolete("UIDs have not meaningful names")]
        public UID(String name, ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 1 && bytes.Length != 2 && bytes.Length != 4 && bytes.Length != 8)
                throw new ArgumentException("Type argument is not valid.");
            this.value = (ulong)BinaryPropertyListParser.ParseLong(bytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        public UID(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 1 && bytes.Length != 2 && bytes.Length != 4 && bytes.Length != 8)
                throw new ArgumentException("Type argument is not valid.");
            this.value = (ulong)BinaryPropertyListParser.ParseLong(bytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 8-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 8-bit number.</param>
        [Obsolete("UIDs have no meaningful names")]
        public UID(String name, byte number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 8-bit number.
        /// </summary>
        /// <param name="number">Unsigned 8-bit number.</param>
        public UID(byte number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 8-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 8-bit number.</param>
        [Obsolete("UIDs must be unsigned values")]
        public UID(String name, sbyte number)
        {
            this.value = (ulong)number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 16-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 16-bit number.</param>
        [Obsolete("UIDs have no meaningful names")]
        public UID(String name, ushort number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 16-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 16-bit number.</param>
        public UID(ushort number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 16-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 16-bit number.</param>
        [Obsolete("UIDs must be unsigned values")]
        public UID(String name, short number)
        {
            this.value = (ulong)number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 32-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 32-bit number.</param>
        [Obsolete("UIDs have no meaningful names")]
        public UID(String name, uint number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 32-bit number.
        /// </summary>
        /// <param name="number">Unsigned 32-bit number.</param>
        public UID(uint number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 32-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 32-bit number.</param>
        [Obsolete("UIDs must be unsigned values")]
        public UID(String name, int number)
        {
            this.value = (ulong)number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 64-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 64-bit number.</param>
        [Obsolete("UIDs have no meaningful names")]
        public UID(String name, ulong number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 64-bit number.
        /// </summary>
        /// <param name="number">Unsigned 64-bit number.</param>
        public UID(ulong number)
        {
            this.value = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 64-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 64-bit number.</param>
        [Obsolete("UIDs must be unsigned values")]
        public UID(String name, long number)
        {
            this.value = (ulong)number;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[this.ByteCount];
                this.GetBytes(bytes);
                return bytes;
            }
        }

        /// <summary>
        /// Gets the number of bytes required to represent this <see cref="UID"/>.
        /// </summary>
        public int ByteCount
        {
            get
            {
                if (this.value <= byte.MaxValue)
                {
                    return 1;
                }
                else if (this.value <= ushort.MaxValue)
                {
                    return 2;
                }
                else if (this.value <= uint.MaxValue)
                {
                    return 4;
                }
                else
                {
                    return 8;
                }
            }
        }

        /// <summary>
        /// Writes the bytes required to represent this <see cref="UID"/> to a byte span.
        /// </summary>
        /// <param name="bytes">
        /// The byte span to which to write the byte representation of this UID.
        /// </param>
        public void GetBytes(Span<byte> bytes)
        {
            switch (this.ByteCount)
            {
                case 1:
                    bytes[0] = (byte)this.value;
                    break;

                case 2:
                    BinaryPrimitives.WriteUInt16BigEndian(bytes, (ushort)this.value);
                    break;

                case 4:
                    BinaryPrimitives.WriteUInt32BigEndian(bytes, (uint)this.value);
                    break;

                case 8:
                    BinaryPrimitives.WriteUInt64BigEndian(bytes, this.value);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [Obsolete("UIDs have no meaningful names")]
        public string Name
        {
            get
            {
                return this.value.ToString();
            }
        }

        /// <summary>
        /// There is no XML representation specified for UIDs.
        /// In this implementation UIDs are represented as strings in the XML output.
        /// </summary>
        /// <param name="xml">The xml StringBuilder</param>
        /// <param name="level">The indentation level</param>
        internal override void ToXml(StringBuilder xml, int level)
        {
            Indent(xml, level);
            xml.Append("<string>");
            Span<byte> bytes = stackalloc byte[this.ByteCount];
            this.GetBytes(bytes);
            foreach (byte b in bytes)
                xml.Append(String.Format("{0:x2}", b));
            xml.Append("</string>");
        }

        internal override void ToBinary(BinaryPropertyListWriter outPlist)
        {
            outPlist.Write(0x80 + this.ByteCount - 1);
            Span<byte> bytes = stackalloc byte[this.ByteCount];
            this.GetBytes(bytes);
            outPlist.Write(bytes);
        }

        internal override void ToASCII(StringBuilder ascii, int level)
        {
            Indent(ascii, level);
            ascii.Append("\"");
            Span<byte> bytes = stackalloc byte[this.ByteCount];
            this.GetBytes(bytes);
            foreach (byte b in bytes)
                ascii.Append(String.Format("{0:x2}", b));
            ascii.Append("\"");
        }

        internal override void ToASCIIGnuStep(StringBuilder ascii, int level)
        {
            ToASCII(ascii, level);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Claunia.PropertyList.NSObject"/> is equal to the current <see cref="Claunia.PropertyList.UID"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Claunia.PropertyList.NSObject"/> to compare with the current <see cref="Claunia.PropertyList.UID"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Claunia.PropertyList.NSObject"/> is equal to the current
        /// <see cref="Claunia.PropertyList.UID"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(NSObject obj)
        {
            return Equals((object)obj);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var uid = obj as UID;

            if (uid == null)
                return false;

            return uid.value == value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}

