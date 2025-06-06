using Dtimu.IndexSchemas;
using System;
using System.IO;

namespace Dtimu.DiscEditor.Core
{
    internal class Utils
    {
        internal static void ExportToFolder(Project workingProj, string path)
        {
            var indeInst = new DireInfo();
            Directory.CreateDirectory(path);
            foreach (var item in workingProj.MusicItemsPath)
            {
                File.Copy(item,Path.Combine(path,Path.GetFileName(item)));
            }
        }
    }
}