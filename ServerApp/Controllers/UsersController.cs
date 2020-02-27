﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Dtos;
using ServerApp.Helper;
using ServerApp.Services;

namespace ServerApp.Controllers
{
	[ServiceFilter(typeof(LogUserActivity))]
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	
	public class UsersController : ControllerBase
	{
		private readonly IDatingRepository _repos;
		private readonly IMapper _mapper;

		public UsersController(IDatingRepository repos, IMapper mapper)
		{
			_repos = repos;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
		{
			int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var userFromRepos = await _repos.GetUser(currentUserId);
			userParams.UserId = currentUserId;
			if(String.IsNullOrEmpty(userParams.Gender)){
				userParams.Gender = userFromRepos.Gender == "male" ? "female":"male";
			}

			var users = await _repos.GetUsers(userParams);
			var usersToReturn = _mapper.Map<IEnumerable<UserListDto>>(users);
			Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalItems, users.TotalPages);
			return Ok(usersToReturn);
		}

		[HttpGet("{id}", Name = "GetUser")]
		public async Task<IActionResult> GetUser(int id)
		{
			var user = await _repos.GetUser(id);
			var userToReturn = _mapper.Map<UserDetailsDto>(user);
			return Ok(userToReturn);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUserInfo(int id, UserForUpdateDto userForUpdateDto)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();
			var userFromRepos = await _repos.GetUser(id);
			_mapper.Map(userForUpdateDto, userFromRepos);
			if(await _repos.SaveAll())
				return NoContent();
			throw new Exception($"could no save user with id = {id}");
		}
	}
}