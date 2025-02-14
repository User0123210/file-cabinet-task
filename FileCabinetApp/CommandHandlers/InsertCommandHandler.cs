using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle insert command in IFileCabinetService.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of InsertCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public InsertCommandHandler(IFileCabinetService service)
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
                if (commandRequest.Command == "insert")
                {
                    string[] arguments = commandRequest.Parameters.Split("values", 2);
                    string[] properties = arguments[0].Replace('(', ' ').Replace(')', ' ').Trim().Split(", ");
                    string[] values = arguments[1].Replace('(', ' ').Replace(')', ' ').Trim().Split(", ");
                    FileCabinetRecordParameterObject newParams = new ();
                    int minNum = Math.Min(properties.Length, values.Length);

                    string firstName = string.Empty;
                    string lastName = string.Empty;
                    DateTime dateOfBirth = DateTime.Now;
                    short status = 0;
                    decimal salary = 0m;
                    char permissions = ' ';
                    bool isValid = false;
                    int? id = null;

                    for (int i = 0; i < minNum; i++)
                    {
                        switch (properties[i].ToUpperInvariant())
                        {
                            case "ID":
                                isValid = int.TryParse(values[i], out int num);
                                id = isValid ? num : null;
                                break;
                            case "FIRSTNAME":
                                firstName = values[i];
                                break;
                            case "LASTNAME":
                                lastName = values[i];
                                break;
                            case "DATEOFBIRTH":
                                isValid = DateTime.TryParseExact(values[i], this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
                                break;
                            case "STATUS":
                                isValid = short.TryParse(values[i], out status);
                                break;
                            case "SALARY":
                                isValid = decimal.TryParse(values[i], out salary);
                                break;
                            case "PERMISSIONS":
                                isValid = char.TryParse(values[i], out permissions);
                                break;
                        }
                    }

                    FileCabinetRecordParameterObject parameterObject = new FileCabinetRecordParameterObject() { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };
                    if (id is null)
                    {
                        this.service.CreateRecord(parameterObject);
                    }
                    else
                    {
                        this.service.CreateRecord((int)id, parameterObject);
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
