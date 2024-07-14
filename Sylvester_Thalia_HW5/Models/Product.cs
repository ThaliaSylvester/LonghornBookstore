using System.ComponentModel.DataAnnotations;
using Sylvester_Thalia_HW5.Models;

namespace Sylvester_Thalia_HW5.Models
{
    public enum ProductType
    {
        [Display(Name = "New Hardback")] New_Hardback,
        [Display(Name = "New Paperback")] New_Paperback,
        [Display(Name = "Used Hardback")] Used_Hardback,
        [Display(Name = "Used Paperback")] Used_Paperback,
        Other
    }
    public class Product
    {
        public Int32 ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required!")]
        [Display(Name = "Product Name:")]
        public String Name { get; set; }

        [Display(Name = "Product Description:")]
        public String Description { get; set; }

        [Required(ErrorMessage = "Product price is required!")]
        [Display(Name = "Product Price:")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal Price { get; set; }

        [Display(Name = "Product Type:")]
        public ProductType ProductType { get; set; }

        
	//ADD NAVIGATIONAL PROPERTIES
     public List<OrderDetail> OrderDetails { get; set; }
     public List<Supplier> Suppliers { get; set; }
        public Product()
        {
            Suppliers ??= new List<Supplier>();
            OrderDetails ??= new List<OrderDetail>();
        }

    }
}
