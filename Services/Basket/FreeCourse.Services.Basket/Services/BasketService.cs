using System.Text.Json;
using System.Threading.Tasks;
using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService;

        public BasketService(RedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Response<bool>> Delete(string userId)
        {
            var redisDb = _redisService.GetDatabase();
            var status = await redisDb.KeyDeleteAsync(userId);

            return status
                ? Response<bool>.Success((int)ResponseCodes.NoContent)
                : Response<bool>.Fail("Basket not found", (int)ResponseCodes.NotFound);
        }

        public async Task<Response<BasketDto>> GetBasket(string userId)
        {
            var redisDb = _redisService.GetDatabase();
            var existBasket = await redisDb.StringGetAsync(userId);

            if (string.IsNullOrEmpty(existBasket))
                return Response<BasketDto>.Fail("Basket not found", (int)ResponseCodes.NotFound);

            return Response<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(existBasket), (int)ResponseCodes.OK);
        }

        public async Task<Response<bool>> SaveOrUpdate(BasketDto basketDto)
        {
            var redisDb = _redisService.GetDatabase();
            var status = await redisDb.StringSetAsync(basketDto.UserId, JsonSerializer.Serialize(basketDto));

            return status
                ? Response<bool>.Success((int)ResponseCodes.NoContent)
                : Response<bool>.Fail("Basket could not update or save", (int)ResponseCodes.InternalServerError);
        }
    }
}
