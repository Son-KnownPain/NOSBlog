using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NOSBlog.Models.Admin
{
    public class EditUserViewModel
    {
        public int id { get; set; }

        [DisplayName("First name"), Required(ErrorMessage = "Fisrt name is required")]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string first_name { get; set; }

        [DisplayName("Last name"), Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string last_name { get; set; }

        [DisplayName("Username"), Required(ErrorMessage = "Username is required")]
        [RegularExpression("[a-z_0-9]{5,16}", ErrorMessage = "Username only accept a-z and 0-9, length from 5 to 15")]
        public string username { get; set; }

        [DisplayName("Email"), Required(ErrorMessage = "Email is required")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email format is incorret")]
        public string email { get; set; }

        [DisplayName("Phone"), Required(ErrorMessage = "Phone is required")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone must have 10 numbers")]
        public string phone { get; set; }

        [DisplayName("Coins"), Required(ErrorMessage = "Coins is required")]
        [Range(0, 999999, ErrorMessage = "Coins value from 0 to 999999")]
        public int coins { get; set; }
    }
}