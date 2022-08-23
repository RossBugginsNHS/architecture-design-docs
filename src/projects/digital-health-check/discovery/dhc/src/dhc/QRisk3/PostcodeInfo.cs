using System;
using Newtonsoft.Json;

namespace dhc
{
    //The full postcode info for a requested postcode.
    public class PostcodeInfo
    {
        //The HTTP status response from Postcodes.IO.
        [JsonProperty("status")]
        public int Status;

        //If an error is returned, it is held here.
        [JsonProperty("error")]
        public string Error;

        //The actual result of the API call.
        [JsonProperty("result")]
        public PostcodeResult Result;
    }

    public class PostcodeResult
    {
        //The returned postcode.
        [JsonProperty("postcode")]
        public string Postcode;

        //The quality of the postcode.
        [JsonProperty("quality")]
        public int Quality;

        //Eastings, northings.
        [JsonProperty("eastings")]
        public int Eastings;
        [JsonProperty("northings")]
        public int Northings;

        //Country the postcode is in, region.
        [JsonProperty("country")]
        public string Country;
        [JsonProperty("region")]
        public string Region;

        //The NHS domain of the postcode, primary care trust.
        [JsonProperty("nhs_ha")]
        public string NHSDomain;
        [JsonProperty("primary_care_trust")]
        public string PrimaryCareTrust;

        //Longitude, latitude.
        [JsonProperty("longitude")]
        public double Longitude;
        [JsonProperty("latitude")]
        public double Latitude;

        //The european electoral region of the postcode.
        [JsonProperty("european_electoral_region")]
        public string EuropeanElectoralRegion;

        //LSOA, MSOA
        public string LSOA, MSOA;

        //Postcode incode/outcode (region and subregion).
        [JsonProperty("incode")]
        public string Incode;
        [JsonProperty("outcode")]
        public string Outcode;

        //Parliamentary/council stuff.
        [JsonProperty("parliamentary_constituency")]
        public string ParliamentaryConstituency;
        [JsonProperty("admin_district")]
        public string AdminDistrict;
        [JsonProperty("parish")]
        public string Parish;
        [JsonProperty("admin_ward")]
        public string AdminWard;
        [JsonProperty("admin_county")]
        public string AdminCounty;
        [JsonProperty("ced")]
        public string CED;
        [JsonProperty("ccg")]
        public string CCG;
        [JsonProperty("nuts")]
        public string NUTS;

        //Codes for this postcode.
        [JsonProperty("codes")]
        public PostcodeCodes Codes;
    }

    //The set of assigned codes for this postcode.
    public class PostcodeCodes
    {
        [JsonProperty("parliamentary_constituency")]
        public string ParliamentaryConstituency;
        [JsonProperty("admin_district")]
        public string AdminDistrict;
        [JsonProperty("parish")]
        public string Parish;
        [JsonProperty("admin_ward")]
        public string AdminWard;
        [JsonProperty("admin_county")]
        public string AdminCounty;
        [JsonProperty("ced")]
        public string CED;
        [JsonProperty("ccg")]
        public string CCG;
        [JsonProperty("nuts")]
        public string NUTS;
        [JsonProperty("lsoa")]
            public string LSOA;
       
    }
}