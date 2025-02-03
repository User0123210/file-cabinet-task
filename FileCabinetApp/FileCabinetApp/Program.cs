using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Serafima Mochalova";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "returns list of records from service", "The 'list' command returns list of records from service." },
            new string[] { "edit", "edits record with the specified id", "The 'edit' command edits record with the specified id." },
            new string[] { "find", "finds records based on the specified property value", "The 'find' command finds record based on the specified property value." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            (string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions) = GetAndValidateData();
            int recordId = Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, status, salary, permissions);
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            foreach (var record in Program.fileCabinetService.GetRecords())
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MM-dd}, {record.Status}, {record.Salary}, {record.Permissions}");
            }
        }

        private static void Edit(string parameters)
        {
            bool isValid = int.TryParse(parameters, out int recordId);

            if (!isValid)
            {
                    Console.WriteLine("Id is not valid.");
            }
            else
            {
                (string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions) = GetAndValidateData();
                try
                {
                    Program.fileCabinetService.EditRecord(recordId, firstName, lastName, dateOfBirth, status, salary, permissions);
                    Console.WriteLine($"Record #{recordId} is updated.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"#{recordId} record is not found.");
                }
            }
        }

        private static (string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions) GetAndValidateData()
        {
            string firstName = string.Empty;

            while (string.IsNullOrWhiteSpace(firstName))
            {
                Console.Write("First name: ");

                if (Console.ReadLine() is string s && !string.IsNullOrWhiteSpace(s))
                {
                    if (s.Length < 2 || s.Length > 60)
                    {
                        Console.WriteLine("First name's length should be more or equal 2 and less or equal 60");
                    }
                    else
                    {
                        firstName = s;
                    }
                }
                else
                {
                    Console.WriteLine("First name shouldn't be empty or whitespace");
                }
            }

            string lastName = string.Empty;

            while (string.IsNullOrWhiteSpace(lastName))
            {
                Console.Write("Last name: ");

                if (Console.ReadLine() is string s && !string.IsNullOrWhiteSpace(s))
                {
                    if (s.Length < 2 || s.Length > 60)
                    {
                        Console.WriteLine("Last name's length should be more or equal 2 and less or equal 60");
                    }
                    else
                    {
                        lastName = s;
                    }
                }
                else
                {
                    Console.WriteLine("Last name shouldn't be empty or whitespace");
                }
            }

            DateTime dateOfBirth = default;
            bool isValid = false;

            while (!isValid)
            {
                Console.Write("Date of birth: ");
                isValid = DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);

                if (!isValid)
                {
                    Console.WriteLine("Please, enter valid date of birth in format \"MM/dd/yyyy\".");
                }

                if (isValid && (dateOfBirth < DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) || dateOfBirth > DateTime.Now))
                {
                    Console.WriteLine("Date of birth shouldn't be less than 01-Jan-1950 or more than current date.");
                    isValid = false;
                }
            }

            short status = default;
            isValid = false;

            while (!isValid)
            {
                Console.Write("Status: ");
                isValid = short.TryParse(Console.ReadLine(), out status);

                if (!isValid)
                {
                    Console.WriteLine("Please, enter valid status in range from -32 768 to 32 767.");
                }
            }

            decimal salary = default;
            isValid = false;

            while (!isValid || salary < 0)
            {
                Console.Write("Salary: ");
                isValid = decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out salary);

                if (!isValid || salary < 0)
                {
                    Console.WriteLine("Please, enter valid numerical representation of salary. Salary should be more than 0.");
                }
            }

            char permissions = default;
            isValid = false;

            while (!isValid)
            {
                Console.Write("Permissions: ");
                isValid = char.TryParse(Console.ReadLine(), out permissions);

                if (!isValid)
                {
                    Console.WriteLine("Please, enter valid character.");
                }
            }

            return (firstName, lastName, dateOfBirth, status, salary, permissions);
        }

        private static void Find(string parameters)
        {
            string[] arguments = parameters is not null ? parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var property = arguments[propertyIndex];
            var value = arguments[propertyIndex + 1];
            FileCabinetRecord[] found = Array.Empty<FileCabinetRecord>();

            if (string.Equals(property, "FirstName", StringComparison.OrdinalIgnoreCase))
            {
                found = fileCabinetService.FindByFirstName(value);
            }
            else if (string.Equals(property, "LastName", StringComparison.OrdinalIgnoreCase))
            {
                found = fileCabinetService.FindByLastName(value);
            }

            foreach (var record in found)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MM-dd}, {record.Status}, {record.Salary}, {record.Permissions}");
            }
        }
    }
}