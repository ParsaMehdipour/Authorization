using System;
using System.ComponentModel.DataAnnotations;

namespace Endpoint.Site.Models
{
    public class Car
    {
        [Key]
        public long Id { get; set; }
        public string Brand { get; set; }
        public string Country { get; set; }
        public string Model { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
