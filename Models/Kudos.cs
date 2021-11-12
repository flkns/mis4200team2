using System;
using System.ComponentModel.DataAnnotations;

namespace mis4200team2.Models
{
  public class Kudos
  {
    public Guid ID { get; set; }

    public Guid SenderID { get; set; }

    public Guid ReceiverID { get; set; }

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