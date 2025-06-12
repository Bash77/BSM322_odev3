using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace gorsel_programlam_odev2
{
    public partial class WeatherPage : ContentPage
    {
        public ObservableCollection<SehirHavaDurumu> Sehirler { get; set; }

        public WeatherPage()
        {
            InitializeComponent();

            Sehirler = new ObservableCollection<SehirHavaDurumu>();
            cityListView.ItemsSource = Sehirler;
        }
        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            App.ToggleTheme(e.Value);
        }

        private async void OnAddCityClicked(object sender, EventArgs e)
        {
            string sehir = await DisplayPromptAsync("sehir:", "sehir ismi", "tamam", "iptal");

            if (!string.IsNullOrEmpty(sehir))
            {
                sehir = sehir.ToUpper(CultureInfo.CurrentCulture);
                sehir = sehir.Replace('Ç', 'C')
                             .Replace('?', 'G')
                             .Replace('?', 'I')
                             .Replace('Ö', 'O')
                             .Replace('Ü', 'U')
                             .Replace('?', 'S');

                var cityWeather = new SehirHavaDurumu() { Name = sehir };
                Sehirler.Add(cityWeather);

                cityWeather.WeatherWebView.IsVisible = true;
                cityWeather.WeatherWebView.Source = new UrlWebViewSource { Url = cityWeather.Source };
            }
        }

        private void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is SehirHavaDurumu selectedCity)
            {
                weatherWebView.IsVisible = true;

                weatherWebView.Source = new UrlWebViewSource { Url = selectedCity.Source };

                cityListView.SelectedItem = null;
            }
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;

                foreach (var cityWeather in Sehirler)
                {
                    cityWeather.WeatherWebView.Source = new UrlWebViewSource { Url = cityWeather.Source };
                }

                await DisplayAlert("basarili", "hava durumu basariyla yenilendi.", "tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
    }
}
