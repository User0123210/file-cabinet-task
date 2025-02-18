namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle import command in the IFileCabinetService.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of the ImportCommandHandler class via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service">Service to handle commands in.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles commands in the seervice.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "import")
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
                var source = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenRead(source);
                    using StreamReader reader = new (stream);
                    FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromCsv(reader);
                        this.service.Restore(snapshot);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromXml(reader);
                        this.service.Restore(snapshot);
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
            }
        }
    }
}
