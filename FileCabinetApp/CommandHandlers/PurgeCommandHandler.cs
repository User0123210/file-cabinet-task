namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class to handle purge command in IFileCabinetService.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes new instance of PurgeCommandHandler class via the IFileCabinetService to handle commands in.
        /// </summary>
        /// <param name="service"></param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles command in the service.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is not null)
            {
                if (commandRequest.Command == "purge")
                {
                    this.service.Purge();
                }
                else
                {
                    this.nextHandler?.Handle(commandRequest);
                }
            }
        }
    }
}
