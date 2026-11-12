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

    /// <summary>Інформація про тест: кількість питань, максимальний бал. null — тест не знайдено.</summary>
    Task<TestInfoDto?> GetInfoAsync(int id);

    /// <summary>Питання та варіанти відповідей для проходження (без правильних відповідей).</summary>
    Task<TestPassDto?> GetForPassingAsync(int id);

    /// <summary>Перевірка відповідей і підрахунок балів. null — тест не знайдено.</summary>
    Task<TestResultDto?> SubmitAsync(int id, TestSubmitRequest request);
}