using System.ComponentModel.DataAnnotations.Schema;

namespace QuizService.Model.Domain;

[Table("Quiz")]
public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; }
}