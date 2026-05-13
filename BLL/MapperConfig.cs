using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;

namespace BLL
{
    public class MapperConfig
    {
        static MapperConfiguration config = new MapperConfiguration(cfg => {
            // User mappings
            cfg.CreateMap<User, UserDTO>().ReverseMap();
            cfg.CreateMap<User, RegistrationDTO>().ReverseMap();
            // LoginDTO doesn't need entity mapping — it's only used for form input,
            // never converted to/from User.

            // More mappings will be added in later phases:
            // cfg.CreateMap<Course, CourseDTO>().ReverseMap();
            // cfg.CreateMap<Enrollment, EnrollmentDTO>().ReverseMap();
            // ... etc.
        });

        public static Mapper GetMapper()
        {
            return new Mapper(config);
        }
    }
}