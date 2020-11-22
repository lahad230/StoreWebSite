using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace StoreWebSite.MVC.Models
{
    //viewmodel for creating new Product form
    public class CreateAdViewModel
    {
        public CreateAdViewModel()
        {

        }

        public CreateAdViewModel(Product product)
        {
            ProductId = product.Id;
            Title = product.Title;
            ShortDescription = product.ShortDescription;
            LongDescription = product.LongDescription;
            Date = product.Date;
            Price = product.Price;
            Picture1 = product.Picture1;
            Picture2 = product.Picture2;
            Picture3 = product.Picture3;
        }
        
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Title must be filled")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "Shrot description")]
        [Required(ErrorMessage = "Short description must be filled")]
        [MaxLength(500)]
        public string ShortDescription { get ; set ; }

        [Display(Name = "Long description")]
        [Required(ErrorMessage = "Long description must be filled")]
        [MaxLength(4000, ErrorMessage = "Last name can't be longer than 50 characters")]
        public string LongDescription { get ; set ; }

        public DateTime Date { get ; set ; }

        [Required(ErrorMessage = "Price must be filled")]
        [DecimalValidation]
        public decimal Price { get ; set ; }

        [Display(Name = "Picture 1")]
        public byte[] Picture1 { get ; set ; }

        [Display(Name = "Picture 2")]
        public byte[] Picture2 { get ; set ; }

        [Display(Name = "Picture 3")]
        public byte[] Picture3 { get ; set ; }
    }
}
