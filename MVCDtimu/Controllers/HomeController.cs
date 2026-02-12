using Microsoft.AspNetCore.Mvc;
using MVCDtimu.Models;
using System.Diagnostics;

namespace MVCDtimu.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult FileView(string path = null)
        {
            path = path?.TrimStart('/') ?? "";
            path = path.Replace('/', Path.DirectorySeparatorChar)
                       .Replace('\\', Path.DirectorySeparatorChar);

            var fullDP = Path.GetFullPath(Path.Combine(Program.RootPath, path));

            if (!fullDP.StartsWith(Program.RootPath, StringComparison.OrdinalIgnoreCase))
                return BadRequest("·Ç·¨Â·¾¶");

            if (!System.IO.File.Exists(fullDP) && !System.IO.Directory.Exists(fullDP))
                return NotFound();

            var attr = System.IO.File.GetAttributes(fullDP);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                var par = System.IO.Path.GetDirectoryName(fullDP);
                if (par.Contains(Program.RootPath))
                {
                    par = par.Replace(Program.RootPath, "").Replace(Path.PathSeparator, '/');
                }
                return StatusCode(403);
                //return View(new FileViewModel() { Path = fullDP, ParentP = par });
            }
            else
            {
                string contentType;
                if (!Program.Provider.TryGetContentType(fullDP, out contentType))
                    contentType = "application/octet-stream";

                return PhysicalFile(fullDP, contentType, enableRangeProcessing: true);
            }
        }


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Code404Page()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
