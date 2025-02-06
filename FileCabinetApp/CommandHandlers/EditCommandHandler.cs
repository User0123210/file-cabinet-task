using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
        public EditCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            bool isValid = int.TryParse(commandRequest?.Parameters, out int recordId);

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
                var status = ReadInput(ShortConverter, StatusValidator);

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

        private static Tuple<bool, string> NameValidator(string name)
        {
            var validator = Program.FileCabinetService.ValidateName();
            Tuple<bool, string> validationResult = validator(name);

            if (!validationResult.Item1)
            {
                return new Tuple<bool, string>(false, validationResult.Item2);
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime date)
        {
            var validator = Program.FileCabinetService.ValidateDateOfBirth();
            Tuple<bool, string> validationResult = validator(date);

            if (!validationResult.Item1)
            {
                return new Tuple<bool, string>(false, validationResult.Item2);
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }

        private static Tuple<bool, string, DateTime> DateConverter(string date)
        {
            bool isValid = DateTime.TryParseExact(date, Program.FileCabinetService.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new Tuple<bool, string, DateTime>(isValid, $"Please, enter valid date of birth in format \"{Program.FileCabinetService.DateFormat}\"", default);
            }

            return new Tuple<bool, string, DateTime>(isValid, "Everything is alright", dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string value) => short.TryParse(value, out short newValue) ? new Tuple<bool, string, short>(true, "Everything is alright", newValue) : new Tuple<bool, string, short>(false, $"Please, enter valid number in range from {short.MinValue} to {short.MaxValue}", default);

        private static Tuple<bool, string> StatusValidator(short status)
        {
            var validator = Program.FileCabinetService.ValidateStatus();
            Tuple<bool, string> validationResult = validator(status);

            if (!validationResult.Item1)
            {
                return new Tuple<bool, string>(false, validationResult.Item2);
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string value) => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newValue) ? new Tuple<bool, string, decimal>(true, "Everything is alright", newValue) : new Tuple<bool, string, decimal>(false, "Please, enter valid decimal number", default);

        private static Tuple<bool, string> SalaryValidator(decimal salary)
        {
            var validator = Program.FileCabinetService.ValidateSalary();
            Tuple<bool, string> validationResult = validator(salary);

            if (!validationResult.Item1)
            {
                return new Tuple<bool, string>(false, validationResult.Item2);
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }

        private static Tuple<bool, string, char> CharConverter(string value) => char.TryParse(value, out char newValue) ? new Tuple<bool, string, char>(true, "Everything is alright", newValue) : new Tuple<bool, string, char>(false, "Please, enter valid character", default);

        private static Tuple<bool, string> PermissionsValidator(char permissions)
        {
            var validator = Program.FileCabinetService.ValidatePermissions();
            Tuple<bool, string> validationResult = validator(permissions);

            if (!validationResult.Item1)
            {
                return new Tuple<bool, string>(false, validationResult.Item2);
            }

            return new Tuple<bool, string>(true, "Everything is alright");
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
    }
}
