using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Validators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

#pragma warning disable IDE0060, CA1303

namespace FileCabinetApp
{
    /// <summary>
    /// Represents Console App to exchange information with the FileCabinet.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Serafima Mochalova";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static bool isRunning = true;
        private static string validationRules = "default";
        private static string storage = "memory";
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());

        /// <summary>
        /// Runs Console Application.
        /// </summary>
        /// <param name="args">The arguments of the application.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine(HintMessage);
            Console.WriteLine();
            FileStream? fileStream = null;

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(new char[] { '=', ' ' }, 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                    continue;
                }

                if (command == "--validation-rules" || command == "-v")
                {
                    if (inputs.Length > 1)
                    {
                        switch (inputs[commandIndex + 1].ToUpperInvariant())
                        {
                            case "DEFAULT":
                                fileCabinetService.ChangeValidatorToDefault();
                                validationRules = "default";
                                break;
                            case "CUSTOM":
                                fileCabinetService.ChangeValidatorToCustom();
                                validationRules = "custom";
                                break;
                        }

                        Console.WriteLine($"Using {validationRules} validation rules.");
                    }

                    continue;
                }
                else if (command == "--storage" || command == "-s")
                {
                    if (inputs.Length > 1)
                    {
                        switch (inputs[commandIndex + 1].ToUpperInvariant())
                        {
                            case "MEMORY":
                                fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
                                storage = "memory";
                                break;
                            case "FILE":
                                fileStream = new (@"cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                                fileCabinetService = new FileCabinetFilesystemService(fileStream, new ValidatorBuilder().CreateDefault());
                                storage = "file";
                                break;
                        }

                        Console.WriteLine($"Using {storage} storage.");
                    }

                    continue;
                }
                else if (command == "use-stopwatch")
                {
                    fileCabinetService = new ServiceMeter(fileCabinetService);
                    Console.WriteLine($"Using stopwatch.");

                    continue;
                }
                else if (command == "use-logger")
                {
                    fileCabinetService = new ServiceLogger(fileCabinetService);
                    Console.WriteLine($"Using logging.");

                    continue;
                }

                var handler = CreateCommandHandlers();

                handler.Handle(new AppCommandRequest(command, inputs.Length > 1 ? inputs[commandIndex + 1] : string.Empty));
            }
            while (isRunning);

            fileStream?.Close();
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exitHandler = new ExitCommandHandler(b =>
            {
                if (b)
                {
                    isRunning = false;
                }
            });

            exitHandler.SetNext(helpHandler);
            purgeHandler.SetNext(exitHandler);
            exportHandler.SetNext(purgeHandler);
            importHandler.SetNext(exportHandler);
            statHandler.SetNext(importHandler);
            createHandler.SetNext(statHandler);
            insertHandler.SetNext(createHandler);
            deleteHandler.SetNext(insertHandler);
            updateHandler.SetNext(deleteHandler);
            selectHandler.SetNext(updateHandler);
            return selectHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> recs, string[] arguments)
        {

                    StringBuilder row = new ();
                    PropertyInfo[] initialProperties = typeof(FileCabinetRecord).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = initialProperties.Where(p => arguments.Any(a => a.Trim().ToUpperInvariant() == p.Name.ToUpperInvariant())).ToArray();
                    properties = properties.Length > 0 ? properties : initialProperties;
                    int cols = properties.Length;
                    int[] widths = new int[cols];
                    StringBuilder edge = new ();

                    for (int c = 0; c < cols; c++)
                    {
                        var pr = properties[c];

                        if (pr is not null)
                        {
                            var lengths = recs.Select(e => e is not null && pr.GetValue(e) is not null ? pr.GetValue(e) !.ToString() !.Length : 0);
                            int max = 0;

                            if (lengths is not null && lengths.Any())
                            {
                                max = lengths.Max(e => e);
                            }

                            widths[c] = pr.Name.Length > max ? pr.Name.Length : max;
                        }

                        edge = edge.Append("+" + new string('-', widths[c] + 2));
                    }

                    edge = edge.Append(CultureInfo.InvariantCulture, $"+{Environment.NewLine}");
                    int colNum = 0;

                    Console.WriteLine(edge);

                    foreach (var p in properties)
                    {
                        row = row.Append(string.Format(CultureInfo.InvariantCulture, "| {0} ", p.Name + new string(' ', widths[colNum] - p!.Name.ToString() !.Length)));
                        colNum++;
                    }

                    row = row.Append(CultureInfo.InvariantCulture, $"|{Environment.NewLine}");

                    Console.WriteLine(row);
                    Console.WriteLine(edge);

                    foreach (var rec in recs)
                    {
                        row = new StringBuilder();

                        for (int j = 0; j < cols; j++)
                        {
                            var p = properties[j].GetValue(rec);
                            row = row.Append(string.Format(CultureInfo.InvariantCulture, "| {0} ", p is string || p is char ? p + new string(' ', widths[j] - p!.ToString() !.Length) : new string(' ', widths[j] - p!.ToString() !.Length) + p!.ToString()));
                        }

                        row = row.Append(CultureInfo.InvariantCulture, $"|{Environment.NewLine}");
                        Console.WriteLine(row);
                        Console.WriteLine(edge);
                    }
        }
    }
}