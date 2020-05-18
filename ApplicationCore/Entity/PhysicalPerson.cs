using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class PhysicalPerson
    {
        public int IdPerson { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string IdentificationNumber { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }

        public virtual Client IdPersonNavigation { get; set; }
    }
}
