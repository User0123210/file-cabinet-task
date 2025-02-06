using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;

#pragma warning disable IDE0060
#pragma warning disable CA1031

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
            new ("export", Export),
            new ("import", Import),
            new ("remove", Remove),
            new ("purge", Purge),
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
            new string[] { "export", "exports data from the service into the specified format and file", "The 'export' command exports data from the service into the specified format and file." },
            new string[] { "import", "imports data from the specified file in the specified format to the service", "The 'import' command imports data from the specified file in the specified format to the service." },
            new string[] { "remove", "removes record with the specified id from the service", "The 'remove' command removes record with the specified id from the service." },
            new string[] { "purge", "purge removes deleted records from the database", "The 'purge' command purge removes deleted records from the database." },
        };

        private static bool isRunning = true;
        private static string validationRules = "default";
        private static string storage = "memory";
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

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

            fileStream?.Close();
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
            var recordsCount = Program.fileCabinetService.GetStat;
            Console.WriteLine($"{recordsCount.Item1} overall records number, {recordsCount.Item2} deleted records number.");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), NameValidator);

            Console.Write("Last name: ");
            var lastName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), NameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(DateConverter, DateOfBirthValidator);

            Console.Write("Status: ");
            var status = ReadInput(ShortConverter, (short status) => new Tuple<bool, string>(true, "Everything alright"));

            Console.Write("Salary: ");
            var salary = ReadInput(DecimalConverter, SalaryValidator);

            Console.Write("Permissions: ");
            var permissions = ReadInput(CharConverter, PermissionsValidator);

            FileCabinetRecordParameterObject recordParameters = new () { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };
            int recordId = Program.fileCabinetService.CreateRecord(recordParameters);
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
                Console.Write("First name: ");
                var firstName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), NameValidator);

                Console.Write("Last name: ");
                var lastName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), NameValidator);

                Console.Write("Date of birth: ");
                var dateOfBirth = ReadInput(DateConverter, DateOfBirthValidator);

                Console.Write("Status: ");
                var status = ReadInput(ShortConverter, (short status) => new Tuple<bool, string>(true, "Everything alright"));

                Console.Write("Salary: ");
                var salary = ReadInput(DecimalConverter, SalaryValidator);

                Console.Write("Permissions: ");
                var permissions = ReadInput(CharConverter, PermissionsValidator);

                FileCabinetRecordParameterObject recordParameters = new () { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };

                try
                {
                    fileCabinetService.EditRecord(recordId, recordParameters);
                    Console.WriteLine($"Record #{recordId} is updated.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"#{recordId} record is not found.");
                }
            }
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input!);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string> NameValidator(string name)
        {
            int minLength = fileCabinetService.MinNameLength;
            int maxLength = fileCabinetService.MaxNameLength;
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name.Length < minLength || name.Length > maxLength)
                {
                    return new Tuple<bool, string>(isValid, $"Name's length should be more or equal {minLength} and less or equal {maxLength}");
                }
                else
                {
                    isValid = true;
                }
            }
            else
            {
                return new Tuple<bool, string>(isValid, $"Name shouldn't be empty or whitespace");
            }

            if (fileCabinetService.IsOnlyLetterName)
            {
                foreach (char character in name)
                {
                    if (!char.IsLetter(character))
                    {
                        isValid = false;
                        return new Tuple<bool, string>(isValid, "Name should contain only letters.");
                    }
                }
            }

            return new Tuple<bool, string>(isValid, "Everything alright.");
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime date)
        {
            DateTime minDate = fileCabinetService.MinDate;

            if (date < minDate || date > DateTime.Now)
            {
                return new Tuple<bool, string>(false, $"Date of birth shouldn't be less than {minDate} or more than current date");
            }

            return new Tuple<bool, string>(true, "Everything alright");
        }

        private static Tuple<bool, string, DateTime> DateConverter(string date)
        {
            bool isValid = DateTime.TryParseExact(date, fileCabinetService.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new Tuple<bool, string, DateTime>(isValid, $"Please, enter valid date of birth in format \"{fileCabinetService.DateFormat}\"", default);
            }

            return new Tuple<bool, string, DateTime>(isValid, "Everything alright", dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string value) => short.TryParse(value, out short newValue) ? new Tuple<bool, string, short>(true, "Everything alright", newValue) : new Tuple<bool, string, short>(false, $"Please, enter valid number in range from {short.MinValue} to {short.MaxValue}", default);

        private static Tuple<bool, string, decimal> DecimalConverter(string value) => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newValue) ? new Tuple<bool, string, decimal>(true, "Everything alright", newValue) : new Tuple<bool, string, decimal>(false, "Please, enter valid decimal number", default);

        private static Tuple<bool, string> SalaryValidator(decimal salary) => salary >= 0 ? new Tuple<bool, string>(true, "Everything alright") : new Tuple<bool, string>(false, "Salary should be more than 0");

        private static Tuple<bool, string, char> CharConverter(string value) => char.TryParse(value, out char newValue) ? new Tuple<bool, string, char>(true, "Everything alright", newValue) : new Tuple<bool, string, char>(false, "Please, enter valid character", default);

        private static Tuple<bool, string> PermissionsValidator(char permissions)
        {
            if (fileCabinetService.GetValidPermissions().Count > 0)
            {
                foreach (char permission in fileCabinetService.GetValidPermissions())
                {
                    if (char.Equals(char.ToLowerInvariant(permissions), permission))
                    {
                        return new Tuple<bool, string>(true, "Everything alright");
                    }
                }

                return new Tuple<bool, string>(false, $"Permissions should be one of {string.Join(", ", fileCabinetService.GetValidPermissions())}");
            }

            return new Tuple<bool, string>(true, "Everything alright");
        }

        private static void Find(string parameters)
        {
            string[] arguments = parameters is not null ? parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var property = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var value = arguments[propertyIndex + 1];
                ReadOnlyCollection<FileCabinetRecord> found = new (Array.Empty<FileCabinetRecord>());

                if (string.Equals(property, "FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    found = fileCabinetService.FindByFirstName(value);
                }
                else if (string.Equals(property, "LastName", StringComparison.OrdinalIgnoreCase))
                {
                    found = fileCabinetService.FindByLastName(value);
                }
                else if (string.Equals(property, "DateOfBirth", StringComparison.OrdinalIgnoreCase))
                {
                    bool isDate = DateTime.TryParseExact(value, fileCabinetService.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                    if (isDate)
                    {
                        found = fileCabinetService.FindByDateOfBirth(date);
                    }
                }

                foreach (var record in found)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MM-dd}, {record.Status}, {record.Salary}, {record.Permissions}");
                }
            }
        }

        private static void Export(string parameters)
        {
            string[] arguments = parameters is not null ? parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var sourceName = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var destination = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenWrite(destination);
                    using StreamWriter writer = new (stream);
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToCsv(writer);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToXml(writer);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }

        private static void Import(string parameters)
        {
            string[] arguments = parameters is not null ? parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var sourceName = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var source = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenRead(source);
                    using StreamReader reader = new (stream);
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromCsv(reader);
                        fileCabinetService.Restore(snapshot);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromXml(reader);
                        fileCabinetService.Restore(snapshot);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }

        private static void Remove(string parameters)
        {
            bool isValid = int.TryParse(parameters, out int recordId);

            if (isValid)
            {
                fileCabinetService.RemoveRecord(recordId);
            }
        }

        private static void Purge(string parameters)
        {
            fileCabinetService.Purge();
        }
    }
}