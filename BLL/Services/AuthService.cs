using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;
using System.Security.Cryptography;

namespace BLL.Services
{
    public class AuthService
    {
        UserRepo userRepo;
        UserTypeRepo userTypeRepo;
        Mapper mapper;

        public AuthService(UserRepo userRepo, UserTypeRepo userTypeRepo)
        {
            this.userRepo = userRepo;
            this.userTypeRepo = userTypeRepo;
            mapper = MapperConfig.GetMapper();
        }

        public bool Register(RegistrationDTO dto)
        {
            //email must be unique
            if (EmailExists(dto.Email))
                return false;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = GetMd5(dto.Password),
                UserTypeId = dto.UserTypeId,
                CreatedAt = DateTime.Now
            };

            return userRepo.Create(user);
        }

        //Login
        public UserDTO? Login(LoginDTO dto)
        {
            var hashedPassword = GetMd5(dto.Password);

            var user = userRepo.Get()
                .FirstOrDefault(u => u.Email == dto.Email
                                  && u.Password == hashedPassword);

            if (user == null)
                return null;

            return mapper.Map<UserDTO>(user);
        }

        public bool EmailExists(string email)
        {
            return userRepo.Get().Any(u => u.Email == email);
        }

        public List<UserType> GetUserTypes()
        {
            return userTypeRepo.Get();
        }

        //MD5 hashing
        static string GetMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public ProfileDTO? GetProfile(int userId)
        {
            var user = userRepo.Get(userId);
            if (user == null) return null;

            var userType = userTypeRepo.Get(user.UserTypeId);
            return new ProfileDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserTypeName = userType?.Name ?? "Unknown",
                CreatedAt = user.CreatedAt
            };
        }

        public bool UpdateProfile(int userId, string newName)
        {
            var user = userRepo.Get(userId);
            if (user == null) return false;

            user.Name = newName;
            return userRepo.Update(user);
        }
    }
}