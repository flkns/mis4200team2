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
        public int ID { get; set; }

        [Required, Display(Name="First Name"), StringLength(50)]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name"), StringLength(50)]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Required, DataType(DataType.Date), DisplayFormat(DataFormatString="{0:yyyy-MM-dd}", ApplyFormatInEditMode=true), Display(Name="Hire Date")]
        public DateTime HireDate { get; set; }

        [Required, Display(Name ="Business Unit"), StringLength(50)]
        public string BusinessUnit { get; set; }

        [Required, Display(Name = "Title"), StringLength(50)]
        public string Title { get; set; }

    }
}