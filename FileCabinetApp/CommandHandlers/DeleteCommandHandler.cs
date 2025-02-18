using System.Globalization;

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

                    if (arguments.Length > 1)
                    {
                        string[] criteria = arguments[1].Split(", ");
                        HashSet<FileCabinetRecord> records = new (new FileCabinetRecordComparer());

                        for (int j = 0; j < criteria.Length; j++)
                        {
                            string property = criteria[j].Split("=", 2)[0].Trim();
                            string value = criteria[j].Split("=", 2)[1].Trim();
                            IEnumerable<FileCabinetRecord>? recordsToAdd = null;

                            switch (property.ToUpperInvariant())
                            {
                                case "FIRSTNAME":
                                    recordsToAdd = this.service.FindByFirstName(value.ToUpperInvariant());
                                    break;
                                case "LASTNAME":
                                    recordsToAdd = this.service.FindByLastName(value.ToUpperInvariant());
                                    break;
                                case "DATEOFBIRTH":
                                    if (DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                                    {
                                        recordsToAdd = this.service.FindByDateOfBirth(newDate);
                                    }

                                    break;
                            }

                            if (recordsToAdd is not null)
                            {
                                foreach (var record in recordsToAdd)
                                {
                                    if (!records.Contains(record))
                                    {
                                        records.Add(record);
                                    }
                                }
                            }
                        }

                        if (records.Count == 0)
                        {
                            records = this.service.GetRecords().ToHashSet();
                        }

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
                                        bool isInt = int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out int id);
                                        toDelete = isInt && record.Id == id && toDelete;
                                        break;
                                    case "FIRSTNAME":
                                        toDelete = record.FirstName.ToUpperInvariant() == value.ToUpperInvariant() && toDelete;
                                        break;
                                    case "LASTNAME":
                                        toDelete = record.LastName.ToUpperInvariant() == value.ToUpperInvariant() && toDelete;
                                        break;
                                    case "DATEOFBIRTH":
                                        bool isDate = DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                                        toDelete = isDate && record.DateOfBirth == date && toDelete;
                                        break;
                                    case "STATUS":
                                        bool isShort = short.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out short newStatus);
                                        toDelete = isShort && record.Status == newStatus && toDelete;
                                        break;
                                    case "SALARY":
                                        bool isDecimal = decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newSalary);
                                        toDelete = isDecimal && record.Salary == newSalary && toDelete;
                                        break;
                                    case "PERMISSIONS":
                                        bool isChar = char.TryParse(value, out char newPermissions);
                                        toDelete = isChar && record.Permissions == newPermissions && toDelete;
                                        break;
                                }
                            }

                            if (toDelete)
                            {
                                this.service.RemoveRecord(record.Id);
                            }
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
