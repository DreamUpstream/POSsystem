﻿using AutoMapper;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;

namespace POSsystem.Core.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Item, ItemDTO>();
        }
    }
}
