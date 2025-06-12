using Microsoft.Maui.Controls;

namespace gorsel_programlam_odev2
{
    public partial class AyrlarPage : ContentPage
    {
        public AyrlarPage()
        {
            InitializeComponent();
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            App.ToggleTheme(e.Value); 
        }
    }
}
