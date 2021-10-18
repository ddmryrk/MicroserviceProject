using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryCollection.Find(c => true).ToListAsync();

            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), (int)ResponseCodes.OK);
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            var category = _mapper.Map<Category>(categoryCreateDto);

            await _categoryCollection.InsertOneAsync(category);

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), (int)ResponseCodes.OK);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return Response<CategoryDto>.Fail("ID is not valid", (int)ResponseCodes.BadRequest);

            var category = await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (category == null)
                return Response<CategoryDto>.Fail("Category not found", (int)ResponseCodes.NotFound);

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), (int)ResponseCodes.OK);
        }
    }
}
