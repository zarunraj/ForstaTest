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
    public class QuizRepository : IQuizRepository
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

        public Quiz Get(int id)
        {
            var quiz = _connection.Get<Quiz>(id);
            return quiz;
        }

        public IEnumerable<Question> GetQuestions(int id)
        {
            const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
            var questions = _connection.Query<Question>(questionsSql, new { QuizId = id });

            return questions;
        }

        public Dictionary<int, IList<Answer>> GetQuizAnswers(int id)
        {
            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";

            var answers = _connection.Query<Answer>(answersSql, new { QuizId = id })
                .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
                {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<Answer>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });
            return answers;
        }

        public int Insert(Quiz quiz)
        {
            var id = _connection.Insert<Quiz>(quiz);
            return (int)id;
        }
    }
}