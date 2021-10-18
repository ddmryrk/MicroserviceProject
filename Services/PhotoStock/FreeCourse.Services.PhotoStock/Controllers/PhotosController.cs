using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.IO;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    public class PhotosController : CustomBaseController
    {
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
        {

            if (photo == null || photo.Length <= 0)
                return CreateActionResultInstance(Response<PhotoDto>.Fail("Photo is empty", (int)ResponseCodes.BadRequest));

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            await photo.CopyToAsync(stream, cancellationToken);

            var returnPath = $"photos/{photo.FileName}";

            PhotoDto photoDto = new() { Url = returnPath };

            return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, (int)ResponseCodes.OK));
        }

        [HttpDelete]
        public IActionResult PhotoDelete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            if (!System.IO.File.Exists(path))
                return CreateActionResultInstance(Response<NoContent>.Fail("Photo not found", (int)ResponseCodes.NotFound));

            System.IO.File.Delete(path);

            return CreateActionResultInstance(Response<NoContent>.Success((int)ResponseCodes.NoContent));
        }
    }
}