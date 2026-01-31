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
    ///
    /// Represents the byte stream input to be converted.
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    [AllowEmptyString]
    public byte InputObject { get; set; }

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
        var dict = PropertyListParser.Parse(_inputBuffer.ToArray()) as NSDictionary;
        WriteObject(dict);
    }

    #endregion Overrides
}

[Cmdlet(VerbsData.ConvertFrom, "PlistWithPath")]
[OutputType(typeof(NSDictionary))]
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
        var dict = PropertyListParser.Parse(Path!.FullName) as NSDictionary;

        WriteObject(dict);
    }

    protected override void EndProcessing()
    {

    }
}
