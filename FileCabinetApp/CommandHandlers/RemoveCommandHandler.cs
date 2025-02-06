using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : CommandHandlerBase
    {
        public RemoveCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            bool isValid = int.TryParse(commandRequest?.Parameters, out int recordId);

            if (isValid)
            {
                Program.FileCabinetService.RemoveRecord(recordId);
            }
        }
    }
}
