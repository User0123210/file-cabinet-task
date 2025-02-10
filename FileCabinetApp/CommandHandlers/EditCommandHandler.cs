using FileCabinetApp.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle edit command in the IFileCabinetService.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of the EditCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service">IFileCabinetService to handle commands in.</param>
        public EditCommandHandler(IFileCabinetService service)
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
                if (commandRequest.Command == "edit")
                {
                    bool isValid = int.TryParse(commandRequest.Parameters, out int recordId);

                    if (!isValid)
                    {
                        Console.WriteLine("Id is not valid.");
                    }
                    else
                    {
                        Console.Write("First name: ");
                        var firstName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), this.GetValidator(typeof(DateOfBirthValidator)) is not null ? this.GetValidator(typeof(FirstNameValidator)) !.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                        Console.Write("Last name: ");
                        var lastName = ReadInput((string name) => new Tuple<bool, string, string>(true, "String is a string", name), this.GetValidator(typeof(DateOfBirthValidator)) is not null ? this.GetValidator(typeof(LastNameValidator)) !.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                        Console.Write("Date of birth: ");
                        var dateOfBirth = ReadInput(this.DateConverter, this.GetValidator(typeof(DateOfBirthValidator)) is not null ? this.GetValidator(typeof(DateOfBirthValidator)) !.ValidateParameters : (d => new Tuple<bool, string>(true, "Everything is alright")));

                        Console.Write("Status: ");
                        var status = ReadInput(ShortConverter, this.GetValidator(typeof(StatusValidator)) is not null ? this.GetValidator(typeof(StatusValidator)) !.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                        Console.Write("Salary: ");
                        var salary = ReadInput(DecimalConverter, this.GetValidator(typeof(SalaryValidator)) is not null ? this.GetValidator(typeof(SalaryValidator)) !.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                        Console.Write("Permissions: ");
                        var permissions = ReadInput(CharConverter, this.GetValidator(typeof(PermissionsValidator)) is not null ? this.GetValidator(typeof(PermissionsValidator)) !.ValidateParameters : (s => new Tuple<bool, string>(true, "Everything is alright")));

                        FileCabinetRecordParameterObject recordParameters = new () { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };

                        try
                        {
                            this.service.EditRecord(recordId, recordParameters);
                            Console.WriteLine($"Record #{recordId} is updated.");
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine($"#{recordId} record is not found.");
                        }
                    }
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }

        private IRecordValidator? GetValidator(Type type)
        {
            if (this.service.GetValidators() is not null)
            {
                foreach (var recordValidator in this.service.GetValidators() !)
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
