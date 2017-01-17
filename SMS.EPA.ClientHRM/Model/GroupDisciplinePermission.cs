namespace SMS.EPA.ClientHrm.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Ok.")]
    public class GroupDisciplinePermission
    {
        public int TeamId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}