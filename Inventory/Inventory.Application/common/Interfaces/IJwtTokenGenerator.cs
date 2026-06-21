using Inventory.Domain.Users.Entity;

namespace Inventory.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserEntity user);
}
