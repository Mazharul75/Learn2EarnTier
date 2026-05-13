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

            // Course mappings  ← ADD THIS LINE
            cfg.CreateMap<Course, CourseDTO>()
               .ForMember(dest => dest.InstructorName,
                          opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.Name : ""))
               .ReverseMap();
        });

        public static Mapper GetMapper()
        {
            return new Mapper(config);
        }
    }
}