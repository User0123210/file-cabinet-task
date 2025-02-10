using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represetns base interface of the command handler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next handler in the chain.
        /// </summary>
        /// <param name="handler">Handler to set as next.</param>
        public void SetNext(ICommandHandler handler);

        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public void Handle(AppCommandRequest commandRequest);
    }
}
