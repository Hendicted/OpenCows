using System;
using System.Collections.Generic;
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
using System.IO;
using Microsoft.Win32;

namespace PictureSlideshowScreensaver
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        private bool _saved = true;

        public Configuration()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private bool LoadSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureSlideshowScreensaver");
                if (key != null)
                {

                    string ass = (string)key.GetValue("ImageFolder");
                    slideInterval.Value = double.Parse((string)key.GetValue("Interval"));
                }
                else
                {
                    slideInterval.Value = 5;
                }
                _saved = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                return false;
            }


        }

        private bool SaveSettings()
        {
            try
            {
                string ass = "pics";
                if (Directory.Exists(ass))
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureSlideshowScreensaver");

                    key.SetValue("ImageFolder", ass);
                    key.SetValue("Interval", slideInterval.Value);
                    _saved = true;
                    return true;
                }
                else
                {
                    MessageBox.Show("The selected folder does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    _saved = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                _saved = false;
                return false;
            }
        }

        private void bBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //txtFolder.Text = dialog.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_saved)
            {
                if (MessageBox.Show("There are unsaved changes. Really exit the configuration?", "Unsaved changes", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        private void slideInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblInterval.Content = slideInterval.Value.ToString() + " seconds";
            _saved = false;
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveSettings())
            {
                _saved = true;
                Application.Current.Shutdown();
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            _saved = true;
            Application.Current.Shutdown();
        }

        private void txtFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            _saved = false;
        }
        
    }
}
