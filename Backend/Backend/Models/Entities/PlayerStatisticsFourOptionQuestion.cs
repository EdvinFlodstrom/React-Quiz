namespace Backend.Models.Entities;

public class PlayerStatisticsFourOptionQuestion
{
    public required int PlayerStatisticsId { get; set; }

    public required PlayerStatistics PlayerStatistics { get; set; }


    public required int QuestionId { get; set; }

    public required FourOptionQuestion Question { get; set; }


    public required int Order {  get; set; } // Represent the randomized order of the questions
}
