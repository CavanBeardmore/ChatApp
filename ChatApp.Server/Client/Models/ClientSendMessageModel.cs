using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Client.Models
{
    public class ClientSendMessageModel
    {
        [Required]
        public string Message { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Username has a max length of 30 chars.")]
        public string Username { get; set; }

        public ClientSendMessageModel(string message, string username)
        {
            Message = message;
            Username = username;
        }
    }
}
