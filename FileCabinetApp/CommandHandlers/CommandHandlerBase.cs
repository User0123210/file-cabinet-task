using System;

#pragma warning disable CA1051, SA1401

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represnts base command handler class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Next handler in the chain.
        /// </summary>
        protected ICommandHandler? nextHandler;

        /// <summary>
        /// Sets next handler in the chain.
        /// </summary>
        /// <param name="handler">Handler to set as next.</param>
        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        /// <summary>
        /// Handles commands.
        /// </summary>
        /// <param name="commandRequest">Command to handle.</param>
        public abstract void Handle(AppCommandRequest commandRequest);
    }
}
