namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Services;

    public class EPASummaryImportModel
    {
        public int Id { get; set; }

        public float OriginFinalScore { get; set; }

        public string FinalScore
        {
            get
            {
                return EncodeService.Encode(OriginFinalScore.ToString());
            }
        }
    }
}