using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SamsAuctions.Models
{
    [DataContract]
    public class Auction
    {
        [DataMember]
        public int AuktionID { get; set; }
        [DataMember]
        public string Titel { get; set; }
        [DataMember]
        public string Beskrivning { get; set; }
        [DataMember]
        public DateTime StartDatum { get; set; }
        [DataMember]
        public DateTime SlutDatum { get; set; }
        [DataMember]
        public int Gruppkod { get; set; }
        [DataMember]
        public int Utropspris { get; set; }
        [DataMember]
        public string SkapadAv { get; set; }

        public bool AnvandarenFarUppdatera { get; set; }

        public bool ArOppen { get; set; }
    }
}
