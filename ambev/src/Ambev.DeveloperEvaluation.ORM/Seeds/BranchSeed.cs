using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Seeds;

public static class BranchSeed
{
    public static readonly Branch[] Data =
    [
        new Branch("Downtown") { Id = Guid.Parse("6d7f5d7a-44d5-4c3f-8b4f-3a7b2a9f6f11") },
        new Branch("Uptown") { Id = Guid.Parse("b2c6a1e3-4f7d-4d8a-9a2f-3f1f7b6c5d21") },
        new Branch("North Side") { Id = Guid.Parse("9b8f1a2c-3d4e-4f5a-8b7c-6d5e4f3a2b10") },
        new Branch("South Side") { Id = Guid.Parse("1f2e3d4c-5b6a-7c8d-9e0f-1a2b3c4d5e6f") },
        new Branch("East End") { Id = Guid.Parse("2a3b4c5d-6e7f-8a9b-0c1d-2e3f4a5b6c7d") },
        new Branch("West End") { Id = Guid.Parse("3b4c5d6e-7f8a-9b0c-1d2e-3f4a5b6c7d8e") },
        new Branch("Central Plaza") { Id = Guid.Parse("4c5d6e7f-8a9b-0c1d-2e3f-4a5b6c7d8e9f") },
        new Branch("Harbor Point") { Id = Guid.Parse("5d6e7f8a-9b0c-1d2e-3f4a-5b6c7d8e9f0a") },
        new Branch("Riverside") { Id = Guid.Parse("6e7f8a9b-0c1d-2e3f-4a5b-6c7d8e9f0a1b") },
        new Branch("Airport Hub") { Id = Guid.Parse("7f8a9b0c-1d2e-3f4a-5b6c-7d8e9f0a1b2c") }
    ];
}
