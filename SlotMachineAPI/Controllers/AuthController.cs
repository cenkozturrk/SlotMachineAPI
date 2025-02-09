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
         
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequests.RegisterRequest request)
        {
            var (accessToken, refreshToken) = await _authService.Register(request.Username, request.Email, request.Password, request.Role);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequests.LoginRequest request)
        {
            var (accessToken, refreshToken) = await _authService.Login(request.Email, request.Password);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] AuthRequests.RefreshRequest request)
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
