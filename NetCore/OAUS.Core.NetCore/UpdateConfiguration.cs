using ESBasic.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core.NetCore
{
    public class UpdateConfiguration : AgileConfiguration
    {
        #region ConfigurationPath
        public static string ConfigurationPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "UpdateConfiguration.xml";
            }
        }
        #endregion

        #region FileList
        private IList<FileUnit> fileList = new List<FileUnit>();
        public IList<FileUnit> FileList
        {
            get { return fileList; }
            set { fileList = value; }
        }
        #endregion

        #region ClientVersion
        private int clientVersion = 0;
        public int ClientVersion
        {
            get { return clientVersion; }
            set { clientVersion = value; }
        }
        #endregion

        public void Save()
        {
            this.Save(UpdateConfiguration.ConfigurationPath);
        }
    }

}