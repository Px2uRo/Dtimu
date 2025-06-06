using Microsoft.Win32;
using System;


namespace Dtimu.Launcher
{
    public class LauncherUtils
    {
        internal static Version GetHighestDotNetVersion()
        {
            Version highestVersion = null;
            try
            {
                // 定位到注册表路径
                using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    if (ndpKey != null)
                    {
                        foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                        {
                            if (versionKeyName.StartsWith("v"))
                            {
                                using (RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName))
                                {
                                    if (versionKey != null)
                                    {
                                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                                        {
                                            using (RegistryKey subKey = versionKey.OpenSubKey(subKeyName))
                                            {
                                                if (subKey != null && subKey.GetValue("Release") != null)
                                                {
                                                    if (int.TryParse(subKey.GetValue("Release").ToString(), out int releaseKey))
                                                    {
                                                        Version currentVersion = CheckDotNetVersion(releaseKey);
                                                        // 更新为更高的版本
                                                        if (currentVersion != null && (highestVersion == null || currentVersion.CompareTo(highestVersion) > 0))
                                                        {
                                                            highestVersion = currentVersion;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误：" + ex.Message);
            }
            return highestVersion;
        }

        static Version CheckDotNetVersion(int releaseKey)
        {
            if (releaseKey >= 533320) return new Version("4.8.1");
            if (releaseKey >= 528040) return new Version("4.8");
            if (releaseKey >= 461808) return new Version("4.7.2");
            if (releaseKey >= 461308) return new Version("4.7.1");
            if (releaseKey >= 460798) return new Version("4.7");
            if (releaseKey >= 394802) return new Version("4.6.2");
            if (releaseKey >= 394254) return new Version("4.6.1");
            if (releaseKey >= 393295) return new Version("4.6");
            if (releaseKey >= 379893) return new Version("4.5.2");
            if (releaseKey >= 378675) return new Version("4.5.1");
            if (releaseKey >= 378389) return new Version("4.5");
            return null; // 如果版本未知，返回 null
        }
    }
}
