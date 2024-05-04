﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Infrastructure.Models.Entities;

public abstract class FourOptionQuestion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // The Id should not be generated by the database
    [JsonIgnore]
    public int Id { get; set; }

    public required string Question { get; set; }

    public required string Option1 { get; set; }

    public required string Option2 { get; set; }

    public required string Option3 { get; set; }

    public required string Option4 { get; set; }

    public required int CorrectOptionNumber { get; set; }

    public ICollection<PlayerStatisticsFourOptionQuestion>? PlayerStatisticsFourOptionQuestion { get; private set; }
}
