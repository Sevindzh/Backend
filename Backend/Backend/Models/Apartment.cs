using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Apartment
    {

        public int ApartmentID { get; set; }
        public int InBldngID { get; set; }
        public int RoomsN { get; set; }
        public double GeneralArea { get; set; }
        public double LivingArea { get; set; }
        public double KitchenArea { get; set; }
        
        public ICollection<DesignProject> ProjectsForApt { get; set; }
        [JsonIgnore]
        public Building Bldng { get; set; }

        public override string ToString()
        {
            return $" Apartment {RoomsN} rooms, {GeneralArea} m^2 (There are {ProjectsForApt.Count()} projrects)";
        }
    }
}
