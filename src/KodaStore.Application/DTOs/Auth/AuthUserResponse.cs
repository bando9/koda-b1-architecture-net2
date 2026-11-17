namespace KodaStore.Application.DTOs;

public sealed record AuthUserResponse
(
    Guid Id,string Email, string DisplayName
);