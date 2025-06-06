using Dtimu.IndexSchemas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using File = System.IO.File;

namespace Dtimu.DiscEditor.Core
{

    public class Project
    {
        public long DiscSize { get; set; }
        [JsonProperty("ItemsPath")]
        public ObservableCollection<string> MusicItemsPath { get; set; }
        public ObservableCollection<string> SubTilteFiles { get; set; }
        public ObservableCollection<MusicFileProjInfo> MusicProjects { get; set; } = new ObservableCollection<MusicFileProjInfo>();
        public ObservableCollection<VideoFileProjInfo> VideoProjects { get; set; } = new ObservableCollection<VideoFileProjInfo>();

        public static Project Load(string filePath)
        {
            var proj = JsonConvert.DeserializeObject<Project>(File.ReadAllText(filePath));
            return proj;
        }
        public Project()
        {

            MusicItemsPath = new ObservableCollection<string>();
            DiscSize = CDSizes.CD_CAPACITY;
        }

        public void Save(string workingPath)
        {
            System.IO.File.Open(workingPath,FileMode.OpenOrCreate).Close();
            File.WriteAllText(workingPath,JsonConvert.SerializeObject(this));
        }
    }
    public class ProjectTemplate
    {
        public string Name { get; set; }
        public long DiscSize { get; set; }
        [JsonProperty("ItemsPath")]
        public List<string> MusicItemsPath { get; set; }
        public List<string> SubtitlePath { get; set; }
        public List<MusicFileProjInfo> Musics { get;set; }
        public string Icon { get;set; }
        public object Page { get; set; }

        public ProjectTemplate()
        {
            MusicItemsPath = new List<string>();
            DiscSize = CDSizes.CD_CAPACITY;
        }
    }

    public class MusicFileProjInfo : INotifyPropertyChanging
    {
        public override string ToString()
        {
            return _filePath;
        }

        private TagLib.Tag _libTag;
        private TagLib.Properties _p;
        private string _filePath;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                OnPropertyChanging(nameof(FilePath));
                _filePath = value;
            }
        }

        public static MusicFileProjInfo OpenFile(string fp)
        {
            return new MusicFileProjInfo(fp);
        }
        public MusicFileProjInfo()
        {
            _libTag = new TagLib.Id3v2.Tag();
            _p = new TagLib.Properties(TimeSpan.Zero);
        }


        public MusicFileProjInfo(string fp) : this()
        {
            _filePath = fp;
            ProcessInfos(fp);
        }

        private void ProcessInfos(string fp)
        {
            using (var f = TagLib.File.Create(fp))
            {
                _libTag = f.Tag;
                _p = f.Properties;
                var l1 = new List<string>();
                var l2 = new List<string>();

                foreach (var item in _libTag.Performers)
                {
                    if (item.Contains("/"))
                    {
                        l1.AddRange(item.Split('/'));
                    }
                    else
                    {
                        l1.Add(item);
                    }
                }
                _libTag.Performers = l1.ToArray();

                foreach (var item in _libTag.AlbumArtists)
                {
                    if (item.Contains("/"))
                    {
                        l2.AddRange(item.Split('/'));
                    }
                    else
                    {
                        l2.Add(item);
                    }
                }
                _libTag.AlbumArtists = l2.ToArray();
                setPicCache();
            }
        }

        public TagTypes TagTypes => _libTag.TagTypes;

        public string Title
        {
            get => _libTag.Title ?? "unknown Title";
            set
            {
                OnPropertyChanging(nameof(Title));
                _libTag.Title = value;
            }
        }

        public string TitleSort => _libTag.TitleSort ?? "unknown TitleSort";
        public string Subtitle => _libTag.Subtitle ?? "unknown Subtitle";
        public string Description => _libTag.Description ?? "unknown Description";
        public TimeSpan Duration =>_p.Duration;

        public string[] Performers
        {
            get => _libTag.Performers ?? new string[0];
            set
            {
                OnPropertyChanging(nameof(Performers));
                _libTag.Performers = value;
            }
        }

        public string[] PerformersSort => _libTag.PerformersSort ?? new string[0];
        public string[] PerformersRole => _libTag.PerformersRole ?? new string[0];

        public string[] AlbumArtists
        {
            get => _libTag.AlbumArtists ?? new string[0];
            set
            {
                OnPropertyChanging(nameof(AlbumArtists));
                _libTag.AlbumArtists = value;
            }
        }

        public string[] AlbumArtistsSort => _libTag.AlbumArtistsSort ?? new string[0];

        public string[] Composers
        {
            get => _libTag.Composers ?? new string[0];
            set
            {
                OnPropertyChanging(nameof(Composers));
                _libTag.Composers = value;
            }
        }

        public string[] ComposersSort => _libTag.ComposersSort ?? new string[0];

        public string Album
        {
            get => _libTag.Album ?? "unknown Album";
            set
            {
                OnPropertyChanging(nameof(Album));
                _libTag.Album = value;
            }
        }

        public string AlbumSort => _libTag.AlbumSort ?? "unknown AlbumSort";
        public string Comment => _libTag.Comment ?? "unknown Comment";

        public string[] Genres
        {
            get => _libTag.Genres ?? new string[0];
            set
            {
                OnPropertyChanging(nameof(Genres));
                _libTag.Genres = value;
            }
        }

        public uint Year
        {
            get => _libTag.Year;
            set
            {
                OnPropertyChanging(nameof(Year));
                _libTag.Year = value;
            }
        }

        public uint Track
        {
            get => _libTag.Track;
            set
            {
                OnPropertyChanging(nameof(Track));
                _libTag.Track = value;
            }
        }

        public uint TrackCount
        {
            get => _libTag.TrackCount;
            set
            {
                OnPropertyChanging(nameof(TrackCount));
                _libTag.TrackCount = value;
            }
        }

        public uint Disc
        {
            get => _libTag.Disc;
            set
            {
                OnPropertyChanging(nameof(Disc));
                _libTag.Disc = value;
            }
        }

        public uint DiscCount
        {
            get => _libTag.DiscCount;
            set
            {
                OnPropertyChanging(nameof(DiscCount));
                _libTag.DiscCount = value;
            }
        }

        public string Lyrics
        {
            get => _libTag.Lyrics ?? "unknown Lyrics";
            set
            {
                OnPropertyChanging(nameof(Lyrics));
                _libTag.Lyrics = value;
            }
        }

        public string Grouping
        {
            get => _libTag.Grouping ?? "unknown Grouping";
            set
            {
                OnPropertyChanging(nameof(Grouping));
                _libTag.Grouping = value;
            }
        }

        public uint BeatsPerMinute
        {
            get => _libTag.BeatsPerMinute;
            set
            {
                OnPropertyChanging(nameof(BeatsPerMinute));
                _libTag.BeatsPerMinute = value;
            }
        }

        public string Conductor
        {
            get => _libTag.Conductor ?? "unknown Conductor";
            set
            {
                OnPropertyChanging(nameof(Conductor));
                _libTag.Conductor = value;
            }
        }

        public string Copyright
        {
            get => _libTag.Copyright ?? "unknown Copyright";
            set
            {
                OnPropertyChanging(nameof(Copyright));
                _libTag.Copyright = value;
            }
        }

        public DateTime? DateTagged
        {
            get => _libTag.DateTagged;
            set
            {
                OnPropertyChanging(nameof(DateTagged));
                _libTag.DateTagged = value;
            }
        }

        public string MusicBrainzArtistId
        {
            get => _libTag.MusicBrainzArtistId ?? "unknown MusicBrainzArtistId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzArtistId));
                _libTag.MusicBrainzArtistId = value;
            }
        }

        public string MusicBrainzReleaseGroupId
        {
            get => _libTag.MusicBrainzReleaseGroupId ?? "unknown MusicBrainzReleaseGroupId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseGroupId));
                _libTag.MusicBrainzReleaseGroupId = value;
            }
        }

        public string MusicBrainzReleaseId
        {
            get => _libTag.MusicBrainzReleaseId ?? "unknown MusicBrainzReleaseId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseId));
                _libTag.MusicBrainzReleaseId = value;
            }
        }

        public string MusicBrainzReleaseArtistId
        {
            get => _libTag.MusicBrainzReleaseArtistId ?? "unknown MusicBrainzReleaseArtistId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseArtistId));
                _libTag.MusicBrainzReleaseArtistId = value;
            }
        }

        public string MusicBrainzTrackId
        {
            get => _libTag.MusicBrainzTrackId ?? "unknown MusicBrainzTrackId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzTrackId));
                _libTag.MusicBrainzTrackId = value;
            }
        }

        public string MusicBrainzDiscId
        {
            get => _libTag.MusicBrainzDiscId ?? "unknown MusicBrainzDiscId";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzDiscId));
                _libTag.MusicBrainzDiscId = value;
            }
        }

        public string MusicIpId
        {
            get => _libTag.MusicIpId ?? "unknown MusicIpId";
            set
            {
                OnPropertyChanging(nameof(MusicIpId));
                _libTag.MusicIpId = value;
            }
        }

        public string AmazonId
        {
            get => _libTag.AmazonId ?? "unknown AmazonId";
            set
            {
                OnPropertyChanging(nameof(AmazonId));
                _libTag.AmazonId = value;
            }
        }

        public string MusicBrainzReleaseStatus
        {
            get => _libTag.MusicBrainzReleaseStatus ?? "unknown MusicBrainzReleaseStatus";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseStatus));
                _libTag.MusicBrainzReleaseStatus = value;
            }
        }

        public string MusicBrainzReleaseType
        {
            get => _libTag.MusicBrainzReleaseType ?? "unknown MusicBrainzReleaseType";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseType));
                _libTag.MusicBrainzReleaseType = value;
            }
        }

        public string MusicBrainzReleaseCountry
        {
            get => _libTag.MusicBrainzReleaseCountry ?? "unknown MusicBrainzReleaseCountry";
            set
            {
                OnPropertyChanging(nameof(MusicBrainzReleaseCountry));
                _libTag.MusicBrainzReleaseCountry = value;
            }
        }

        public double ReplayGainTrackGain
        {
            get => _libTag.ReplayGainTrackGain;
            set
            {
                OnPropertyChanging(nameof(ReplayGainTrackGain));
                _libTag.ReplayGainTrackGain = value;
            }
        }

        public double ReplayGainTrackPeak
        {
            get => _libTag.ReplayGainTrackPeak;
            set
            {
                OnPropertyChanging(nameof(ReplayGainTrackPeak));
                _libTag.ReplayGainTrackPeak = value;
            }
        }

        public double ReplayGainAlbumGain
        {
            get => _libTag.ReplayGainAlbumGain;
            set
            {
                OnPropertyChanging(nameof(ReplayGainAlbumGain));
                _libTag.ReplayGainAlbumGain = value;
            }
        }

        public double ReplayGainAlbumPeak
        {
            get => _libTag.ReplayGainAlbumPeak;
            set
            {
                OnPropertyChanging(nameof(ReplayGainAlbumPeak));
                _libTag.ReplayGainAlbumPeak = value;
            }
        }

        public string InitialKey
        {
            get => _libTag.InitialKey ?? "unknown InitialKey";
            set
            {
                OnPropertyChanging(nameof(InitialKey));
                _libTag.InitialKey = value;
            }
        }

        public string RemixedBy
        {
            get => _libTag.RemixedBy ?? "unknown RemixedBy";
            set
            {
                OnPropertyChanging(nameof(RemixedBy));
                _libTag.RemixedBy = value;
            }
        }

        public string Publisher
        {
            get => _libTag.Publisher ?? "unknown Publisher";
            set
            {
                OnPropertyChanging(nameof(Publisher));
                _libTag.Publisher = value;
            }
        }

        public string ISRC
        {
            get => _libTag.ISRC ?? "unknown ISRC";
            set
            {
                OnPropertyChanging(nameof(ISRC));
                _libTag.ISRC = value;
            }
        }
        public List<string> Base64OfPictures { get; set; } = new List<string>();
        [JsonIgnore]
        public IPicture[] Pictures
        {
            get 
                {
                    if (_libTag.Pictures.Length == 0)
                    {
                    return getPicChace();
                    }
                else
                {
                    return _libTag.Pictures;
                }
                } 
            set
            {
                OnPropertyChanging(nameof(Pictures));
                _libTag.Pictures = value;
            }
        }

        private IPicture[] getPicChace()
        {
            var res = new List<IPicture>();
            foreach (var b in Base64OfPictures)
            {
                var p = new TagLib.Picture(new TagLib.ByteVector(
                    Convert.FromBase64String(b)));
                res.Add(p);
            }
            return res.ToArray();
        }
        private void setPicCache()
        {
            Base64OfPictures.Clear();
            foreach (var item in _libTag.Pictures)
            {
                Base64OfPictures.Add(Convert.ToBase64String(item.Data.ToArray()));
            }
        }

        public int AudioBitrate { get => _p.AudioBitrate; }
        public int AudioSampleRate => _p.AudioSampleRate;
        public string FirstArtist => Performers?.FirstOrDefault();
        public string FirstAlbumArtist => AlbumArtists?.FirstOrDefault();
        public string FirstAlbumArtistSort => AlbumArtistsSort?.FirstOrDefault();
        public string FirstPerformer => Performers?.FirstOrDefault();
        public string FirstPerformerSort => PerformersSort?.FirstOrDefault();
        public string FirstComposerSort => ComposersSort?.FirstOrDefault();
        public string FirstComposer => Composers?.FirstOrDefault();
        public string FirstGenre => Genres?.FirstOrDefault();
        public string JoinedAlbumArtists => string.Join("; ", AlbumArtists ?? new string[0]);
        public string JoinedPerformers => string.Join("; ", Performers ?? new string[0]);
        public string JoinedPerformersSort => string.Join("; ", PerformersSort ?? new string[0]);
        public string JoinedComposers => string.Join("; ", Composers ?? new string[0]);
        public string JoinedGenres => string.Join("; ", Genres ?? new string[0]);

        private bool _noError = false;

        public bool NoError
        {
            get { return _noError; }
            set { _noError = value;
                OnPropertyChanging(nameof(NoError));
            }
        }
    }
    public class VideoFileProjInfo : INotifyPropertyChanging
    {
        public override string ToString()
        {
            return _filePath;
        }

        private TagLib.Properties _p;
        private string _filePath;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                OnPropertyChanging(nameof(FilePath));
                _filePath = value;
            }
        }

        public static VideoFileProjInfo OpenFile(string fp)
        {
            return new VideoFileProjInfo(fp);
        }

        public VideoFileProjInfo()
        {
            _p = new TagLib.Properties(TimeSpan.Zero);
        }

        public VideoFileProjInfo(string fp) : this()
        {
            _filePath = fp;
            ProcessInfos(fp);
        }

        private void ProcessInfos(string fp)
        {
            using (var f = TagLib.File.Create(fp))
            {
                _p = f.Properties;
            }
        }

        // 与视频文件相关的信息
        public TimeSpan Duration => _p.Duration;
        public int AudioBitrate => _p.AudioBitrate;
        public int AudioSampleRate => _p.AudioSampleRate;

        private bool _noError = false;

        public bool NoError
        {
            get { return _noError; }
            set
            {
                _noError = value;
                OnPropertyChanging(nameof(NoError));
            }
        }
    }


    public class CodeSetting
    {
        #region Icons
        public const string ImageOfDisc = "iVBORw0KGgoAAAANSUhEUgAAAKAAAACgCAYAAACLz2ctAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAFxEAABcRAcom8z8AABrzSURBVHhe7Z0JdBTXlYZJMjnJTIxa6qW6BUIIEFq6uiVArEaYHSMWdZWQAFlsBmNANjuYfTWLZUDsO2bfQWAEmM1sxpiJjY3H9iSOcWIncUz2M5NxJsmcGb95t/qV3I1vo26pqrq7qv5zvtNGrq7quvevV/VevaWBqUi06LtJzUssllxfYoMGJd9jfzRlqn5Kdpekct6iXi5eLHd6itZyvHje6RFvOT3C+/TzUycv/pb+7Sv63yQQ9rcHdLv7HC+8R7nJecSzdPs19HOcky/qbs0tbswOY8pUgwacV+zodIuzqHmOU/Pco59/5dwDiCOjJ7E1f5xYm7QmtqZ5QUZTgL/Q49yln4cpkxxuXyv2c0zpXcleIY/zCNNoKXXGyQv/wWUVSEZLasQTi70JaWhJIg0bNgyieYaXLN1ykVS8co2sP3SbbK96l+w99xE5evk+Of3Gr8iZW1+QY1c+lf627eRdsu7gm+SlXdfIkk2vkZkVJ8iIaZtJn7JFJLfneOLyFmGGpKWn8Hv6u07S/57o4MVc9nNN6UEut1hMb4EnKX9wgOGatiOJXDOSYLF+y2wYzdOzyLk7DxTh7O0vya7T75NlWy+R5xbuI71K55PUvKGYIX9HL5Kd9Df3ZqdhKp5k9/q60BJlM+VPjpY9iDUlh1hsjVCDhQNmJiWBUrN84V7Sa+i3DUnP4XNqxJddnkFt2emZikXZc4QMmqyFNFk/gWc4a0qu/7aKGCpS9rx6FzWOWqzafYMMKV9DmncoCzIj5R6tGM1xtSp2sNM2FW05+KLOUIng+EJia9aJ3l7TUBPVh00Hr6FG0YIlmy+QAU8vC35+5IX/pZ/rHO6B6SwMprQWrb32oCVetSOzD62ttqrXLbY2lq49hJpDS05e/4zMWnWS5IszAktEeF7ck+wV27CwmFJbLq9QQAN/yX+b9RKL1YmaRkmmLVyPmiJarNx5lfR+akGQESlV0NbIwmRKaUkVC168wWX3I0mNsklCQiJqFjUY/fwC1AjRBpqFCkevCDIivSvssmUWNmJhM1VfOXN6/4jeZtZzWX39t1qrCzWJmpSMeB41QKyw4+R7UqWlxoS88N8OXpzBQmiqrnK5hZGu7P6/taa2pTXaFNQcWtBn4FA08bFG5f5b5IlBswJKROE9WmEZwMJpKlxxXp+X84jV9vTuJMnVEjWFlnTs0htNeKzyQsVJktF5ZIARxcONsgQbC6+pR4lesTMhaLam7VWt2UZCtrcNmuhYpurG52T41E01JqQX9K/oZ38WZlMPy+EueQyuVHjWg3ezCRbtKhm10SgllZy9/Rs00bHOxsN3SPv+kwNLw2Us5KZkOdxCPrzBsLfIJxZHU9QE0ebAuX9DExwvlE5a940JefF1e3ZRSxZ+Y8vp8U108oVS16dEm/ptenUlmm9DlGLe2jOkadtSyYS0pvwVrSmPYGkwpjhefAUalJNcGWjSY4mVm06iSY03dld/QHoMnvtNaegpmsPSYRw5c4b9iJ78JUfL7iQh0Y4mPNaYvWw7mtB4ZdSMrQEmFNay1Ohf9iwxmdZ079ih44BN+0blujJh+nI0kfHMpKUHvzEhLxxiKdKv7FlSl6mPbGkd4sp8wFOjp6JJjHfguTDAhJct3v5JLF36EusO/0tbWnuSZI8v8wH9xOFoAvVAxa5rpFm7p5gRhfedOcXNWNr0ISfv60YrHH+G7vBWe+zWdB9Ffvd+aPL0wpbj75CcnuPk0vBecsYAO0tffAsG2fjN15Yk2Tg0ufGAt1V7NHF6Yu/Zj2pMSHP2Rnp63x+wNManoFsQPZGfwdDGRGt81HZDkZqWjiZNb8DIvozOo5gJhfMslXGorl3/iZ7ELWtqG2JJDG8EWiyTmJhEjl7+GE2a3oBBUk3aDPGb0CMcYRmNL9EfX2VL64gmM17RenBSNIGxzWBAxg6W1viQkxe32ls8QRI06C6vJau2n0GTpVcWbzpfY0LOIy5g6Y1t0eeGeVzmk8TiUH50WrSZ99IraKL0zAsvn/ymJOR9A1maY1MwUs3JC8TayI0mMN55flYFmiS9E9CT5oHVK6awdMeWkvMG/Aut8X5sS81Dk6cHRo6fhSbICHT2+YeD0krJBZby2BJ9Rthrb54fl285wsU3ZAyaHCMAA59qasa8uIKlPTZEr4rxjozeJDFGO5MqRfcnRTQ5RmH+umr5VkwRBrP0R1cwvx3nLvw6KTkLTZqeaNO+C5oYIzFimjzORPjSllnYkNkgeoJXNtCbGUuY3miR4UaTYjTa9ZskmZDmfiOzQXQEt157eldiSXKgCdMbdgdHqq7/HE2KkQhspHZ5hK7MDtrKmt43geOF3xvh1hvI7lNvo0kxGiUTVssmvM0soa3ogdfZm3eOqeGTWrB292toQozGkSv3SRrrQ8h5xanMFtrI6Rbaw4GVmgAyHoDOCG5vni5GxynF9JXH/Ab0iH9Ldg9IZfZQX9AYCW1+WKL0AJjt8a59SenoKdJgJDDdqZufo0kwOt1KZssm3Mvsoa7ogcpgirRETl9tfsmNm5CeBYOk122bzVIubGatrpKfBYnLXehmNlFPMKINZqvCkhiPpGd6yJR5leTIxZ+gATapnYLhi/0m5MXtzCbqiJpvAEyJa7E3RpMZT+R16CqVdgfPf4AG1SR8Kve9UVMKOnihBbOL8qIHuARLHmAJjRdate1MymesIPuq76HBNKkbvUr9UwZzvLCB2UVZubxFT8BCL1pOjas0BUKZ2ZCsEjC0Uy4FVemyxfHicZgUHEtsrJPtaU0mzVmNBs5EOboVz2EmFFYx2ygjaWhldgGx2JLRBMcyvfoVk4qtp9GAmSjL8m2X/Ldhj/j3pk27/pDZp/6i9/X1MKMBluBYZlDZBHLmzV+jwTJRh9Z9yiUTwvzezD71Fy1SH8ACf1iSY5UhoyaRE1fvo0EyUY9x83azUlChntPQ9GJv0QVNcqwCbzDMykZ02H3mA/YcSEvB7KKmzEZ1Fy399sdTj5fBMb6mhxHoVux/Pefki15gNqqbYG4QeKAMd23daPNkYSkaEBNtgWUi/AYU3mNWqpscfNHweLn9du5WQHaceAsNiIm2wMKKrhz/6p4uXmzH7BS5aOl31trYgyY8loD3ucvWH0WDYRIdfGPYGna8uIbZKTJJt1/eF9UlssIhISGBTJz9MhoEk+ixaOM5yYAcL7zLLBWZaOnXG169JcXw0gnAgEEj0QCYRJdDFz/2l4BwG67LCu/0i8vgy4VjlpOXt71Kxk5eLI2LTUmNnfbAzOwcsmHfFTQAJtGnbcFEvwG9YjGzVfiC4Zbw5dmrq4J2uuf0O2Tuyp2keFg5yc17HDWGVox+bn7QbzOJLcomb2C34QiHb6akFP8zfBHYfSb0ElXVt78ga3adl5Yw6NW/hKQ1125lS+jPZ45Qi22WbL7gvw3z4gfMWuHJwRc9CV/0dB+L7jgU0MduQcUeMvTpyZJBEiwW1DxKYPZuiX2OXfnUb0BKRKu4Oz3CcvhS0bP1m44MZhSF7u5dexUSu0O5ygyMTsOOZxJ7dBjgX6mTcwulzF61y8kLl+FLc9Yo143p0GsfksVrDpBhY2eQDvm9iM1W90nLx01dih5DbWANXuh+PnHJAVI4egUZOHq5hEgv1CnLDpO1B26j3zMyNWsWR9IeyHmEz+FLMFk1tlMl2HjgKpk8d420AExGdvgdXV3JjcnG/a+j+1QLWHO3pHw1rc35W/cfRfOOw8iI6ZvJjqr30H0ZjRkVJ6S4cB6xmtnr0YKJJuVgHr2sTXcmmH0e3mSMnDBbGo/r4ELPMQiVHWwfanD48ifER0s6OR6M+/QCPerkfS/QR5XnARrcBfQKP03//svAbUdO34Lu10is3nNTigWtCX/MLPZowXRr8IX0jtFbmgqGSMKAcBjDAauVBxpw5pLN6HeUZuH6sySj88gaM1Gj7Uv2+rqwMIUU5xYL6SPMafl7bfo8p+qdJNbZ/9pP5Rh+TcPzHX+UHiGXVyyBL+QL09EdRoO1uy+Q0c8vkJ4dtejnN7fyVTlo0KPjjjO7sAMLT9iipaJIv/tz2Aesw1a5/xZ6LCMgL5Jt8xZmsvCEFsxyDxsXjTPm+1Uo+WTzURNtYWGpk6zpZQl0PxdhX6l5Q8kG+iyJHVPvyPMJhjXLPtxqYOPyhXvQnemZgxd++s2qkbxSi7Ms+i69qM/BPjsN1OeSr7UBLQVw/vQZeRoLSmjRDW/Dxos3GW8aMmhakQLFizdYOBQRrDxJL+wvYd9Pz9yGHlvPjJ29k13UwjYWktCC1yaw8ao919Gd6RU4XylIFJdnUFsWDsVEDThY3j/0FMF+g16ZuvyI/8L2CEdZOEKLXv2fwcbQ9oXtTK/IzS00SJtZKBQXlKxwjPHzjfV4M2f1KX9s6aMIC0Vo0QfvP8LGO08ZZ+4UePaDcwZcnkLFSz9ZMMQBjsF3i+wde7yzhD7O+Q0YxqMNLQH+ARtD+w22Mz2ydMtFOUB3WRhUkxxfGL6I/RY9snLnVbkEfHTvaOiGDxsCJ659hu5Mjzwze4d0zhTVlyWlz9hvwbEWbTiH/hY9snb/m/748uLPWBhw+WtrfgOefetLdGd6pGZqMY84joVCNdFSdiMcy0iv6bYef0c24G9YGHA5c4qbwYbQaIrtSK/0HDqPGVAYxkKhmqgBV8KxYMUh7Lfokd3VNbMl/CcLA65GvK8JbJjSejC6I71iGlBd9p77yB9fXvyKhQGXxds/CTYEztz+At2ZHnly2CL/FeoWn2OhUE3U5JvgWKNmbkV/ix7ZdvKuP74e4QELQwjl5X1fNuCxq8aZ3Gfs3F3SOdNa2m4WCdVUUwnZaJxKCPQGYgb8hIUhtOiD+N9g4wPnjTNrvDyxIg3Q+ywMqonG9+9wrEcN9tIb8tpytTbDgGiA/gAbG6khGjqe+g0oPQd2YqFQXE5v0Sg4Rk7Pcejv0CuRNUQb9FUcDMDyG1DcxUKhuGgJcBOOUb5wL/ob9ErNqziPeJaFIrRqOiPsvoHuTK/AgCI4b3+glC8FHbw4Qt7/4Us/Q3+DEsC6JzA1McyXAzNaxMICPDBgi537YRaO0JKvUuiYie1MzwjPrGQGFH/MwqGIYG4UemeRHm1gClvs2EoxY/HGoCEMMDYbpjCBobFDRk6UBoK9tKWK7D3zLvp9NZAreU7eV/tKSvRBfCds/Oy8V9Cd6RkYSJ2ZP8pvQl58hYWkXoJBXvJF3dk3Az2ukgwqGx9kwFBYrTZpfDWsj1f2zHQyfdEGsmpHNTlwTvnKUc3ALl6YycISWv7RXqI0vxu2M72zeNN5f7AkhP0sLHWSM0fk6CPN67Cv9E4jvt58/B30mErSpn39JhSFEYne1h1In4FDycjxs8isF7dKY3KOXqp755QOA6b4L2qv4GOhCS3O4xNgY/gStjMjEGxC8Z7TLfZg4Qlb1HhDacn3O9hH8w5lX2868q/osZRky+EbJClJnemUkxulkNbt8kk/cZg0KdTcFTul8dnhrEQgD3Nw8UI2C09owZKbsDF8CduZUXhx6yWS9cToGiPSislRmDOHhSmkYAoKudQDHi+crlmLApgCM4+awAwX3lbtpSG046ctI0vXHZamZJF/08GL3/S1bNCg5HssTI9QwNuQgxeM1XX8YWCu4yHla2oCCNBS7df01nyGGnIR/fcsWjrOopWWCjAdfW78c+C2Y2ZtR/erFlDRwEwSDWAuoNy8TqRHwRBiS+tYe1esQNHg3YcAGnksayBb6LMbzHUnj299FFBqQq1P6w69a3aeI05XI9QM0SbRmQ4Xbu3d8WXRK/k8BBOm28dO1qi8+uavpfea05YfJSUTVpPi8asoq0npxHVSYyv0e8O+pwUw6ROW/FjA2qQV1IArmb1qFy0uF4MBjVoTjjfgmQuaVLDkxwL2FvlwCx7K7FW7nHxRd+l20uVp9IRNYouJs1ehiY8JEiyEy+ob2QSV0mh+NnjGnGYstoHSr2OX3njyYwCLrTE8/33IjBW+6D1bmqRy+spj6ImbxAbQWIwlPlZISs6AJqzIx1pT10qTFA14ehl64ibRZ+Wmk2FPDN8jrYxMabW3TnRvWobuMxzszaAJpmgIs1X4smf7uoABYY487ORNosupG59Jr8uwpGNEw4CwyCWX+SSxZ4nJzFaRiePFv4IJo9m8YIIDE3ViSQ9FNAxosTeB2m9kSzQECjoQggGfnbMLDYJJdFi69pDUxQpLeiiiYUBo/+N43wpmp8hFDVgGBsw1WBfyWAY6nNZlQaBoGNCR0YvAlM/MTnVQScn3nB7hv8CEa/beRANioh3Q5AKTtGPJrg2tDZjoSCWce2D959qhVehdYMAhz1WiQTHRhqrrvwi7symG1ga0NmkNr99q74BamzhvUS8wYJrBu2dFk8MX/50MGTUJTXS4aGlAqP3C7deZ6UtjNqqfqAE/BRMaaUanWOGVUz8mhSWj0URHgpYGhN4v8CKD2af+ouaT1g7uO2wRGiQTdYDVpHr3H4wmOVK0NKAtrQOsDTeW2af+4rw+LxgQMN8NawNUODLduWiC64JWBkxItNOab+Efwuv9HIGo+arAgIPL16ABM1GG469/QsZNe5G0zAp//bxw0MqAUPmgFdeFzDbKyeURusqlIMz3hgXPpH7Au926NrPUhhYGtNiSiSOzz/+kuIutzDbKCrpVgwGfmrQeDaBJ3Th183MydvLiei1hWxtaGNCakguv3sJfljVSOXihj1wKGmkSc7XYcfw2Nd4ikpv3OJpQJVHbgAlJDn/TS65CTS+hRB0uDTkcOV2bVSv1CLxOKx09RdNBRGob0JqSQwsmYSeziXqCBefAgI1zSwy34k99gFrtlHmV0tBJNW+1oVDTgFD6cXwhcfBiLrOJuuJ48Q0w4aBxq9Bgm/jZeuSmVNIp2ZxSV9Q0oL25NOhoNbOH+pI7qwIrd2q7fH48AVOkYQmLBmoZELrcu7zib505vX/E7KGNqPnWgQHbFUxEg2/ygFRsPY0mLRqoYUCLlSP29G709is+w2yhncDxTl74AkxotMX3wgU6EaSkNkOTpzVqGBCaXTiPcI1ZQnsFzvq56/T7aBKMTqcn+qDJ0xqlDWhJ4khSo2zS0Oo6SP+9UgmYrSITvQKqwYD9RixBE2B0SoY/963kRQMwEWaucHjUM6CSMEtFJmeO6JFLwckvHkKTYGSmzl+HBhsDSik1AAPV14AAtm8lkM+fWSpy0VJwmmzCyn1voIkwKtAUE2iyR4El3wjI58/sVDfBdLZgwJwe48jxa79Ak2FEoHdLWouMIKOFAkuOEZDPn1mpbmratOsPYQ4QMOGAUeZsCoHkd+8XZLRQYMkxAvL5MyvVXZxX7CjfissXmk0zMvA2JNBoocCSYwTk82c2qp8cvDBBNqHaa2LEC+HOYoAlxwjI588sVH85eXE7GDCldYm0XDuWFCOxYd+VIKOFAkuOEZDPn9lHGXEeUWofTO843PBzy8BEQukZfJDZMLDkGAH5/Jl1FFLes9+nJrwOJvR0H0v2VH+IJscodOsjBJkNA0uOEZDPnzlHOcHYAFozfhdM2LZgIjl6ufZFTfTK8GdfCDIbBpYcIyCfP7ONsnJlFzV1eoRPwIT54kxSdf1zNEF6Z/GaA0Fmw8CSYwTk82eWUV7sdd0DMCEs2GfE8SRbDl0PMhsGlhwjIJ8/s4s6cvC+x2nt+DdgQm+PcWTzsbfRROmV6ttfkCy+VZDhHgZLjhGQz59ZRT3Zc4QMasB7YMK0dqVk5c6raLL0Sm3TbGDJMQLy+TObqCtLri+RGvASmBCYW3kGTZYeGVU+N8hwD4MlxwjI588soo3o7fiAbMKJSw6iCdMbCyr2BBnuYeRuT2qBJT8csH0piXz+zBraCdYPk00IHRhgxXIscXoBBqInJiYFmU4rINGYucIh0CRqwmyhraj5JskmzH5iNFmx/QqaPL1Ql/mdlQA6fmLmCgfZgI899tgd+ol2p1cCZgnt5XQL7TlevCsb8ZnZO9Dk6YG+hU99yxxaoIQBKdEziRbiPMIm2YTdiueQl3ZdQ5MYz8BcMIHG0ArTgGGKmnAYNeBfZCPqbY2SJZUHg4yhNjB6DT5NA0Ygh3tgurxIDpDTcxxZsL4aTWi8sfvU28Tu8JtCTRISbcSa4vUPnaT/Ng1YB3FuoVR+jwz0KVtENh+L/65dak/HZnGkEXvzzjRmwpsWq+sQ/M00YD3kZCu4ywyfujGuu3cVCDUJVZSEhESp1HPwhX/n3OJUiB39O9QwTQPWV/Aajz4fHg00IszOFY8l4oTpy4OMU19gejRrYw9xZPSEWaoONOJ9TVjYTAMqLZfH14/jxdcCjdh/5NK4WkpsxaYTQQaqK4HGoxfnLljNgIWpRnQ704BqyMEXdaZBPxJoxO4lc8jcNafJ6Td+hSY+Vqi6/vMgI0UKLIUgTQyU3U9ajdyeXdSSheVbotubBlRTUl9DXtgWaMRGrYqJb8wKsmzrJdQAsUDbjl2DTFUb8HyX6GzhXwQmu9//wWvMZHdJKgtDSNHvmgbUQsnuAam0NFgoD46XadlpBCmduJ5U7r+FGiFa+AaPCTJYKBKSOGn+ZUdGb0LP7SqsQGTLLGzITrtW0X2YBtRa/gHyQqU8f6FMat5Q0mvofFK+cC9ZdzC6w0VDzp6akEASHU1JEjWdPb0r4dy+j6jx5jvcxens9CIS3adpwGjK5RUKaAL3UH4XaMYaQ5b6Dblk8wWyvepdcubWF6hhlADmzNl4+A6Zv66aDBqzsMZ0UJmAqW5taR0Jl1VAn+vEn9CK1kaHW8hnp1FngWngGKYBY0Awizs13kR6qz5BDfn7hw0pA8NIew6dR8ombyDTlh8lcytfJS9uvUhW7bkuGWjnqXvSuBYw1JErn5C9Zz8k207eJesP3SYVu65JZobvPL94PymZsJp0EWeSzPxRQceAWeVlw9HS+j7UYh28bzg8SrCfq4hMA8awYDl5aoZJ1AD76O36DjXBnwJNogr0sYAe55q/4iSMdvBCC/ZzVJFpwDgT5xnolFYB4H1jqFEqaEl5ghrlIjXPbScvfkBvjZ/RW+Qf6f/7h2wqus1XdBsY9XefAmNebkltlbxwkG63EF4pJnuFvEgqD0rJNKCOlZ7e9wf04zv+f8WmTAOaiqpMA5qKqkwDmoqqTAOaiqpMA5qKqmQDKoRpQFORCUzzkInqg4oGbNDg/wH2mraqOXaX5wAAAABJRU5ErkJggg==";

        #endregion
        public const string ProjExName = ".dtmproj";
        public const string TemplateExName = ".dtmte";
        public static readonly string[] MusicExtensions = {
    ".mp3", ".ogg", ".flac", ".wav", ".aac", ".m4a", ".wma",
    ".aiff", ".alac", ".opus", ".amr", ".midi", ".pcm",
    ".ac3", ".dsd", ".mp2", ".caf" };
        public static readonly string[] VideoExtensions = {
    ".mp4", ".mkv", ".avi", ".mov", ".wmv",
    ".flv", ".webm", ".m4v", ".3gp", ".mpg",
    ".mpeg", ".ts", ".vob", ".rm", ".rmvb",
    ".ogv", ".asf", ".f4v", ".divx", ".mts",
    ".m2ts"
};


        public static IEnumerable<ProjectTemplate> GetDefalutTemplates()
        {
            var res = new List<ProjectTemplate>
            {
                new ProjectTemplate
                {
                    Name = "空白 CD/DVD/BD",
                    DiscSize = CDSizes.CD_CAPACITY,
                    Icon =ImageOfDisc,
                }
            };
            return res;
        }

        
    }
}
    