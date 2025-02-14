using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle delete command in IFileCabinetService.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of InsertCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles commands from the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "delete")
                {
                    string[] arguments = commandRequest.Parameters.Split(" ", 2);
                    string[] criteria = arguments[1].Split(", ");
                    var records = this.service.GetRecords().ToArray();

                    foreach (var record in records)
                    {
                        bool toDelete = true;

                        for (int i = 0; i < criteria.Length; i++)
                        {
                            string property = criteria[i].Split("=", 2)[0].Trim();
                            string value = criteria[i].Split("=", 2)[1].Trim();

                            switch (property.ToUpperInvariant())
                            {
                                case "ID":
                                    if (record.Id != int.Parse(value, CultureInfo.InvariantCulture))
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "FIRSTNAME":
                                    if (record.FirstName.ToUpperInvariant() != value.ToUpperInvariant())
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "LASTNAME":
                                    if (record.LastName.ToUpperInvariant() != value.ToUpperInvariant())
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "DATEOFBIRTH":
                                    if (record.DateOfBirth != DateTime.ParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None))
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "STATUS":
                                    if (record.Status != short.Parse(value, CultureInfo.InvariantCulture))
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "SALARY":
                                    if (record.Salary != decimal.Parse(value, CultureInfo.InvariantCulture))
                                    {
                                        toDelete = false;
                                    }

                                    break;
                                case "PERMISSIONS":
                                    if (record.Permissions == char.Parse(value))
                                    {
                                        this.service.RemoveRecord(record.Id);
                                    }

                                    break;
                            }
                        }

                        if (toDelete)
                        {
                            this.service.RemoveRecord(record.Id);
                        }
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
