﻿using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Linq;
using QuizService.Contracts.Repositories;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : Controller
{
    private readonly IDbConnection _connection;
    private readonly IQuizRepository quizRepository;

    public QuizController(IDbConnection connection, IQuizRepository quizRepository)
    {
        _connection = connection;
        this.quizRepository = quizRepository;
    }

    // GET api/quizzes
    [HttpGet]
    public IEnumerable<QuizResponseModel> Get()
    {
        var quizzes = quizRepository.GetAll();
        return quizzes.Select(quiz =>
            new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title
            });
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public object Get(int id)
    {

        var quiz = quizRepository.Get(id);
        if (quiz == null)
            return NotFound();

        var questions = quizRepository.GetQuestions(id);
        var answers = quizRepository.GetQuizAnswers(id);

        return new QuizResponseModel(quiz, questions, answers);

    }

    // POST api/quizzes
    [HttpPost]
    public IActionResult Post([FromBody] QuizCreateModel value)
    {
        var id = quizRepository.Insert(new Quiz(value.Title));
        return Created($"/api/quizzes/{id}", id);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] QuizUpdateModel value)
    {
        const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
        int rowsUpdated = _connection.Execute(sql, new { Id = id, Title = value.Title });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        const string sql = "DELETE FROM Quiz WHERE Id = @Id";
        int rowsDeleted = _connection.Execute(sql, new { Id = id });
        if (rowsDeleted == 0)
            return NotFound();
        return NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public IActionResult PostQuestion(int id, [FromBody] QuestionCreateModel value)
    {
        const string sqlVerifyQuizExists = "SELECT 1 from Quiz WHERE Id =@Id";
        bool isExists = _connection.ExecuteScalar<bool>(sqlVerifyQuizExists, new { Id = id });
        if (!isExists)
        {
            return NotFound();
        }

        const string sql = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";
        var questionId = _connection.ExecuteScalar(sql, new { Text = value.Text, QuizId = id });
        return Created($"/api/quizzes/{id}/questions/{questionId}", questionId);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public IActionResult PutQuestion(int id, int qid, [FromBody] QuestionUpdateModel value)
    {
        const string sql = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
        int rowsUpdated = _connection.Execute(sql, new { QuestionId = qid, Text = value.Text, CorrectAnswerId = value.CorrectAnswerId });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public IActionResult DeleteQuestion(int id, int qid)
    {
        const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
        _connection.ExecuteScalar(sql, new { QuestionId = qid });
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public IActionResult PostAnswer(int id, int qid, [FromBody] AnswerCreateModel value)
    {
        const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
        var answerId = _connection.ExecuteScalar(sql, new { Text = value.Text, QuestionId = qid });
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", answerId);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateModel value)
    {
        const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
        int rowsUpdated = _connection.Execute(sql, new { AnswerId = qid, Text = value.Text });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult DeleteAnswer(int id, int qid, int aid)
    {
        const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
        _connection.ExecuteScalar(sql, new { AnswerId = aid });
        return NoContent();
    }
}