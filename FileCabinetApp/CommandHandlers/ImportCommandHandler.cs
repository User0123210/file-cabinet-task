using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : CommandHandlerBase
    {
        public ImportCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            string[] arguments = commandRequest?.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var sourceName = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var source = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenRead(source);
                    using StreamReader reader = new(stream);
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromCsv(reader);
                        Program.FileCabinetService.Restore(snapshot);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.LoadFromXml(reader);
                        Program.FileCabinetService.Restore(snapshot);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }
    }
}
