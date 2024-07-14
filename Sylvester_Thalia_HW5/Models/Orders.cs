using System.ComponentModel.DataAnnotations;
using Sylvester_Thalia_HW5.Models;

namespace Sylvester_Thalia_HW5.Models
{
    public class Order
    {
        private const Decimal TAX_RATE = 0.0825m;

        public Int32 OrderID { get; set; }

        [Display(Name = "Order Number")]
        public Int32 OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Notes")]
        public String OrderNotes { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(od => od.ExtendedPrice); }
        }

        [Display(Name = "Order Tax")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTax
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + OrderTax; }
        }
        //ADD NAVIGATIONAL PROPERTIES
        public List<OrderDetail> OrderDetails { get; set; }

        public AppUser User { get; set; }


        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }

    }
}