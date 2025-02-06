using System;

namespace FileCabinetApp
{
    public class AppCommandRequest
    {
        private readonly string command;

        private readonly string parameters;

        public AppCommandRequest(string command, string parameters)
        {
            this.command = command;
            this.parameters = parameters;
        }

        public string Command
        {
            get => this.command;
        }

        public string Parameters
        {
            get => this.parameters;
        }
    }
}
