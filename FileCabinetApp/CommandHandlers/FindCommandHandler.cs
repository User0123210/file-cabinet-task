using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle find command in the IFileCabinetService.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>> print;

        /// <summary>
        /// Initializes new instance of the FindCommandHandler via the IFileCabinetService to handle commands in, and Action delegate to print results.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="print"></param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> print)
            : base(service)
        {
            this.print = print;
        }

        /// <summary>
        /// Handles commands in the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
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
                        IEnumerable<FileCabinetRecord> found = new List<FileCabinetRecord>();

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

                        this.print(found);
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
