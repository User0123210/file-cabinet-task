using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        public StatCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "stat")
                {
                    var recordsCount = this.service.GetStat;
                    Console.WriteLine($"{recordsCount.Item1} overall records number, {recordsCount.Item2} deleted records number.");
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
