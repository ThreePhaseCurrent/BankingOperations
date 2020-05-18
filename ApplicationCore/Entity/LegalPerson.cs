using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class LegalPerson
    {
        public int IdEdrpou { get; set; }
        public string Name { get; set; }
        public string OwnershipType { get; set; }
        public string Director { get; set; }

        public virtual Client IdEdrpouNavigation { get; set; }
    }
}
