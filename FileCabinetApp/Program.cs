using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Validators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(b =>
            {
                if (b)
                {
                    isRunning = false;
                }
            });

            purgeHandler.SetNext(exitHandler);
            findHandler.SetNext(purgeHandler);
            removeHandler.SetNext(findHandler);
            exportHandler.SetNext(removeHandler);
            importHandler.SetNext(exportHandler);
            editHandler.SetNext(importHandler);
            statHandler.SetNext(editHandler);
            listHandler.SetNext(statHandler);
            createHandler.SetNext(listHandler);
            helpHandler.SetNext(createHandler);
            insertHandler.SetNext(helpHandler);
            deleteHandler.SetNext(insertHandler);
            updateHandler.SetNext(deleteHandler);
            return updateHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            if (records is not null)
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MM-dd}, {record.Status}, {record.Salary}, {record.Permissions}");
                }
            }
        }
    }
}