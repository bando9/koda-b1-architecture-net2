namespace KodaStore.Application.DTOs;

public enum RegisterStatus
{
    Created, DuplicateEmail, ValidationError
}

public sealed record RegisterResult(
    RegisterStatus Status,
    AuthUserResponse? User,
    string[] Error
);