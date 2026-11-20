namespace KodaStore.Application.DTOs;

public sealed record CurrentUserResponse(
    Guid Id,
    string Email
);