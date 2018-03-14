using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;

namespace RMSAutoAPI.Infrastructure
{
    public class AutoMapperWebConfiguration
    {
        private int _clientGroup;
        public static void Configure()
        {
            ConfigureUserMapping();
        }

        //Автомаппер позволяет произвести соответствие между двумя классами.
        //Если в сгенеренном классе много полей, а нужна лишь часть, чтобы вручную не выбирать каждый раз можно произвести соответствия.
        //Хорошая документация по работе Автомаппера здесь: https://metanit.com/sharp/mvc5/23.4.php
        private static void ConfigureUserMapping()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<spSearchBrands_Result, Brand>()
                         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Manufacturer))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

                cfg.CreateMap<spSearchCrossesWithPriceSVC_Result, PartNumber>()
                     .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.PartNumber))
                     .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Manufacturer))
                     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PartName))
                     .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.QtyInStock))
                     .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.CrossType))
                     .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
            });

        }
    }
}
