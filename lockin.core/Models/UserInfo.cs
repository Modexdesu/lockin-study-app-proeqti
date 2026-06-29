using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lockin.core.Models
{
    /// <summary>
    /// User profile tracking XP, streaks, and competitive stats.
    /// </summary>
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        public int Age { get; set; }

        /// <summary>Total experience points earned from correct answers.</summary>
        public int Xp { get; set; }

        /// <summary>Current active winning streak.</summary>
        public int CurrentStreak { get; set; }

        /// <summary>Highest single-game winning streak achieved.</summary>
        public int HighestStreak { get; set; }

        /// <summary>ID of the topic where the user performs the best.</summary>
        public int? BestTopicId { get; set; }
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        /// <summary>The user's city or region name.</summary>
        [Required]
        public string Location { get; set; } = null!;
    }
}