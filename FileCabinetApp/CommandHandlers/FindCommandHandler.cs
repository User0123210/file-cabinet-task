using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : CommandHandlerBase
    {
        public FindCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            string[] arguments = commandRequest?.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
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
    }
}
