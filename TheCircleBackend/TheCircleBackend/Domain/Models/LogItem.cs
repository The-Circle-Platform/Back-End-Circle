﻿using System.ComponentModel.DataAnnotations;

namespace TheCircleBackend.Domain.Models
{
    public class LogItem
    {
        public int Id { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string Ip { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Action { get; set; }
    }
}
