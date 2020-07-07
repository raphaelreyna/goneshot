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
        /*
        private string port;
        private string timeOut;
        private string noDownload;
        private string exitOnFial;
        private string archiveMethod;
        private string mdns;
        private string fileName;
        private string fileExt;
        private string fileMime;
        private string certFile;
        private string keyFile;
        private string ssTLS;
        private string username;
        private string password;
        private string cgi;
        private string cgiStrict;
        private string shellCommand;
        private string replaceHeaders;
        private string dir;
        */

        private Dictionary<String, String> args;

        public MainWindow()
        {
            InitializeComponent();
            args = new Dictionary<String, String>();
        }

        private void run()
        {
            var process = new Process();
            process.StartInfo.FileName = "C:\\Tools\\goneshot\\oneshot.exe";
            String arguments = "";
            foreach(var entry in args)
            {
                String value = entry.Value;
                if (value != "")
                {
                    arguments += value + " ";
                }
            }
            Console.WriteLine(arguments);
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox) sender;
            String name = checkbox.Name;

            switch (name) 
            {
                case "downloadCheckbox":
                    Console.WriteLine("download unset");
                    args[name] = "";
                    break;
            }
            
        }

        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            String name = checkbox.Name;

            switch (name)
            {
                case "downloadCheckbox":
                    Console.WriteLine("download set");
                    args[name] = "-D";
                    break;
            }
        }

        private void handleStart(object sender, RoutedEventArgs e)
        {
            run();
        }
    }
}
