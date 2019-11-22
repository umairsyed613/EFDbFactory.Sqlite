using System.Collections.Generic;

namespace EFDbFactory.Sqlite.Tests
{
    public class Answer
    {
        public Answer()
        {
            QuestionNavigation = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<Question> QuestionNavigation { get; set; }
    }
}
