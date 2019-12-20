using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cirice.Data.Cloud
{
    public interface ICloudUploader
    {
        string UploadImg(IFormFile file);
    }
}
