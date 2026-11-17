namespace KodaStore.Application.DTOs;

public sealed record LoginResponse(
    string Message,
    AuthUserResponse User
);