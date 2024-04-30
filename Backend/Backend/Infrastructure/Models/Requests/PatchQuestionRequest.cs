using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Models.Requests;

public class PatchQuestionRequest
{
    public string? Question { get; set; }

    public string? Option1 { get; set; }

    public string? Option2 { get; set; }

    public string? Option3 { get; set; }

    public string? Option4 { get; set; }

    [Range(1, 4, ErrorMessage = "Question answer must be in the range 1-4.")]
    public int? CorrectOptionNumber { get; set; }
}
