namespace SMS.EPA.ClientHrm.Model
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    public class TokenAccess
    {
        public TokenAccess()
        {
            Value = string.Empty;
            Type = string.Empty;
            ExpiresIn = 0;
        }

        [JsonProperty("access_token")]
        public string Value { get; set; }

        [JsonProperty("token_type")]
        public string Type { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }

        public DateTime GeneratedDate => DateTime.ParseExact(Issued, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);

        public DateTime ExpireDate => DateTime.ParseExact(Expires, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);

        public bool IsValid()
        {
#pragma warning disable S6566 // Use "DateTimeOffset" instead of "DateTime"
            return !string.IsNullOrEmpty(Value) && ExpireDate > DateTime.UtcNow;
#pragma warning restore S6566 // Use "DateTimeOffset" instead of "DateTime"
        }
    }
}