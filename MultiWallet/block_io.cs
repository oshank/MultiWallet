using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MultiWallet
{
    class block_io
    {

        // Block.io base API Url
        public const string baseUrl = "https://block.io/api/v2/";
        private string localBalance = "";


        // Get current account balance for specific api
        internal string getBalance(string apiKey)
        {

            setLocalBalance(apiKey);
            return this.localBalance;

            
        }

        // Helper method for getBalance()
        private async void setLocalBalance(string apiKey)
        {
            HttpClient client = new HttpClient();
            string url = baseUrl + "get_balance/?api_key=" + apiKey;

            try
            {
                string result = await client.GetStringAsync(url);


                block_io_root apiData = JsonConvert.DeserializeObject<block_io_root>(result);


                localBalance = apiData.data.available_balance;

            }
            catch
            {
                localBalance = "error";
            }
        }

        // Get a new address from the api provided
        internal static string getNewAddress()
        {
            return "";
        }

        // Get the address with the ID of 0
        internal static string getOldAddress()
        {
            return "";
        }

        // Get the last 5 Transactions for the entire account
        internal static string getLast5Transactions()
        {
            return "";
        }

    }

    public class block_io_data
    {
        public string network { get; set; }
        public string available_balance { get; set; }
        public string pending_received_balance { get; set; }
    }

    public class block_io_root
    {
        public string status { get; set; }
        public block_io_data data { get; set; }
    }
}
