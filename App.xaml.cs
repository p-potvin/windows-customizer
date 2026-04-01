using Microsoft.UI.Xaml;
using System.IO;
using System;

namespace WindowsCustomizer
{
    public partial class App : Application
    {
        private Window m_window;

        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("crash.log", e.Exception.ToString());
            e.Handled = true;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
            catch (Exception ex)
            {
                File.WriteAllText("launch_crash.log", ex.ToString());
            }
        }
    }
}