namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents command request, made to the app.
    /// </summary>
    public class AppCommandRequest
    {
        private readonly string command;

        private readonly string parameters;

        /// <summary>
        /// Initializes new instance of the AppCommandRequest via the command and parameters.
        /// </summary>
        /// <param name="command">Command from the request.</param>
        /// <param name="parameters">Parameters from the request.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.command = command;
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets command from the request.
        /// </summary>
        /// <value>command</value>
        public string Command
        {
            get => this.command;
        }

        /// <summary>
        /// Gets parameters from the request.
        /// </summary>
        /// <value>parameters.</value>
        public string Parameters
        {
            get => this.parameters;
        }
    }
}
