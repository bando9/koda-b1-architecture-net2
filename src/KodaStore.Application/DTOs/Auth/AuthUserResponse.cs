namespace KodaStore.Application.DTOs;

public sealed record AuthUserResponse
(
    Guid Id,string Emal, string DisplayName
);