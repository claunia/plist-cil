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
        if (value == null)
            return new(null);

        switch(value)
        {
            case NSDictionary dict:
                var result = new PSObject();
                foreach(var key in dict.Keys)
                {
                    result.Properties.Add(new PSNoteProperty(key.ToString(), FromNSObject(dict.ObjectForKey(key))));
                }
                return result;
            case NSArray array:
                var psArray = new object[array.Count];
                int i = 0;
                foreach (var element in array)
                {
                    psArray[i++] = FromNSObject(element);
                }

                return new PSObject(psArray[..i]);
            case NSNumber number:
                switch(number.GetNSNumberType())
                {
                    case NSNumber.BOOLEAN:
                        return new PSObject(number.ToBool());
                    case NSNumber.INTEGER:
                        return new PSObject(number.ToLong());
                    case NSNumber.REAL:
                        return new PSObject(number.ToDouble());
                    default:
                        throw new NotSupportedException("Unsupported NSNumber type");
                }
            case NSString:
                return new PSObject(value.ToString());
            case NSDate date:
                return new PSObject(date.Date);
            case NSData data:
                return new PSObject(data.Bytes);
            default:
                throw new NotSupportedException($"Unsupported NSObject type: {value.GetType().FullName}");
        }
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
