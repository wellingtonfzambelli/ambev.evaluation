using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public sealed class Branch : BaseEntity
{
    public string Name { get; private set; }

    public Branch(string name) =>
        Name = name;
}