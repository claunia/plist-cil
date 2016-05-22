#plist-cil
This library enables .NET (CLR) applications to work with property lists in various formats.
It is mostly based on [dd-plist for Java](https://code.google.com/p/plist/).

You can parse existing property lists (e.g. those created by an iOS application) and work with the contents on any operating system.

The library also enables you to create your own property lists from scratch and store them in various formats.

The provided API mimics the Cocoa/NeXTSTEP API, and where applicable, the .NET API, granting access to the basic functions of classes like NSDictionary, NSData, etc.

###Supported formats

| Format                 | Read | Write |
| ---------------------- | ---- | ----- |
| OS X / iOS XML         |  yes |  yes  |
| OS X / iOS Binary (v0) |  yes |  yes  |
| OS X / iOS ASCII       |  yes |  yes  |
| GNUstep ASCII          |  yes |  yes  |

###Requirements
.NET Framework 4.0, Mono or .NET Core.

###Download

The latest releases can be downloaded [here](https://github.com/claunia/plist-cil/releases).

###NuGet support
You can download the NuGet package directly from the [release](https://github.com/claunia/plist-cil/releases) page or from the [NuGet Gallery](https://www.nuget.org/) or from [here](https://www.nuget.org/packages/plist-cil/).

###Help
The API documentation is included in the download.

When you encounter a bug please report it by on the [issue tracker](https://github.com/claunia/plist-cil/issues).
