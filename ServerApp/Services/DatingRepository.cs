﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Services
{
	public class DatingRepository:IDatingRepository
	{
		private readonly DataContext _context;


		public DatingRepository(DataContext context)
		{
			_context = context;
		}

		public async Task<T> Add<T>(T entity) where T : class
		{
			await _context.AddAsync(entity);
			return entity;
		}

		public void Delete<T>(T entity) where T : class
		{
			_context.Remove(entity);
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			var users = await _context.Users
				.Include(p => p.Photos)
				.ToListAsync();
			return users;
		}

		public async Task<User> GetUser(int id)
		{
			var user = await _context.Users
				.Include(p=>p.Photos)
				.FirstOrDefaultAsync(x => x.Id == id);
			return user;
		}

		public async Task<bool> SaveAll()
		{
			return await _context.SaveChangesAsync() > 0;
		}
	}
}