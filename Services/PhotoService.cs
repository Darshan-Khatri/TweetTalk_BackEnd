using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApplicationBackEnd.Helper;
using DatingApplicationBackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary cloudinary;

        //this is to take appsetting.json file contains.
        /*We need three things to connect with cloudinary server and those things are store in appsetting.json file.
         */
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
                (
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );

            cloudinary = new Cloudinary(acc);
        }

        //When client uploads the image then i will be called and i will send this image file cloudinary server.
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                //Here we are getting file as a stream of data
                using var stream = file.OpenReadStream();
                //Here we are sepcifying image upload constraints like image ratio, size etc
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                //Here the photo is uploaded to cloudinary server.
                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deleteParams);
            return result;
        }
    }
}
