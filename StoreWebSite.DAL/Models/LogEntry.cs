using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StoreWebSite.DAL.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
