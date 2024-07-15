using Sample.DTOs;
using Sample.Models;
using AutoMapper;

namespace Sample.Mappers;

public class CustomerMapperProfile : Profile
{
    public CustomerMapperProfile()
    {
        CreateMap<CustomerInfo,CustomerInfoDTO>().ReverseMap();
        CreateMap<Customers,CustomersRegisterDTO>().ReverseMap();
        CreateMap<Customers,CustomerLoginDTO>().ReverseMap();
    }
}