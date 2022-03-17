using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Authorization_Server.Data;

namespace Authorization_Server.Models
{
    public class Operation
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string URI { get; set; }

        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
        public ICollection<Authorization> Authorizations { get; set; }
    }
}