using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;

namespace RMSAutoAPI.Infrastructure
{
    public class AutoMapperWebConfiguration
    {
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

                cfg.CreateMap<spSearchCrossesWithPriceSVC_Result, SparePart>()
                         .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.PartNumber))
                         .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Manufacturer))
                         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PartName))
                         .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.QtyInStock))
                         .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.CrossType))
                         .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

                cfg.CreateMap<spGetFranches_Result, Partner>()
                         .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Franch))
                         .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.InternalFranchName));

                cfg.CreateMap<OrderLines, SparePart>()
                            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Manufacturer))
                            .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.PartNumber))
                            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Qty))
                            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PartName))
                            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice));

                cfg.CreateMap<SparePart, OrderLines>()
                            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Brand))
                            .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.Article))
                            .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Count))
                            .ForMember(dest => dest.PartName, opt => opt.MapFrom(src => src.Name))
                            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));


                cfg.CreateMap<OrderLines, OrderSparePart>()
                            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Manufacturer))
                            .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.PartNumber))
                            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Qty))
                            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice));

                cfg.CreateMap<OrderSparePart, OrderLines>()
                            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Brand))
                            .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.Article))
                            .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Count))
                            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));

                cfg.CreateMap<OrderLines, ResponseSparePart>()
                            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Manufacturer))
                            .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.PartNumber))
                            .ForMember(dest => dest.CountApproved, opt => opt.MapFrom(src => src.Qty))
                            .ForMember(dest => dest.PriceApproved, opt => opt.MapFrom(src => src.UnitPrice));

                cfg.CreateMap<Orders, Order<SparePart>>()
                            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Users.Username))
                            .ForMember(dest => dest.SpareParts, opt => opt.MapFrom(src => src.OrderLines));

                cfg.CreateMap<Orders, Order<OrderSparePart>>()
                            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Users.Username));

                cfg.CreateMap<Order<OrderSparePart>, Orders>()
                            .ForMember(dest => dest.OrderLines, opt => opt.MapFrom(src => src.SpareParts))
                            .ForMember(dest => dest.OrderNotes, opt => opt.MapFrom(src => src.OrderName));

                cfg.CreateMap<Orders, Order<ResponseSparePart>>()
                            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Users.Username))
                            .ForMember(dest => dest.SpareParts, opt => opt.MapFrom(src => src.OrderLines))
                            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderID))
                            .ForMember(dest => dest.OrderName, opt => opt.MapFrom(src => src.OrderNotes));

                cfg.CreateMap<Methods, ApiMethod>();
                cfg.CreateMap<Parameters, ApiParameter>()
                            .ForMember(dest => dest.TypeParameter, opt => opt.MapFrom(src => (TypeParameter)src.TypeParameter));

            });

        }
    }
}
