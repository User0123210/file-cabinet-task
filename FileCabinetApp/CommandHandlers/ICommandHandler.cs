using System;

namespace FileCabinetApp
{
    public interface ICommandHandler
    {
        public void SetNext(ICommandHandler handler);

        public void Handle(AppCommandRequest commandRequest);
    }
}
