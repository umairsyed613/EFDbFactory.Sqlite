using System.Collections.Generic;

namespace EFDbFactory.Sqlite.Tests
{
    public class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuizId { get; set; }
        public int? CorrectAnswerId { get; set; }
        public virtual Answer CorrectAnswer { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
