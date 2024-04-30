namespace Backend.Infrastructure.Models.Requests;

public abstract class BaseRequest
{
    public required string PlayerName { get; set; }
}