using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace lockin.core.Models
{
    public class Location
    {
      [Key]  [Required]
        public int LocationId { get; set; }
        [Required]
  
        public string Country {  get; set; }
        [Required]
        public string City { get; set; }

    }
}
