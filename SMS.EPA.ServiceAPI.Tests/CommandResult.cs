namespace SMS.EPA.ServiceAPI.ClientConsole
{
    using Newtonsoft.Json;

    public class CommandResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }
    }
}
