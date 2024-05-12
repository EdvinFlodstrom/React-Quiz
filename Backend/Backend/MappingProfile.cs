using AutoMapper;
using Backend.Infrastructure.Models.Dtos;
using Backend.Infrastructure.Models.Entities;

namespace Backend;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FourOptionQuestion, FourOptionQuestionDto>();

        CreateMap<FourOptionQuestion, FourOptionQuestionByIdDto>();
    }
}