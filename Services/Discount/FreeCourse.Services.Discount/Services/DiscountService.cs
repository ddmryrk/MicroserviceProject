using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var deleteStatus = await _dbConnection.ExecuteAsync("DELETE FROM discount WHERE id=@Id",
                                    new { Id = id });

            if (deleteStatus > 0)
                return Response<NoContent>.Success((int)ResponseCodes.NoContent);

            return Response<NoContent>.Fail("Discount not found", (int)ResponseCodes.NotFound);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount");
            return Response<List<Models.Discount>>.Success(discounts.ToList(), (int)ResponseCodes.OK);
        }

        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount WHERE code=@Code AND userId=@UserId",
                                new { Code = code, UserId = userId })).SingleOrDefault();

            if (discount == null)
                return Response<Models.Discount>.Fail("Discount not found", (int)ResponseCodes.NotFound);

            return Response<Models.Discount>.Success(discount, (int)ResponseCodes.OK);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount WHERE id=@Id",
                                new { Id = id })).SingleOrDefault();

            if (discount == null)
                return Response<Models.Discount>.Fail("Discount not found", (int)ResponseCodes.NotFound);

            return Response<Models.Discount>.Success(discount, (int)ResponseCodes.OK);
        }

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid, rate, code) VALUES (@UserId, @Rate, @Code)", discount);

            if (saveStatus > 0)
                return Response<NoContent>.Success((int)ResponseCodes.NoContent);

            return Response<NoContent>.Fail("An error accured while adding", (int)ResponseCodes.InternalServerError);
        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var updateStatus = await _dbConnection.ExecuteAsync("UPDATE discount SET userid=@UserId, code=@Code, rate=@Rate WHERE id=@Id",
                                    new
                                    {
                                        Id = discount.Id,
                                        UserId = discount.UserId,
                                        Code = discount.Code,
                                        Rate = discount.Rate
                                    });

            if (updateStatus > 0)
                return Response<NoContent>.Success((int)ResponseCodes.NoContent);

            return Response<NoContent>.Fail("Discount not found", (int)ResponseCodes.NotFound);
        }
    }
}
