using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core.NetCore.Contract
{
    public class DownloadFilesContract
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
    }
}
