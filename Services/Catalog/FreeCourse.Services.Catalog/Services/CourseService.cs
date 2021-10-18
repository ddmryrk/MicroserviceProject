﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            if (!courses.Any())
                return Response<List<CourseDto>>.Success(new List<CourseDto>(), (int)ResponseCodes.OK);

            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find(c => c.Id == course.CategoryId).FirstAsync();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), (int)ResponseCodes.OK);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return Response<CourseDto>.Fail("ID is not valid", (int)ResponseCodes.BadRequest);

            var course = await _courseCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (course == null)
                return Response<CourseDto>.Fail("Course not found", (int)ResponseCodes.NotFound);

            course.Category = await _categoryCollection.Find(c => c.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), (int)ResponseCodes.OK);
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find(c => c.UserId == userId).ToListAsync();

            if (!courses.Any())
                return Response<List<CourseDto>>.Success(new List<CourseDto>(), (int)ResponseCodes.OK);

            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find(c => c.Id == course.CategoryId).FirstAsync();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), (int)ResponseCodes.OK);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);

            newCourse.CreatedTime = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), (int)ResponseCodes.OK);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(c => c.Id == courseUpdateDto.Id, updateCourse);

            if (result == null)
                return Response<NoContent>.Fail("Course not found", (int)ResponseCodes.NotFound);

            return Response<NoContent>.Success((int)ResponseCodes.NoContent);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(c => c.Id == id);

            if (result == null)
                return Response<NoContent>.Fail("Course not found", (int)ResponseCodes.NotFound);

            return Response<NoContent>.Success((int)ResponseCodes.NoContent);
        }
    }
}
