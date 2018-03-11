using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DynamicsNAVMaps

{
    public class BingMaps
    {
        private HttpClient client = new HttpClient();
        private string apikey;
        private string requestUrl = "http://dev.virtualearth.net/REST/v1/Locations?";
        
        public BingMaps(string apikey)
        {
            this.apikey = apikey;
        }
        private string BuildUrl(string coutryRegion, string address, string location, string plz)
        {
            string url = null;

            string qAddress = null;
            string qLocation = null;
            string qPlz = null;
            string qCountryRegion = null;

            if (!String.IsNullOrEmpty(coutryRegion))
            {
                qCountryRegion = "&countryRegion=" + coutryRegion;
            }

            if (!String.IsNullOrEmpty(location))
            {
                qLocation = "&locality=" + location;
            }

            if (!String.IsNullOrEmpty(address))
            {
                qAddress = "&addressLine=" + address;
            }

            if (!String.IsNullOrEmpty(plz))
            {
                qPlz = "&postalcode=" + plz;
            }

            url = requestUrl + qCountryRegion + qLocation + qAddress + qPlz + "&key=" + apikey;

            return url;
        }
        public BingMapsJSON MakeRequest(string country, string city, string address, string plz)
        {
            BingMapsJSON bingjson = null;

            string request = BuildUrl(country, address,city,plz);


            var task = client.GetAsync(request)
                .ContinueWith((taskwithresponse) =>
                {
                    HttpResponseMessage response = taskwithresponse.Result;
                 
                    Task<string> jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();
                    bingjson = JsonConvert.DeserializeObject<BingMapsJSON>(jsonString.Result);
                });
            task.Wait();

            return bingjson;
        }

        private void ExceptionHttpResponseMessage(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        #region BingMapsJSON
        public class Point
        {

            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("coordinates")]
            public IList<double> coordinates { get; set; }
        }

        public class Address
        {

            [JsonProperty("addressLine")]
            public string addressLine { get; set; }

            [JsonProperty("adminDistrict")]
            public string adminDistrict { get; set; }

            [JsonProperty("adminDistrict2")]
            public string adminDistrict2 { get; set; }

            [JsonProperty("countryRegion")]
            public string countryRegion { get; set; }

            [JsonProperty("formattedAddress")]
            public string formattedAddress { get; set; }

            [JsonProperty("locality")]
            public string locality { get; set; }

            [JsonProperty("postalCode")]
            public string postalCode { get; set; }
        }

        public class GeocodePoint
        {

            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("coordinates")]
            public IList<double> coordinates { get; set; }

            [JsonProperty("calculationMethod")]
            public string calculationMethod { get; set; }

            [JsonProperty("usageTypes")]
            public IList<string> usageTypes { get; set; }
        }

        public class Resource
        {

            [JsonProperty("__type")]
            public string __type { get; set; }

            [JsonProperty("bbox")]
            public IList<double> bbox { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("point")]
            public Point point { get; set; }

            [JsonProperty("address")]
            public Address address { get; set; }

            [JsonProperty("confidence")]
            public string confidence { get; set; }

            [JsonProperty("entityType")]
            public string entityType { get; set; }

            [JsonProperty("geocodePoints")]
            public IList<GeocodePoint> geocodePoints { get; set; }

            [JsonProperty("matchCodes")]
            public IList<string> matchCodes { get; set; }
        }

        public class ResourceSet
        {

            [JsonProperty("estimatedTotal")]
            public int estimatedTotal { get; set; }

            [JsonProperty("resources")]
            public IList<Resource> resources { get; set; }
        }

        public class BingMapsJSON
        {
            public decimal GetDecimalLatitude()
            {
                decimal latitude;
                string strLatitude = this.resourceSets[0].resources[0].point.coordinates[0].ToString();

                Decimal.TryParse(strLatitude,out latitude);
                return latitude;
   
            }

            public decimal GetDecimalLongitude()
            {
                decimal longitude;
                string strLongitude = this.resourceSets[0].resources[0].point.coordinates[1].ToString();

                Decimal.TryParse(strLongitude, out longitude);
                return longitude;
            }


            public string GetLatitude()
            {
                try
                {
                    return this.resourceSets[0].resources[0].point.coordinates[0].ToString();
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
                    return this.resourceSets[0].resources[0].point.coordinates[1].ToString();
                }
                catch
                {
                    return null;
                }
            }

            [JsonProperty("authenticationResultCode")]
            public string authenticationResultCode { get; set; }

            [JsonProperty("brandLogoUri")]
            public string brandLogoUri { get; set; }

            [JsonProperty("copyright")]
            public string copyright { get; set; }

            [JsonProperty("resourceSets")]
            public IList<ResourceSet> resourceSets { get; set; }

            [JsonProperty("statusCode")]
            public int statusCode { get; set; }

            [JsonProperty("statusDescription")]
            public string statusDescription { get; set; }

            [JsonProperty("traceId")]
            public string traceId { get; set; }
        }
        #endregion

        
    }
}