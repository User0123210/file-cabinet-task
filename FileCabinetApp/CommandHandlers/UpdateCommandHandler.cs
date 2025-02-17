using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

                        HashSet<FileCabinetRecord> records = new ();

                        for (int j = 0; j < values.Length; j++)
                        {
                            string[] propertyValue = values[j].Split("=", 2);

                            if (propertyValue.Length > 1)
                            {
                                string property = propertyValue[0].Trim();
                                string value = propertyValue[1].Trim();

                                switch (property.ToUpperInvariant())
                                {
                                    case "FIRSTNAME":
                                        var firstNameRecords = this.service.FindByFirstName(value.ToUpperInvariant());

                                        foreach (var record in firstNameRecords)
                                        {
                                            if (!records.Contains(record))
                                            {
                                                records.Add(record);
                                            }
                                        }

                                        break;
                                    case "LASTNAME":
                                        var lastNameRecords = this.service.FindByLastName(value.ToUpperInvariant());

                                        foreach (var record in lastNameRecords)
                                        {
                                            if (!records.Contains(record))
                                            {
                                                records.Add(record);
                                            }
                                        }

                                        break;
                                    case "DATEOFBIRTH":
                                        if (DateTime.TryParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
                                        {
                                            var dateRecords = this.service.FindByDateOfBirth(newDate);

                                            foreach (var record in dateRecords)
                                            {
                                                if (!records.Contains(record))
                                                {
                                                    records.Add(record);
                                                }
                                            }
                                        }

                                        break;
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
                                            if (record.Id != int.Parse(value, CultureInfo.InvariantCulture))
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "FIRSTNAME":
                                            if (record.FirstName.ToUpperInvariant() != value.ToUpperInvariant())
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "LASTNAME":
                                            if (record.LastName.ToUpperInvariant() != value.ToUpperInvariant())
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "DATEOFBIRTH":
                                            if (record.DateOfBirth != DateTime.ParseExact(value, this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None))
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "STATUS":
                                            if (record.Status != short.Parse(value, CultureInfo.InvariantCulture))
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "SALARY":
                                            if (record.Salary != decimal.Parse(value, CultureInfo.InvariantCulture))
                                            {
                                                toEdit = false;
                                            }

                                            break;
                                        case "PERMISSIONS":
                                            if (record.Permissions != char.Parse(value))
                                            {
                                                toEdit = false;
                                            }

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
