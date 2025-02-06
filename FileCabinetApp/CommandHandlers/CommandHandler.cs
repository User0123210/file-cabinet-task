using FileCabinetApp;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

#pragma warning disable SA1000

namespace FileCabinetApp
{
    public class CommandHandler : CommandHandlerBase
    {
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

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        public CommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(commandRequest.Command, StringComparison.OrdinalIgnoreCase));

                if (index >= 0)
                {
                    Commands[index].Item2(commandRequest.Parameters);
                }
                else
                {
                    PrintMissedCommandInfo(commandRequest.Command);
                }
            }
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat;
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

            FileCabinetRecordParameterObject recordParameters = new() { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };
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

                FileCabinetRecordParameterObject recordParameters = new() { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };

                try
                {
                    Program.FileCabinetService.EditRecord(recordId, recordParameters);
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
            int minLength = Program.FileCabinetService.MinNameLength;
            int maxLength = Program.FileCabinetService.MaxNameLength;
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

            if (Program.FileCabinetService.IsOnlyLetterName)
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
            DateTime minDate = Program.FileCabinetService.MinDate;

            if (date < minDate || date > DateTime.Now)
            {
                return new Tuple<bool, string>(false, $"Date of birth shouldn't be less than {minDate} or more than current date");
            }

            return new Tuple<bool, string>(true, "Everything alright");
        }

        private static Tuple<bool, string, DateTime> DateConverter(string date)
        {
            bool isValid = DateTime.TryParseExact(date, Program.FileCabinetService.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new Tuple<bool, string, DateTime>(isValid, $"Please, enter valid date of birth in format \"{Program.FileCabinetService.DateFormat}\"", default);
            }

            return new Tuple<bool, string, DateTime>(isValid, "Everything alright", dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string value) => short.TryParse(value, out short newValue) ? new Tuple<bool, string, short>(true, "Everything alright", newValue) : new Tuple<bool, string, short>(false, $"Please, enter valid number in range from {short.MinValue} to {short.MaxValue}", default);

        private static Tuple<bool, string, decimal> DecimalConverter(string value) => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newValue) ? new Tuple<bool, string, decimal>(true, "Everything alright", newValue) : new Tuple<bool, string, decimal>(false, "Please, enter valid decimal number", default);

        private static Tuple<bool, string> SalaryValidator(decimal salary) => salary >= 0 ? new Tuple<bool, string>(true, "Everything alright") : new Tuple<bool, string>(false, "Salary should be more than 0");

        private static Tuple<bool, string, char> CharConverter(string value) => char.TryParse(value, out char newValue) ? new Tuple<bool, string, char>(true, "Everything alright", newValue) : new Tuple<bool, string, char>(false, "Please, enter valid character", default);

        private static Tuple<bool, string> PermissionsValidator(char permissions)
        {
            if (Program.FileCabinetService.GetValidPermissions().Count > 0)
            {
                foreach (char permission in Program.FileCabinetService.GetValidPermissions())
                {
                    if (char.Equals(char.ToLowerInvariant(permissions), permission))
                    {
                        return new Tuple<bool, string>(true, "Everything alright");
                    }
                }

                return new Tuple<bool, string>(false, $"Permissions should be one of {string.Join(", ", Program.FileCabinetService.GetValidPermissions())}");
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
                ReadOnlyCollection<FileCabinetRecord> found = new(Array.Empty<FileCabinetRecord>());

                if (string.Equals(property, "FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    found = Program.FileCabinetService.FindByFirstName(value);
                }
                else if (string.Equals(property, "LastName", StringComparison.OrdinalIgnoreCase))
                {
                    found = Program.FileCabinetService.FindByLastName(value);
                }
                else if (string.Equals(property, "DateOfBirth", StringComparison.OrdinalIgnoreCase))
                {
                    bool isDate = DateTime.TryParseExact(value, Program.FileCabinetService.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                    if (isDate)
                    {
                        found = Program.FileCabinetService.FindByDateOfBirth(date);
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
                    using StreamWriter writer = new(stream);
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();

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
                    using StreamReader reader = new(stream);
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromCsv(reader);
                        Program.FileCabinetService.Restore(snapshot);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromXml(reader);
                        Program.FileCabinetService.Restore(snapshot);
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
                Program.FileCabinetService.RemoveRecord(recordId);
            }
        }

        private static void Purge(string parameters)
        {
            Program.FileCabinetService.Purge();
        }
    }
}
