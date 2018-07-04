using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using SimpleImageGallery.Models;

namespace SimpleImageGallery.Controllers {
    public class GalleryController : Controller {
        private readonly IImage _imageService;
        private readonly SimpleImageGalleryDbContext _dbContext;

        public GalleryController (IImage imageService, SimpleImageGalleryDbContext dbContext) {
            _imageService = imageService;
            _dbContext = dbContext;
        }
        public IActionResult Index () {
            var imageList = _imageService.GetAll ();
            var model = new GalleryIndexModel () {
                Images = imageList,
                SearchQuery = ""
            };

            return View (model);
        }

        public IActionResult Detail (int id) {
            var image = _imageService.GetById (id);

            var model = new GalleryDetailModel () {
                Id = image.Id,
                Title = image.Title,
                CreatedOn = image.Created,
                Url = image.Url,
                Tags = image.Tags.Select (t => t.Description).ToList ()
            };

            return View (model);
        }

        public IActionResult GetStatistics(int id)
        {
            var imageUrl = _imageService.GetById(id).Url;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");//,imageUrl);
            path = path + imageUrl;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string postData = Convert.ToBase64String(fileBytes);

            WebRequest request = WebRequest.Create("http://127.0.0.1:5000/value");
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            //var a = dataStream.Position;
            // Close the Stream object.  
            dataStream.Close();



            WebResponse response = request.GetResponse();



            // Display the status.  
            // Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            // Console.WriteLine(responseFromServer);
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();

            return RedirectToAction("Statistics", "Gallery");
        }

        public IActionResult Statistics()
        {
            return View();
        }

        public IActionResult Edit () {
            return View ();
        }
        public IActionResult Contact () {
            return View ();
        }
        public IActionResult Delete (int id) {
            var image = _dbContext.GalleryImages.Find (id);
            _dbContext.GalleryImages.Remove (image);
            _dbContext.SaveChanges ();
            return RedirectToAction ("Index");
        }

        [Route ("gallerypost")]
        [HttpPost]
        public IActionResult PostImage ([FromBody] GalleryImage image) {
            var tempImage = new GalleryImage {
                Created = DateTime.Now,
                Tags = image.Tags,
                Title = image.Title,
                Url = image.Url
            };
            _dbContext.GalleryImages.Add (tempImage);
            _dbContext.SaveChanges ();
            return Ok ();
        }
    }
}