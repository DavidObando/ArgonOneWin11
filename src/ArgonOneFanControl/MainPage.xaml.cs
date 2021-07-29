using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace ArgonOneFanControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable
    {
        private I2cDevice i2cBus;
        private byte selectedSpeed = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            if (i2cBus != null)
            {
                i2cBus.Dispose();
                i2cBus = null;
            }
        }

        private async Task InitializeConnectionAsync()
        {
            if (i2cBus == null)
            {
                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);
                if (devices != null && devices.Count > 0)
                {
                    var address = 0x1a;
                    var i2cConnectionSettings = new I2cConnectionSettings(address);
                    i2cBus = await I2cDevice.FromIdAsync(devices[0].Id, i2cConnectionSettings);
                }
            }
        }

        private async void UpdateButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            try
            {
                await InitializeConnectionAsync();
                if (i2cBus != null)
                {
                    i2cBus.Write(new byte[] { selectedSpeed });
                }
            }
            catch (Exception ex)
            {
                // just print for diagnostics
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                UpdateButton.IsEnabled = true;
            }
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender == OnFifty)
            {
                selectedSpeed = 50;
            }
            else if (sender == OnSeventyFive)
            {
                selectedSpeed = 75;
            }
            else if (sender == OnOneHundred)
            {
                selectedSpeed = 100;
            }
            else
            {
                selectedSpeed = 0;
            }
        }
    }
}
