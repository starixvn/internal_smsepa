using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.Model
{
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

        public bool IsValid()
        {
            return ExpiresIn > 0 && !string.IsNullOrEmpty(Value);
        }
    }
}
