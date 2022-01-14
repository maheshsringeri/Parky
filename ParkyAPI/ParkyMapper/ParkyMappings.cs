
using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;

namespace ParkyAPI.ParkyMapper
{
    public class ParkyMappings : Profile
    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDtos>().ReverseMap();
            CreateMap<Trail, TrailDtos>().ReverseMap();
            CreateMap<Trail, TrailUpdateDto>().ReverseMap();
            CreateMap<Trail, TrailInsertDto>().ReverseMap();
        }
    }
}
