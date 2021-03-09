﻿using System;
using System.Diagnostics;
using System.Linq;

namespace Ensconce.Cli
{
    public static class CliProgram
    {
        public static void MainLogic(string[] args)
        {
            Arguments.SetUpAndParseOptions(args);

            Logging.Log("Arguments parsed");

            if (!string.IsNullOrWhiteSpace(Arguments.DictionarySavePath) || !string.IsNullOrWhiteSpace(Arguments.DictionaryPostUrl))
            {
                DictionaryExport.ExportTagDictionary();
                return;
            }

            if (Arguments.BackupSources.Any())
            {
                Backup.DoBackup();
                return;
            }

            if (Arguments.ReadFromStdIn)
            {
                using (var input = System.Console.In)
                {
                    System.Console.Out.Write(input.ReadToEnd().Render());
                }
                // No other operations can be performed when reading from stdin
                return;
            }

            if (Arguments.DeployReports || Arguments.DeployReportingRole)
            {
                Reporting.RunReportingServices();
                // No other operations can be performed when deploying reports
                return;
            }

            if (Arguments.UpdateConfig)
            {
                TagSubstitution.DefaultUpdate();
            }

            if (!string.IsNullOrEmpty(Arguments.TemplateFilters))
            {
                TagSubstitution.UpdateFiles();
            }

            if (!string.IsNullOrEmpty(Arguments.ConnectionString) || !string.IsNullOrEmpty(Arguments.DatabaseName))
            {
                DatabaseInteraction.DoDeployment();
            }

            if (Arguments.Replace)
            {
                Arguments.DeployTo.ForEach(FileInteraction.DeleteDirectory);
            }

            if (Arguments.CopyTo || Arguments.Replace)
            {
                Arguments.DeployTo.ForEach(dt => FileInteraction.CopyDirectory(Arguments.DeployFrom, dt));
            }

            Logging.Log("Ensconce operation complete");
        }
    }
}
