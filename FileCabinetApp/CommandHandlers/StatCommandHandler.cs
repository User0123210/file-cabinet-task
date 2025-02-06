using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        public StatCommandHandler()
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            var recordsCount = Program.FileCabinetService.GetStat;
            Console.WriteLine($"{recordsCount.Item1} overall records number, {recordsCount.Item2} deleted records number.");
        }
    }
}
