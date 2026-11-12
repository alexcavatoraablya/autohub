using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Tests;

/// <summary>Інформація про тест (без питань).</summary>
public record TestInfoDto(
    int Id,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    DateTime CreatedAt,
    int QuestionsCount,
    int MaxScore);
