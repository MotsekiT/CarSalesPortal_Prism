﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarSalesPortal.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string? Name { get; set; }
        [DisplayName("Item No")]
        [Range(1, 100, ErrorMessage = "The field Item Number must be between 1 - 100.")]
        public int ItemNumber { get; set; }





    }
}
