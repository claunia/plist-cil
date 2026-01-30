namespace Claunia.PropertyList.Shell;

using System;
using System.IO;
using System.Management.Automation;

[Cmdlet(VerbsData.ConvertFrom, "Plist")]
[OutputType(typeof(NSDictionary))]
public class ConvertFromPlist : PSCmdlet
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
