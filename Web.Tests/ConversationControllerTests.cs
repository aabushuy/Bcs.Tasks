using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NuGet.Protocol;
using Web.Controllers;
using Web.Repository;

namespace Web.Tests
{
    [TestClass]
    public class ConversationControllerTests
    {
        private readonly Mock<IMessageRepository> _messageRepository = new();
        private readonly Mock<IUserRepository> _userRepository = new();

        private readonly ConversationController _conversationController;

        public ConversationControllerTests()
        {
            _conversationController = new (_messageRepository.Object, _userRepository.Object);
        }

        [TestMethod]
        [DataRow(1, "Alex")]
        [DataRow(1, "Bob")]
        [DataRow(1, "Jack")]
        public async Task HttpPostAdd_ManyUser_OperationStateSussess(int expectedCode, string user)
        {
            //Arrange
            _userRepository
                .Setup(r => r.GetOrAddUser(user))
                .Returns(Task.Run(() => new Entity.DialogUser(1, user)));

            //Act
            ActionResult result = await _conversationController.Add(new Models.MessageModel(user, string.Empty));            

            //Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult), $"Unexpected response type {result.GetType().Name}");
            CreatedAtActionResult actionResult = (CreatedAtActionResult)result;

            int statusCodeResult = (int)actionResult.StatusCode!;
            Assert.IsTrue(statusCodeResult == 201, $"Unexpected status code {statusCodeResult}");

            int? status = int.Parse(GetStatusFromResponse(actionResult));
            Assert.IsNotNull(status);
            Assert.AreEqual(expectedCode, status);
        }

        [TestMethod]
        public async Task HttpPostList_WithoutParams_AllMessages()
        {
            //Arrange
            Entity.DialogUser user1 = new Entity.DialogUser(1, "Alex");
            Entity.DialogUser user2 = new Entity.DialogUser(2, "Bob");

            _messageRepository
                .Setup(r => r.GetAll())
                .Returns(Task.Run(() => new List<Entity.UserMessage>() 
                {
                    new Entity.UserMessage(user1, "Test 1"),
                    new Entity.UserMessage(user2, "Test 1"),
                    new Entity.UserMessage(user1, "Test 2"),                    
                    new Entity.UserMessage(user2, "Test 2"),
                    new Entity.UserMessage(user2, "Test 3"),
                }.AsEnumerable()));

            //Act
            ActionResult result = await _conversationController.GetList(null);

            //Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult), $"Unexpected response type {result.GetType().Name}");
            string valueResult = ((CreatedAtActionResult)result!).Value!.ToJson();
            
            Assert.IsNotNull(valueResult);
            dynamic response = JsonConvert.DeserializeObject(valueResult);

            Assert.IsInstanceOfType(response.messages, typeof(IEnumerable<object>));
        }


        private static string GetStatusFromResponse(CreatedAtActionResult actionResult)
        {
            var statusProperty = actionResult.Value!
                .GetType()
                .GetProperty("status");

            return statusProperty!
                .GetValue(actionResult.Value!, null)!
                .ToString() ?? string.Empty;
        }
    }
}