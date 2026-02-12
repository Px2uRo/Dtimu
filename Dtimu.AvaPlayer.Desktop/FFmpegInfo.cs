using Dtimu.AvaPlayer.Singletons;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtimu.AvaPlayer.Desktop
{
    internal class FFmpegInfo : IFFmpegInfo
    {
        public string GetBinaryPath() => Path.Combine(Environment.CurrentDirectory, "FFmpeg_bin");
    }
}
