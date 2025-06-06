using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dtimu.Core
{
    public class Image
    {
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public string SmallPic {  get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public DateTime ShotTime { get; set; }
        public string FilePath { get; set; }
    }
    public class Collection
    {
        public string Title { get; set; }   
        public string Description { get; set; }
        public List<string> Images { get; set; }
    }
}
