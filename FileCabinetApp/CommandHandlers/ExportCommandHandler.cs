using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : CommandHandlerBase
    {
        public ExportCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            string[] arguments = commandRequest?.Parameters is not null ? commandRequest.Parameters.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int propertyIndex = 0;
            var sourceName = arguments[propertyIndex];

            if (arguments.Length > 1)
            {
                var destination = arguments[propertyIndex + 1];

                try
                {
                    Stream stream = File.OpenWrite(destination);
                    using StreamWriter writer = new(stream);
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();

                    if (string.Equals(sourceName, "csv", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToCsv(writer);
                    }
                    else if (string.Equals(sourceName, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        snapshot.SaveToXml(writer);
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
