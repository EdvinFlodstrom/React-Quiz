using AutoMapper;
using Backend.Data;
using Backend.Handlers.Questions;
using Backend.Models;

namespace Backend.Services;

public class QuizService(QuizDbContext quizDbContext, IMapper mapper)
{
    private readonly QuizDbContext _quizDbContext = quizDbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<CreateQuestionCommandResponse> CreateQuestion(FourOptionQuestion fourOptionQuestion)
    {
        try
        {
            // See README.md in root...
            

            
        }
        catch (Exception ex)
        {

        }



        throw new NotImplementedException();
    }
}
