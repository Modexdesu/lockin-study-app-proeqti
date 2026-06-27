using System;
using System.Collections.Generic;
using System.Text;

namespace lockin.core.Models;
  public class Topic
{
    public int TopicId { get; set; }
    public string TopicName { get; set; }
    public List<Question> Question { get; set; } = new List<Question>();
}