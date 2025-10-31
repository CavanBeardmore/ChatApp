using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Client.Models
{
    public class ClientStartModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Username has a max length of 30 chars.")]
        public string Username { get; set; }

        public ClientStartModel(string username)
        {
            Username = username;
        }
    }
}
