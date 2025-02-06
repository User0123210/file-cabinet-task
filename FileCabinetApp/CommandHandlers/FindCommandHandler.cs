using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        public FindCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "find")
                {
                    string[] arguments = commandRequest.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                    const int propertyIndex = 0;
                    var property = arguments[propertyIndex];

                    if (arguments.Length > 1)
                    {
                        var value = arguments[propertyIndex + 1];
                        ReadOnlyCollection<FileCabinetRecord> found = new(Array.Empty<FileCabinetRecord>());

                        if (string.Equals(property, "FirstName", StringComparison.OrdinalIgnoreCase))
                        {
                            found = this.service.FindByFirstName(value);
                        }
                        else if (string.Equals(property, "LastName", StringComparison.OrdinalIgnoreCase))
                        {
                            found = this.service.FindByLastName(value);
                        }
                        else if (string.Equals(property, "DateOfBirth", StringComparison.OrdinalIgnoreCase))
                        {
                            bool isDate = DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                            if (isDate)
                            {
                                found = this.service.FindByDateOfBirth(date);
                            }
                        }

                        this.printer.Print(found);
                    }
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
