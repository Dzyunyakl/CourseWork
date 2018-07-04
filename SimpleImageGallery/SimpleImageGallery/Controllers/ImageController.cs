using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using SimpleImageGallery.Models;

namespace SimpleImageGallery.Controllers
{
    public class ImageController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly SimpleImageGalleryDbContext _ctx;
        public ImageController(SimpleImageGalleryDbContext ctx)
        {
            _ctx = ctx;

        }
        public IActionResult Upload()
        {
            var model = new UploadImageModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadNewImage(UploadImageModel imageModel)
        {
            if (imageModel.ImageUpload == null || imageModel.ImageUpload.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/images",
                imageModel.ImageUpload.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageModel.ImageUpload.CopyToAsync(stream);
            }
            var imageUrl = imageModel.ImageUpload.FileName;
            var tempImage = new GalleryImage
            {
                Created = DateTime.Now,
                Title = imageModel.Title,
                Url = "/images/" + imageUrl
            };
            await _ctx.GalleryImages.AddAsync(tempImage);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index", "Gallery");
        }
    }
}