using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OAUS.Core;
using System.IO;

namespace OAUS.Server
{
    public partial class FileVersionForm : Form
    {
        private bool changed = false;
        private UpdateConfiguration fileConfig;
        public FileVersionForm(UpdateConfiguration _fileConfig)
        {
            InitializeComponent();
            this.fileConfig = _fileConfig;            
            this.BindData();
        }

        private void BindData()
        {
            if (this.fileConfig.FileList.Count == 0)
            {
                List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
                foreach (string fileRelativePath in files)
                {
                    FileInfo info = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\" + fileRelativePath);
                    this.fileConfig.FileList.Add(new FileUnit(fileRelativePath, 1 ,(int)info.Length ,info.LastWriteTime));
                }                

                this.fileConfig.Save();
                this.changed = true;
            } 
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = this.fileConfig.FileList;
            this.label1.Text = "最后更新时间：" + DateTime.Now.ToString();
            this.label_lastVersion.Text = "最后综合版本：" + this.fileConfig.ClientVersion;
        }  

        private void FileVersionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.changed)
            {
                ++this.fileConfig.ClientVersion;
                this.fileConfig.Save();
                MessageBox.Show(string.Format("综合版本更新为：{0}" ,this.fileConfig.ClientVersion));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IList<FileUnit> fileList = this.fileConfig.FileList;
            int changedCount = 0;
            int addedCount = 0;
            List<FileUnit> deleted = new List<FileUnit>();
            //获取最新的FileFolder文件夹下的所有文件名的相对路径
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
            foreach (string fileRelativePath in files)
            {          
                FileInfo info = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\" + fileRelativePath);
                FileUnit unit = this.GetFileUnit(fileRelativePath);
                if (unit == null)
                {
                    unit = new FileUnit(fileRelativePath, 1, (int)info.Length, info.LastWriteTime);
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
                    if (fileRelativePath == unit.FileRelativePath)
                    {
                        found = true;
                        break;
                    }
                } 
                if (!found)
                {
                    deleted.Add(unit);
                }
            }
            foreach (FileUnit unit in deleted)
            {
                this.fileConfig.FileList.Remove(unit);
            }
            this.fileConfig.Save();

            if (changedCount > 0 || addedCount > 0 || deleted.Count > 0)
            {
                this.changed = true;
                this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = this.fileConfig.FileList;
                string msg = string.Format("更新：{0}，新增：{1}，删除：{2}" ,changedCount, addedCount, deleted.Count);
                MessageBox.Show(msg);
            }       
        }

        private FileUnit GetFileUnit(string fileRelativePath)
        {
            foreach (FileUnit unit in this.fileConfig.FileList)
            {
                if (unit.FileRelativePath == fileRelativePath)
                {
                    return unit;
                }
            }

            return null;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
