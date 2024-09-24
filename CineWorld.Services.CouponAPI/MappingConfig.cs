using AutoMapper;
using CineWorld.Services.CouponAPI.Models;
using CineWorld.Services.CouponAPI.Models.Dtos;

namespace CineWorld.Services.CouponAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<CouponDto, Coupon>();
        config.CreateMap<Coupon, CouponDto>();
      });

      return mappingConfig;
    }
  }
}
