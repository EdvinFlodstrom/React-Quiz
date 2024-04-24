﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public abstract class FourOptionQuestion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // The Id should not be generated by the database
    public int Id { get; set; }
    
    public required string Question { get; set; }
    
    public required string Option1 { get; set; }

    public required string Option2 { get; set; }

    public required string Option3 { get; set; }

    public required string Option4 { get; set; }

    public required int CorrectOptionNumber { get; set; }
}
