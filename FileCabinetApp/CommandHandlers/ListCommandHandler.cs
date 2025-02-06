using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "list")
                {
                    this.printer.Print(this.service.GetRecords());
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
