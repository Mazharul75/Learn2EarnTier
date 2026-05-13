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

        // ===== Registration =====
        public bool Register(RegistrationDTO dto)
        {
            // Business rule: email must be unique
            if (EmailExists(dto.Email))
                return false;

            // Convert DTO → entity, hash password, set CreatedAt
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

        // ===== Login =====
        public UserDTO? Login(LoginDTO dto)
        {
            var hashedPassword = GetMd5(dto.Password);

            // Find a user with matching email and password
            var user = userRepo.Get()
                .FirstOrDefault(u => u.Email == dto.Email
                                  && u.Password == hashedPassword);

            if (user == null)
                return null;

            return mapper.Map<UserDTO>(user);
        }

        // ===== Email uniqueness check (used by Register AND by custom validator) =====
        public bool EmailExists(string email)
        {
            return userRepo.Get().Any(u => u.Email == email);
        }

        // ===== User types for the dropdown =====
        public List<UserType> GetUserTypes()
        {
            return userTypeRepo.Get();
        }

        // ===== MD5 hashing (private helper) =====
        static string GetMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // lowercase hex
                }

                return sb.ToString();
            }
        }
    }
}