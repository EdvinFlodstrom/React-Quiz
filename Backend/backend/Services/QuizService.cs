using AutoMapper;
using Backend.Data;

namespace Backend.Services;

public class QuizService(QuizDbContext quizDbContext, IMapper mapper)
{
    private readonly QuizDbContext _quizDbContext = quizDbContext;
    private readonly IMapper _mapper = mapper;
}
