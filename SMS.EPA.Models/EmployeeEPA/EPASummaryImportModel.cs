namespace SMS.EPA.Model
{
    using System.Globalization;
    using Core.Services;

    public class EPASummaryImportModel
    {
        public int Id { get; set; }

        public float OriginFinalScore { get; set; }

        public string FinalScore
        {
            get
            {
                ICryptographyService cryptographyService = new CryptographyService();
                return cryptographyService.Encrypt(OriginFinalScore.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}