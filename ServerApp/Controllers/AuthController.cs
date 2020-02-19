﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Dtos;
using ServerApp.Models;
using ServerApp.Services;

namespace ServerApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthRepository _repos;
		private readonly IConfiguration _config;

		public AuthController( IAuthRepository repos, IConfiguration config)
		{
			_repos = repos;
			_config = config;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(UserForRegisterDto userToRegiser)
		{
			userToRegiser.Username = userToRegiser.Username.ToLower();
			if (await _repos.UserExists(userToRegiser.Username))
				return BadRequest("Username already taken");
			var userToReturn = new User
			{
				Username = userToRegiser.Username
			};
			await _repos.Register(userToReturn, userToRegiser.Password);
			return Ok();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(UserForLoginDto userForLogin)
		{
			var userFromRepos = await _repos.Login(userForLogin.Username.ToLower(), userForLogin.Password);
			
			if (userFromRepos == null)
				return Unauthorized();

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, userFromRepos.Id.ToString()),
				new Claim(ClaimTypes.Name, userFromRepos.Username)
			};
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = creds
			};
			
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return Ok(new
			{
				token = tokenHandler.WriteToken(token)
			});
		}
	}
}