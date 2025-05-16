namespace DateApp.Dtos.AccountDto
{
    public class UserProfileUpdateDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
