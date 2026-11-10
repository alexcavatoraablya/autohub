using Application.Models.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Inetrfaces;

public interface ITestService
{
    Task<IReadOnlyList<TestItemDto>> GetAllAsync();
    Task<TestItemDto?> GetByIdAsync(int id);
    Task<TestItemDto> CreateAsync(TestSaveRequest request);
    Task<TestItemDto?> UpdateAsync(int id, TestSaveRequest request);
    Task<bool> DeleteAsync(int id);
}