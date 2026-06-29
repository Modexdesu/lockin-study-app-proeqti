using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace lockin.core.Models;
  public class Topic
{
    [Required]
    public int TopicId { get; set; }
    [Required]
    public string TopicName { get; set; }
    public List<Question> Question { get; set; } = new List<Question>();
}