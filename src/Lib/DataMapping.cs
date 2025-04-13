using AIApp.Lib.Entities;
using AIApp.Lib.Models;
using AutoMapper;

namespace AIApp.Lib
{
    public class DataMapping : Profile
    {
        public DataMapping()
        {
            CreateMap<MemoryEntity, MemoryModel>().ReverseMap();
        }
    }
}