using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Host.Models
{
    public class HostSendMessageModel
    {
        [Required]
        public string Message { get; set; }
        [Required]
        [StringLength(37, ErrorMessage = "Username has a max length of 30 chars.")]
        public string Username { get; set; }

        public HostSendMessageModel(string message, string username)
        {
            Message = message;
            Username = username + " - Host";
        }
    }
}
