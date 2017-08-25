﻿using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Mappers
{
    public class DTOAutoMapperProfileConfiguration : Profile
    {
        public DTOAutoMapperProfileConfiguration()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
