using Backend.Infrastructure.Models.Dtos;

namespace Backend.Infrastructure.Models.Responses;

public class GetQuestionResponse
{
    public FourOptionQuestionDto? FourOptionQuestion { get; set; }

    public string? Details { get; set; }
}
