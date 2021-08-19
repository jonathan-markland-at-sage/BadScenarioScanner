using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BadScenarioScanner
{
    class Program
    {
        /// <summary>
        /// Return process exit code 1, which will be readable by %ERRORLEVEL% in a .bat file.
        /// This indicates that the program failed for some reason.
        /// </summary>
        const int processExitFail = 1;

        /// <summary>
        /// Return process exit code 0, which will be readable by %ERRORLEVEL% in a .bat file.
        /// This indicates that the program succeeded.
        /// </summary>
        const int processExitSuccess = 0;

        static void ScanForSuspiciousFeatureFiles(string rootPath)
        {
            var scenarioRegex = new Regex("^[ \t]*Scenario:");
            var scenarioOutlineRegex = new Regex("^[ \t]*Scenario Outline:");
            var examplesRegex = new Regex("^[ \t]*Examples:");

            int countOfFilesExamined = 0;
            int bugsFound = 0;

            foreach (var path in Directory.EnumerateFiles(rootPath, "*.feature", SearchOption.AllDirectories))
            {
                ++countOfFilesExamined;

                bool lineIsScenarioOrOutlineOrExamples(string line)
                {
                    return scenarioRegex.IsMatch(line) || scenarioOutlineRegex.IsMatch(line) || examplesRegex.IsMatch(line);
                }

                bool isSuspectedBug(string previousInterestingLine, string line)
                {
                    return examplesRegex.IsMatch(line)
                        && !scenarioOutlineRegex.IsMatch(previousInterestingLine);
                }

                void showLineReportInVisualStudioStyle(string path, int lineNumber, string message)
                {
                    Console.WriteLine($"{path}({lineNumber}): {message}");
                }

                var fileContent = File.ReadAllLines(path);
                int lineNumber = 1;
                string previousInterestingLine = ""; // TODO: Communicate WHY we do this.

                foreach (var line in fileContent)
                {
                    if (lineIsScenarioOrOutlineOrExamples(line))
                    {
                        if (isSuspectedBug(previousInterestingLine, line))
                        {
                            showLineReportInVisualStudioStyle(path, lineNumber, "Suspicious Examples table without Scenario Outline.");
                            bugsFound++;
                        }

                        previousInterestingLine = line;
                    }

                    ++lineNumber;
                }
            }

            Console.WriteLine($"{countOfFilesExamined} Feature files were examined and {bugsFound} bugs were found");
        }


        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Missing command line argument:  Path required to folder to scan.");
                Console.WriteLine(@"eg: C:\dev\myapp\src");
                Console.WriteLine("SpecFlow .feature files are automatically selected.");

                return processExitFail;  
            }

            try
            {
                var rootPath = args[0];
                ScanForSuspiciousFeatureFiles(rootPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                // Possibly log the detail eg: stack trace in a log file, to hide it from the user.

                return processExitFail;
            }

            return processExitSuccess;
        }
    }
}
