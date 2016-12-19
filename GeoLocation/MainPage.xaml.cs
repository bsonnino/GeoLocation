using OpenWeatherMap;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GeoLocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            InitializeLocator();
        }

        public enum RainStatus
        {
            Rain,
            Sun
        };

        private async void InitializeLocator()
        {
            var userPermission = await Geolocator.RequestAccessAsync();
            switch (userPermission)
            {
                case GeolocationAccessStatus.Allowed:
                    RainText.Text = "Updating rain status...";
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    try
                    {
                        RainText.Text = await GetRainStatus(pos) == RainStatus.Rain ?
                            "It's possible to rain, you'd better take an umbrella" :
                            "It will be a bright an shiny day, go out and enjoy";
                    }
                    catch
                    {
                        RainText.Text = "Got an error while getting weather";
                    }
                    break;

                case GeolocationAccessStatus.Denied:
                    RainText.Text = "I cannot check the weather if you don't give me the access to your location...";
                    break;

                case GeolocationAccessStatus.Unspecified:
                    RainText.Text = "I got an error while getting location permission. Please try again...";
                    break;
            }
        }

        private async Task<RainStatus> GetRainStatus(Geoposition pos)
        {
            var client = new OpenWeatherMapClient("YourApiKey");
            var weather = await client.CurrentWeather.GetByCoordinates(new Coordinates()
            {
                Latitude = pos.Coordinate.Point.Position.Latitude,
                Longitude = pos.Coordinate.Point.Position.Longitude
            });
            return weather.Precipitation.Mode == "no" ? RainStatus.Sun : RainStatus.Rain;
        }
    }
}
