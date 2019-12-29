using System;
using System.Collections.Generic;
using System.Text;

namespace ScmBackup
{
    internal static class Extensions
    {
        public static void LogCmdOutput(this ILogger logger, string output)
        {
            var outputLines = output.Split("\n");
            foreach (var line in outputLines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                logger.Log(ErrorLevel.Info, $"      | {line}");
            }
        }
    }
}
