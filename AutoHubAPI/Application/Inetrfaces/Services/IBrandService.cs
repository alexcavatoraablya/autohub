using Application.DTOs.Brands;

namespace Application.Interfaces.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync();

    Task<BrandDto?> GetByIdAsync(int id);

    Task<BrandDto> CreateAsync(CreateBrandDto dto);

    Task<bool> UpdateAsync(int id, UpdateBrandDto dto);

    Task<bool> DeleteAsync(int id);
}