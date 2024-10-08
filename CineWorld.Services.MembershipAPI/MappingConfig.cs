using AutoMapper;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;

namespace CineWorld.Services.MovieAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<Coupon, CouponDto>().ReverseMap();
        config.CreateMap<Package, PackageDto>().ReverseMap();
        config.CreateMap<Receipt, ReceiptDto>().ReverseMap();

      });

      return mappingConfig;
    }
  }
}
