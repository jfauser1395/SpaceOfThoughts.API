namespace Artblog.API.Models.DTOs
{
    public class UserResponseDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
