

using KodaStore.Application.DTOs;

namespace KodaStore.Application.Repositories;

public interface ICategoryRepository
{
    Task<CategoryCreateResult> CreateWithProductAsync (string name, IReadOnlyList<string> productCodes,CancellationToken ct);

    Task<CategoryProductsResponse?> GetProductsAsync (long categoryId,int page, int pageSize,CancellationToken ct);

    Task<ProductCategoryStateResponse?> GetProductAssignmentAsync (string productCode, CancellationToken ct);

    Task<CategoryAssignStatus> AssignProductCompareAndSwapAsync(string productCode,int newCategoryId,int? expectedCategoryId,CancellationToken ct);
}