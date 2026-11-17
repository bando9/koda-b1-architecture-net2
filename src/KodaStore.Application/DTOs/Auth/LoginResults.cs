namespace KodaStore.Application.DTOs;

public sealed record LoginResults(
    bool Succeeeded,
    AuthUserResponse? User
);