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
        readonly byte length;
        readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bytes">Bytes.</param>
        public UID(String name, ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 1 && bytes.Length != 2 && bytes.Length != 4 && bytes.Length != 8)
                throw new ArgumentException("Type argument is not valid.");
            this.name = name;
            this.length = (byte)bytes.Length;
            this.value = (ulong)BinaryPropertyListParser.ParseLong(bytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 8-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 8-bit number.</param>
        public UID(String name, byte number)
        {
            this.name = name;
            this.value = number;
            this.length = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 8-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 8-bit number.</param>
        public UID(String name, sbyte number)
        {
            this.name = name;
            this.value = (ulong)number;
            this.length = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 16-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 16-bit number.</param>
        public UID(String name, ushort number)
        {
            this.name = name;
            this.value = number;
            if (number <= byte.MaxValue)
                this.length = 1;
            else
            {
                this.length = 2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 16-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 16-bit number.</param>
        public UID(String name, short number)
        {
            this.name = name;
            this.value = (ulong)number;
            if (number >= sbyte.MinValue && number <= sbyte.MaxValue)
                this.length = 1;
            else
            {
                this.length = 2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 32-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 32-bit number.</param>
        public UID(String name, uint number)
        {
            this.name = name;
            this.value = number;
            if (number <= byte.MaxValue)
                this.length = 1;
            else if (number <= ushort.MaxValue)
            {
                this.length = 2;
            }
            else
            {
                this.length = 4;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 32-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 32-bit number.</param>
        public UID(String name, int number)
        {
            this.name = name;
            this.value = (ulong)number;
            if (number >= sbyte.MinValue && number <= sbyte.MaxValue)
                this.length = 1;
            if (number >= short.MinValue && number <= short.MaxValue)
            {
                this.length = 2;
            }
            else
            {
                this.length = 4;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using an unsigned 64-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Unsigned 64-bit number.</param>
        public UID(String name, ulong number)
        {
            this.name = name;
            this.value = number;
            if (number <= byte.MaxValue)
                this.length = 1;
            else if (number <= ushort.MaxValue)
            {
                this.length = 2;
            }
            else if (number <= uint.MaxValue)
            {
                this.length = 4;
            }
            else
            {
                this.length = 8;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Claunia.PropertyList.UID"/> class using a signed 64-bit number.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Signed 64-bit number.</param>
        public UID(String name, long number)
        {
            this.name = name;
            this.value = (ulong)number;
            if (number >= sbyte.MinValue && number <= sbyte.MaxValue)
                this.length = 1;
            if (number >= short.MinValue && number <= short.MaxValue)
            {
                this.length = 2;
            }
            if (number >= int.MinValue && number <= int.MaxValue)
            {
                this.length = 4;
            }
            else
            {
                this.length = 8;
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[this.length];
                this.GetBytes(bytes);
                return bytes;
            }
        }

        public void GetBytes(Span<byte> bytes)
        {
            switch (this.length)
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
        public string Name
        {
            get
            {
                return name;
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
            Span<byte> bytes = stackalloc byte[this.length];
            foreach (byte b in bytes)
                xml.Append(String.Format("{0:x2}", b));
            xml.Append("</string>");
        }

        internal override void ToBinary(BinaryPropertyListWriter outPlist)
        {
            outPlist.Write(0x80 + this.length - 1);
            Span<byte> bytes = stackalloc byte[this.length];
            outPlist.Write(bytes);
        }

        internal override void ToASCII(StringBuilder ascii, int level)
        {
            Indent(ascii, level);
            ascii.Append("\"");
            Span<byte> bytes = stackalloc byte[this.length];
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
            var uid = obj as UID;

            if (uid == null)
                return false;

            return uid.name == name
                && uid.length == length
                && uid.value == value;
        }
    }
}

