using Spirebyte.Services.Operations.Application.DTO;
using System;

namespace Spirebyte.Services.Operations.Application.Services.Args
{
    public class OperationUpdatedEventArgs : EventArgs
    {
        public OperationDto Operation { get; }

        public OperationUpdatedEventArgs(OperationDto operation)
        {
            Operation = operation;
        }
    }
}
