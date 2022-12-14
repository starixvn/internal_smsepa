namespace SMS.EPA.AutoJob.Models
{
    using System;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using SMS.EPA.AutoJob.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class UserMapping
    {
        public int UserId { get; set; }

        public int RefId { get; set; }

        public int? NewUserId { get; set; }

        [XmlIgnore]
        public DateTime? ModifiedDate { get; set; }

        [JsonIgnore]
        public string ModifiedDateXml => ModifiedDate.HasValue ? ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss:fff") : string.Empty;
    }
}