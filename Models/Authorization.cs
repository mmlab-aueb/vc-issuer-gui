using Authorization_Server.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization_Server.Models
{
    public class Authorization
    {
        [Key]
        public int ID { get; set; }

        public int ClientID { get; set; }
        public int OperationID { get; set; }
        public virtual Operation Operation { get; set; }
        public Client Client { get; set; }
    }
}
