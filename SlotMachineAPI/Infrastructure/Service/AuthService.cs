using Microsoft.IdentityModel.Tokens;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SlotMachineAPI.Infrastructure.Service
{
    /// <summary>
    /// Service responsible for handling authentication and authorization operations.
    /// Provides functionalities for user login, registration, and token refreshing.
    /// Manages JWT and refresh token generation to maintain secure authentication.
    /// </summary>
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }
        public async Task<(string AccessToken, string RefreshToken)> Login(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("Invalid email or password!");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password!");

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user.Id, user);

            var accessToken = GenerateJwtToken(user);
            return (accessToken, user.RefreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> Register(string username, string email, string password, string role = "User")
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("User already exists!");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = role, 
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await _userRepository.AddAsync(newUser);
            var accessToken = GenerateJwtToken(newUser);
            return (accessToken, newUser.RefreshToken);
        }


        public async Task<(string AccessToken, string RefreshToken)> RefreshToken(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token!");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user.Id, user);

            return (newAccessToken, newRefreshToken);
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
