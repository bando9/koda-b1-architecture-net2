using System.ComponentModel.DataAnnotations;
using KodaStore.Domain.Entities;

namespace KodaStore.Application.DTOs;

public sealed record CategoryResponse(
    long Id,
    string Name)
{
    public static CategoryResponse FromModel(Category category) =>
        new(category.Id, category.Name);
}



public sealed class CreateCategoryWithProductsRequest : IValidatableObject
{
    [Required,StringLength(30)]
    public string Name {get;set;} = string.Empty;

    [Required,MinLength(1),MaxLength(20)]
    public List<string> ProductCodes {get;set;} =[];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(ProductCodes is null) yield break;
        if(ProductCodes.Any(code => string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30))
        {
            yield return new ValidationResult("setiap product code wajib diisi, maksimal 30 karakter.", [nameof(ProductCodes)]);
        }
        if(ProductCodes.Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s=>s.Trim().ToUpperInvariant())
                        .Distinct(StringComparer.Ordinal).Count() != ProductCodes.Count)
        {
            yield return new ValidationResult("Product Codes tidak boleh diulang");
        }
    }
}


public sealed record CategoryProductResponse(string ProductCode,string Name,decimal Price,int Stock,string CategoryName);

public sealed record CategoryProductsResponse(
    long Id,
    string CategoryName,
    int Page,
    int PageSize,
    long TotalCount,
    IReadOnlyList<CategoryProductResponse> Items
);

public sealed record CategoryCreatedResponse(long Id,string Name,IReadOnlyList<string> AssignedProductCode);

public enum CategoryCreateStatus{Created, DuplicateName, ProductNotFound, Conflict}

public sealed record CategoryCreateResult(
    CategoryCreateStatus Status,
    CategoryCreatedResponse? Data = null,
    string? Detail = null
);


public sealed record AssignProductCategoryRequest
{
    [Range(1,int.MaxValue)]
    public int NewCategoryId{get;set;}
    public int? ExpectedCategoryId {get;set;}

}

public enum CategoryAssignStatus{Updated,ProductNotFound,CategoryNotFound,Conflict}

public sealed record ProductCategoryStateResponse(string ProductCode,long? CategoryId,string CategoryName,string RowVersionBase64);
