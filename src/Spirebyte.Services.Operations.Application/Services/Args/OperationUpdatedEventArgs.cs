using System;
using Spirebyte.Services.Operations.Application.DTO;

namespace Spirebyte.Services.Operations.Application.Services.Args;

public class OperationUpdatedEventArgs : EventArgs
{
    public OperationUpdatedEventArgs(OperationDto operation)
    {
        Operation = operation;
    }

    public OperationDto Operation { get; }
}