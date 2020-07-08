using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
        private string selectedHeader;

        public MainWindow()
        {
            InitializeComponent();
            args = new Dictionary<String, String>();
            downloadCheckbox.IsChecked = true;
            headersListBox.Items.Add("Content-Type: text/plain");
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

            switch (sendTabControl.SelectedIndex)
            {
                case 0:
                    arguments += args["path"];
                    break;
                case 1:
                    arguments += args["path"];
                    break;
                case 2:
                    foreach (string header in headersListBox.Items)
                    {
                        arguments += "-H \"" + header + "\" ";
                    }
                    arguments += @"-s C:\Windows\System32\WindowsPowerShell\v1\powershell.exe -S 'ls'";
                    break;
            }

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
                    args[name] = "-D";
                    break;
                case "mdnsCheckbox":
                    args[name] = "";
                    break;
                case "eofCheckbox":
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
                    args[name] = "";
                    break;
                case "mdnsCheckbox":
                    args[name] = "-M";
                    break;
                case "eofCheckbox":
                    args[name] = "-F";
                    break;
            }
        }

        private void handleStart(object sender, RoutedEventArgs e)
        {
            run();
        }

        private void handleFileSelectClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string name = b.Name;

            switch (name)
            {
                case "fileDialogButton":
                    OpenFileDialog fd = new OpenFileDialog();

                    bool? result = fd.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        args["path"] = fd.FileName;
                        fileLabel.Content = fd.FileName;
                        folderLabel.Content = "None";
                        dirLabel.Content = "None"; 
                    }
                    break;
                case "folderBrowserButton":
                    FolderBrowserDialog fb = new FolderBrowserDialog();

                    if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        args["path"] = fb.SelectedPath;
                        folderLabel.Content = fb.SelectedPath;
                        fileLabel.Content = "None";
                        dirLabel.Content = "None";
                    }
                    break;
            }
        }

        private void handleAddRemButton(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string name = b.Name;

            switch (name)
            {
                case "remHeaderButton":
                    if (selectedHeader != "")
                    {
                        headersListBox.Items.Remove(selectedHeader);
                        selectedHeader = "";
                    }
                    break;
                case "addHeaderButton":
                    string userInput = headerTextBox.Text;
                    if (userInput != "" && !headersListBox.Items.Contains(userInput))
                    {
                        headersListBox.Items.Add(userInput);
                    }
                    break;
            }
        }

        private void handleHeaderSelection(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedHeader = (string)e.AddedItems[0];
                headerTextBox.Text = selectedHeader;
            }
        }

        private void handleDirButton(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();

            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                args["dir"] = "-d \"" + fb.SelectedPath + "\"";
                dirLabel.Content = fb.SelectedPath;
                fileLabel.Content = "None";
                folderLabel.Content = "None";
            }
        }
    }
}
