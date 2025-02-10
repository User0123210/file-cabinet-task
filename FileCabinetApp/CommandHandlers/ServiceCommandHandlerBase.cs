using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CA1051, SA1401

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents base command handler for the service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// IFileCabinetService to handle commands in.
        /// </summary>
        protected readonly IFileCabinetService service;

        /// <summary>
        /// Initializes new instance of the ServiceCommandHandler via the IFileCabinetService parameter.
        /// </summary>
        /// <param name="service">Service, in which commands should be handled.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.service = service;
        }
    }
}
