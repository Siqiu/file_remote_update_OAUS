using ESPlus.Rapid;
using System;
using System.IO;
using System.Collections.Generic;
using OAUS.Core.NetCore;
using ESBasic.Loggers;
using System.Text;

namespace OAUS.Server.NerCore
{
    class MainShow
    {
        private IRapidServerEngine rapidServerEngine;
        private bool changed = false;
        private UpdateConfiguration fileConfig;
        public int count = 0;
        public static string expertSystemPath;
        public string filefolder;
        public MainShow(IRapidServerEngine _rapidServerEngine)
        {
            expertSystemPath = ESBasic.Helpers.FileHelper.GetFilePathSeparatorChar().ToString();
            if(expertSystemPath == "\\")
            {
                expertSystemPath = "FileFolder\\";
            }
            else
            {
                expertSystemPath = "FileFolder/";
            }
            rapidServerEngine = _rapidServerEngine;
            fileConfig = Program.UpgradeConfiguration;
            BindData();
            Show();
        }

        public void Show()
        {
            string msg =  "";
            if (count > 0)
            {
                 msg = "重新开始扫描（请输入Scan），退出（请输入Exit）";
            }
            else
            {
                msg = "开始扫描（请输入Scan），退出（请输入Exit）";
            }
            Console.WriteLine(msg);
            string str = Console.ReadLine();
            switch (str)
            {
                case "Scan":
                    StartScan();
                    break;
                case "Exit":
                    Exit();
                    break;
                default:
                    Console.WriteLine("输入有误，请重新输入");
                    Console.WriteLine("......");
                    Show();
                    break;
            }
        }

        private void BindData()
        {
            if (this.fileConfig.FileList.Count == 0)
            {
                List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + expertSystemPath);
                foreach (string fileRelativePath in files)
                {
                    FileInfo info = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + expertSystemPath + fileRelativePath);
                    this.fileConfig.FileList.Add(new FileUnit(ComPuters.LinuxConvertWindows(fileRelativePath), 1, (int)info.Length, info.LastWriteTime));
                }
                this.fileConfig.Save();
                this.changed = true;
            } 
           
        }

        public void StartScan()
        {            
            count = 1;
            IList<FileUnit> fileList = this.fileConfig.FileList;
            int changedCount = 0;
            int addedCount = 0;
            List<FileUnit> deleted = new List<FileUnit>();
            //获取最新的FileFolder文件夹下的所有文件名的相对路径
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + expertSystemPath);
            foreach (string fileRelativePath in files)
            {
                FileInfo info = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + expertSystemPath + fileRelativePath);
                FileUnit unit = this.GetFileUnit(fileRelativePath);
                if (unit == null)
                {
                    unit = new FileUnit(ComPuters.LinuxConvertWindows(fileRelativePath), 1, (int)info.Length, info.LastWriteTime);
                    this.fileConfig.FileList.Add(unit);
                    ++addedCount;
                }
                else
                {
                    string v = unit.LastUpdateTime.ToString();
                    if (unit.FileSize != info.Length || unit.LastUpdateTime.ToString() != info.LastWriteTime.ToString())
                    {
                        unit.Version += 1;
                        unit.FileSize = (int)info.Length;
                        unit.LastUpdateTime = info.LastWriteTime;
                        ++changedCount;
                    }
                }
            }
            foreach (FileUnit unit in this.fileConfig.FileList)
            {
                bool found = false;
                foreach (string fileRelativePath in files)
                {
                    if (fileRelativePath == ComPuters.IsLinuxInstallPath(unit.FileRelativePath))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    unit.FileRelativePath = ComPuters.LinuxConvertWindows(unit.FileRelativePath);
                    deleted.Add(unit);
                }
            }
            foreach (FileUnit unit in deleted)
            {
                unit.FileRelativePath = ComPuters.LinuxConvertWindows(unit.FileRelativePath);
                this.fileConfig.FileList.Remove(unit);
            }          

            if (changedCount > 0 || addedCount > 0 || deleted.Count > 0)
            {
                this.changed = true;
                this.fileConfig.ClientVersion += 1;
                this.fileConfig.Save();
                string msg = string.Format("更新：{0}，新增：{1}，删除：{2}", changedCount, addedCount, deleted.Count);
                string lastUpdateTime = "最后更新时间：" + DateTime.Now.ToString();
                string lastVersion = "最后综合版本：" + this.fileConfig.ClientVersion;
                Console.WriteLine(msg);
                Console.WriteLine(lastUpdateTime);
                Console.WriteLine(lastVersion);
                Console.WriteLine("......");
                Show();
            }
            else
            {
                Console.WriteLine("没有更新的内容，版本号不变");
                Console.WriteLine("......");
                Show();
            }
        }



        private FileUnit GetFileUnit(string fileRelativePath)
        {
            foreach (FileUnit unit in this.fileConfig.FileList)
            {
                if (ComPuters.IsLinuxInstallPath(unit.FileRelativePath) == fileRelativePath)
                {
                    return unit;
                }
            }
            return null;
        }

        private void Exit()
        {
            Console.WriteLine("======程序正在退出中======");
            System.Threading.Thread.Sleep(2500);
            Environment.Exit(0);
        }
    }
}
