using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Seeds;

public static class ProductSeed
{
    public static readonly Product[] Data =
    [
        new Product("Acoustic Guitar", 899.90m) { Id = Guid.Parse("9c0b75c6-9c3b-4d25-8a3a-2a91f0ec5c7a") },
        new Product("Electric Guitar", 1299.00m) { Id = Guid.Parse("5a10a8a4-42ef-4e9c-8e0e-2d6f0c0a2d9b") },
        new Product("Bass Guitar", 1099.50m) { Id = Guid.Parse("a3e7b0c7-8d2a-4e3a-b0df-9d3d5fd8d2dd") },
        new Product("Digital Piano", 2499.99m) { Id = Guid.Parse("0f1b00d2-0ff3-4b2b-8b8c-1c6f1e9a1b16") },
        new Product("Drum Kit", 2199.00m) { Id = Guid.Parse("3c6e8c9b-7b61-4d6b-95f0-2c1f4dbbe6d2") },
        new Product("Studio Microphone", 699.00m) { Id = Guid.Parse("7f8d4b9b-1a6d-45a3-8e9c-8f0c4ddc0d1b") },
        new Product("Audio Interface", 899.00m) { Id = Guid.Parse("c9d2c7d4-1f53-4ef2-9b5a-0d9a7c4d2b29") },
        new Product("MIDI Controller", 499.90m) { Id = Guid.Parse("4b2b2f87-1e3e-4e1c-9fd3-6e9a7d6f1f5b") },
        new Product("Stage Monitor", 1399.00m) { Id = Guid.Parse("d1a6b2f9-8e4b-4b74-9b8a-7c0d5f1e3a6c") },
        new Product("Guitar Strings Set", 49.90m) { Id = Guid.Parse("f0b9d7a2-6b7c-4f9e-9b3c-5e1d2f7a9c10") }
    ];
}
