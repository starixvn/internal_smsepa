namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class UpdateProcessSettingPeriodCriteria : CriteriaBase
    {
        private const string ProcessItemXmTemplate = "<ProcessSetting ProcessId='{0}' Duration='{1}' NoticeAtBegin='{2}' RemindDay='{3}'></ProcessSetting>";

        public int PeriodId { get; set; }

        public string ProcessSettings => ConvertProcessSettingToXml();

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<FlowModel> Processes { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && ModifiedBy > 0 && !string.IsNullOrEmpty(ProcessSettings);
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_UpdateProcessSetting";
        }

        private string ConvertProcessSettingToXml()
        {
            string processXml = string.Empty;

            if (Processes != null)
            {
                var processXmlBuilder = new StringBuilder(Constants.BeginRootTag);

                foreach (FlowModel item in Processes)
                {
                    processXmlBuilder.AppendFormat(ProcessItemXmTemplate, item.ProcessId, item.Duration, item.HasSendMail, item.RemindDay);
                }

                processXmlBuilder.Append(Constants.EndRootTag);
                processXml = processXmlBuilder.ToString();
            }

            return processXml;
        }
    }
}