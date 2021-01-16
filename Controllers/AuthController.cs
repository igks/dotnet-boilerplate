using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using dotnet.boilerplate.Dto;
using dotnet.boilerplate.Persistance.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using dotnet.boilerplate.Models;

namespace dotnet.boilerplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        private readonly IAuthRepository authRepository;
        private readonly IUnitOfWork unitOfWork;

        public AuthController(IConfiguration config, IMapper mapper, IAuthRepository authRepository, IUnitOfWork unitOfWork)
        {
            this.config = config;
            this.mapper = mapper;
            this.authRepository = authRepository;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userLogin)
        {
            var email = userLogin.Email;
            var password = userLogin.Password;

            var user = await authRepository.Login(email, password);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = JWTTokenHandler(user);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto userRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = mapper.Map<RegisterDto, User>(userRegister);
            var password = userRegister.Password;
            authRepository.Register(user, password);

            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Register failed");
            }

            var result = JWTTokenHandler(user);
            return Ok(result);
        }

        private object JWTTokenHandler(User user)
        {
            var userId = user.Id.ToString();

            var claims = new List<Claim>();
            claims.Add(new Claim("Id", userId));

            var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(this.config.GetSection("AppSettings:Token").Value));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userData = mapper.Map<ViewUserDto>(user);
            return (
            new
            {
                token = tokenHandler.WriteToken(token),
                userData
            });
        }
    }
}