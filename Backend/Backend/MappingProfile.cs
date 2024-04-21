using AutoMapper;
using Backend.Dtos;
using Backend.Models;

namespace Backend;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FourOptionQuestion, FourOptionQuestionDto>();
    }
}