using KodaStore.Domain.Entities;

namespace KodaStore.Application.DTOs;

public sealed record CategoryResponse(
    long Id,
    string Name)
{
    public static CategoryResponse FromModel(Category category) =>
        new(category.Id, category.Name);
}