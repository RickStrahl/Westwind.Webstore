using System.Linq;
using AutoMapper;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Service;
using Westwind.Webstore.Web.Views;

namespace Westwind.WebStore.App
{
    /// <summary>
    /// AutoMapper Model Mappings
    /// </summary>
    public class ApplicationMapper
    {
        public static IMapper Current
        {
            get
            {
                if (_Current == null)
                    _Current = CreateAutoMapperMappings();

                return _Current;
            }
            set
            {
                _Current = value;
            }
        }
        private static IMapper _Current;

        public static IMapper CreateAutoMapperMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // MapInstrumentationParameterModels(cfg);
                MapAccountModels(cfg);
                MapProductModels(cfg);
            });

            return config.CreateMapper();
        }

        public static void MapInstrumentationParameterModels(IMapperConfigurationExpression cfg)
        {
            // // Instrumentation Parameter Model Mapping outbound
            // cfg.CreateMap<Measurement, InstrumentationParameterModel>()
            //     .ReverseMap();
            //
            // cfg.CreateMap<Effectivity, InstrumentationParameterModel>()
            //     .IncludeMembers(e => e.MeasPkNavigation)
            //     .ReverseMap();
        }

        public static void MapAccountModels(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Customer, CustomerViewModel>()
                .ReverseMap();

            cfg.CreateMap<Address, Address>();
                //.ForMember(a => a.Id, opt => opt.Ignore())
                //.ForMember(a => a.CustomerId, opt => opt.Ignore());

            cfg.CreateMap<Customer, OrderFormFastViewModel>()
                .ReverseMap();
        }

        public static void MapProductModels(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Product, ProductApiModel>()
                .ReverseMap();

        }
    }
}
