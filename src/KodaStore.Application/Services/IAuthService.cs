using KodaStore.Application.DTOs;

namespace KodaStore.Application.Services;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct);

    Task<LoginResults> LoginAsync (LoginRequest request, CancellationToken ct);
 }