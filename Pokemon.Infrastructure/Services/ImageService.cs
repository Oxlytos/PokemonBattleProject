using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Services.Interfaces;

namespace Pokemon.Services.Services
{
    public class ImageService : IImageService
    {
        public Task<SpriteCollection> GetImage(string name)
        {
            throw new NotImplementedException();
        }

        public Task SaveImage(string name)
        {
            throw new NotImplementedException();
        }
    }
}
