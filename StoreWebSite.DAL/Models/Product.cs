using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StoreWebSite.DAL.Models
{
    public class Product
    {
        public int Id { get; set; }

        public virtual User Owner { get; set; }

        public virtual User Buyer { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public byte[] Picture1 { get; set; }

        public byte[] Picture2 { get; set; }

        public byte[] Picture3 { get; set; }

        public bool IsPurchased { get; set; }
    }
}
