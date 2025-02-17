using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class of the command handler to handle help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private static readonly string[][] HelpMessages = new string[][]
{
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "update", "updates records based on the specified criteria", "The 'update' command updates records based on the specified criteria." },
            new string[] { "export", "exports data from the service into the specified format and file", "The 'export' command exports data from the service into the specified format and file." },
            new string[] { "import", "imports data from the specified file in the specified format to the service", "The 'import' command imports data from the specified file in the specified format to the service." },
            new string[] { "purge", "purge removes deleted records from the database", "The 'purge' command purge removes deleted records from the database." },
            new string[] { "insert", "inserts new record with the specified values", "The 'insert' command inserts new record with the specified values." },
            new string[] { "delete", "deletes records based on the specified criteria", "The 'delete' command deletes records based on the specified criteria." },
            new string[] { "select", "selects info about records based on the specified criteria", "The 'select' command selects info about records based on the specified criteria." },
            new string[] { "--validation-rules", "sets current validation rules as custom or default, short form -v", "The '--validation-rules' parameter sets current validation rules as custom or default, short form -v." },
            new string[] { "--storage", "sets current storage as file or memory, short form -s", "The '--storage' parameter sets current storage as file or memory, short form -s." },
            new string[] { "use-stopwatch", "enables using stopwatch for the current file cabinet service", "The 'use-stopwatch' enables using stopwatch for the current file cabinet service." },
            new string[] { "use-logger", "enables using logger for the current file cabinet service", "The 'use-logger' parameter enables using logger for the current file cabinet service." },
};

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        /// <summary>
        /// Initializes new instance of the HelpCommandHandler.
        /// </summary>
        public HelpCommandHandler()
        {
        }

        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "help")
                {
                    if (!string.IsNullOrEmpty(commandRequest.Parameters))
                    {
                        var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], commandRequest.Parameters, StringComparison.OrdinalIgnoreCase));
                        if (index >= 0)
                        {
                            Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                        }
                        else
                        {
                            Console.WriteLine($"There is no explanation for '{commandRequest.Parameters}' command.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Available commands:");

                        foreach (var helpMessage in HelpMessages)
                        {
                            Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                        }
                    }

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"The {commandRequest.Command} is not one of possible commands.");

                    string[] mostSimisliar = HelpMessages.Select(m => m[0]).Where(s => string.Join(string.Empty, commandRequest.Command.Distinct()).Count(c => s.Contains(c, StringComparison.InvariantCultureIgnoreCase)) >= Math.Ceiling((double)s.Length / 2)).ToArray();
                    Console.WriteLine("The most similiar commands are:");

                    foreach (var command in mostSimisliar)
                    {
                        Console.WriteLine(command);
                    }

                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
