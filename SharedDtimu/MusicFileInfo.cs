using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TagLib;

namespace Dtimu.Core
{
    public static class TagAnalyser
    {
        public static MusicFileInfo OpenFile(string fp)=>MusicFileInfo.OpenFile(fp);
    }
    public class MusicFileInfo
    {
        private TagLib.Tag _libTag;
        private string _filePath;

        public string FilePath { get => _filePath; set => _filePath = value; }
        public static MusicFileInfo OpenFile(string fp)
        {
            return new MusicFileInfo(fp);
        }
        public MusicFileInfo()
        {

        }
        public MusicFileInfo(string fp) : this()
        {
            _filePath = fp;
            ProcessInfos(fp);
        }

        private void ProcessInfos(string fp)
        {
            using (var f = TagLib.File.Create(fp))
            {
                _libTag = f.Tag;
                var bol1 = false; List<string> l1 = new List<string>();
                var bol2 = false; List<string> l2 = new List<string>();
                foreach (var item in _libTag.Performers)
                {
                    if (item.Contains(@"/"))
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
                    if (item.Contains(@"/"))
                    {
                        l2.AddRange(item.Split('/'));
                    }
                    else
                    {
                        l2.Add(item);
                    }
                }
                _libTag.AlbumArtists = l2.ToArray();
            }
        }

        public TagTypes TagTypes { get => _libTag.TagTypes; }
        public string Title { get => _libTag.Title ?? "unknown Title"; set => _libTag.Title = value; }
        public string TitleSort { get => _libTag.TitleSort ?? "unknown TitleSort"; }
        public string Subtitle { get => _libTag.Subtitle ?? "unknown Subtitle"; }
        public string Description { get => _libTag.Description ?? "unknown Description"; }
        public string[] Performers { get => _libTag.Performers ?? new string[0]; }
        public string[] PerformersSort { get => _libTag.PerformersSort ?? new string[0]; }
        public string[] PerformersRole { get => _libTag.PerformersRole ?? new string[0]; }
        public string[] AlbumArtists { get => _libTag.AlbumArtists ?? new string[0]; }
        public string[] AlbumArtistsSort { get => _libTag.AlbumArtistsSort ?? new string[0]; }
        public string[] Composers { get => _libTag.Composers ?? new string[0]; }
        public string[] ComposersSort { get => _libTag.ComposersSort ?? new string[0]; }
        public string Album { get => _libTag.Album ?? "unknown Album"; }
        public string AlbumSort { get => _libTag.AlbumSort ?? "unknown AlbumSort"; }
        public string Comment { get => _libTag.Comment ?? "unknown Comment"; }
        public string[] Genres { get => _libTag.Genres ?? new string[0]; }
        public uint Year { get => _libTag.Year; }
        public uint Track { get => _libTag.Track; }
        public uint TrackCount { get => _libTag.TrackCount; }
        public uint Disc { get => _libTag.Disc; }
        public uint DiscCount { get => _libTag.DiscCount; }
        public string Lyrics { get => _libTag.Lyrics ?? "unknown Lyrics"; }
        public string Grouping { get => _libTag.Grouping ?? "unknown Grouping"; }
        public uint BeatsPerMinute { get => _libTag.BeatsPerMinute; }
        public string Conductor { get => _libTag.Conductor ?? "unknown Conductor"; }
        public string Copyright { get => _libTag.Copyright ?? "unknown Copyright"; }
        public DateTime? DateTagged { get => _libTag.DateTagged; }
        public string MusicBrainzArtistId { get => _libTag.MusicBrainzArtistId ?? "unknown MusicBrainzArtistId"; }
        public string MusicBrainzReleaseGroupId { get => _libTag.MusicBrainzReleaseGroupId ?? "unknown MusicBrainzReleaseGroupId"; }
        public string MusicBrainzReleaseId { get => _libTag.MusicBrainzReleaseId ?? "unknown MusicBrainzReleaseId"; }
        public string MusicBrainzReleaseArtistId { get => _libTag.MusicBrainzReleaseArtistId ?? "unknown MusicBrainzReleaseArtistId"; }
        public string MusicBrainzTrackId { get => _libTag.MusicBrainzTrackId ?? "unknown MusicBrainzTrackId"; }
        public string MusicBrainzDiscId { get => _libTag.MusicBrainzDiscId ?? "unknown MusicBrainzDiscId"; }
        public string MusicIpId { get => _libTag.MusicIpId ?? "unknown MusicIpId"; }
        public string AmazonId { get => _libTag.AmazonId ?? "unknown AmazonId"; }
        public string MusicBrainzReleaseStatus { get => _libTag.MusicBrainzReleaseStatus ?? "unknown MusicBrainzReleaseStatus"; }
        public string MusicBrainzReleaseType { get => _libTag.MusicBrainzReleaseType ?? "unknown MusicBrainzReleaseType"; }
        public string MusicBrainzReleaseCountry { get => _libTag.MusicBrainzReleaseCountry ?? "unknown MusicBrainzReleaseCountry"; }
        public double ReplayGainTrackGain { get => _libTag.ReplayGainTrackGain; }
        public double ReplayGainTrackPeak { get => _libTag.ReplayGainTrackPeak; }
        public double ReplayGainAlbumGain { get => _libTag.ReplayGainAlbumGain; }
        public double ReplayGainAlbumPeak { get => _libTag.ReplayGainAlbumPeak; }
        public string InitialKey { get => _libTag.InitialKey ?? "unknown InitialKey"; }
        public string RemixedBy { get => _libTag.RemixedBy ?? "unknown RemixedBy"; }
        public string Publisher { get => _libTag.Publisher ?? "unknown Publisher"; }
        public string ISRC { get => _libTag.ISRC ?? "unknown ISRC"; }
        public IPicture[] Pictures { get => _libTag.Pictures ?? new IPicture[0]; }
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

    }
}
