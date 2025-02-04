using System.Collections.Generic;
using System.Globalization;
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
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static readonly FileCabinetService FileCabinetService = new (new DefaultValidator());

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new ("help", PrintHelp),
            new ("exit", Exit),
            new ("stat", Stat),
            new ("create", Create),
            new ("list", List),
            new ("edit", Edit),
            new ("find", Find),
            new ("find", Find),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "returns list of records from service", "The 'list' command returns list of records from service." },
            new string[] { "edit", "edits record with the specified id", "The 'edit' command edits record with the specified id." },
            new string[] { "find", "finds records based on the specified property value", "The 'find' command finds record based on the specified property value." },
        };

        private static bool isRunning = true;
        private static string validationRules = "default";

        /// <summary>
        /// Runs Console Application.
        /// </summary>
        /// <param name="args">The arguments of the application.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(new char[] { '=', ' ' }, 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                if (command == "--validation-rules" || command == "-v")
                {
                    if (inputs.Length > 1)
                    {
                        switch (inputs[commandIndex + 1].ToUpperInvariant())
                        {
                            case "DEFAULT":
                                FileCabinetService.ChangeValidatorToDefault();
                                validationRules = "default";
                                break;
                            case "CUSTOM":
                                FileCabinetService.ChangeValidatorToCustom();
                                validationRules = "custom";
                                break;
                        }

                        Console.WriteLine($"Using {validationRules} validation rules.");
                    }

                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
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
            var recordsCount = Program.FileCabinetService.GetStat;
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            FileCabinetRecordParameterObject recordParameters = GetAndValidateRecordData();
            int recordId = Program.FileCabinetService.CreateRecord(recordParameters);
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            foreach (var record in Program.FileCabinetService.GetRecords())
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
                FileCabinetRecordParameterObject recordParameters = GetAndValidateRecordData();
                try
                {
                    FileCabinetService.EditRecord(recordId, recordParameters);
                    Console.WriteLine($"Record #{recordId} is updated.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"#{recordId} record is not found.");
                }
            }
        }

        private static FileCabinetRecordParameterObject GetAndValidateRecordData()
        {
            string firstName = GetParameterInput("First name", GetAndValidateName);
            string lastName = GetParameterInput("Last name", GetAndValidateName);
            DateTime dateOfBirth = GetParameterInput("Date of birth", GetAndValidateDateOfBirth);

            short status = GetParameterInput("Status", (string? parameterInput) =>
            {
                bool isValid = short.TryParse(parameterInput, out short newStatus);

                if (!isValid)
                {
                    Console.WriteLine($"Please, enter valid status in range from {short.MinValue} to {short.MaxValue}.");
                }

                return (isValid, newStatus);
            });

            decimal salary = GetParameterInput("Salary", (string? parameterInput) =>
            {
                bool isValid = decimal.TryParse(parameterInput, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newSalary);

                if (!isValid || newSalary < 0)
                {
                    isValid = false;
                    Console.WriteLine("Please, enter valid numerical representation of salary. Salary should be more than 0.");
                }

                return (isValid, newSalary);
            });

            char permissions = GetParameterInput("Permissions", GetAndValidatePermissions);

            return new FileCabinetRecordParameterObject() { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };
        }

        private static T GetParameterInput<T>(string parametersName, Func<string?, (bool, T)> validatorFunction)
        {
            bool isValid = false;
            T? parameter = default;

            while (!isValid)
            {
                Console.Write($"{parametersName}: ");
                string? parameterInput = Console.ReadLine();
                (isValid, parameter) = validatorFunction(parameterInput);
            }

            return parameter!;
        }

        private static (bool, string) GetAndValidateName(string? name)
        {
            int minLength = FileCabinetService.MinNameLength;
            int maxLength = FileCabinetService.MaxNameLength;
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name.Length < minLength || name.Length > maxLength)
                {
                    Console.WriteLine($"Name's length should be more or equal {minLength} and less or equal {maxLength}");
                }
                else
                {
                    isValid = true;
                }
            }
            else
            {
                name = string.Empty;
                Console.WriteLine($"Name shouldn't be empty or whitespace");
            }

            foreach (char character in name)
            {
                if (!char.IsLetter(character))
                {
                    isValid = false;
                    Console.WriteLine("Name should contain only letters.");
                }
            }

            return (isValid, name);
        }

        private static (bool, DateTime) GetAndValidateDateOfBirth(string? date)
        {
            bool isValid;
            DateTime minDate = FileCabinetService.MinDate;

            isValid = DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                Console.WriteLine($"Please, enter valid date of birth in format \"MM/dd/yyyy\".");
            }

            if (isValid && (dateOfBirth < minDate || dateOfBirth > DateTime.Now))
            {
                Console.WriteLine($"Date of birth shouldn't be less than {minDate} or more than current date.");
                isValid = false;
            }

            return (isValid, dateOfBirth);
        }

        private static (bool, char) GetAndValidatePermissions(string? permissions)
        {
            bool isValid = char.TryParse(permissions, out char newPermissions);

            if (!isValid)
            {
                Console.WriteLine("Please, enter valid character.");
                return (isValid, newPermissions);
            }

            if (FileCabinetService.GetValidPermissions().Length > 0)
            {
                isValid = false;

                foreach (char permission in FileCabinetService.GetValidPermissions())
                {
                    if (char.Equals(char.ToLowerInvariant(newPermissions), permission))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    Console.WriteLine($"Permissions should be one of {string.Join(", ", FileCabinetService.GetValidPermissions())}.");
                }
            }

            return (isValid, newPermissions);
        }

        private static void Find(string parameters)
        {
            string[] arguments = parameters is not null ? parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var property = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var value = arguments[propertyIndex + 1];
                FileCabinetRecord[] found = Array.Empty<FileCabinetRecord>();

                if (string.Equals(property, "FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    found = FileCabinetService.FindByFirstName(value);
                }
                else if (string.Equals(property, "LastName", StringComparison.OrdinalIgnoreCase))
                {
                    found = FileCabinetService.FindByLastName(value);
                }
                else if (string.Equals(property, "DateOfBirth", StringComparison.OrdinalIgnoreCase))
                {
                    bool isDate = DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                    if (isDate)
                    {
                        found = FileCabinetService.FindByDateOfBirth(date);
                    }
                }

                foreach (var record in found)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MM-dd}, {record.Status}, {record.Salary}, {record.Permissions}");
                }
            }
        }
    }
}