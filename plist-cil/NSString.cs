﻿// plist-cil - An open source library to parse and generate property lists for .NET
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
    /// A NSString contains a string.
    /// </summary>
    /// @author Daniel Dreibrodt
    public class NSString : NSObject, IComparable
    {
        string content;

        /// <summary>
        /// Creates an NSString from its binary representation.
        /// </summary>
        /// <param name="bytes">The binary representation.</param>
        /// <param name="encoding">The encoding of the binary representation, the name of a supported charset.</param>
        /// <exception cref="ArgumentException">The encoding charset is invalid or not supported by the underlying platform.</exception>
        public NSString(byte[] bytes, String encoding) {
            Encoding enc = Encoding.GetEncoding(encoding);
            content = enc.GetString(bytes);
        }

        /// <summary>
        /// Creates a NSString from a string.
        /// </summary>
        /// <param name="string">The string that will be contained in the NSString.</param>
        public NSString(string text) {
            content = text;
        }

        /// <summary>
        /// Gets this strings content.
        /// </summary>
        /// <returns>This NSString as .NET string object.</returns>
        public string GetContent() {
            return content;
        }

        /// <summary>
        /// Sets the contents of this string.
        /// </summary>
        /// <param name="c">The new content of this string object.</param>
        public void SetContent(string c) {
            content = c;
        }

        /// <summary>
        /// Appends a string to this string.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void Append(NSString s) {
            Append(s.GetContent());
        }

        /// <summary>
        /// Appends a string to this string.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void Append(string s) {
            content += s;
        }

        /// <summary>
        /// Prepends a string to this string.
        /// </summary>
        /// <param name="s">The string to prepend.</param>
        public void Prepend(string s) {
            content = s + content;
        }

        /// <summary>
        /// Prepends a string to this string.
        /// </summary>
        /// <param name="s">The string to prepend.</param>
        public void Prepend(NSString s) {
            Prepend(s.GetContent());
        }
            
        public override bool Equals(Object obj) {
            if (!(obj is NSString)) return false;
            return content.Equals(((NSString) obj).content);
        }

        public override int GetHashCode() {
            return content.GetHashCode();
        }

        /// <summary>
        /// The textual representation of this NSString.
        /// </summary>
        /// <returns>The NSString's contents.</returns>
        public override string ToString() {
            return content;
        }

        static Encoding asciiEncoder, utf16beEncoder, utf8Encoder;

        internal override void ToXml(StringBuilder xml, int level) {
            Indent(xml, level);
            xml.Append("<string>");

            //Make sure that the string is encoded in UTF-8 for the XML output
            lock (typeof(NSString))
            {
                if (utf8Encoder == null)
                    utf8Encoder = Encoding.GetEncoding("UTF-8");

                try {
                    byte[] bytes = utf8Encoder.GetBytes(content);
                    content = utf8Encoder.GetString(bytes);
                } catch (Exception ex) {
                    throw new SystemException("Could not encode the NSString into UTF-8: " + ex.Message);
                }
            }

            //According to http://www.w3.org/TR/REC-xml/#syntax node values must not
            //contain the characters < or &. Also the > character should be escaped.
            if (content.Contains("&") || content.Contains("<") || content.Contains(">")) {
                xml.Append("<![CDATA[");
                xml.Append(content.Replace("]]>", "]]]]><![CDATA[>"));
                xml.Append("]]>");
            } else {
                xml.Append(content);
            }
            xml.Append("</string>");
        }

        internal override void ToBinary(BinaryPropertyListWriter outPlist) {
            int kind;
            byte[] byteBuf;
            lock (typeof(NSString)) {
                if (asciiEncoder == null)
                    asciiEncoder = Encoding.GetEncoding("ascii", EncoderExceptionFallback.ExceptionFallback, DecoderExceptionFallback.ExceptionFallback);

                try {
                    kind = 0x5; // standard ASCII
                    byteBuf = asciiEncoder.GetBytes(content);
                } catch {
                    if (utf16beEncoder == null)
                        utf16beEncoder = Encoding.BigEndianUnicode;

                    kind = 0x6; // UTF-16-BE
                    byteBuf = utf16beEncoder.GetBytes(content);
                }
            }
            outPlist.WriteIntHeader(kind, content.Length);
            outPlist.Write(byteBuf);
        }

        internal override void ToASCII(StringBuilder ascii, int level) {
            Indent(ascii, level);
            ascii.Append("\"");
            //According to https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html
            //non-ASCII characters are not escaped but simply written into the
            //file, thus actually violating the ASCII plain text format.
            //We will escape the string anyway because current Xcode project files (ASCII property lists) also escape their strings.
            ascii.Append(EscapeStringForASCII(content));
            ascii.Append("\"");
        }

        internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
            Indent(ascii, level);
            ascii.Append("\"");
            ascii.Append(EscapeStringForASCII(content));
            ascii.Append("\"");
        }

        /// <summary>
        /// Escapes a string for use in ASCII property lists.
        /// </summary>
        /// <returns>The unescaped string.</returns>
        /// <param name="s">S.</param>
        internal static string EscapeStringForASCII(string s) {
            string outString = "";
            char[] cArray = s.ToCharArray();
            for (int i = 0; i < cArray.Length; i++) {
                char c = cArray[i];
                if (c > 127) {
                    //non-ASCII Unicode
                    outString += "\\U";
                    string hex = String.Format("{0:x}", c);
                    while (hex.Length < 4)
                        hex = "0" + hex;
                    outString += hex;
                } else if (c == '\\') {
                    outString += "\\\\";
                } else if (c == '\"') {
                    outString += "\\\"";
                } else if (c == '\b') {
                    outString += "\\b";
                } else if (c == '\n') {
                    outString += "\\n";
                } else if (c == '\r') {
                    outString += "\\r";
                } else if (c == '\t') {
                    outString += "\\t";
                } else {
                    outString += c;
                }
            }
            return outString;
        }

        public int CompareTo(Object o) {
            if (o is NSString) {
                return GetContent().CompareTo(((NSString) o).GetContent());
            } else if (o is String) {
                return GetContent().CompareTo(((String) o));
            } else {
                return -1;
            }
        }
    }
}

