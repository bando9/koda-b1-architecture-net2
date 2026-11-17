using System.ComponentModel.DataAnnotations;

namespace KodaStore.Application.DTOs;

public sealed record LoginRequest{
    [Required]
    [EmailAddress]
    public string Email {get;init;} =string.Empty;
    [Required]
    public string Pasword {get;init;} =string.Empty;
};