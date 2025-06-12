using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace gorsel_programlam_odev2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        public static void ToggleTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                // Switch to dark mode
                Application.Current.Resources["BackgroundColor"] = Application.Current.Resources["BackgroundColorDark"];
                Application.Current.Resources["BackgroundColor"] = Colors.Black;
                Application.Current.Resources["TextColor"] = Colors.White;
                // ... other color updates for dark mode
            }
            else
            {
                // Switch to light mode
                Application.Current.Resources["BackgroundColor"] = Application.Current.Resources["BackgroundColorLight"];
                Application.Current.Resources["BackgroundColor"] = Colors.White;
                Application.Current.Resources["TextColor"] = Colors.Black;
                // ... other color updates for light mode
            }
        }
    }
}
