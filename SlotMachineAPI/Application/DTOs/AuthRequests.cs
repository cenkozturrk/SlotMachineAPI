namespace SlotMachineAPI.Application.DTOs
{
    public class AuthRequests
    {
        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; } = "User";

        }
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class RefreshRequest
        {
            public string RefreshToken { get; set; }
        }                   
    }                               
}
