using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;
using CPF.Styling;
using ESBasic;
using ESBasic.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdater.NetCore
{
    /// <summary>
    /// 说明：
    /// OAUS使用的是免费版的通信框架ESFramework，最多支持10个人同时在线更新。如果要突破10人限制，请联系 www.oraycn.com
    /// </summary>
    public class MainForm : Window
    {
        private Updater updater;
        private int fileCount = 0; //要升级的文件个数。
        //private Timer timer;
        private string callBackExeName;   //自动升级完成后，要启动的exe的名称。
        private string callBackPath = ""; //自动升级完成后，要启动的exe的完整路径。        
        private bool startAppAfterClose = false; //关闭升级窗体前，是否启动应用程序。

        #region
        private ProgressBar progressBar1;
        private TextBlock label1;
        private TextBlock label2;
        private TextBlock label_reconnect;
        private TextBlock label_progress;
        public static string systemType;

        public string serverIP;
        public int serverPort; 
        #endregion

        public MainForm(string ServerIP, int ServerPort, string _callBackExeName)
        {
            CommandContext = this;
            serverIP = ServerIP;
            serverPort = ServerPort;
            callBackExeName = _callBackExeName;
            
        }

        protected override void InitializeComponent()
        {  
            ViewFill color = "#fff";
            ViewFill hoverColor = "255,255,255,40";
            Title = "标题";
            Width = 418;
            Height = 160;
            Background = Color.FromRgb(203, 216, 230);
            Children.Add(new Panel()
            {
                
                BorderFill = Color.Black,
                BorderThickness = new Thickness(1),
                BorderType = BorderType.BorderThickness,
                ClipToBounds = true,
                Background = "#fff",
                MarginRight = 0f,
                Size = SizeField.Fill,
                Children = //内容元素放这里
                {
                    new Panel
                    {
                        Height = 30,
                        Width = "100%",
                        Background = "#619fd7",
                        MarginTop = 0,
                        Children =
                        {
                            new Picture
                            {
                                MarginTop = 4,
                                MarginLeft = 4,
                                Width = 20,
                                Height = 20,
                                Stretch = Stretch.Fill,
                                Source="res://AutoUpdater.NetCore/32.ico",
                            },
                            new TextBlock
                            {
                                MarginTop = 6,
                                MarginLeft = 37,
                                Text = "文件更新",
                            },
                            new Panel
                            {
                                MarginRight = 1,
                                MarginLeft = "Auto",
                                MarginTop = -3,
                                Name = "close",
                                ToolTip = "关闭",
                                Width = 30,
                                Height = 30f,
                                Children =
                                {
                                    new Line
                                    {
                                        MarginTop=8,
                                        MarginLeft=8,
                                        StartPoint = new Point(1, 1),
                                        EndPoint = new Point(14, 13),
                                        StrokeStyle = "2",
                                        IsAntiAlias=true,
                                        StrokeFill=color
                                    },
                                    new Line
                                    {
                                        MarginTop=8,
                                        MarginLeft=8,
                                        StartPoint = new Point(14, 1),
                                        EndPoint = new Point(1, 13),
                                        StrokeStyle = "2",
                                        IsAntiAlias=true,
                                        StrokeFill=color
                                    }
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.MouseDown),
                                        (s,e)=>
                                        {

                                        }
                                    },
                                    {
                                        nameof(Button.MouseDown),
                                        (s,e)=>
                                        {
                                            (e as MouseButtonEventArgs).Handled=true;
                                            
                                            this.Close();
                                        }
                                    }
                                },
                                Triggers =
                                {
                                    new Trigger(nameof(Panel.IsMouseOver), Relation.Me)
                                    {
                                        Setters =
                                        {
                                            {
                                                nameof(Panel.Background),
                                                hoverColor
                                            }
                                        }
                                    }
                                },
                            },
                        },
                        Commands =
                        {
                            {
                                nameof(MouseDown),
                                (s,e)=>this.DragMove()
                            }
                        }
                    },
                    new Panel
                    {
                        Width = "100%",
                        Height = 156,
                        MarginTop = 30,
                        Children =
                        {
                            new TextBlock
                            {
                                Name = "label1",
                                PresenterFor = this,
                                MarginLeft = 20,
                                MarginTop = 17,
                                Text = "正在分析升级信息......"
                            },
                            new TextBlock
                            {
                                Name = "label2",
                                PresenterFor = this,
                                MarginTop = 40,
                                FontSize = 13,
                                MarginLeft = 20,
                                Width = 378,
                                Height = 20,
                                TextTrimming = TextTrimming.CharacterEllipsis
                            },
                            new ProgressBar
                            {
                                Name = "progressBar1",
                                PresenterFor = this,
                                Width = 378,
                                Height = 24,
                                MarginLeft = 20,
                                MarginTop = 66,
                            },
                            new TextBlock
                            {
                                Name = "label_reconnect",
                                PresenterFor = this,
                                MarginLeft = 20,
                                MarginTop = 100,
                                Text = "连接断开，正在重连中...",
                                Foreground = Color.FromRgb(255,0,0),
                                FontSize = 14,
                                Visibility = Visibility.Hidden,
                            },
                            new TextBlock
                            {
                                Name = "label_progress",
                                PresenterFor = this,
                                MarginRight = 20,
                                MarginTop = 101,
                                Foreground = Color.Brown,
                                FontSize = 13
                            }
                        }
                    }
                }
            });
            this.progressBar1 = this.FindPresenterByName<ProgressBar>("progressBar1");
            this.label1 = this.FindPresenterByName<TextBlock>("label1");
            this.label2 = this.FindPresenterByName<TextBlock>("label2");
            this.label_reconnect = this.FindPresenterByName<TextBlock>("label_reconnect");
            this.label_progress = this.FindPresenterByName<TextBlock>("label_progress");
            LoadStyleFile("res://AutoUpdater/Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源
            if (!DesignMode)//设计模式下不执行，也可以用#if !DesignMode
            {
                
            }

            this.Initialize();
        } 
        public void Initialize()
        {
            try
            {
                //Thread.Sleep(1000);
                systemType = ESBasic.Helpers.FileHelper.GetFilePathSeparatorChar().ToString();
                this.Closed += MainForm_Closed;
                this.updater = new Updater(serverIP, serverPort);
                this.updater.ToBeUpdatedFilesCount += new CbGeneric<int>(updater_ToBeUpdatedFilesCount);
                this.updater.UpdateStarted += new CbGeneric(updater_UpdateStarted);
                this.updater.FileToBeUpdated += new CbGeneric<int, string, ulong>(updater_FileToBeUpdated);
                this.updater.CurrentFileUpdatingProgress += new CbGeneric<ulong, ulong>(updater_CurrentFileUpdatingProgress);
                this.updater.UpdateDisruptted += new CbGeneric<string>(updater_UpdateDisruptted);
                this.updater.UpdateCompleted += new CbGeneric(updater_UpdateCompleted);
                this.updater.ConnectionInterrupted += new CbGeneric(updater_ConnectionInterrupted);
                this.updater.UpdateContinued += new CbGeneric(updater_UpdateContinued);
                //this.timer = new Timer(timer_Tick,null,1000,500);
                DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                this.callBackPath = dir.Parent.FullName + systemType + this.callBackExeName; //自动升级完成后，要启动的exe的完整路径。（1）被分发的程序的可执行文件exe必须位于部署目录的根目录。（2）OAUS的客户端（即整个AutoUpdater文件夹)也必须位于这个根目录。
                this.updater.Start();
            }
            catch(Exception ee)
            {
                MessageBoxEx.Show(ee.Message);                   
            }
        }

        void updater_UpdateContinued()
        {  
            //this.label_reconnect.Visible = false;
            this.label_reconnect.Text = "重连成功，正在续传...";
            
        }

        void updater_ConnectionInterrupted()
        {
            this.label_reconnect.Visibility = Visibility.Visible;
            this.label_reconnect.Text = "连接断开，正在重连中...";
           
        }

        void updater_UpdateCompleted()
        {           
            this.label1.Text = "更新成功！";
            //this.timer   = new Timer(timer_Tick, null, 1000, 500);           
        }
        

        void updater_UpdateDisruptted(string disrupttedType)
        {            
            this.label1.Text = "更新失败！";
            this.label1.Foreground = Color.Red;
            MessageBoxEx.Show("网络中断，升级失败，请稍后再试！");
            this.startAppAfterClose = false;
            this.Close();          
    
        }

        void updater_CurrentFileUpdatingProgress(ulong total, ulong transfered)
        {
            this.SetProgress(total, transfered);
        }

        void updater_FileToBeUpdated(int fileIndex, string fileName, ulong fileSize)
        { 
            this.label1.Text = string.Format("{0}{1}{2}{3}{4}", "共需更新", this.fileCount, "个文件，正在更新第", fileIndex + 1, "个文件......");
            this.label2.Text = fileName;
            this.label_reconnect.Text = "";
        }

        void updater_UpdateStarted()
        {
            this.ShowMessage(2);
        }

        void updater_ToBeUpdatedFilesCount(int needUpdatedFileCount)
        {
            this.fileCount = needUpdatedFileCount;
            if (needUpdatedFileCount == 0)
            {
                this.ShowMessage(1);
            }
        }

        private void ShowMessage(int messageType)
        {             
            string message = "";
            if (messageType == 1)
            {
                if (updater.removedCount() > 0)
                {
                    message = "文件更新完成";
                    this.label1.Text = message;
                    this.label_reconnect.Text = ""; 
                }
                else
                { 
                    message = "没有文件需要更新";
                    this.label1.Text = message;
                    this.label_reconnect.Text = "";
                    this.Invalidate();
                    Timer time = new Timer(timer_Tick, null, 1000, 500); 
                } 
            }
            if (messageType == 2)
            {
                message = string.Format("{0}{1}{2}", "共需更新", this.fileCount, "个文件，正在分析中......");
                this.progressBar1.Visibility = Visibility.Visible;                    
            }
            if (messageType == 3)
            {
                MessageBoxEx.Show("网络中断，升级失败，请稍后再试！");
                this.startAppAfterClose = false;
                this.Close();
            } 
            this.label1.Text = message;
            this.Invalidate();    
        }

        void timer_Tick(object sender )
        {
            this.TimeUp();
        }

        private void TimeUp()
        {
            CPF.Threading.Dispatcher.MainThread.BeginInvoke(new Action(() => {
                this.startAppAfterClose = true; 
            }));
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
 


        #region SetProgress
        private DateTime lastShowTime = DateTime.Now;
        /// <summary>
        /// 设置UI显示的进度表。
        /// </summary>       
        private void SetProgress(ulong total, ulong transmitted)
        { 
            this.progressBar1.Value = (int)(transmitted * 100/ total); 
            TimeSpan span = DateTime.Now - this.lastShowTime;
            if (span.TotalSeconds >= 1)
            {
                this.lastShowTime = DateTime.Now;
                this.label_progress.Text = string.Format("{0}/{1}", PublicHelper.GetSizeString(transmitted), PublicHelper.GetSizeString(total));
            }
        }
        #endregion

        #region MainForm_Closed 
        private void MainForm_Closed(object sender, EventArgs e)
        {
            
            string processName = this.callBackExeName.Substring(0, this.callBackExeName.Length - 4);
            ESBasic.Helpers.ApplicationHelper.ReleaseAppInstance(processName);

            if (!this.startAppAfterClose)
            {
                Thread.Sleep(1500);
                Environment.Exit(0);
            }

            if (File.Exists(this.callBackPath))
            {
                System.Diagnostics.Process myProcess = System.Diagnostics.Process.Start(this.callBackPath);
            } 
            

        }
        #endregion
    }
}

 
