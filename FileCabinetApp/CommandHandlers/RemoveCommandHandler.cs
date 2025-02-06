using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        public RemoveCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "remove")
                {
                    bool isValid = int.TryParse(commandRequest.Parameters, out int recordId);

                    if (isValid)
                    {
                        this.service.RemoveRecord(recordId);
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
