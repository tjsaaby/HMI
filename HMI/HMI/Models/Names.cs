using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HMI.Models
{
    public class Names
    {
        [Key]
        public string NameID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
