using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.ORM.Seeds;

public static class UserSeed
{
    public static readonly User[] Data =
    [
        new User
        {
            Id = Guid.Parse("9f3c6ce6-4a4a-4f2e-86a6-2c92f39e5e58"),
            Username = "Wellington",
            Email = "wellington@test.com",
            Password = "$2a$11$2L5/nxUCsZ9pE0qwCnp8V.NlRoXlhYhF4U6NCOD0sF1fjd2mV2O.a", // 12345678Wm@
            Phone = "(11) 99999-9999",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        }
    ];
}
