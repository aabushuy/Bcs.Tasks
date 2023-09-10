using Microsoft.AspNetCore.Mvc;
using Web.Entity;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    [ApiController]
    public class ConversationController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(
            IMessageRepository messageRepository, 
            IUserRepository userRepository, 
            ILogger<ConversationController> logger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add(MessageModel model)
        {
            DialogUser user = await _userRepository.GetOrAddUser(model.Name);

            await _messageRepository.Add(new UserMessage(user, model.Message));

            object response = new
            {
                status = (int)OperationState.Success,
                user_id = user.Id
            };

            _logger.LogInformation($"Add a new message from user {user.Id}");

            return CreatedAtAction(nameof(Add), response);
        }

        [HttpGet("delete/{user_Id}")]
        public async Task<ActionResult> Delete(int user_Id)
        {
            OperationState operationState = OperationState.Success;
            try
            {
                await _userRepository.DeleteUser(user_Id);

                _logger.LogInformation($"Delete user and messages {user_Id}");
            }
            catch (KeyNotFoundException ex)
            {
                operationState = OperationState.UserNotFound;

                _logger.LogWarning(ex.Message);
            }

            object response = new { status = (int)operationState };

            return CreatedAtAction(nameof(Delete), response);
        }

        [HttpGet("list")]
        [HttpGet("list/{user_Id}")]
        public async Task<ActionResult> GetList(int? user_Id)
        {
            object response = user_Id is null
                ? await GetAllMessagesResponse()
                : await GetSingleUserMessagesResponse(user_Id.Value);

            return CreatedAtAction(nameof(GetList), response);
        }

        private async Task<object> GetAllMessagesResponse()
        {
            var messages = (await _messageRepository.GetAll())
                .Select(m => new
                {
                    name = m.User.Name,
                    messages = m.Message
                })
                .ToArray();

            _logger.LogInformation($"Get all messages");

            return new
            {
                status = (int)OperationState.Success,
                messages
            };
        }
        
        private async Task<object> GetSingleUserMessagesResponse(int userId)
        {
            DialogUser? user = await _userRepository.GetUser(userId);

            _logger.LogInformation($"Get all messages from user {user?.Id ?? -1}");

            return user is null
                ? new { status = (int)OperationState.UserNotFound }
                : await CreateMessageListResponse(user!);
        }

        private async Task<object> CreateMessageListResponse(DialogUser dialogUser)
        {
            string[] messages = (await _messageRepository.GetMessagesByUserId(dialogUser.Id))
                .Select(m => m.Message)
                .ToArray();

            return new
            {
                status = (int)OperationState.Success,
                name = dialogUser.Name,
                messages
            };
        }
    }
}