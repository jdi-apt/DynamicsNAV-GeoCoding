using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace DynamicsNAVMaps
{
    public class GoogleMaps   
    {
        private HttpClient client = new HttpClient();
        private string apikey;
        private string requestUrl = "https://maps.googleapis.com/maps/api/geocode/json?";


        public GoogleMaps(string apikey)
        {
            this.apikey = apikey;
        }

        private string BuildUrl(string country, string location, string address,string plz)
        {
            string url = null;
       
            url = requestUrl + "address=" + country + ","+ location + "," + address + ","  + plz + "&key=" + apikey;

            return url;
        }
        public GoogleMapsJSON MakeRequest(string country, string city, string address, string plz)
        {
            GoogleMapsJSON bingjson = null;

            string request = BuildUrl(country,address, city, plz);

            var task = client.GetAsync(request)
                .ContinueWith((taskwithresponse) =>
                {
                    HttpResponseMessage response = taskwithresponse.Result;

                    Task<string> jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();
                    bingjson = JsonConvert.DeserializeObject<GoogleMapsJSON>(jsonString.Result);
                });
            task.Wait();

            return bingjson;
        }

        #region GoogleMapsJSON
        public class AddressComponent
        {

            [JsonProperty("long_name")]
            public string long_name { get; set; }

            [JsonProperty("short_name")]
            public string short_name { get; set; }

            [JsonProperty("types")]
            public IList<string> types { get; set; }
        }

        public class Location
        {

            [JsonProperty("lat")]
            public double lat { get; set; }

            [JsonProperty("lng")]
            public double lng { get; set; }
        }

        public class Northeast
        {

            [JsonProperty("lat")]
            public double lat { get; set; }

            [JsonProperty("lng")]
            public double lng { get; set; }
        }

        public class Southwest
        {

            [JsonProperty("lat")]
            public double lat { get; set; }

            [JsonProperty("lng")]
            public double lng { get; set; }
        }

        public class Viewport
        {

            [JsonProperty("northeast")]
            public Northeast northeast { get; set; }

            [JsonProperty("southwest")]
            public Southwest southwest { get; set; }
        }

        public class Geometry
        {

            [JsonProperty("location")]
            public Location location { get; set; }

            [JsonProperty("location_type")]
            public string location_type { get; set; }

            [JsonProperty("viewport")]
            public Viewport viewport { get; set; }
        }

        public class Result
        {

            [JsonProperty("address_components")]
            public IList<AddressComponent> address_components { get; set; }

            [JsonProperty("formatted_address")]
            public string formatted_address { get; set; }

            [JsonProperty("geometry")]
            public Geometry geometry { get; set; }

            [JsonProperty("place_id")]
            public string place_id { get; set; }

            [JsonProperty("types")]
            public IList<string> types { get; set; }
        }

        public class GoogleMapsJSON
        {
            public decimal GetDecimalLatitude()
            {
                decimal latitude;
                string strLatitude = this.results[0].geometry.location.lat.ToString();
                Decimal.TryParse(strLatitude, out latitude);
                return latitude;
            }

            public decimal GetDecimalLongitude()
            {
                decimal longitude;
                string strLongitude = this.results[0].geometry.location.lng.ToString();

                Decimal.TryParse(strLongitude, out longitude);
                return longitude;
            }

            public string GetLatitude()
            {
                try
                {
                    return this.results[0].geometry.location.lat.ToString();
                }
                catch
                {
                    return null;
                }
                
            }
            public string GetLongitude()
            {
                try
                {
                    return this.results[0].geometry.location.lng.ToString();
                }
                catch
                {
                    return null;
                }
            }


            [JsonProperty("results")]
            public IList<Result> results { get; set; }

            [JsonProperty("status")]
            public string status { get; set; }
        }
        #endregion
    }
}
