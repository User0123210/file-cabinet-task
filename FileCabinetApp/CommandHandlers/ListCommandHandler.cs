using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle list command in the IFileCabinetSErvice.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>> print;

        /// <summary>
        /// Initializes new instance of ListCommandHandler class via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="print"></param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> print)
            : base(service)
        {
            this.print = print;
        }

        /// <summary>
        /// Handles commands in the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "list")
                {
                    this.print(this.service.GetRecords());
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
