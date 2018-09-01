using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SamsAuctions.Models
{
    [DataContract]
    public class Bid
    {
        [DataMember]
        public int BudID { get; set; }
        [DataMember]
        public int Summa { get; set; }
        [DataMember]
        public int AuktionID { get; set; }
        [DataMember]
        public string Budgivare { get; set; }
    }
}
