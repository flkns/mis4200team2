﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mis4200team2.Models
{
  public class Kudos
  {
    public Guid ID { get; set; }

    
    public Guid SenderID { get; set; }
    [ForeignKey("SenderID")] 
    public virtual Employee Employee { get; set; }

 
    public Guid ReceiverID { get; set; }
    [ForeignKey("ReceiverID")]
    public virtual Employee Employees { get; set; }

    public DateTime SendTime { get; set; }

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