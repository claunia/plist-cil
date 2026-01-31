namespace Claunia.PropertyList.Shell;

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

[Cmdlet(VerbsData.ConvertFrom, "Plist")]
public class ConvertFromPlist : PSCmdlet
{
    readonly List<byte> _inputBuffer = new();

    #region Parameters

    /// <summary>
    /// Gets or sets the InputObject property.
    /// Represents the byte stream input to be converted.
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    [AllowEmptyString]
    public byte InputObject { get; set; }

    /// <summary>
    /// Whether to return the result as a Claunia.PropertyList.NSObject instance.
    /// </summary>
    [Parameter]
    public SwitchParameter AsNSObject { get; set; }

    #endregion Parameters

    #region Overrides

    protected override void BeginProcessing()
    {
    }

    protected override void ProcessRecord()
    {
        _inputBuffer.Add(InputObject);
    }

    protected override void EndProcessing()
    {
        WriteDebug($"Buffer length: [{_inputBuffer.Count}]");
        var result = PropertyListParser.Parse(_inputBuffer.ToArray());
        if (AsNSObject.IsPresent)
        {
            WriteObject(result);
            return;
        }

        WriteObject(FromNSObject(result));
    }

    #endregion Overrides

    PSObject FromNSObject(NSObject value)
    {
        var result = new PSObject();

        if (value is NSDictionary dict)
        {
            foreach(var key in dict.Keys)
            {
                result.Properties.Add(new PSNoteProperty(key.ToString(), FromNSObject(dict.ObjectForKey(key))));
            }
        }
        else if (value is NSArray array)
        {
            var psArray = new object[array.Count];
            int i = 0;
            foreach (var element in array)
            {
                psArray[i++] = FromNSObject(element);
            }

            result = new PSObject(psArray[..i]);
        }
        else if (value is NSNumber number)
        {
            switch(number.GetNSNumberType())
            {
                case NSNumber.BOOLEAN:
                    result = new PSObject(number.ToBool());
                    break;
                case NSNumber.INTEGER:
                    result = new PSObject(number.ToLong());
                    break;
                case NSNumber.REAL:
                    result = new PSObject(number.ToDouble());
                    break;
                default:
                    throw new NotSupportedException("Unsupported NSNumber type");
            }
        }
        else if (value is NSString str)
        {
            result = new PSObject(str.ToString());
        }
        else if (value is NSDate date)
        {
            result = new PSObject(date.Date);
        }
        else if (value is NSData data)
        {
            result = new PSObject(data.Bytes);
        }
        else
        {
            throw new NotSupportedException($"Unsupported NSObject type: {value.GetType().FullName}");
        }

        return result;
    }
}

//TODO: Integrate into main cmdlet
[Cmdlet(VerbsData.ConvertFrom, "PlistWithPath")]
[OutputType(typeof(NSObject))]
public class ConvertFromPlistWithPath : PSCmdlet
{
    private FileInfo? _path;

    [Parameter(Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public FileInfo? Path
    {
        get => _path;

        set
        {
            _path = new FileInfo(System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, value!.ToString()));
        }
    }

    protected override void BeginProcessing()
    {
        WriteDebug($"Setting current directory: {SessionState.Path.CurrentFileSystemLocation.Path}");
        Environment.CurrentDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
        WriteDebug($"Current directory: {Environment.CurrentDirectory}");
    }

    protected override void ProcessRecord()
    {
        var dict = PropertyListParser.Parse(Path!.FullName) as NSObject;

        WriteObject(dict);
    }

    protected override void EndProcessing()
    {

    }
}
