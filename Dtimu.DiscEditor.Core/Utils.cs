using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Dtimu.DiscEditor.Core
{
    internal static class Base64Util
    {

    }
    internal static class ProjectUtil
    {
        public static void Save(this Project p, string fp)
        {
            File.WriteAllText(fp, JsonConvert.SerializeObject(p));
        }
        public static Project Load(string fp)
        {
            var c = File.ReadAllText(fp);
            return JsonConvert.DeserializeObject<Project>(c);
        }
    }

    internal static class CDSizes
    {
        // CD
        public const long CD_CAPACITY = 700 * 1024 * 1024; // 700 MB

        // DVD
        public const long DVD_SINGLE_LAYER = 4_700_000_000; // 4.7 GB
        public const long DVD_DUAL_LAYER = 8_500_000_000; // 8.5 GB
        public const long DVD_SINGLE_SIDED_SINGLE_LAYER = 9_400_000_000; // 9.4 GB
        public const long DVD_SINGLE_SIDED_DUAL_LAYER = 17_100_000_000; // 17.1 GB

        // Blu-ray
        public const long BLU_RAY_SINGLE_LAYER = 25_000_000_000; // 25 GB
        public const long BLU_RAY_DUAL_LAYER = 50_000_000_000; // 50 GB
        public const long BLU_RAY_TRIPLE_LAYER = 100_000_000_000; // 100 GB
        public const long BLU_RAY_QUAD_LAYER = 128_000_000_000; // 128 GB

        // HD-DVD
        public const long HD_DVD_SINGLE_LAYER = 15_000_000_000; // 15 GB
        public const long HD_DVD_DUAL_LAYER = 30_000_000_000; // 30 GB

        // Enhanced CD
        public const long ENHANCED_CD_CAPACITY = 700 * 1024 * 1024; // 700 MB (音频和数据结合)
    }

    internal static class IMAPIUtil
    {
        static void GenISO(string sourceFolder, string isoFilePath,ISOType type)
        {

            // 创建 IMAPI 实例
            Type discImageCreatorType = Type.GetTypeFromProgID("Microsoft.IMAPI.DiscImageCreator");
            IDiscImageCreator discImageCreator = (IDiscImageCreator)Activator.CreateInstance(discImageCreatorType);

            // 设置文件系统类型（示例，具体值需要查阅文档）
            discImageCreator.SetFileSystemType((int)type);

            // 创建 ISO 文件
            discImageCreator.CreateImageFromFolder(sourceFolder, isoFilePath);

            Console.WriteLine("ISO 文件已创建: " + isoFilePath);
        }

    }

    [ComImport]
    [Guid("A0C4B8E0-78A0-4C41-9E46-24B6C8D0B5D3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDiscRecorder2
    {
        // 省略具体方法以简化示例
    }

    [ComImport]
    [Guid("A5C0D1C8-4B8D-4C68-AE83-4E7EED2DFDA4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDiscImageCreator
    {
        void CreateImageFromFolder(string folderPath, string isoFilePath);
        void SetFileSystemType(int type); // 示例方法，用于设置文件系统类型
    }

    public enum ISOType
    {
        FsiFileSystemISO9660 = 1,
        FsiFileSystemJoliet = 2,
        FsiFileSystemUDF = 4
    }

}
