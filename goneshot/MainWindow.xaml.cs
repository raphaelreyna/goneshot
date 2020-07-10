using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Button = System.Windows.Controls.Button;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace goneshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         * Missing options:
         * 
         * -t, --timeout
         * -e, --ext
         * --tls-cert
         * --tls-key
         * -T, --ss-tls
         * -U, --username
         * -P, --password
         * -s, --shell
         * -S, --shell-command
         * -E, --env
        */

        private const String oneshotLocation = "C:\\Tools\\goneshot\\oneshot.exe";

        private Dictionary<String, String> paths;
        private String selectedHeader;
        private OpenFileDialog fd;
        private FolderBrowserDialog fb;

        private int mode = 0;

        public MainWindow()
        {
            InitializeComponent();
            paths = new Dictionary<String, String>();
            downloadCheckbox.IsChecked = true;
            sourceTypeComboBox.SelectedIndex = 0;
            sendFileMode();
        }

        private void run()
        {
            if (!paths.ContainsKey("path"))
            {
                if (paths.ContainsKey("saveDir"))
                {
                    receive();
                    return;
                } else
                {
                    return;
                }
            }
            send();
        }

        private void receive()
        {
            var process = new Process();
            process.StartInfo.FileName = oneshotLocation;
            String arguments = "";

            try
            {
                Int32.Parse(portTextBox.Text);
                arguments += "-p " + portTextBox.Text + " ";
            }
            catch
            {
                return;
            }

            bool? isChecked = mdnsCheckbox.IsChecked;
            if (isChecked.HasValue && isChecked.Value)
            {
                arguments += "-M ";
            }
            isChecked = eofCheckbox.IsChecked;
            if (isChecked.HasValue && isChecked.Value)
            {
                arguments += "-F ";
            }

            arguments += "-u " + paths["saveDir"];

            Console.WriteLine("oneshot " + arguments);
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.EnvironmentVariables.Add("ONESHOT_DONT_EXIT", "T");
            process.Start();
        }

        private void send()
        {
            var process = new Process();
            process.StartInfo.FileName = oneshotLocation;
            String arguments = "";

            try
            {
                Int32.Parse(portTextBox.Text);
                arguments += "-p " + portTextBox.Text + " ";
            }
            catch
            {
                return;
            }

            bool? isChecked = mdnsCheckbox.IsChecked;
            if (isChecked.HasValue && isChecked.Value)
            {
                arguments += "-M ";
            }
            isChecked = eofCheckbox.IsChecked;
            if (isChecked.HasValue && isChecked.Value)
            {
                arguments += "-F ";
            }

            isChecked = downloadCheckbox.IsChecked;
            if (isChecked.HasValue && !isChecked.Value)
            {
                arguments += "-D ";
            }
            if (nameTextBox.Text != "")
            {
                arguments += "-n " + nameTextBox.Text + " ";
            }

            switch (sourceTypeComboBox.SelectedIndex)
            {
                case 0: //File
                    if (mimeTextBox.Text != "")
                    {
                        arguments += "-m " + mimeTextBox.Text + " ";
                    }
                    arguments += paths["path"];
                    break;
                case 1: // Folder
                    ComboBoxItem selectedItem = (ComboBoxItem)compressionComboBox.SelectedItem;
                    arguments += "-a " + selectedItem.Content + " ";
                    arguments += paths["path"];
                    break;
                case 2: // Executable

                    if (mimeTextBox.Text != "")
                    {
                        arguments += "-m " + mimeTextBox.Text + " ";
                    }
                    foreach (string header in headersListBox.Items)
                    {
                        arguments += "-H \"" + header + "\" ";
                    }

                    isChecked = replaceHeadersCheckBox.IsChecked;
                    if (isChecked.HasValue && !isChecked.Value)
                    {
                        arguments += "-R ";
                    }

                    isChecked = strictCGICheckBox.IsChecked;
                    if (isChecked.HasValue && isChecked.Value)
                    {
                        arguments += "-C ";
                    }

                    string dir;
                    if (paths.TryGetValue("dir", out dir))
                    {
                        arguments += dir + " ";
                    }
                    arguments += "--cgi " + paths["path"];
                    break;
            }
            Console.WriteLine("oneshot " + arguments);
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.EnvironmentVariables.Add("ONESHOT_DONT_EXIT", "T");
            process.Start();
        }

        private void sendFileMode()
        {
            startButton.IsEnabled = false;
            compressionPanel.IsEnabled = false;
            compressionComboBox.SelectedIndex = -1;
            dirPanel.IsEnabled= false;
            headerPanel.IsEnabled = false;
            sourceLabel.Content = "None";
            paths.Remove("path");
            paths.Remove("saveDir");
            saveDirLabel.Content = "None";
            sourceButton.Content = "Select a file";
            headersListBox.Items.Clear();
            headersListBox.Items.Add("Content-Type: text/plain");
            downloadPanel.IsEnabled = true;
            mimePanel.IsEnabled = true;
            mainWindow.Height = 495.3;
        }

        private void sendFolderMode()
        {
            startButton.IsEnabled = false;
            compressionPanel.IsEnabled = true;
            compressionComboBox.SelectedIndex = 0;
            sourceLabel.Content = "None";
            paths.Remove("path");
            paths.Remove("saveDir");
            saveDirLabel.Content = "None";
            sourceButton.Content = "Select a folder";
            headersListBox.Items.Clear();
            headersListBox.Items.Add("Content-Type: text/plain");
            downloadCheckbox.IsChecked = true;
            downloadPanel.IsEnabled = false;
            mimePanel.IsEnabled = false;
            mainWindow.Height = 495.3;
        }

        private void sendExecutableOutputMode()
        {
            startButton.IsEnabled = false;
            compressionPanel.IsEnabled = false;
            compressionComboBox.SelectedIndex = -1;
            dirPanel.IsEnabled = true;
            paths.Remove("path");
            paths.Remove("saveDir");
            saveDirLabel.Content = "None";
            headerPanel.IsEnabled = true;
            sourceLabel.Content = "None";
            sourceButton.Content = "Select an executable";
            downloadPanel.IsEnabled = true;
            mimePanel.IsEnabled = true;
            mainWindow.Height = 495.3;
        }

        private void receiveMode()
        {
            paths.Clear();
            sourceLabel.Content = "None";
            dirLabel.Content = "None";
            mainWindow.Height = 193.0;
        }

        private void handleStart(object sender, RoutedEventArgs e)
        {
            run();
        }

        private void handleSourceSelectClick(object sender, RoutedEventArgs e)
        {
            switch (sourceTypeComboBox.SelectedIndex)
            {
                case 0:
                    this.fd = new OpenFileDialog();

                    bool? result = this.fd.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        paths["path"] = this.fd.FileName;
                        sourceLabel.Content = this.fd.FileName;
                        dirLabel.Content = "None";
                        startButton.IsEnabled = true;
                    }
                    this.fd = null;
                    break;
                case 1:
                    this.fb = new FolderBrowserDialog();

                    if (this.fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        paths["path"] = this.fb.SelectedPath;
                        sourceLabel.Content = this.fb.SelectedPath;
                        dirLabel.Content = "None";
                        startButton.IsEnabled = true;
                    }
                    this.fb = null;
                    break;
                case 2:
                    this.fd = new OpenFileDialog();

                    result = this.fd.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        paths["path"] = fd.FileName;
                        sourceLabel.Content = fd.FileName;
                        startButton.IsEnabled = true;
                    }
                    this.fd = null;
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
                paths["dir"] = "-d \"" + fb.SelectedPath + "\"";
                dirLabel.Content = fb.SelectedPath;
            }
        }

        private void sourceTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (sourceTypeComboBox.SelectedIndex)
            {
                case 0:
                    sendFileMode();
                    break;
                case 1:
                    sendFolderMode();
                    break;
                case 2:
                    sendExecutableOutputMode();
                    break;
            }
        }

        private void handleSaveDirClick(object sender, RoutedEventArgs e)
        {
            this.fb = new FolderBrowserDialog();

            if (this.fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                paths.Clear();
                paths["saveDir"] = this.fb.SelectedPath;
                saveDirLabel.Content = this.fb.SelectedPath;
                startButton.IsEnabled = true;
            }
            this.fb = null;
        }

        private void modeTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!sender.Equals(modeTabControl))
            {
                return;
            }
            switch (modeTabControl.SelectedIndex)
            {
                case 0:
                    if (mode == 0)
                    {
                        return;
                    }
                    sendFileMode();
                    sourceTypeComboBox.SelectedIndex = 0;
                    mode = 0;
                    break;
                case 1:
                    if (mode == 1)
                    {
                        return;
                    }
                    receiveMode();
                    mode = 1;
                    break;
            }
        }
    }
}
