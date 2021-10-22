using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mis4200team2.Models
{
    public class Employee
    {
        public Guid ID { get; set; }

        [Required, Display(Name="First Name"), StringLength(50)]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name"), StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [
            Display(Name = "Email"),
            DataType(DataType.EmailAddress)
        ]
        public string Email { get; set; }

        [
            Display(Name = "Phone #"),
            DataType(DataType.PhoneNumber),
            RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter a valid phone number.")
        ]
        public string Phone { get; set; }

        [DataType(DataType.Date), Display(Name = "Registered Date")]
        public DateTime RegisteredDate { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString="{0:yyyy-MM-dd}", ApplyFormatInEditMode=true), Display(Name="Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Title"), StringLength(50)]
        public string Title { get; set; }

        [Display(Name ="Business Unit")]
        public BusinessUnits BusinessUnit { get; set; }

        public enum BusinessUnits
        {
            start = 0,
            admin = 1,

        }

    }
}