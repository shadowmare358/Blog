﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PhotoSauce.MagicScaler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private readonly string _imagePath;

        public FileManager(IConfiguration config)
        {
            _imagePath = config["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            if(File.Exists(Path.Combine(_imagePath, image)))
            {
                return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
            }
            else
            {
                return new FileStream(Path.Combine(_imagePath, "placeholder.png"), FileMode.Open, FileAccess.Read);
            }

        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
   
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                var save_path = Path.Combine(_imagePath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }
                var mime = image.FileName.Substring(image.FileName.LastIndexOf("."));
                var fileName = $"img_{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{mime}";
                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageOptions());
                };
               

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
         
        }
        private ProcessImageSettings ImageOptions() => new ProcessImageSettings
        {
            Width = 800,
            Height = 500,
            ResizeMode = CropScaleMode.Crop,
            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 100,
            JpegSubsampleMode = ChromaSubsampleMode.Subsample420,
        };
    }
}
