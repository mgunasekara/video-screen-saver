using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoScreenSaver
{
    /// <summary>
    /// Interaction logic for Configurations.xaml
    /// </summary>
    public partial class Configurations : Window
    {
        private Dictionary<string, string> settings = new Dictionary<string, string>();
        private string settingsDir;
        private string settingFile;

        public Configurations()
        {
            settingsDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoScreenSaver");
            settingFile = System.IO.Path.Combine(settingsDir, "settings.ini");
            InitializeComponent();

            if (File.Exists(settingFile))
            {
                File.ReadAllLines(settingFile);
                settings = File.ReadAllLines(settingFile).Select(c => new KeyValuePair<string, string>(c.Split('|')[0], c.Split('|')[1])).ToDictionary(c => c.Key, c => c.Value);
                txtUrl.Text = settings.FirstOrDefault(c => c.Key.Equals("file")).Value;
                cmbStretch.Text = settings.FirstOrDefault(c => c.Key.Equals("fill")).Value ?? "Fill";
                cmbDisplays.Text = settings.FirstOrDefault(c => c.Key.Equals("screens")).Value ?? "All";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(settingsDir);
            File.WriteAllLines(settingFile, new List<string>() { "file|" + (File.Exists(txtUrl.Text) ? txtUrl.Text : string.Empty), "fill|" + cmbStretch.Text, "screens|" + cmbDisplays.Text });
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4;*.m4v;*.mov;*.asf;*.avi;*.wmv;*.m2ts;*.3g2;*.3gp2;*.3gpp)|*.mp4;*.m4v;*.mov;*.asf;*.avi;*.wmv;*.m2ts;*.3g2;*.3gp2;*.3gpp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                txtUrl.Text = openFileDialog.FileName;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtUrl.Text = string.Empty;
        }
    }
}
