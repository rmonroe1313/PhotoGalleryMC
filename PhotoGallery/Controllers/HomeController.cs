using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Models;
using PhotoGallery.Services;

namespace PhotoGallery.Controllers
{
    public class HomeController : Controller
    {
        private PhotoDbContext _dbContext;
        private PhotoImageStorageService _photoImageStorageService;

        public HomeController(PhotoDbContext dbContext, PhotoImageStorageService photoImageStorageService)
        {
            _dbContext = dbContext;
            _photoImageStorageService = photoImageStorageService;
        }

        public IActionResult Index()
        {
            List<string> imageUrls = new List<string>();

            var photoImages = _dbContext.PhotoImages.ToList();

            foreach(var image in photoImages)
            {
                imageUrls.Add(_photoImageStorageService.UrlFor(image.Url));
            }

            return View(imageUrls);
        }
        
        public async Task<IActionResult> Upload(IFormCollection formCollection)
        {
            try
            {
                var file = formCollection.Files.FirstOrDefault();

                // Store photo to Azure Storage
                string imageIdentifier = await _photoImageStorageService.SaveImage(file?.OpenReadStream(), DateTime.UtcNow.Millisecond.ToString() + file?.FileName.Split('\\').Last());

                // Update database
                _dbContext.PhotoImages.Add(new PhotoImage
                {
                    Url = imageIdentifier,
                    Caption = "None"
                });

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
