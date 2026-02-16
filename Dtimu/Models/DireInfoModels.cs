using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Dtimu.Models
{
    public class Image
    {
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public string SmallPic { get; set; }
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

    public static class DireInfoUtil
    {
        public static DireInfo Load(string jsonText)
        {
            var result = new DireInfo();
            var root = JsonObject.Parse(jsonText);

            // Albums
            if (root.ContainsKey("albums"))
            {
                var albumsObj = root.GetNamedObject("albums");
                foreach (var pair in albumsObj)
                {
                    var albumObj = pair.Value.GetObject();
                    var album = new Album();

                    // Pictures
                    if (albumObj.ContainsKey("pictures"))
                    {
                        foreach (var item in albumObj.GetNamedArray("pictures"))
                            album.Pictures.Add(item.GetString());
                    }

                    // AlbumPerformers
                    if (albumObj.ContainsKey("albumPerformers"))
                    {
                        foreach (var item in albumObj.GetNamedArray("albumPerformers"))
                            album.AlbumPerformers.Add(item.GetString());
                    }

                    // Musics
                    if (albumObj.ContainsKey("musics"))
                    {
                        foreach (var item in albumObj.GetNamedArray("musics"))
                            album.Musics.Add(item.GetString());
                    }

                    result.Albums[pair.Key] = album;
                }
            }

            // Musics
            if (root.ContainsKey("musics"))
            {
                var musicsObj = root.GetNamedObject("musics");
                foreach (var pair in musicsObj)
                {
                    var musicObj = pair.Value.GetObject();
                    var music = new Music();

                    if (musicObj.ContainsKey("fileName"))
                        music.FileName = musicObj.GetNamedString("fileName");

                    if (musicObj.ContainsKey("album"))
                        music.Album = musicObj.GetNamedString("album");

                    if (musicObj.ContainsKey("title"))
                        music.Title = musicObj.GetNamedString("title");

                    if (musicObj.ContainsKey("pictrue"))
                        music.Pictrue = musicObj.GetNamedString("pictrue");

                    if (musicObj.ContainsKey("performers"))
                    {
                        foreach (var item in musicObj.GetNamedArray("performers"))
                            music.Performers.Add(item.GetString());
                    }

                    result.Musics[pair.Key] = music;
                }
            }

            // Images
            if (root.ContainsKey("images"))
            {
                var imagesObj = root.GetNamedObject("images");
                foreach (var pair in imagesObj)
                {
                    var imageObj = pair.Value.GetObject();
                    var image = new Image();

                    if (imageObj.ContainsKey("cameraMake"))
                        image.CameraMake = imageObj.GetNamedString("cameraMake");

                    if (imageObj.ContainsKey("cameraModel"))
                        image.CameraModel = imageObj.GetNamedString("cameraModel");

                    if (imageObj.ContainsKey("smallPic"))
                        image.SmallPic = imageObj.GetNamedString("smallPic");

                    if (imageObj.ContainsKey("location"))
                        image.Location = imageObj.GetNamedString("location");

                    if (imageObj.ContainsKey("title"))
                        image.Title = imageObj.GetNamedString("title");

                    if (imageObj.ContainsKey("filePath"))
                        image.FilePath = imageObj.GetNamedString("filePath");

                    if (imageObj.ContainsKey("shotTime"))
                    {
                        var timeStr = imageObj.GetNamedString("shotTime");
                        DateTime dt;
                        if (DateTime.TryParse(timeStr, out dt))
                            image.ShotTime = dt;
                    }

                    result.Images[pair.Key] = image;
                }
            }

            // Collections
            if (root.ContainsKey("collections"))
            {
                var collectionsObj = root.GetNamedObject("collections");
                foreach (var pair in collectionsObj)
                {
                    var colObj = pair.Value.GetObject();
                    var col = new Collection();

                    if (colObj.ContainsKey("title"))
                        col.Title = colObj.GetNamedString("title");

                    if (colObj.ContainsKey("description"))
                        col.Description = colObj.GetNamedString("description");

                    if (colObj.ContainsKey("images"))
                    {
                        col.Images = new List<string>();
                        foreach (var item in colObj.GetNamedArray("images"))
                            col.Images.Add(item.GetString());
                    }

                    result.Collections[pair.Key] = col;
                }
            }

            // Pictures
            if (root.ContainsKey("pictures"))
            {
                var picObj = root.GetNamedObject("pictures");
                foreach (var pair in picObj)
                {
                    result.Pictures[pair.Key] = pair.Value.GetString();
                }
            }

            // Performers
            if (root.ContainsKey("performers"))
            {
                var performersObj = root.GetNamedObject("performers");
                foreach (var pair in performersObj)
                {
                    var list = new List<string>();
                    foreach (var item in pair.Value.GetArray())
                        list.Add(item.GetString());

                    result.Performers[pair.Key] = list;
                }
            }

            return result;
        }
    }

    public class DireInfo
    {
        public Dictionary<string, Album> Albums { get; set; }
        public Dictionary<string, Music> Musics { get; set; }
        public Dictionary<string, Video> Videos { get; set; }
        public Dictionary<string, Image> Images { get; set; }
        public Dictionary<string, Collection> Collections { get; set; }
        public Dictionary<string, string> Pictures { get; set; }
        public Dictionary<string, List<string>> Performers { get; set; }
        public DireInfo()
        {
            Albums = new Dictionary<string, Album>();
            Musics = new Dictionary<string, Music>();
            Videos = new Dictionary<string, Video>();
            Images = new Dictionary<string, Image>();
            Collections = new Dictionary<string, Collection>();
            Pictures = new Dictionary<string, string>();
            Performers = new Dictionary<string, List<string>>();
        }
    }
    public class Video
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public IEnumerable<string> Tags { get; set; }
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

    public class Album
    {
        public List<string> Pictures { get; set; }
        public List<string> AlbumPerformers { get; set; }
        public List<string> Musics { get; set; }
        public Album()
        {
            AlbumPerformers = new List<string>();
            Musics = new List<string>();
            Pictures = new List<string>();
        }
    }
    public class Music
    {
        public string FileName { get; set; }
        public string Album { get; set; }
        public List<string> Performers { get; set; }
        public string Title { get; set; }
        public string Pictrue { get; set; }

        public Music()
        {
            Performers = new List<string>();
        }
    }
}
