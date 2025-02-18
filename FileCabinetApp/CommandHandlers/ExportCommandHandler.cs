namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle export command in the IFileCabinetService.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of the ExportCommandHandler class.
        /// </summary>
        /// <param name="service">IFileCabinetService to handle commands in.</param>
        public ExportCommandHandler(IFileCabinetService service)
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
                if (commandRequest.Command == "export")
                {
                    string[] arguments = commandRequest.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
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
            const int propertyIndex = 0;
            var sourceName = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var destination = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenWrite(destination);
                    using StreamWriter writer = new (stream);
                    FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToCsv(writer);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToXml(writer);
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine($"Destination directory not found: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid destination directory: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Can't get access to the destination directory: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Something went wrong during the writing: {ex.Message}");
                }
            }
        }
    }
}
