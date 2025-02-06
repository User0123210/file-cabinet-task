using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        public PurgeCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            this.service.Purge();
        }
    }
}
