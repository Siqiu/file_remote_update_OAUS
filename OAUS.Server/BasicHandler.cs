using ESFramework;
using ESPlus.Application.Basic.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAUS.Server
{
    class BasicHandler : IBasicHandler
    {
        public string HandleQueryBeforeLogin(AgileIPE clientAddr, int queryType, string query)
        {
            return "";
        }

        public bool VerifyUser(string systemToken, string userID, string password, out string failureCause)
        {
            failureCause = Program.UpgradeConfiguration.ClientVersion.ToString();
            return true;
        }
    }
}
