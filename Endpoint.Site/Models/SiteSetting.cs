using System;
using System.ComponentModel.DataAnnotations;

namespace Endpoint.Site.Models
{
    public class SiteSetting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? LatsTimeChanged { get; set; }
    }
}
