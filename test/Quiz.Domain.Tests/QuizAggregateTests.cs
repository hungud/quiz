using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Quiz.Domain;
using Quiz.Domain.Events;
using Xunit;

namespace Quiz.Domain.Tests
{
    public class QuizAggregateTests
    {
        [Fact]
        public void Given_A_Quiz_When_Closing_Then_QuizClosedEvent_Raised()
        {
            // Arrange
            var quiz = new QuizAggregate();

            // Act
            quiz.Close();

            // Assert
            var closedEvent = quiz.GetPendingEvents().FirstOrDefault();
            Assert.NotNull(closedEvent);
            Assert.IsAssignableFrom(typeof(QuizClosedEvent), closedEvent);
        }

        [Fact]
        public void Given_A_Quiz_When_Starting_Then_QuizStartedEvent_Raised()
        {
            // Arrange
            var quiz = new QuizAggregate();
            var quizModel = CreateQuiz();

            // Act
            quiz.Start(quizModel);

            // Assert
            var startEvent = quiz.GetPendingEvents().FirstOrDefault();
            Assert.NotNull(startEvent);
            Assert.IsAssignableFrom(typeof(QuizStartedEvent), startEvent);
        }

        [Fact]
        public void Given_An_Started_Quiz_When_Voting_For_RightAnswer_Then_QuestionRightAnsweredEvent_Raised()
        {
            // Arrange
            var quiz = new QuizAggregate();
            var quizModel = CreateQuiz();
            var selectedQuestion = quizModel.Questions.FirstOrDefault();
            var selectedOption = selectedQuestion.Options.FirstOrDefault(x => x.IsCorrect);

            // Act
            quiz.Start(quizModel);
            quiz.Vote(selectedQuestion.Id, selectedOption.Id);

            // Assert
            var startedEvent = quiz.GetPendingEvents().FirstOrDefault();
            var answeredEvent = quiz.GetPendingEvents().LastOrDefault();
            Assert.NotNull(startedEvent);
            Assert.IsAssignableFrom(typeof(QuizStartedEvent), startedEvent);
            Assert.NotNull(answeredEvent);
            Assert.IsAssignableFrom(typeof(QuestionRightAnsweredEvent), answeredEvent);            
        }

        [Fact]
        public void Given_An_Started_Quiz_When_Voting_For_WrongAnswer_Then_QuestionWrongAnsweredEvent_Raised()
        {
            // Arrange
            var quiz = new QuizAggregate();
            var quizModel = CreateQuiz();
            var selectedQuestion = quizModel.Questions.FirstOrDefault();
            var selectedOption = selectedQuestion.Options.FirstOrDefault(x => !x.IsCorrect);

            // Act
            quiz.Start(quizModel);
            quiz.Vote(selectedQuestion.Id, selectedOption.Id);

            // Assert
            var startedEvent = quiz.GetPendingEvents().FirstOrDefault();
            var answeredEvent = quiz.GetPendingEvents().LastOrDefault();

            Assert.NotNull(startedEvent);
            Assert.IsAssignableFrom(typeof(QuizStartedEvent), startedEvent);
            Assert.NotNull(answeredEvent);
            Assert.IsAssignableFrom(typeof(QuestionWrongAnsweredEvent), answeredEvent);            
        }

        private QuizModel CreateQuiz() =>
            JsonConvert.DeserializeObject<QuizModel>(File.ReadAllText("quiz.json"));
    }
}
