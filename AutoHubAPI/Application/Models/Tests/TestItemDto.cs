using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Tests;
/// <summary>Елемент списку тестів.</summary>
public record TestItemDto(
    int Id,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    DateTime CreatedAt,
    int QuestionsCount)
{
    internal List<global::AutoHubAPI.Controllers.QuestionDto> Questions { get; public set; }
}
