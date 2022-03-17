﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Authorization_Server.Models
{
    public class Endpoint
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string URI { get; set; }

        public ICollection<Resource> Resources { get; set; }
    }
}
