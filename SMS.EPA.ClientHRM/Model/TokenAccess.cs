namespace SMS.EPA.ClientHrm.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

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

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Ok.")]
        [JsonProperty("token_type")]
        public string Type { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }

        public DateTime GeneratedDate
        {
            get
            {
                return DateTime.ParseExact(Issued, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
            }
        }

        public DateTime ExpireDate
        {
            get
            {
                return DateTime.ParseExact(Expires, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Value) && ExpireDate > DateTime.UtcNow;
        }
    }
}