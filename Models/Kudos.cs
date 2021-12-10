using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mis4200team2.Models
{
  public class Kudos
  {
    public Guid KudosID { get; set; }

    public Guid SenderID { get; set; }
    public virtual Employee SenderEmployee { get; set; }

    public Guid ReceiverID { get; set; }
    public virtual Employee ReceiverEmployee { get; set; }

    [DataType(DataType.Date), Display(Name = "Send Time")]
    public DateTime SendTime { get; set; }

    [Display(Name = "Kudos Type")]
    public KudosTypes Type { get; set; }
    
    public enum KudosTypes
    {
      Excellence = 0,
      Integrity = 1,
      Stewardship = 2,
      Culture = 3,
      Passion = 4,
      Innovation = 5,
      Balance= 6
    }
  }
}