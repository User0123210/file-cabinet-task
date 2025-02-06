using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        public PurgeCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            Program.FileCabinetService.Purge();
        }
    }
}
