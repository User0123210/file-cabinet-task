using System.Globalization;

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
                    this.ParseArguments(arguments);
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }

        private void ParseArguments(string[] arguments)
        {
            string[] properties = arguments[0].Replace('(', ' ').Replace(')', ' ').Trim().Split(", ");

            if (arguments.Length > 1)
            {
                string[] values = arguments[1].Replace('(', ' ').Replace(')', ' ').Trim().Split(", ");
                int minNum = Math.Min(properties.Length, values.Length);

                string firstName = string.Empty;
                string lastName = string.Empty;
                DateTime dateOfBirth = DateTime.Now;
                short status = 0;
                decimal salary = 0m;
                char permissions = ' ';
                int? id = null;

                for (int i = 0; i < minNum; i++)
                {
                    switch (properties[i].ToUpperInvariant())
                    {
                        case "ID":
                            bool isValid = int.TryParse(values[i], out int num);
                            id = isValid ? num : null;
                            break;
                        case "FIRSTNAME":
                            firstName = values[i];
                            break;
                        case "LASTNAME":
                            lastName = values[i];
                            break;
                        case "DATEOFBIRTH":
                            _ = DateTime.TryParseExact(values[i], this.service.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
                            break;
                        case "STATUS":
                            _ = short.TryParse(values[i], out status);
                            break;
                        case "SALARY":
                            _ = decimal.TryParse(values[i], out salary);
                            break;
                        case "PERMISSIONS":
                            _ = char.TryParse(values[i], out permissions);
                            break;
                    }
                }

                FileCabinetRecordParameterObject parameterObject = new () { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions };

                if (id is null)
                {
                    this.service.CreateRecord(parameterObject);
                }
                else
                {
                    this.service.CreateRecord((int)id, parameterObject);
                }
            }
        }
    }
}
