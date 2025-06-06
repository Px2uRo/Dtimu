using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dtimu.Core
{
    public class Video
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get;set; }
        public IEnumerable<string> Tags {  get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> Gallery { get; set; } = new List<string>();
        public string FilePath { get; set; }

        public Video(string title, string description, string author, IEnumerable<string> tags, TimeSpan duration, string hash, List<string> gallery) : this()
        {
            Title = title;
            Description = description;
            Author = author;
            Tags = tags;
            Duration = duration;
            Gallery = gallery;
        }
        public Video()
        {
            Tags = new string[0];
#if DEBUG
            Title = "title";
            Description = "description";
            Author = "author";
            Duration = TimeSpan.FromMinutes(24);
#endif
        }
    }
}
