using Dapper;
using Dapper.Contrib.Extensions;
using QuizService.Contracts.Repositories;
using QuizService.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizService.DataAccess
{
    public class QuizRepository: IQuizRepository
    {
        private readonly IDbConnection _connection;

        public QuizRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Quiz> GetAll()
        { 
            var quizzes = _connection.GetAll<Quiz>(); 
            return quizzes;
        }
    }
}
