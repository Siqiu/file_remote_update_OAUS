using ESPlus;
using OAUS.Core;
using OAUS.Core.NetCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OAUS.Server.NerCore
{
    class Program
    {
        private static ESPlus.Rapid.IRapidServerEngine RapidServerEngine = ESPlus.Rapid.RapidEngineFactory.CreateServerEngine();
        internal static UpdateConfiguration UpgradeConfiguration = null;

        static void Main(string[] args)
        {
            initialization(); 
        }

        public static void initialization()
        {
            try
            { 
                //如果是其它类型的授权用户，请使用下面的语句设定正确的授权用户ID和密码。              
                GlobalUtil.SetAuthorizedUser("FreeUser", "");
                GlobalUtil.SetMaxLengthOfMessage(1024 * 1024);

                //初始化服务端引擎
                CustomizeHandler customizeHandler = new CustomizeHandler();
                BasicHandler basic = new BasicHandler();
                int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                RapidServerEngine.WriteTimeoutInSecs = -1;
                RapidServerEngine.Initialize(port, customizeHandler, basic);

                //动态生成或加载配置信息                               
                if (!File.Exists(UpdateConfiguration.ConfigurationPath))
                {
                    Program.UpgradeConfiguration = new UpdateConfiguration();
                    Program.UpgradeConfiguration.Save();
                }
                else
                {
                    Program.UpgradeConfiguration = (UpdateConfiguration)UpdateConfiguration.Load(UpdateConfiguration.ConfigurationPath);
                }
                customizeHandler.Initialize(RapidServerEngine.FileController, Program.UpgradeConfiguration);

                
                MainShow mainForm = new MainShow(Program.RapidServerEngine); 
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message + " - " + ee.StackTrace);
            }
        }
    }
}
