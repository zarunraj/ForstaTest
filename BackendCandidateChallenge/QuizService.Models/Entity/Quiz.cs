using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizService.Model.Domain;

[Table("Quiz")]
public class Quiz
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }

    public Quiz()
    {

    }

    public Quiz(string title)
    {
        this.Title = title;
    }
}