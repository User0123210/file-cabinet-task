using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle update command in IFileCabinetService.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of UpdateCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public UpdateCommandHandler(IFileCabinetService service)
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
                if (commandRequest.Command == "update")
                {
                    string[] arguments = commandRequest.Parameters.Split("where", 2);
                    var regex = new Regex(Regex.Escape("set"));

                    string[] properties = regex.Replace(arguments[0], string.Empty, 1).Trim().Split(", ");

                    if (arguments.Length > 1)
                    {
                        string[] values = arguments[1].Trim().Split(", ");
                        FileCabinetRecordParameterObject newParams = new ();

                        string? firstName = null;
                        string? lastName = null;
                        DateTime? dateOfBirth = null;
                        short? status = null;
                        decimal? salary = null;
                        char? permissions = null;

                        for (int i = 0; i < properties.Length; i++)
                        {
                            string[] propertyValue = properties[i].Split("=", 2);

                            if (propertyValue.Length > 1)
                            {
                                string property = propertyValue[0].Trim();
                                string value = propertyValue[1].Trim();

                                switch (property.ToUpperInvariant())
                                {
                                    case "FIRSTNAME":
                                        firstName = value;
                                        break;
                                    case "LASTNAME":
                                        lastName = value;
                                        break;
                                    case "DATEOFBIRTH":
                                        dateOfBirth = DateTime.ParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                                        break;
                                    case "STATUS":
                                        status = short.Parse(value, CultureInfo.InvariantCulture);
                                        break;
                                    case "SALARY":
                                        salary = decimal.Parse(value, CultureInfo.InvariantCulture);
                                        break;
                                    case "PERMISSIONS":
                                        permissions = char.Parse(value);
                                        break;
                                }
                            }
                        }

                        HashSet<FileCabinetRecord> records = new (new FileCabinetRecordComparer());

                        for (int j = 0; j < values.Length; j++)
                        {
                            string[] propertyValue = values[j].Split("=", 2);

                            if (propertyValue.Length > 1)
                            {
                                string property = propertyValue[0].Trim();
                                string value = propertyValue[1].Trim();
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
                        }

                        if (records.Count == 0)
                        {
                            records = this.service.GetRecords().ToHashSet();
                        }

                        foreach (var record in records)
                        {
                            bool toEdit = true;

                            for (int i = 0; i < values.Length; i++)
                            {
                                string[] propertyValue = values[i].Split("=", 2);

                                if (propertyValue.Length > 1)
                                {
                                    string property = propertyValue[0].Trim();
                                    string value = propertyValue[1].Trim();

                                    switch (property.ToUpperInvariant())
                                    {
                                        case "ID":
                                            bool isInt = int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out int id);
                                            toEdit = isInt && record.Id == id && toEdit;
                                            break;
                                        case "FIRSTNAME":
                                            toEdit = record.FirstName.ToUpperInvariant() == value.ToUpperInvariant() && toEdit;
                                            break;
                                        case "LASTNAME":
                                            toEdit = record.LastName.ToUpperInvariant() == value.ToUpperInvariant() && toEdit;
                                            break;
                                        case "DATEOFBIRTH":
                                            bool isDate = DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                                            toEdit = isDate && record.DateOfBirth == date && toEdit;
                                            break;
                                        case "STATUS":
                                            bool isShort = short.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out short newStatus);
                                            toEdit = isShort && record.Status == newStatus && toEdit;
                                            break;
                                        case "SALARY":
                                            bool isDecimal = decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal newSalary);
                                            toEdit = isDecimal && record.Salary == newSalary && toEdit;
                                            break;
                                        case "PERMISSIONS":
                                            bool isChar = char.TryParse(value, out char newPermissions);
                                            toEdit = isChar && record.Permissions == newPermissions && toEdit;
                                            break;
                                    }
                                }
                            }

                            if (toEdit)
                            {
                                firstName = firstName is null ? record.FirstName : firstName;
                                lastName = lastName is null ? record.LastName : lastName;
                                dateOfBirth = dateOfBirth is null ? record.DateOfBirth : dateOfBirth;
                                status = status is null ? record.Status : status;
                                salary = salary is null ? record.Salary : salary;
                                permissions = permissions is null ? record.Permissions : permissions;

                                FileCabinetRecordParameterObject parameterObject = new FileCabinetRecordParameterObject() { FirstName = firstName, LastName = lastName, DateOfBirth = (DateTime)dateOfBirth, Status = (short)status, Salary = (decimal)salary, Permissions = (char)permissions };
                                this.service.EditRecord(record.Id, parameterObject);
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
