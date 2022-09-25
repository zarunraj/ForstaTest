using QuizService.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizService.Contracts.Repositories
{
    public interface IQuizRepository
    {
        Quiz Get(int id);
        IEnumerable<Quiz> GetAll();
        IEnumerable<Question> GetQuestions(int id);
        Dictionary<int, IList<Answer>> GetQuizAnswers(int id);
        int Insert(Quiz quiz);
    }
}
