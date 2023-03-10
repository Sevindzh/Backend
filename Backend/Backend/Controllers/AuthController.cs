using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography; //для кодирвания паролей
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]  // http://localhost:5000/api/Auth
    [ApiController]
    public class AuthController : Controller
    {
        private readonly BuildingContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BuildingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt) //функция создания хешей для паролей
        {
            using(var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (!UserExists(request.UserEmail))
            {
                User user = new User();
                CreatePasswordHash(request.UserPassword, out byte[] passwordhash, out byte[] passwordsalt);

                user.UEmail = request.UserEmail;
                user.PasswordHash = passwordhash;
                user.PasswordSalt = passwordsalt;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok($"User {request.UserEmail} registered successfully");
            }
            else 
                return BadRequest("You are already registered");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (!UserExists(request.UserEmail))
            {
                return BadRequest("Wrong Email");
            }

            var user = _context.Users.FirstOrDefaultAsync(u => u.UEmail == request.UserEmail).Result;

            if (!VerifyPasswordHash(request.UserPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private bool UserExists(string Email)
        {
            return _context.Users.Any(e => e.UEmail == Email);
        }

        private bool VerifyPasswordHash(string password, byte[] passwordhash, byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512(passwordsalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordhash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin==true ? "Admin":"User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var SCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // создаем зкодированный токен

            var token = new JwtSecurityToken( //какая информация закодирована
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: SCred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token); 

            return jwt;
        }
    }
}
