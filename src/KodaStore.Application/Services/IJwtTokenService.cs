
using KodaStore.Application.DTOs;

namespace KodaStore.Application.Services;

public interface IJwtTokenService
{
    IssuedAccessToken IssuedAccessToken(AuthUserResponse user);
}