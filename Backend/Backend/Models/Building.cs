using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Building
    {
        public int BuildingID { get; set; }
        public string CodeName { get; set; }
        public int FloorsN { get; set; }
        public string DemolitionPerspective { get; set; }
        public double CeilingH { get; set; }
        public string Photo { get; set; }
        public string Review { get; set; }

        public ICollection<Apartment> Apts { get; set; }
        public string GetInfo()
        {
            string AptTypes= "\n";
            foreach (Apartment apt in Apts)
                AptTypes = AptTypes+apt.ToString()+"\n";
            return $" Building {CodeName} with {Apts.Count()} apartment options:" + AptTypes;
        }
    }
}
