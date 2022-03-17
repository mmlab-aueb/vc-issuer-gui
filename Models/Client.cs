using Authorization_Server.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Authorization_Server.Models
{
    public class Client
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ICollection<Authorization> Authorizations { get; set; }
    }

}
