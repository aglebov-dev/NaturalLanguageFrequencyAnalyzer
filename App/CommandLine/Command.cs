using CommandLine;

namespace App.CommandLine
{
    internal class Command
    {
        [Value(0, MetaName = "folder", Required = true, HelpText = "Source folder")]
        public string FolderPath { get; set; }

        [Value(1, MetaName = "report", Required = true, HelpText = "Report file path")]
        public string ReportPath { get; set; }
    }
}
