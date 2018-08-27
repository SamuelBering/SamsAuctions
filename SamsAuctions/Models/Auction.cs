using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamsAuctions.Models
{
    public class Auction
    {
        public int AuktionID { get; set; }
        public string Titel { get; set; }
        public string Beskrivning { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime SlutDatum { get; set; }
        public int Gruppkod { get; set; }
        public int Utropspris { get; set; }
        public string SkapadAv { get; set; }
    }
}
