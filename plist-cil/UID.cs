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
using System.Text;

namespace Claunia.PropertyList
{
    /// <summary>
    /// A UID. Only found in binary property lists that are keyed archives.
    /// </summary>
    /// @author Daniel Dreibrodt
    public class UID : NSObject
    {
        byte[] bytes;
        string name;

        public UID(String name, byte[] bytes)
        {
            this.name = name;
            this.bytes = bytes;
        }

        public byte[] Bytes
        {
            get
            {
                return bytes;
            }
        }

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
        internal override void ToXml(StringBuilder xml, int level) {
            Indent(xml, level);
            xml.Append("<string>");
            for (int i = 0; i < bytes.Length; i++) {
                byte b = bytes[i];
                xml.Append(String.Format("{0:x2}", b));
            }
            xml.Append("</string>");
        }

        // TODO: Implement BinaryPropertyListWriter
        /*
        void override toBinary(BinaryPropertyListWriter out) throws IOException {
            out.write(0x80 + bytes.length - 1);
            out.write(bytes);
        }*/

        internal override void ToASCII(StringBuilder ascii, int level) {
            Indent(ascii, level);
            ascii.Append("\"");
            for (int i = 0; i < bytes.Length; i++) {
                byte b = bytes[i];
                ascii.Append(String.Format("{0:x2}", b));
            }
            ascii.Append("\"");
        }

        internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
            ToASCII(ascii, level);
        }
    }
}

