using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IJwtGenerator
{
    string CreateToken(User user);
}