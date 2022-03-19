using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.NetCore
{
    internal class MessageBoxEx
    {
        public static void Show(string message)
        {
            Show("提示", message);
        }

        public static void Show(string title, string message)
        {
             doShow( title, message);
        }

        private static void doShow(string title, string message)
        {
            MessageBoxExManager.GetMessageBoxExWindow(title, message, ButtonEnum.Ok).Show();
        }

        public static Task<ButtonResult> ShowQuery(string title, string message)
        {
            //return null;
            return ShowQuery(title, message, ButtonEnum.YesNo);
        }

        public static Task<ButtonResult> ShowQuery(string message)
        {
            return ShowQuery("提示", message, ButtonEnum.YesNo);
        }

        public static Task<ButtonResult> ShowQuery(string title, string message, ButtonEnum buttonEnum)
        {
            return MessageBoxExManager.GetMessageBoxExWindow(title, message, buttonEnum).Show();
        }

        public static Task<ButtonResult> ShowDialogQuery(CPF.Controls.Window ownerWindow, string title, string message)
        {
            return ShowDialogQuery(ownerWindow, title, message, ButtonEnum.YesNo);
        }

        public static Task<ButtonResult> ShowDialogQuery(CPF.Controls.Window ownerWindow, string message)
        {
            return ShowDialogQuery(ownerWindow, "提示", message, ButtonEnum.YesNo);
        }

        public static Task<ButtonResult> ShowDialogQuery(CPF.Controls.Window ownerWindow, string title, string message, ButtonEnum buttonEnum)
        {
            return MessageBoxExManager.GetMessageBoxExWindow(title, message, buttonEnum).ShowDialog(ownerWindow);
        }


    }
}
