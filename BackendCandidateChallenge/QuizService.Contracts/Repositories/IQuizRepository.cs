using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizService.Contracts.Repositories
{
    public interface IQuizRepository
    {
        IEnumerable<QuizService.Model.Domain.Quiz> GetAll();
    }
}
