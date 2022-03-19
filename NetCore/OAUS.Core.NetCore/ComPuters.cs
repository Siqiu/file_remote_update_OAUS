using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core.NetCore
{
    public class ComPuters
    {
        /// <summary>
        /// 将UpdateConfiguration.xml文件中的FileRelativePath转换为linux路径
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string IsLinuxInstallPath(string filepath)
        {
            string IsPath = ESBasic.Helpers.FileHelper.GetFilePathSeparatorChar().ToString();
            if (IsPath != "\\")
            {
                string linuxPathConvert = "";
                char[] vs = filepath.ToCharArray();
                for (int i = 0; i < vs.Length; i++)
                {
                    if (vs[i] != '\\')
                    {
                        linuxPathConvert += vs[i];
                    }
                    else
                    {
                        linuxPathConvert += '/';
                    }
                }
                return linuxPathConvert;
            }

            return filepath;
        }

        /// <summary>
        /// 将UpdateConfiguration.xml文件中的FileRelativePath转换为Windows路径
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string LinuxConvertWindows(string filepath)
        {
            string IsPath = ESBasic.Helpers.FileHelper.GetFilePathSeparatorChar().ToString();
            if (IsPath != "\\")
            {
                string linuxPathConvert = "";
                char[] vs = filepath.ToCharArray();
                for (int i = 0; i < vs.Length; i++)
                {
                    if (vs[i] != '/')
                    {
                        linuxPathConvert += vs[i];
                    }
                    else
                    {
                        linuxPathConvert += '\\';
                    }
                }
                return linuxPathConvert;
            }

            return filepath;
        }
    }
}
