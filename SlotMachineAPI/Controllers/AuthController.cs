using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SlotMachineAPI.Application.DTOs;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Service;
using System.Security.Claims;

namespace SlotMachineAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (accessToken, refreshToken) = await _authService.Register(request.Username, request.Email, request.Password);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (accessToken, refreshToken) = await _authService.Login(request.Email, request.Password);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var (accessToken, refreshToken) = await _authService.RefreshToken(request.RefreshToken);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }       
    }
}
