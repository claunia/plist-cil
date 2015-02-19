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
using System.IO;

namespace Claunia.PropertyList
{
    /// <summary>
    /// This class provides methods to parse property lists. It can handle files,
    /// input streams and byte arrays. All known property list formats are supported.
    ///
    /// This class also provides methods to save and convert property lists.
    ///
    /// </summary>
    /// @author Daniel Dreibrodt
    public static class PropertyListParser
    {
        const int TYPE_XML = 0;
        const int TYPE_BINARY = 1;
        const int TYPE_ASCII = 2;
        const int TYPE_ERROR_BLANK = 10;
        const int TYPE_ERROR_UNKNOWN = 11;

        /// <summary>
        /// Determines the type of a property list by means of the first bytes of its data
        /// </summary>
        /// <returns>The type of the property list</returns>
        /// <param name="dataBeginning">The very first bytes of data of the property list (minus any whitespace) as a string</param>
        private static int DetermineType(string dataBeginning) {
            dataBeginning = dataBeginning.Trim();
            if(dataBeginning.Length == 0) {
                return TYPE_ERROR_BLANK;
            }
            if(dataBeginning.StartsWith("bplist")) {
                return TYPE_BINARY;
            }
            if(dataBeginning.StartsWith("(") || dataBeginning.StartsWith("{") || dataBeginning.StartsWith("/")) {
                return TYPE_ASCII;
            }
            if(dataBeginning.StartsWith("<")) {
                return TYPE_XML;
            }
            return TYPE_ERROR_UNKNOWN;
        }

        /// <summary>
        /// Determines the type of a property list by means of the first bytes of its data
        /// </summary>
        /// <returns>The very first bytes of data of the property list (minus any whitespace)</returns>
        /// <param name="bytes">The type of the property list</param>
        private static int DetermineType(byte[] bytes) {
            //Skip any possible whitespace at the beginning of the file
            int offset = 0;
            while(offset < bytes.Length && bytes[offset] == ' ' || bytes[offset] == '\t' || bytes[offset] == '\r' || bytes[offset] == '\n' || bytes[offset] == '\f') {
                offset++;
            }
            return DetermineType(Encoding.ASCII.GetString(bytes, offset, Math.Min(8, bytes.Length - offset)));
        }

        /// <summary>
        /// Determines the type of a property list by means of the first bytes of its data
        /// </summary>
        /// <returns>The type of the property list</returns>
        /// <param name="fs">An input stream pointing to the beginning of the property list data.
        /// The stream will be reset to the beginning of the property
        /// list data after the type has been determined.</param>
        private static int DetermineType(Stream fs) {
            //Skip any possible whitespace at the beginning of the file
            byte[] magicBytes = new byte[8];
            int b;
            long mark;
            do {
                mark = fs.Position;
                b = fs.ReadByte();
            }
            while(b != -1 && b == ' ' || b == '\t' || b == '\r' || b == '\n' || b == '\f');
            magicBytes[0] = (byte)b;
            int read = fs.Read(magicBytes, 1, 7);

            // Check for UTF-8 BOM prefixed XMLs first.
            if (magicBytes[0] == 0xEF && magicBytes[1] == 0xBB && magicBytes[2] == 0xBF && magicBytes[3] == (byte)'<')
                return TYPE_XML;

            int type = DetermineType(Encoding.ASCII.GetString(magicBytes, 0, read));
            fs.Seek(mark, SeekOrigin.Begin);
            //if(fs.markSupported())
              //  fs.reset();
            return type;
        }

        /// <summary>
        /// Reads all bytes from an Stream and stores them in an array, up to
        /// a maximum count.
        /// </summary>
        /// <param name="fs">The Stream pointing to the data that should be stored in the array.</param>
        internal static byte[] ReadAll(Stream fs) {
            MemoryStream outputStream = new MemoryStream();
            byte[] buf = new byte[512];
            int read = 512;
            while (read == 512) {
                read = fs.Read(buf, 0, 512);
                if(read != -1)
                    outputStream.Write(buf, 0, read);
            }
            return outputStream.ToArray();
        }

        /// <summary>
        /// Parses a property list from a file.
        /// </summary>
        /// <param name="filePath">Path to the property list file.</param>
        /// <returns>The root object in the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static NSObject Parse(string filePath) {
            return Parse(new FileInfo(filePath));
        }

        /// <summary>
        /// Parses a property list from a file.
        /// </summary>
        /// <param name="f">The property list file.</param>
        /// <returns>The root object in the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static NSObject Parse(FileInfo f) {
            FileStream fis = f.OpenRead();
            int type = DetermineType(fis);
            fis.Close();
            switch(type) {
                case TYPE_BINARY:
                    return BinaryPropertyListParser.Parse(f);
                case TYPE_XML:
                    return XmlPropertyListParser.Parse(f);
                case TYPE_ASCII:
                    return ASCIIPropertyListParser.Parse(f);
                default:
                    throw new PropertyListFormatException("The given file is not a property list of a supported format.");
            }
        }

        /// <summary>
        /// Parses a property list from a byte array.
        /// </summary>
        /// <param name="bytes">The property list data as a byte array.</param>
        /// <returns>The root object in the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static NSObject Parse(byte[] bytes) {
            switch(DetermineType(bytes)) {
                case TYPE_BINARY:
                    return BinaryPropertyListParser.Parse(bytes);
                case TYPE_XML:
                    return XmlPropertyListParser.Parse(bytes);
                case TYPE_ASCII:
                    return ASCIIPropertyListParser.Parse(bytes);
                default:
                    throw new PropertyListFormatException("The given data is not a property list of a supported format.");
            }
        }

        /// <summary>
        /// Parses a property list from an Stream.
        /// </summary>
        /// <param name="fs">The Stream delivering the property list data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static NSObject Parse(Stream fs) {
            return Parse(ReadAll(fs));
        }

        /// <summary>
        /// Saves a property list with the given object as root into a XML file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsXml(NSObject root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            SaveAsXml(root, fous);
            fous.Close();
        }

        /// <summary>
        /// Saves a property list with the given object as root in XML format into an output stream.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outStream">The output stream.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsXml(NSObject root, Stream outStream) {
            StreamWriter w = new StreamWriter(outStream, Encoding.UTF8);
            w.Write(root.ToXmlPropertyList());
            w.Close();
        }

        /// <summary>
        /// Converts a given property list file into the OS X and iOS XML format.
        /// </summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static void ConvertToXml(FileInfo inFile, FileInfo outFile) {
            NSObject root = Parse(inFile);
            SaveAsXml(root, outFile);
        }

        /// <summary>
        /// Saves a property list with the given object as root into a binary file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile"></param>The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsBinary(NSObject root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            BinaryPropertyListWriter.Write(outFile, root);
        }

        /// <summary>
        /// Saves a property list with the given object as root in binary format into an output stream.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outStream">The output stream.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsBinary(NSObject root, Stream outStream) {
            //BinaryPropertyListWriter.write(outStream, root);
        }

        /// <summary>
        /// Converts a given property list file into the OS X and iOS binary format.
        /// </summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static void ConvertToBinary(FileInfo inFile, FileInfo outFile) {
            NSObject root = Parse(inFile);
            SaveAsBinary(root, outFile);
        }

        /// <summary>
        /// Saves a property list with the given object as root into a ASCII file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsASCII(NSDictionary root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fous, Encoding.ASCII);
            w.Write(root.ToASCIIPropertyList());
            w.Close();
            fous.Close();
        }

        /// <summary>
        /// Saves a property list with the given object as root into a ASCII file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsASCII(NSArray root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fous, Encoding.ASCII);
            w.Write(root.ToASCIIPropertyList());
            w.Close();
            fous.Close();
        }

        /// <summary>
        /// Converts a given property list file into ASCII format.
        /// </summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static void ConvertToASCII(FileInfo inFile, FileInfo outFile) {
            NSObject root = Parse(inFile);
            if(root is NSDictionary) {
                SaveAsASCII((NSDictionary) root, outFile);
            }
            else if(root is NSArray) {
                SaveAsASCII((NSArray) root, outFile);
            }
            else {
                throw new PropertyListFormatException("The root of the given input property list "
                    + "is neither a Dictionary nor an Array!");
            }
        }

        /// <summary>
        /// Saves a property list with the given object as root into a ASCII file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsGnuStepASCII(NSDictionary root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fous, Encoding.ASCII);
            w.Write(root.ToGnuStepASCIIPropertyList());
            w.Close();
            fous.Close();
        }

        /// <summary>
        /// Saves a property list with the given object as root into a ASCII file.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static void SaveAsGnuStepASCII(NSArray root, FileInfo outFile) {
            string parent = outFile.DirectoryName;
            if (!Directory.Exists(parent))
                Directory.CreateDirectory(parent);
            Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fous, Encoding.ASCII);
            w.Write(root.ToGnuStepASCIIPropertyList());
            w.Close();
            fous.Close();
        }

        /// <summary>
        /// Converts a given property list file into ASCII format.
        /// </summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static void ConvertToGnuStepASCII(FileInfo inFile, FileInfo outFile) {
            NSObject root = Parse(inFile);
            if(root is NSDictionary) {
                SaveAsGnuStepASCII((NSDictionary) root, outFile);
            }
            else if(root is NSArray) {
                SaveAsGnuStepASCII((NSArray) root, outFile);
            }
            else {
                throw new PropertyListFormatException("The root of the given input property list "
                    + "is neither a Dictionary nor an Array!");
            }
        }
    }
}

