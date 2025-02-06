﻿using FileCabinetApp.CommandHandlers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;

#pragma warning disable IDE0060

namespace FileCabinetApp
{
    /// <summary>
    /// Represents Console App to exchange information with the FileCabinet.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Serafima Mochalova";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        public static bool IsRunning = true;
        private static string validationRules = "default";
        private static string storage = "memory";
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

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
                                fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                                storage = "memory";
                                break;
                            case "FILE":
                                fileStream = new (@"cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                                fileCabinetService = new FileCabinetFilesystemService(fileStream, new DefaultValidator());
                                storage = "file";
                                break;
                        }

                        Console.WriteLine($"Using {storage} storage.");
                    }

                    continue;
                }

                var handler = CreateCommandHandlers();

                handler.Handle(new AppCommandRequest(command, inputs.Length > 1 ? inputs[commandIndex + 1] : string.Empty));
            }
            while (IsRunning);

            fileStream?.Close();
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler();

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
            return helpHandler;
        }
    }
}