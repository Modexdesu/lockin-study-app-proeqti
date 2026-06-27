using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace lockin.core.Models
{
    public class UserInfo
    { public int UserId { get; set; }

        public int Age { get; set; }
        public int Xp { get; set; }
        public string Username { get; set; }
        public int HighestStreak { get; set; }
        public int CurrentStreak { get; set; }
         public int? BestTopicId { get; set; }
        public int LocationId { get; set; }
      public Location Location { get; set;  }
    }
}
