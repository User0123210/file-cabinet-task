using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles remove command in the FileCabinetService.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of RemoveCommandHandler via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles commands from thhe service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
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
