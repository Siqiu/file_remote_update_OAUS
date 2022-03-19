using ESBasic.Loggers;
using ESPlus.Application;
using ESPlus.Application.CustomizeInfo;
using ESPlus.Application.FileTransfering;
using ESPlus.Application.FileTransfering.Server;
using ESPlus.FileTransceiver;
using ESPlus.Serialization;
using OAUS.Core.NetCore;
using OAUS.Core.NetCore.Contract;
using System; 
using System.Collections.Generic;
using System.Text;

namespace OAUS.Server.NerCore
{
    class CustomizeHandler : ICustomizeHandler
    {
        private IFileController fileController;
        private UpdateConfiguration fileConfig;

        public void Initialize(IFileController _fileController, UpdateConfiguration _fileConfig)
        {
            this.fileController = _fileController;
            this.fileConfig = _fileConfig;
            this.fileController.FileRequestReceived += new CbFileRequestReceived(fileController_FileRequestReceived);
        }

        void fileController_FileRequestReceived(string projectID, string senderID, string fileName, ulong totalSize, ResumedProjectItem resumedFileItem, string savePath)
        {
            this.fileController.BeginReceiveFile(projectID, savePath);
        }

        public void HandleInformation(string sourceUserID, int informationType, byte[] info)
        {
            if (informationType == InformationTypes.DownloadFiles)
            {
                DownloadFilesContract contract = CompactPropertySerializer.Default.Deserialize<DownloadFilesContract>(info, 0); 
                string fileID; 
                string filePath = string.Format("{0}"+ MainShow.expertSystemPath + "{1}", AppDomain.CurrentDomain.BaseDirectory, ComPuters.IsLinuxInstallPath(contract.FileName));

                this.fileController.BeginSendFile(sourceUserID, filePath, contract.FileName, out fileID);
            }
        }

        public byte[] HandleQuery(string sourceUserID, int informationType, byte[] info)
        {
            if (informationType == InformationTypes.GetAllFilesInfo)
            {
                FilesInfoContract contract = new FilesInfoContract();
                contract.AllFileInfoList = this.fileConfig.FileList;
                return CompactPropertySerializer.Default.Serialize<FilesInfoContract>(contract);
            }
            if (informationType == InformationTypes.GetLastUpdateTime)
            {
                LastUpdateTimeContract contract = new LastUpdateTimeContract(this.fileConfig.ClientVersion);                
                return CompactPropertySerializer.Default.Serialize<LastUpdateTimeContract>(contract);
            }
            return null;
        }

        public void OnTransmitFailed(Information information)
        {
            
        }
    }
}
