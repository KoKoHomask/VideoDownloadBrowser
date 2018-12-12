using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 视频下载器
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            request request = new request(new List<string>() { ".ts",".hxk" });
            request.NotifyMsg += Request_NotifyMsg;
            Browser.RequestHandler = request;
            Browser.LifeSpanHandler = new OpenPageSelf();
            Browser.TitleChanged += Browser_TitleChanged;
        }
        string bwTitle = "NOOP";
        long index = 0;
        private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var bw = sender as CefSharp.Wpf.ChromiumWebBrowser;
            bwTitle = bw.Title;
            index = 0;
            //throw new NotImplementedException();
        }

        
        private void Request_NotifyMsg(string fileType,byte[] obj)
        {
            using (FileStream fs = new FileStream(bwTitle + ++index + fileType, FileMode.Create))
            {
                fs.Position = fs.Length;
                fs.Write(obj, 0, obj.Length);
                fs.Close();
            }
        }

        private void BtnGO_Click(object sender, RoutedEventArgs e)
        {
            //if(IsUrl(tbUrl.Text))
            //{
            //    Browser.Address = tbUrl.Text;
            //}
            Browser.Address = tbUrl.Text;

            //string EvaluateJavaScriptResult;
            //var frame = Browser.GetMainFrame();
            //var task = Browser.EvaluateScriptAsync("(function() { return document.getElementById('searchInput').value; })();", null);

            //task.ContinueWith(t =>
            //{
            //    if (!t.IsFaulted)
            //    {
            //        var response = t.Result;
            //        EvaluateJavaScriptResult = response.Success ? (response.Result.ToString() ?? "null") : response.Message;
            //        MessageBox.Show(EvaluateJavaScriptResult);
            //    }
            //}, TaskScheduler.FromCurrentSynchronizationContext());

        }
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
