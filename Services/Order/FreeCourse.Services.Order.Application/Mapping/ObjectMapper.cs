using System;
using AutoMapper;

namespace FreeCourse.Services.Order.Application.Mapping
{
    public static class ObjectMapper
    {
        private static Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => lazy.Value;
    }
}
