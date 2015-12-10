// ----------------------------------------------------------------------
// <copyright file="UpdatePeriodProcessSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/22/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.FlowManagement;

    public class UpdatePeriodProcessSettingCriteria : CriteriaBase
    {
        private const string ProcessItemXmTemplate = "<ProcessSetting ProcessId='{0}' Duration='{1}' NoticeAtBegin='{2}' RemindDay='{3}'></ProcessSetting>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int PeriodId { get; set; }

        public string ProcessSettings { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<FlowModel> Processes { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && !string.IsNullOrWhiteSpace(ProcessSettings);
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_UpdateProcessSetting";
        }

        public string ConvertViolationItemToXml()
        {
            string processXml = string.Empty;

            if (Processes != null)
            {
                var processXmlBuilder = new StringBuilder(BeginRootTag);

                foreach (var item in Processes)
                {
                    processXmlBuilder.AppendFormat(ProcessItemXmTemplate, item.ProcessId, item.Duration, item.HasSendMail, item.RemindDay);
                }

                processXml = processXmlBuilder.Append(EndRootTag).ToString();
            }

            return processXml;
        }
    }
}
