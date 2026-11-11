using Application.Inetrfaces;
using Application.Models.Tests;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuizzesController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Отримати список усіх тестів (без питань, суто для списку/меню)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetTests()
    {
        var quizzes = await _context.Tests
            .Select(q => new { q.Id, q.Title, q.Description })
            .ToListAsync();

        return Ok(quizzes);
    }

    // 2. Отримати конкретний тест з питаннями та варіантами для проходження
    [HttpGet("{id}")]
    public async Task<ActionResult<TestItemDto>> GetQuiz(int id)
    {
        var testDto = await _context.Tests
            .Where(q => q.Id == id)
            .Select(q => new TestItemDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                Questions = q.Questions.Select(question => new QuestionDto
                {
                    Id = question.Id,
                    Text = question.Text,
                    AnswerOptions = question.AnswerOptions.Select(option => new AnswerOptionDto
                    {
                        Id = option.Id,
                        Text = option.Text
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (testDto == null)
        {
            return NotFound(new { message = "Тест не знайдено" });
        }

        return Ok(testDto);
    }
}