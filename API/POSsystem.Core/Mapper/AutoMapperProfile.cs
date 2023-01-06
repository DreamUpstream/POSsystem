using AutoMapper;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;

namespace POSsystem.Core.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Item, ItemDTO>();
            CreateMap<ServiceReservation, ServiceReservationDTO>();
            CreateMap<Branch, BranchDTO>();
            CreateMap<BranchWorkingDay, BranchWorkingDayDTO>();
            CreateMap<Customer, CustomerDTO>();
        }
    }
}
