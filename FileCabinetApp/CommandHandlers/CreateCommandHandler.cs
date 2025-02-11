using FileCabinetApp.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Reprsents class to handle create command in the IFileCabinetService.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of the CreateCommandHandler class via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service">IFileCabinetService to handle commands in.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles commands in the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "create")
                {
                    var firstNameValidator = this.GetValidator(typeof(FirstNameValidator));

                    Console.Write("First name: ");
                    var firstName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), firstNameValidator is not null ? firstNameValidator.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                    var lastNameValidator = this.GetValidator(typeof(LastNameValidator));

                    Console.Write("Last name: ");
                    var lastName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), lastNameValidator is not null ? lastNameValidator.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                    var dateValidator = this.GetValidator(typeof(DateOfBirthValidator));

                    Console.Write("Date of birth: ");
                    var dateOfBirth = ReadInput(this.DateConverter, dateValidator is not null ? dateValidator.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                    var statusValidator = this.GetValidator(typeof(StatusValidator));
                    Console.Write("Status: ");
                    var status = ReadInput(ShortConverter, statusValidator is not null ? statusValidator.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                    var salaryValidator = this.GetValidator(typeof(SalaryValidator));

                    Console.Write("Salary: ");
                    var salary = ReadInput(DecimalConverter, salaryValidator is not null ? salaryValidator.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                    var permissionsValidator = this.GetValidator(typeof(PermissionsValidator));

                    Console.Write("Permissions: ");
                    var permissions = ReadInput(CharConverter, permissionsValidator is not null ? permissionsValidator.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                    FileCabinetRecordParameterObject recordParameters = new () { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };
                    int recordId = this.service.CreateRecord(recordParameters);
                    Console.WriteLine($"Record #{recordId} is created.");
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }

        private IRecordValidator? GetValidator(Type type)
        {
            var validators = this.service.GetValidators();

            if (validators is not null)
            {
                foreach (var recordValidator in validators)
                {
                    if (recordValidator.GetType() == type)
                    {
                        return recordValidator;
                    }
                }
            }

            return null;
        }

        private Tuple<bool, string, DateTime> DateConverter(string date)
        {
            bool isValid = DateTime.TryParseExact(date, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new Tuple<bool, string, DateTime>(isValid, $"Please, enter valid date of birth in format \"{this.service.DateFormat}\"", default);
            }

            return new Tuple<bool, string, DateTime>(isValid, "Everything is alright", dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string value) => short.TryParse(value, out short newValue) ? new Tuple<bool, string, short>(true, "Everything is alright", newValue) : new Tuple<bool, string, short>(false, $"Please, enter valid number in range from {short.MinValue} to {short.MaxValue}", default);

        private static Tuple<bool, string, decimal> DecimalConverter(string value) => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newValue) ? new Tuple<bool, string, decimal>(true, "Everything is alright", newValue) : new Tuple<bool, string, decimal>(false, "Please, enter valid decimal number", default);

        private static Tuple<bool, string, char> CharConverter(string value) => char.TryParse(value, out char newValue) ? new Tuple<bool, string, char>(true, "Everything is alright", newValue) : new Tuple<bool, string, char>(false, "Please, enter valid character", default);

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<object, Tuple<bool, string>> validator)
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

                var validationResult = value is not null ? validator(value) : new Tuple<bool, string>(false, "Something went wrong");
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
