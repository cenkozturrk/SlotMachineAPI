﻿//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SlotMachineAPI.Application.DTOs;
//using SlotMachineAPI.Domain.Entities;

//namespace SlotMachineAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController(IAuthService authService) : ControllerBase
//    {
//        //[HttpPost("register")]
//        public async Task<ActionResult<User>> Register(UserDto request)
//        {
//            var user = await authService.RegisterAsync(request);
//            if (user is null)
//                return BadRequest(new
//                {
//                    error = "Username already exists"
//                });

//            return Ok(user);
//        }

//        //[HttpPost("login")]
//        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
//        {
//            var result = await authService.LoginAsync(request);
//            if (result is null)
//                return BadRequest(new
//                {
//                    error = "Invalid username or password."
//                });

//            return Ok(result);
//        }

//        //[HttpPost("refresh-token")]
//        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
//        {
//            var result = await authService.RefreshTokensAsync(request);
//            if (result is null || result.AccessToken is null || result.RefreshToken is null)
//                return Unauthorized(new
//                {
//                    error = "Invalid refresh token."
//                });

//            return Ok(result);
//        }

//        //[Authorize]
//        //[HttpGet]
//        public IActionResult AuthenticatedOnlyEndPoint()
//        {
//            return Ok("You are in!!");
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpGet("admin-only")]
//        public IActionResult AdminOnlyEndPoint()
//        {
//            return Ok("You are in and admin!!");
//        }

//    }
//}
