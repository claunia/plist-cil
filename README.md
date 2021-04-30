# plist-cil - A C# library for working with property lists

[![Build Status](https://travis-ci.org/claunia/plist-cil.svg?branch=faster)](httsp://travis-ci.org/claunia/plist-cil)

This library enables .NET (CLR) applications to handle property lists of various formats.
It is mostly based on [dd-plist for Java](https://github.com/3breadt/dd-plist).
And as it, this is licensed under the terms of the MIT license.

Property lists are files used to store user settings and serialized objects.
They originate from the NeXTSTEP programming environment and are now a basic part of thhe Cocoa framework (macOS and iOS) as well as the GNUstep framework.

## Features

 * Read / write property lists from / to files, streams or byte arrays
 * Convert between property lists formats
 * Property list contents are provided as objects from the NeXTSTEP environment (NSDictionary, NSArray, NSString, etc.)
 * Serialize native .NET data structures to property list objects
 * Deserialize from property list objects to native .NET data structures

## Supported formats

* Cocoa XML
* Cocoa Binary (v0)
* Cocoa / NeXTSTEP / GNUstep ASCII

## Requirements
plist-cil targets:

- .NET Framework 4.5,
- .NET Standard 2.0
- .NET Core 2.1
- .NET Core 3.1.
- .NET 5.0

This means it should be compatible with Mono, Xamarin.iOS, Xamarin.Mac, UWP, etc. If you find an incompatibility, please create an issue.

## Download

The latest releases can be downloaded [here](https://github.com/claunia/plist-cil/releases).

## NuGet support
You can download the NuGet package directly from the [release](https://github.com/claunia/plist-cil/releases) page or from the [NuGet Gallery](https://www.nuget.org/) or from [here](https://www.nuget.org/packages/plist-cil/).

## Help
The API documentation is included in the download.

When you encounter a bug please report it by on the [issue tracker](https://github.com/claunia/plist-cil/issues).

## Usage examples

### Reading

Parsing can be done with the PropertyListParser class. You can feed the `PropertyListParser` with a `FileInfo`, a `Stream` or a `byte` array.
The `Parse` method of the `PropertyListParser` will parse the input and give you a `NSObject` as result. Generally this is a `NSDictionary` but it can also be a `NSArray`.

_Note: Property lists created by `NSKeyedArchiver` are not yet supported._

You can then navigate the contents of the property lists using the various classes extending `NSObject`. These are modeled in such a way as to closely resemble the respective Cocoa classes.

You can also directly convert the contained `NSObject` objects into native .NET Objects with the `NSOBject.ToObject()` method. Using this method you can avoid working with `NSObject` instances altogether.

### Writing

You can create your own property list using the various constructors of the different `NSObject` classes. Or you can wrap existing native .NET structures with the method `NSObject.Wrap(Object o)`. Just make sure that the root object of the property list is either a `NSDictionary` (can be created from objects of the type `Dictionary<string, Object>`) or a `NSArray` (can be created from object arrays).

For building a XML property list you can then call the `ToXml` method on the root object of your property list. It will give you an UTF-8 `string` containing the property list in XML format.

If you want to have the property list in binary format use the `BinaryPropertyListWriter` class. It can write the binary property list directly to a file or to a `Stream`.

When you directly want to save your property list to a file, you can also use the `SaveAsXml` or `SaveAsBinary` methods of the `PropertyListParser` class.

### Converting

For converting a file into another format there exist convenience methods in the `PropertyListParser` class: `ConvertToXml`, `ConvertToBinary`, `ConvertToASCII` and `ConvertToGnuStepASCII`.

## Code snippets

### Reading

```csharp
try
{
	FileInfo file = new FileInfo("Info.plist");
	NSDictionary rootDict = (NSDictionary)PropertyListParser.Parse(file);
	string name = rootDict.ObjectForKey("Name").ToString();
	NSObject[] parameters = ((NSArray)rootDict.ObjectForKey("Parameters")).GetArray();
	foreach(NSObject param in parameters)
	{
		if(param.GetType().Equals(typeof(NSNumber)))
		{
			NSNumber num = (NSNumber)param;
			switch(num.GetNSNumberType())
			{
				case NSNumber.BOOLEAN:
				{
					boolean bool = num.ToBool();
					// ...
					break;
				}
				case NSNumber.INTEGER:
				{
					long l = num.ToLong();
					// or int i = num.ToInt();
					// ...
					break;
				}
				case NSNumber.REAL:
				{
					double d = num.ToDouble();
					// ...
					break;
				}
			}
		}
		// else...
	}
}
catch(Exception ex)
{
	Console.WriteLine(ex.StackTrace);
}
```

### Writing

```csharp
// Creating the root object
NSDictionary root = new NSDictionary();

// Creation of an array of length 2
NSArray people = new NSArray(2);

// Creation of the first object to be stored in the array
NSDictionary person1 = new NSDictionary();
// The NSDictionary will automatically wrap strings, numbers and dates in the respective NSObject subclasses
person1.Add("Name", "Peter"); // This will become a NSString
// Use the DateTime class
person1.Add("RegistrationDate", new DateTime(2011, 1, 13, 9, 28, 0)); // This will become a NSDate
person1.Add("Age", 23); // This will become a NSNumber
person1.Add("Photo", new NSData(new FileInfo("peter.jpg")));

// Creation of the second object to be stored in the array
NSDictionary person2 = new NSDictionary();
person2.Add("Name", "Lisa");
person2.Add("Age", 42);
person2.Add("RegistrationDate", new NSDate("2010-09-23T12:32:42Z"));
person2.Add("Photo", new NSData(new FileInfo("lisa.jpg")));

// Put the objects into the array
people.SetValue(0, person1);
people.SetValue(1, person2);

// Put the array into the property list
root.Add("People", people);

// Save the property list
PropertyListParser.SaveAsXml(root, new FileInfo("people.plist"));
```
