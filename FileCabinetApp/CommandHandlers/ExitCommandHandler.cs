using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> exit;

        /// <summary>
        /// Initializes new instance of the ExitCommandHandler class.
        /// </summary>
        /// <param name="exit"></param>
        public ExitCommandHandler(Action<bool> exit)
        {
            this.exit = exit;
        }

        /// <summary>
        /// Handles commands.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "exit")
                {
                    Console.WriteLine("Exiting an application...");
                    this.exit(true);
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
