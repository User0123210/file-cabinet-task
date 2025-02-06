using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        public ImportCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "import")
                {
                    string[] arguments = commandRequest.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                    const int propertyIndex = 0;
                    var sourceName = arguments[propertyIndex];

                    if (arguments.Length > 1)
                    {
                        var source = arguments[propertyIndex + 1];

                        try
                        {
                            Stream stream = File.OpenRead(source);
                            using StreamReader reader = new(stream);
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
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occured: {ex.Message}");
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
