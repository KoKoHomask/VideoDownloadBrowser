using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace CefSharp.MinimalExample.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            request request = new request();
            request.NotifyMsg += Request_NotifyMsg;
            Browser.RequestHandler = request;
            Browser.LifeSpanHandler = new OpenPageSelf();
        }
        int index = 0;
        private void Request_NotifyMsg(byte[] obj)
        {
            using (FileStream fs = new FileStream(Browser.Title+ ++index + ".ts", FileMode.Create))
            {
                fs.Position = fs.Length;
                fs.Write(obj, 0, obj.Length);
                fs.Close();
            }
        }
    }
   
}
