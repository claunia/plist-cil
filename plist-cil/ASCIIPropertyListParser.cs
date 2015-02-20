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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using System.Runtime.CompilerServices;

namespace Claunia.PropertyList
{
    /// <summary>
    /// <p>
    /// Parser for ASCII property lists. Supports Apple OS X/iOS and GnuStep/NeXTSTEP format.
    /// This parser is based on the recursive descent paradigm, but the underlying grammar
    /// is not explicitely defined.
    /// </p>
    /// <p>
    /// Resources on ASCII property list format:
    /// </p>
    /// <ul>
    /// <li><a href="https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html">
    /// Property List Programming Guide - Old-Style ASCII Property Lists
    /// </a></li>
    /// <li><a href="http://www.gnustep.org/resources/documentation/Developer/Base/Reference/NSPropertyList.html">
    /// GnuStep - NSPropertyListSerialization class documentation
    /// </a></li>
    /// </ul>
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public class ASCIIPropertyListParser
    {
        /// <summary>
        /// Parses an ASCII property list file.
        /// </summary>
        /// <param name="f">The ASCII property list file..</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="FormatException">When an error occurs during parsing.</exception>
        /// <exception cref="IOException">When an error occured while reading from the input stream.</exception>
        public static NSObject Parse(FileInfo f)
        {
            return Parse(f.OpenRead());
        }

        /// <summary>
        /// Parses an ASCII property list from an input stream.
        /// </summary>
        /// <param name="fs">The input stream that points to the property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="FormatException">When an error occurs during parsing.</exception>
        /// <exception cref="IOException"></exception>
        public static NSObject Parse(Stream fs)
        {
            byte[] buf = PropertyListParser.ReadAll(fs);
            fs.Close();
            return Parse(buf);
        }

        /// <summary>
        /// Parses an ASCII property list from a byte array.
        /// </summary>
        /// <param name="bytes">The ASCII property list data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="FormatException">When an error occurs during parsing.</exception>
        public static NSObject Parse(byte[] bytes)
        {
            ASCIIPropertyListParser parser = new ASCIIPropertyListParser(bytes);
            return parser.Parse();
        }

        public const char WHITESPACE_SPACE = ' ';
        public const char WHITESPACE_TAB = '\t';
        public const char WHITESPACE_NEWLINE = '\n';
        public const char WHITESPACE_CARRIAGE_RETURN = '\r';

        public const char ARRAY_BEGIN_TOKEN = '(';
        public const char ARRAY_END_TOKEN = ')';
        public const char ARRAY_ITEM_DELIMITER_TOKEN = ',';

        public const char DICTIONARY_BEGIN_TOKEN = '{';
        public const char DICTIONARY_END_TOKEN = '}';
        public const char DICTIONARY_ASSIGN_TOKEN = '=';
        public const char DICTIONARY_ITEM_DELIMITER_TOKEN = ';';

        public const char QUOTEDSTRING_BEGIN_TOKEN = '"';
        public const char QUOTEDSTRING_END_TOKEN = '"';
        public const char QUOTEDSTRING_ESCAPE_TOKEN = '\\';

        public const char DATA_BEGIN_TOKEN = '<';
        public const char DATA_END_TOKEN = '>';

        public const char DATA_GSOBJECT_BEGIN_TOKEN = '*';
        public const char DATA_GSDATE_BEGIN_TOKEN = 'D';
        public const char DATA_GSBOOL_BEGIN_TOKEN = 'B';
        public const char DATA_GSBOOL_TRUE_TOKEN = 'Y';
        public const char DATA_GSBOOL_FALSE_TOKEN = 'N';
        public const char DATA_GSINT_BEGIN_TOKEN = 'I';
        public const char DATA_GSREAL_BEGIN_TOKEN = 'R';

        public const char DATE_DATE_FIELD_DELIMITER = '-';
        public const char DATE_TIME_FIELD_DELIMITER = ':';
        public const char DATE_GS_DATE_TIME_DELIMITER = ' ';
        public const char DATE_APPLE_DATE_TIME_DELIMITER = 'T';
        public const char DATE_APPLE_END_TOKEN = 'Z';

        public const char COMMENT_BEGIN_TOKEN = '/';
        public const char MULTILINE_COMMENT_SECOND_TOKEN = '*';
        public const char SINGLELINE_COMMENT_SECOND_TOKEN = '/';
        public const char MULTILINE_COMMENT_END_TOKEN = '/';

        /**
        * Property list source data
        */
        private byte[] data;
        /**
        * Current parsing index
        */
        private int index;

        /**
        * Only allow subclasses to change instantiation.
        */
        protected ASCIIPropertyListParser()
        {

        }

        /// <summary>
        /// Creates a new parser for the given property list content.
        /// </summary>
        /// <param name="propertyListContent">The content of the property list that is to be parsed.</param>
        private ASCIIPropertyListParser(byte[] propertyListContent)
        {
            data = propertyListContent;
        }

        /// <summary>
        /// Checks whether the given sequence of symbols can be accepted.
        /// </summary>
        /// <returns>Whether the given tokens occur at the current parsing position.</returns>
        /// <param name="sequence">The sequence of tokens to look for.</param>
        private bool AcceptSequence(params char[] sequence)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                if (data[index + i] != sequence[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the given symbols can be accepted, that is, if one
        /// of the given symbols is found at the current parsing position.
        /// </summary>
        /// <param name="acceptableSymbols">The symbols to check.</param>
        /// <returns>Whether one of the symbols can be accepted or not.</returns>
        private bool Accept(params char[] acceptableSymbols)
        {
            bool symbolPresent = false;
            foreach (char c in acceptableSymbols)
            {
                if (data[index] == c)
                    symbolPresent = true;
            }
            return symbolPresent;
        }

        /// <summary>
        /// Checks whether the given symbol can be accepted, that is, if
        /// the given symbols is found at the current parsing position.
        /// </summary>
        /// <param name="acceptableSymbol">The symbol to check.</param>
        /// <returns>Whether the symbol can be accepted or not.</returns>
        private bool Accept(char acceptableSymbol)
        {
            return data[index] == acceptableSymbol;
        }

        /// <summary>
        /// Expects the input to have one of the given symbols at the current parsing position.
        /// </summary>
        /// <param name="expectedSymbols">The expected symbols.</param>
        /// <exception cref="FormatException">If none of the expected symbols could be found.</exception>
        private void Expect(params char[] expectedSymbols)
        {
            if (!Accept(expectedSymbols))
            {
                String excString = "Expected '" + expectedSymbols[0] + "'";
                for (int i = 1; i < expectedSymbols.Length; i++)
                {
                    excString += " or '" + expectedSymbols[i] + "'";
                }
                excString += " but found '" + (char)data[index] + "'";
                throw new FormatException(String.Format("{0} at {1}", excString, index));
            }
        }

        /// <summary>
        /// Expects the input to have the given symbol at the current parsing position.
        /// </summary>
        /// <param name="expectedSymbol">The expected symbol.</param>
        /// <exception cref="FormatException">If the expected symbol could be found.</exception>
        private void Expect(char expectedSymbol)
        {
            if (!Accept(expectedSymbol))
                throw new FormatException(String.Format("Expected '{0}' but found '{1}' at {2}", expectedSymbol, data[index], index));
        }

        /// <summary>
        /// Reads an expected symbol.
        /// </summary>
        /// <param name="symbol">The symbol to read.</param>
        /// <exception cref="FormatException">If the expected symbol could not be read.</exception>
        private void Read(char symbol)
        {
            Expect(symbol);
            index++;
        }

        /**
     * Skips the current symbol.
     */
        private void Skip()
        {
            index++;
        }

        /// <summary>
        /// Skips several symbols
        /// </summary>
        /// <param name="numSymbols">The amount of symbols to skip.</param>
        private void Skip(int numSymbols)
        {
            index += numSymbols;
        }

        /**
     * Skips all whitespaces and comments from the current parsing position onward.
     */
        private void SkipWhitespacesAndComments()
        {
            bool commentSkipped;
            do
            {
                commentSkipped = false;

                //Skip whitespaces
                while (Accept(WHITESPACE_CARRIAGE_RETURN, WHITESPACE_NEWLINE, WHITESPACE_SPACE, WHITESPACE_TAB))
                {
                    Skip();
                }

                //Skip single line comments "//..."
                if (AcceptSequence(COMMENT_BEGIN_TOKEN, SINGLELINE_COMMENT_SECOND_TOKEN))
                {
                    Skip(2);
                    ReadInputUntil(WHITESPACE_CARRIAGE_RETURN, WHITESPACE_NEWLINE);
                    commentSkipped = true;
                }
                //Skip multi line comments "/* ... */"
                else if (AcceptSequence(COMMENT_BEGIN_TOKEN, MULTILINE_COMMENT_SECOND_TOKEN))
                {
                    Skip(2);
                    while (true)
                    {
                        if (AcceptSequence(MULTILINE_COMMENT_SECOND_TOKEN, MULTILINE_COMMENT_END_TOKEN))
                        {
                            Skip(2);
                            break;
                        }
                        Skip();
                    }
                    commentSkipped = true;
                }
            }
            while (commentSkipped); //if a comment was skipped more whitespace or another comment can follow, so skip again
        }

        /// <summary>
        /// Reads input until one of the given symbols is found.
        /// </summary>
        /// <returns>The input until one the given symbols.</returns>
        /// <param name="symbols">The symbols that can occur after the string to read.</param>
        private string ReadInputUntil(params char[] symbols)
        {
            string s = "";
            while (!Accept(symbols))
            {
                s += (char)data[index];
                Skip();
            }
            return s;
        }

        /// <summary>
        /// Reads input until the given symbol is found.
        /// </summary>
        /// <returns>The input until the given symbol.</returns>
        /// <param name="symbol">The symbol that can occur after the string to read.</param>
        private string ReadInputUntil(char symbol)
        {
            String s = "";
            while (!Accept(symbol))
            {
                s += (char)data[index];
                Skip();
            }
            return s;
        }

        /// <summary>
        /// Parses the property list from the beginning and returns the root object
        /// of the property list.
        /// </summary>
        /// <returns>The root object of the property list. This can either be a NSDictionary or a NSArray.</returns>
        /// <exception cref="FormatException">When an error occured during parsing</exception>
        public NSObject Parse()
        {
            index = 0;
            SkipWhitespacesAndComments();
            Expect(DICTIONARY_BEGIN_TOKEN, ARRAY_BEGIN_TOKEN, COMMENT_BEGIN_TOKEN);
            try
            {
                return ParseObject();
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(String.Format("Reached end of input unexpectedly at {0}.", index));
            }
        }

        /// <summary>
        /// Parses the NSObject found at the current position in the property list
        /// data stream.
        /// </summary>
        /// <returns>The parsed NSObject.</returns>
        /// <seealso cref="ASCIIPropertyListParser.index"/>
        private NSObject ParseObject()
        {
            switch (data[index])
            {
                case (byte)ARRAY_BEGIN_TOKEN:
                    {
                        return ParseArray();
                    }
                case (byte)DICTIONARY_BEGIN_TOKEN:
                    {
                        return ParseDictionary();
                    }
                case (byte)DATA_BEGIN_TOKEN:
                    {
                        return ParseData();
                    }
                case (byte)QUOTEDSTRING_BEGIN_TOKEN:
                    {
                        string quotedString = ParseQuotedString();
                        //apple dates are quoted strings of length 20 and after the 4 year digits a dash is found
                        if (quotedString.Length == 20 && quotedString[4] == DATE_DATE_FIELD_DELIMITER)
                        {
                            try
                            {
                                return new NSDate(quotedString);
                            }
                            catch (Exception ex)
                            {
                                //not a date? --> return string
                                return new NSString(quotedString);
                            }
                        }
                        else
                        {
                            return new NSString(quotedString);
                        }
                    }
                default:
                    {
                        //0-9
                        if (data[index] > 0x2F && data[index] < 0x3A)
                        {
                            //could be a date or just a string
                            return ParseDateString();
                        }
                        else
                        {
                            //non-numerical -> string or boolean
                            string parsedString = ParseString();
                            return new NSString(parsedString);
                        }
                    }
            }
        }

        /// <summary>
        /// Parses an array from the current parsing position.
        /// The prerequisite for calling this method is, that an array begin token has been read.
        /// </summary>
        /// <returns>The array found at the parsing position.</returns>
        private NSArray ParseArray()
        {
            //Skip begin token
            Skip();
            SkipWhitespacesAndComments();
            List<NSObject> objects = new List<NSObject>();
            while (!Accept(ARRAY_END_TOKEN))
            {
                objects.Add(ParseObject());
                SkipWhitespacesAndComments();
                if (Accept(ARRAY_ITEM_DELIMITER_TOKEN))
                {
                    Skip();
                }
                else
                {
                    break; //must have reached end of array
                }
                SkipWhitespacesAndComments();
            }
            //parse end token
            Read(ARRAY_END_TOKEN);
            return new NSArray(objects.ToArray());
        }

        /// <summary>
        /// Parses a dictionary from the current parsing position.
        /// The prerequisite for calling this method is, that a dictionary begin token has been read.
        /// </summary>
        /// <returns>The dictionary found at the parsing position.</returns>
        private NSDictionary ParseDictionary()
        {
            //Skip begin token
            Skip();
            SkipWhitespacesAndComments();
            NSDictionary dict = new NSDictionary();
            while (!Accept(DICTIONARY_END_TOKEN))
            {
                //Parse key
                string keyString;
                if (Accept(QUOTEDSTRING_BEGIN_TOKEN))
                {
                    keyString = ParseQuotedString();
                }
                else
                {
                    keyString = ParseString();
                }
                SkipWhitespacesAndComments();

                //Parse assign token
                Read(DICTIONARY_ASSIGN_TOKEN);
                SkipWhitespacesAndComments();

                NSObject nso = ParseObject();
                dict.Add(keyString, nso);
                SkipWhitespacesAndComments();
                Read(DICTIONARY_ITEM_DELIMITER_TOKEN);
                SkipWhitespacesAndComments();
            }
            //skip end token
            Skip();
            return dict;
        }

        /// <summary>
        /// Parses a data object from the current parsing position.
        /// This can either be a NSData object or a GnuStep NSNumber or NSDate.
        /// The prerequisite for calling this method is, that a data begin token has been read.
        /// </summary>
        /// <returns>The data object found at the parsing position.</returns>
        private NSObject ParseData()
        {
            NSObject obj = null;
            //Skip begin token
            Skip();
            if (Accept(DATA_GSOBJECT_BEGIN_TOKEN))
            {
                Skip();
                Expect(DATA_GSBOOL_BEGIN_TOKEN, DATA_GSDATE_BEGIN_TOKEN, DATA_GSINT_BEGIN_TOKEN, DATA_GSREAL_BEGIN_TOKEN);
                if (Accept(DATA_GSBOOL_BEGIN_TOKEN))
                {
                    //Boolean
                    Skip();
                    Expect(DATA_GSBOOL_TRUE_TOKEN, DATA_GSBOOL_FALSE_TOKEN);
                    if (Accept(DATA_GSBOOL_TRUE_TOKEN))
                    {
                        obj = new NSNumber(true);
                    }
                    else
                    {
                        obj = new NSNumber(false);
                    }
                    //Skip the parsed boolean token
                    Skip();
                }
                else if (Accept(DATA_GSDATE_BEGIN_TOKEN))
                {
                    //Date
                    Skip();
                    string dateString = ReadInputUntil(DATA_END_TOKEN);
                    obj = new NSDate(dateString);
                }
                else if (Accept(DATA_GSINT_BEGIN_TOKEN, DATA_GSREAL_BEGIN_TOKEN))
                {
                    //Number
                    Skip();
                    string numberString = ReadInputUntil(DATA_END_TOKEN);
                    obj = new NSNumber(numberString);
                }
                //parse data end token
                Read(DATA_END_TOKEN);
            }
            else
            {
                string dataString = ReadInputUntil(DATA_END_TOKEN);
                dataString = Regex.Replace(dataString, "\\s+", "");

                int numBytes = dataString.Length / 2;
                byte[] bytes = new byte[numBytes];
                for (int i = 0; i < bytes.Length; i++)
                {
                    string byteString = dataString.Substring(i * 2, 2);
                    int byteValue = Convert.ToInt32(byteString, 16);
                    bytes[i] = (byte)byteValue;
                }
                obj = new NSData(bytes);

                //skip end token
                Skip();
            }

            return obj;
        }

        /// <summary>
        /// Attempts to parse a plain string as a date if possible.
        /// </summary>
        /// <returns>A NSDate if the string represents such an object. Otherwise a NSString is returned.</returns>
        private NSObject ParseDateString()
        {
            string numericalString = ParseString();
            if (numericalString.Length > 4 && numericalString[4] == DATE_DATE_FIELD_DELIMITER)
            {
                try
                {
                    return new NSDate(numericalString);
                }
                catch (Exception ex)
                {
                    //An exception occurs if the string is not a date but just a string
                }
            }
            return new NSString(numericalString);
        }

        /// <summary>
        /// Parses a plain string from the current parsing position.
        /// The string is made up of all characters to the next whitespace, delimiter token or assignment token.
        /// </summary>
        /// <returns>The string found at the current parsing position.</returns>
        private string ParseString()
        {
            return ReadInputUntil(WHITESPACE_SPACE, WHITESPACE_TAB, WHITESPACE_NEWLINE, WHITESPACE_CARRIAGE_RETURN,
                ARRAY_ITEM_DELIMITER_TOKEN, DICTIONARY_ITEM_DELIMITER_TOKEN, DICTIONARY_ASSIGN_TOKEN, ARRAY_END_TOKEN);
        }

        /// <summary>
        /// Parses a quoted string from the current parsing position.
        /// The prerequisite for calling this method is, that a quoted string begin token has been read.
        /// </summary>
        /// <returns>The quoted string found at the parsing method with all special characters unescaped.</returns>
        /// <exception cref="FormatException">If an error occured during parsing.</exception>
        private string ParseQuotedString()
        {
            //Skip begin token
            Skip();
            string quotedString = "";
            bool unescapedBackslash = true;
            //Read from opening quotation marks to closing quotation marks and skip escaped quotation marks
            while (data[index] != QUOTEDSTRING_END_TOKEN || (data[index - 1] == QUOTEDSTRING_ESCAPE_TOKEN && unescapedBackslash))
            {
                quotedString += (char)data[index];
                if (Accept(QUOTEDSTRING_ESCAPE_TOKEN))
                {
                    unescapedBackslash = !(data[index - 1] == QUOTEDSTRING_ESCAPE_TOKEN && unescapedBackslash);
                }
                Skip();
            }
            string unescapedString;
            try
            {
                unescapedString = ParseQuotedString(quotedString);
            }
            catch (Exception ex)
            {
                throw new FormatException(String.Format("The quoted string could not be parsed at {0}.", index));
            }
            //skip end token
            Skip();
            return unescapedString;
        }

        /**
     * Used to encode the parsed strings
     */
        private static Encoding asciiEncoder;

        /// <summary>
        /// Parses a string according to the format specified for ASCII property lists.
        /// Such strings can contain escape sequences which are unescaped in this method.
        /// </summary>
        /// <returns>The unescaped string in UTF-8 or ASCII format, depending on the contained characters.</returns>
        /// <param name="s">The escaped string according to the ASCII property list format, without leading and trailing quotation marks.</param>
        /// <exception cref="ArgumentException">If the en-/decoder for the UTF-8 or ASCII encoding could not be loaded</exception>
        /// <exception cref="EncoderFallbackException">If the string is encoded neither in ASCII nor in UTF-8</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string ParseQuotedString(string s)
        {
            List<byte> strBytes = new List<byte>();
            CharEnumerator c = s.GetEnumerator();

            while (c.MoveNext())
            {
                switch (c.Current)
                {
                    case '\\':
                        { //An escaped sequence is following
                            byte[] bts = Encoding.UTF8.GetBytes(ParseEscapedSequence(c));
                            foreach (byte b in bts)
                                strBytes.Add(b);
                            break;
                        }
                    default:
                        { //a normal ASCII char
                            strBytes.Add((byte)0);
                            strBytes.Add((byte)c.Current);
                            break;
                        }
                }
            }
            byte[] bytArr = new byte[strBytes.Count];
            int i = 0;
            foreach (byte b in strBytes)
            {
                bytArr[i] = b;
                i++;
            }
            //Build string
            string result = Encoding.BigEndianUnicode.GetString(bytArr);
            //emoryStream charBuf = CharBuffer.wrap(result);

            //If the string can be represented in the ASCII codepage
            // --> use ASCII encoding
            try
            {
                if (asciiEncoder == null)
                    asciiEncoder = Encoding.GetEncoding("ascii", EncoderExceptionFallback.ExceptionFallback, DecoderExceptionFallback.ExceptionFallback);
                return asciiEncoder.GetString(Encoding.Convert(Encoding.BigEndianUnicode, asciiEncoder, bytArr));
            }
            catch
            {
                //The string contains characters outside the ASCII codepage
                // --> use the UTF-8 encoded string
                return result;
            }
        }

        /// <summary>
        /// Unescapes an escaped character sequence, e.g. \\u00FC.
        /// </summary>
        /// <returns>The unescaped character as a string.</returns>
        /// <param name="iterator">The string character iterator pointing to the first character after the backslash</param>
        /// <exception cref="EncoderFallbackException">If an invalid Unicode or ASCII escape sequence is found.</exception>
        private static string ParseEscapedSequence(CharEnumerator iterator)
        {
            iterator.MoveNext();
            char c = iterator.Current;
            if (c == '\\')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\\' });
            }
            else if (c == '"')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\"' });
            }
            else if (c == 'b')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\b' });
            }
            else if (c == 'n')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\n' });
            }
            else if (c == 'r')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\r' });
            }
            else if (c == 't')
            {
                return Encoding.UTF8.GetString(new byte[]{ 0, (byte)'\t' });
            }
            else if (c == 'U' || c == 'u')
            {
                //4 digit hex Unicode value
                string byte1 = "";
                iterator.MoveNext();
                byte1 += iterator.Current;
                iterator.MoveNext();
                byte1 += iterator.Current;
                string byte2 = "";
                iterator.MoveNext();
                byte2 += iterator.Current;
                iterator.MoveNext();
                byte2 += iterator.Current;
                byte[] stringBytes = { (byte)Convert.ToInt32(byte1, 16), (byte)Convert.ToInt32(byte2, 16) };
                return Encoding.UTF8.GetString(stringBytes);
            }
            else
            {
                //3 digit octal ASCII value
                string num = "";
                num += c;
                iterator.MoveNext();
                num += iterator.Current;
                iterator.MoveNext();
                num += iterator.Current;
                int asciiCode = Convert.ToInt32(num, 8);
                byte[] stringBytes = { 0, (byte)asciiCode };
                return Encoding.UTF8.GetString(stringBytes);
            }
        }
    }
}

