namespace QuizService.Model.Domain;

public class Question
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Text { get ; set; }
    public int CorrectAnswerId { get; set; }
}