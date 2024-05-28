namespace SMS.EPA.Model.API
{
    using System;

    [Serializable]
    public class Photo
    {
        public string Code { get; set; }

        public byte[] PhotoData { get; set; }
    }
}