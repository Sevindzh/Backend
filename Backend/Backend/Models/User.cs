using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UName { get; set; }
        public string USurame { get; set; }
        public string UEmail { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsAdmin => UEmail == "admin@vip.com";
        [JsonIgnore]
        public ICollection<DesignProject> Projects { get; set; }
    }
}
