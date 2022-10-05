using System.Collections.Generic;

namespace QuizClient;

public struct Quiz
{
    public int Id;
    public string Title;
    public static Quiz NotFound => default(Quiz);
}



public class QuizResponseModel
{
    public static QuizResponseModel NotFound => default(QuizResponseModel);

    public class AnswerItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class QuestionItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<AnswerItem> Answers { get; set; }
        public int CorrectAnswerId { get; set; }
    }

    public long Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<QuestionItem> Questions { get; set; }
    public IDictionary<string, string> Links { get; set; }
}