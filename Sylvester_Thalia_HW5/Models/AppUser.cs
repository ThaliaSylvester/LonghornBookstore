using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Sylvester_Thalia_HW5.Models
{
    public class AppUser : IdentityUser
    {
        //Add additional user fields here
        //First name is provided as an example

        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Display(Name = "Full Name")]
        public String FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public List<Order> Orders { get; set; }

        public AppUser()
        {
            if (Orders == null)
            {
                Orders = new List<Order>();
            }
        }
    }
}