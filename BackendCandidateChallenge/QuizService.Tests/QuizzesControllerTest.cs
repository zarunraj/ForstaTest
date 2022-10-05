using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using QuizService.Model.Domain;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]

    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    //We can split this big method and assign priority 
    public async Task PostNewQuizAddsQuizWithQuestions()
    {

        var quiz = new QuizCreateModel("Game of thrones");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();

            //Create quiz
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            int quizId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            //Create questions
            var question = new QuestionCreateModel("What is the motto of House Stark?");
            response = await CreateQuestion(testHost, client, quizId, question);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            int qId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            //  Create answer
            string[] answers = { "Dead Is Coming", "Winter Is Coming", "Hear Me Roar", "A Lannister Always Pays His Debts" };
            var correctAnswerId = 0;
            for (int i = 0; i < answers.Length; i++)

            {
                var answer = new AnswerCreateModel(answers[i]);
                response = await CreateAnswer(testHost, client, quizId, qId, answer);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var answerId = Convert.ToInt32(await response.Content.ReadAsStringAsync());
                if (i == 1)
                {
                    correctAnswerId++;
                }
            }

            var updateQuestion = new QuestionUpdateModel();
            updateQuestion.Text = "What is the motto of House Stark?";
            updateQuestion.CorrectAnswerId = correctAnswerId;

            string QuizQuestionApiEndPoint = $"/api/quizzes/{quizId}/questions";
            var strContent = new StringContent(JsonConvert.SerializeObject(updateQuestion));
            strContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response = await client.PostAsync(new Uri(testHost.BaseAddress, QuizQuestionApiEndPoint), strContent);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


            //Take test 
            response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            QuizResponseModel quizResponse = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quizResponse.Id);


        }
    }


    async Task<HttpResponseMessage> CreateQuestion(TestServer testHost, HttpClient client, int quizId, QuestionCreateModel question)
    {
        string QuizQuestionApiEndPoint = $"/api/quizzes/{quizId}/questions";
        return await PostMessage(testHost, client, QuizQuestionApiEndPoint, question);
    }

    async Task<HttpResponseMessage> CreateAnswer(TestServer testHost, HttpClient client, int quizId, int qid, AnswerCreateModel answer)
    {
        string QuizQuestionApiEndPoint = $"/api/quizzes/{quizId}/questions/{qid}/answers";
        return await PostMessage(testHost, client, QuizQuestionApiEndPoint, answer);
    }

    async Task<HttpResponseMessage> PostMessage(TestServer testHost, HttpClient client, string uri, object content)
    {
        var strContent = new StringContent(JsonConvert.SerializeObject(content));
        strContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(testHost.BaseAddress, uri), strContent);
        return response;
    }
}