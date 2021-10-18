using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Application.Mapping;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Services.Order.Application.Handlers
{
    public class GetOrdersByUserIdHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<OrderDto>>>
    {
        private readonly OrderDbContext _orderDbContext;

        public GetOrdersByUserIdHandler(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderDbContext.Orders.Include(o => o.OrderItems).Where(o => o.BuyerId == request.UserId).ToListAsync();

            if (!orders.Any())
                return Response<List<OrderDto>>.Success(new List<OrderDto>(), (int)ResponseCodes.OK);

            var ordersDto = ObjectMapper.Mapper.Map<List<OrderDto>>(orders);

            return Response<List<OrderDto>>.Success(ordersDto, (int)ResponseCodes.OK);
        }
    }
}
