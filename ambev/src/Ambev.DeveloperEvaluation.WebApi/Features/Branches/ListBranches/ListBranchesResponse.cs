namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.ListBranches;

public sealed class ListBranchesResponse
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
}
