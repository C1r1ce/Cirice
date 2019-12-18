using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Cloud;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cirice.Data.Services
{
    public class CloudUploadService : ICloudUploader
    {
        private Cloudinary cloudinary;
        public CloudUploadService(IOptions<CloudinaryOptions> authOptions)
        {
            var options = authOptions.Value;
            Account account=new Account(options.CloudinaryCloudName,options.CloudinaryApiKey,options.CloudinarySecret);
            cloudinary=new Cloudinary(account);
        }

        public string UploadImg(IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription("", stream),
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult.SecureUri.AbsoluteUri;
        }
    }
}
