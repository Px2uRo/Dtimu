using Dtimu.Core;
using Dtimu.IndexSchemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MVCDtimu.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class Lists : ControllerBase
    {
        [HttpGet("VideoList")]
        public IEnumerable<Video> VideoList()
        {
            return Program.DireInfo.Videos.Values.ToArray();
        }

        [HttpGet("AlumbList")]
        public Dictionary<string, Album> AlumbList()
        {
            return Program.DireInfo.Albums;
        }
        [HttpGet("PicturesList")]
        public Dictionary<string, string> PicturesList()
        {
            return Program.DireInfo.Pictures;
        }

    }

    [Route("api/[Controller]")]
    [ApiController]
    public class VersionInfos: ControllerBase
    {
        [HttpGet("Version")]
        public IResult Version()
        {
            var jsonText = "" +
                "{" + $"\"Version\":\"{Program.Version}\" }}";
                var mimeType = "text/json";

            return Results.Text(jsonText, mimeType);

        }

    }


    [Route("api/[Controller]")]
    [ApiController]
    public class Files : ControllerBase
    {
        /*[HttpGet("Vedio/{hash}")]
        public IActionResult GetVideoFile(string hash)
        {
            if (Program.DireInfo.Videos.TryGetValue(hash, out var video))
            {
                var filePath = video.FilePath;
                var fileName = System.IO.Path.GetFileName(filePath);
                var mimeType = "application/octet-stream"; // You can set the appropriate MIME type based on your file type
                return PhysicalFile(filePath, mimeType, fileName);
            }
            return NotFound();
        }*/ //先不管你
        
        [HttpGet("Music/{hash}")]
        public IActionResult GetMusicFile(string hash)
        {
            var filePath = Path.Combine(Program.RootPath, hash[..2], hash[..6]);
            var pD = System.IO.Path.GetDirectoryName(filePath);
            foreach (var item in Directory.GetFiles(pD))
            {
                if (item.Contains(hash[..6]))
                {
                    filePath = item;
                    break;
                }
            }
            if (System.IO.File.Exists(filePath))
            {
                var fileName = System.IO.Path.GetFileName(filePath);
                var mimeType = "application/octet-stream"; // You can set the appropriate MIME type based on your file type
                
                return PhysicalFile(filePath, mimeType, fileName);
            }
            return NotFound();
        }
    }
}
