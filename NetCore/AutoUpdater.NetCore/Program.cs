using AutoUpdater.NetCore;
using CPF.Controls;
using CPF.Linux;//如果需要支持Linux才需要
using CPF.Mac;//如果需要支持Mac才需要
using CPF.Platform;
using CPF.Skia;
using CPF.Windows;
using ESPlus;
using System;
using System.Configuration;

namespace AutoUpdater.NetCore
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
                (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory())
                , (OperatingSystemType.OSX, new MacPlatform(), new SkiaDrawingFactory())//如果需要支持Mac才需要
                , (OperatingSystemType.Linux, new LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
            );
            try
            {            


                GlobalUtil.SetMaxLengthOfMessage(1024 * 1024);
                //192.168.0.69

                string serverIP = ConfigurationManager.AppSettings["ServerIP"];
                int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
                string callBackExeName = ConfigurationManager.AppSettings["CallbackExeName"];
                string processName = callBackExeName.Substring(0, callBackExeName.Length - 4);
                bool haveRun = ESBasic.Helpers.ApplicationHelper.IsAppInstanceExist(processName);

                if (haveRun)
                {
                    MessageBoxEx.Show("目标程序正在运行中，请先退出程序，再执行升级！");
                    return;
                }

                MainForm form = new MainForm(serverIP, serverPort, callBackExeName);
                Application.Run(form);
            }
            catch (Exception ee)
            { 
                MessageBoxEx.Show(ee.Message);
                
            }
        }
    }
}
