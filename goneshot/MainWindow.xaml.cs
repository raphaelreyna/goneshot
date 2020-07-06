using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace goneshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string port;
        private string timeOut;
        private bool noDownload;
        private bool exitOnFial;
        private string archiveMethod;
        private bool mdns;
        private string fileName;
        private string fileExt;
        private string fileMime;
        private string certFile;
        private string keyFile;
        private bool ssTLS;
        private string username;
        private string password;
        private bool cgi;
        private bool cgiStrict;
        private string shellCommand;
        private bool replaceHeaders;
        private string dir;

        public MainWindow()
        {
            InitializeComponent();
            run();
        }

        private void downloadChecked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void run()
        {
            var process = new Process();
            process.StartInfo.FileName = "C:\\Tools\\goneshot\\oneshot.exe";
            process.StartInfo.Arguments = "-u";
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}
