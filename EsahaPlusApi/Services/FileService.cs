using EsahaPlusApi.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;

namespace EsahaPlusApi.Services
{
    public class FileService : IFileService
    {
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _environment;

        public FileService(IOptions<AppSettings> appSettings, IHostingEnvironment environment)
        {
            _appSettings = appSettings.Value;
            _environment = environment;
        }

        public byte[] FileToByteArray(string fileKey)
        {
            //byte[] file = File.ReadAllBytes(fileName);

            byte[] file = null;
            string filePath = Directory.GetFiles(_environment.WebRootPath + "\\Assets\\File").Where(f => f.ToUpperInvariant().IndexOf(fileKey.ToUpperInvariant()) != -1).FirstOrDefault();

            if (string.IsNullOrEmpty(filePath))
                return file;

            using (FileStream fs = File.OpenRead(filePath))
            {
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    file = binaryReader.ReadBytes((int)fs.Length);
                }
            }

            return file;
        }
    }
}
