using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using dotnet.boilerplate.Dto;
using dotnet.boilerplate.Helpers;
using dotnet.boilerplate.Helpers.Params;
using dotnet.boilerplate.Models;
using dotnet.boilerplate.Persistance.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.boilerplate.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    // [Authorize(Roles = "Bus Time.R, Administrator")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserController(IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAll();
            var result = _mapper.Map<IEnumerable<ViewUserDto>>(users);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<User, ViewUserDto>(user);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] UserParams userParams)
        {
            var users = await _userRepository.GetPaged(userParams);
            var result = _mapper.Map<IEnumerable<ViewUserDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<AddUserDto, User>(userDto);
            var password = userDto.Password;
            _userRepository.Add(user, password);

            if (await _unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Create user data failed on save");
            }

            user = await _userRepository.GetById(user.Id);
            var result = _mapper.Map<User, ViewUserDto>(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EditUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            user = _mapper.Map(userDto, user);
            _userRepository.Update(user);

            if (await _unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Update user failed in save");
            }

            user = await _userRepository.GetById(user.Id);
            var result = _mapper.Map<User, ViewUserDto>(user);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _userRepository.Remove(user);

            if (await _unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Delete department failed on save");
            }

            return Ok(id);
        }

        [HttpPut("photo/{id}")]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            if (file != null)
            {
                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                // var folder = Path.Combine(hostingEnvironment.WebRootPath, "Profiles");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                if (file.Length > 0)
                {
                    var user = await _userRepository.GetById(id);

                    var currenttime = DateTime.Now.ToString("dd-mm-yyyy-HH-mm-ss");
                    var fileName = $"{user.Id}-{currenttime}-{file.FileName}";

                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    // var filePath = Path.Combine(folder, fileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    user.Photo = dbPath;
                    _userRepository.Update(user);

                    if (await _unitOfWork.CompleteAsync() == false)
                    {
                        throw new Exception(message: "Update user failed in save");
                    }

                    user = await _userRepository.GetById(user.Id);
                    var result = _mapper.Map<User, ViewUserDto>(user);
                    return Ok(result);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return Ok();
            }

        }

    }
}