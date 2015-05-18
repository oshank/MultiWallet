using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MultiWallet.Resources;
using System.IO.IsolatedStorage;
using BlockIo;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;


namespace MultiWallet
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string settingsAppLaunched = "appLaunched";
        //private string localBalance = "";

        // Array set ups for default currency with list pickers
        private static string[] defaultBitcoin = {"Bitcoin","Litecoin","Dogecoin"};
        private static string[] defaultLitecoin = { "Litecoin", "Bitcoin", "Dogecoin" };
        private static string[] defaultDogecoin = { "Dogecoin", "Bitcoin", "Litecoin" };

        // Default set up, will change when a new default currency is set
        private static string[] defaultCurrencyArray = defaultBitcoin;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Check to see if First Run

            if (IsFirstLaunch() || global_methods.AreKeysSet() == 0)
            {
                global_methods.DisplayMessage("Please Set API Keys", "First Launch");
                

                // Change View to set API keys
                global_methods.SetDefaultCurrency("Bitcoin");
                
                HomeLoaded.Visibility = Visibility.Collapsed;
                SettingsLoaded.Visibility = Visibility.Visible;
                


            }
            else
            {
                // Get and Set Balances before changing to Home Screen

                // get Deflaut Currency balance
                // get balance for default currency (Passing APIKey for Default Currency)
                
                var blockioClient = new BlockIoClient(global_methods.ReadAPIKey(global_methods.GetDefaultCurrency()));

                setBalance(global_methods.GetDefaultCurrency());
                
                // Change View to Home

                SettingsLoaded.Visibility = Visibility.Collapsed;
                HomeLoaded.Visibility = Visibility.Visible;

            }
            

        }

        // Main Method for setting balance with default currency on app loading
        private void setBalance()
        {
            HomeLoadingBar.Visibility = Visibility.Visible;
            setBalance(global_methods.GetDefaultCurrency());
            HomeLoadingBar.Visibility = Visibility.Collapsed;
        }

        // Helper to setBalance, also able to change currency when using the selection method
        private async void setBalance(string currencyOverride)
        {
            HomeLoadingBar.Visibility = Visibility.Visible;
            string currency = global_methods.GetDefaultCurrency();

            // Check to see if currency override is different then default currency
            if (!currencyOverride.Equals(currency))
            {
                currency = currencyOverride;
            }

            var blockioClient = new BlockIoClient(global_methods.ReadAPIKey(currency));

            var getBalance = await blockioClient.GetBalance();
            var balance = getBalance.AvailableBalance;
            // Get default currency from global methods
            switch (currency){
                case "Bitcoin":
                    BalanceBlock.Text = balance + " ฿";
                    break;
                case "Litecoin":
                    BalanceBlock.Text = balance + " Ł";
                    break;
                case "Dogecoin":
                    BalanceBlock.Text = balance + " Ð";
                    break;
            }

            // Get QR code for default address
            // TODO:
            // Possibly save qr for offline access???
            // 

            var blockioGet = await blockioClient.GetAddressByLabel("default");
            var defaultAdd = blockioGet.Value;
            string getQR = "https://chart.googleapis.com/chart?cht=qr&chs=200x200&chl=" + defaultAdd;

            QRImage.Source = new BitmapImage(new Uri(getQR));

            HomeLoadingBar.Visibility = Visibility.Collapsed;
            
            
        }

        // Check if first run
        private static bool IsFirstLaunch()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            
            return !(settings.Contains(settingsAppLaunched));
        }

        // Run after first launch
        private static void Launched()
        {
            if (IsFirstLaunch())
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Add(settingsAppLaunched, true);
                settings.Save();
            }
        }

        // Changed Currency on home page
        private void CurrencyPickerHome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CurrencyPickerHome == null);
            //setBalance();

            else if (CurrencyPickerHome.SelectedIndex.Equals(0))
                setBalance(defaultCurrencyArray[0]);

            else if (CurrencyPickerHome.SelectedIndex.Equals(1))
                setBalance(defaultCurrencyArray[1]);

            else if (CurrencyPickerHome.SelectedIndex.Equals(2))
                setBalance(defaultCurrencyArray[2]);
            
                

        }


        // Bitcoin API box lost focus, only used in first run settings page

        private async void BitcoinAPIBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var blockClient = new BlockIoClient(BitcoinAPIBox.Text);
            string balString = "";

            try
            {
                var bal = await blockClient.GetBalance();
                balString = bal.AvailableBalance.ToString();
                
            }
            catch
            {
                balString = "error";
                global_methods.DisplayMessage("Please try again!", "Invalid Bitcoin API Key");
                
            }

            
            
        }

        // Litecoin API box lost focus, only used in first run settings page

        private async void LitecoinAPIBox_LostFocus(object sender, RoutedEventArgs e)
        {

            var blockClient = new BlockIoClient(LitecoinAPIBox.Text);
            string balString = "";

            try
            {
                var bal = await blockClient.GetBalance();
                balString = bal.AvailableBalance.ToString();

            }
            catch
            {
                balString = "error";
                global_methods.DisplayMessage("Please try again!", "Invalid Litecoin API Key");

            }

        }

        // Dogecoin API box lost focus, only used in first run settings page

        private async void DogecoinAPIBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var blockClient = new BlockIoClient(DogecoinAPIBox.Text);
            string balString = "";

            try
            {
                var bal = await blockClient.GetBalance();
                balString = bal.AvailableBalance.ToString();

            }
            catch
            {
                balString = "error";
                global_methods.DisplayMessage("Please try again!", "Invalid Dogecoin API Key");

            }
        }

        private async void SaveButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var bitClient = new BlockIoClient(BitcoinAPIBox.Text);
            var liteClient = new BlockIoClient(LitecoinAPIBox.Text);
            var dogeClient = new BlockIoClient(DogecoinAPIBox.Text);

            bool bitPass = false;
            bool litePass = false;
            bool dogePass = false;


            // Try Bitcoin

            try
            {
                var bal = await bitClient.GetBalance();
                //string balString = bal.AvailableBalance.ToString();


                bitPass = true;
            }
            catch
            {
                global_methods.DisplayMessage("Incorrect Bitcoin API Key!", "Error!");

                BitcoinAPIBox.Text = "";
            }

            // Try Litecoin

            try
            {
                var bal = await liteClient.GetBalance();


                litePass = true;
            }
            catch
            {
                global_methods.DisplayMessage("Incorrect Litecoin API Key!", "Error!");
                LitecoinAPIBox.Text = "";
            }

            // Try Dogecoin

            try
            {
                var bal = await dogeClient.GetBalance();



                dogePass = true;
            }
            catch
            {
                global_methods.DisplayMessage("Incorrect Dogecoin API Key!", "Error!");
                DogecoinAPIBox.Text = "";
            }

            if (bitPass && litePass && dogePass)
            {
                global_methods.DisplayMessage("API Keys saved successfully!", "Saved!");

                global_methods.SaveAPIKey("Bitcoin", BitcoinAPIBox.Text);
                global_methods.SaveAPIKey("Litecoin", LitecoinAPIBox.Text);
                global_methods.SaveAPIKey("Dogecoin", DogecoinAPIBox.Text);

                // change view for home view
                SettingsLoaded.Visibility = Visibility.Collapsed;
                HomeLoaded.Visibility = Visibility.Visible;
                Launched();

                setBalance();
                

            }
        }

        private void SettingsListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrencyPickerHome == null)
                global_methods.SetDefaultCurrency("Bitcoin");
            else if (CurrencyPickerHome.SelectedIndex.Equals(0))
                global_methods.SetDefaultCurrency("Bitcoin");
            else if (CurrencyPickerHome.SelectedIndex.Equals(1))
                global_methods.SetDefaultCurrency("Litecoin");
            else if (CurrencyPickerHome.SelectedIndex.Equals(2))
                global_methods.SetDefaultCurrency("Dogecoin");
        }

        
            

        
    }
}