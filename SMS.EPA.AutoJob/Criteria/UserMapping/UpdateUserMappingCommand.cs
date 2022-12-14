namespace SMS.EPA.AutoJob.Criteria
{
    public class UpdateUserMappingCommand : BaseNonQueryCommand
    {
        public string UserMappingXml { get; set; }

        public override string GetSettingKey() => "Sync_UserMapping";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UserMappingXml);
    }
}