using Spirebyte.Services.Operations.Core.Types;

namespace Spirebyte.Services.Operations.Application.DTO;

public class OperationDto
{
    public string Id { get; set; }
    public string OwnUserId { get; set; }
    public string Name { get; set; }
    public OperationState State { get; set; }
    public string Code { get; set; }
    public string Reason { get; set; }
    public string ProjectId { get; set; }
    public string IssueId { get; set; }
    public string SprintId { get; set; }
    public string UserId { get; set; }
}