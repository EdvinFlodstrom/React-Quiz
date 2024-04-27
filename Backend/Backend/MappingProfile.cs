using AutoMapper;
using Backend.Models.Dtos;
using Backend.Models.Entities;

namespace Backend;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FourOptionQuestion, FourOptionQuestionDto>();
    }
}