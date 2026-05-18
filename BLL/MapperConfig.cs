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

            // Course mappings 
            cfg.CreateMap<Course, CourseDTO>()
               .ForMember(dest => dest.InstructorName,
                          opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.Name : ""))
               .ForMember(dest => dest.PrerequisiteTitle,      // ← ADD
                          opt => opt.MapFrom(src => src.Prerequisite != null
                                                    ? src.Prerequisite.Title : null))
               .ReverseMap();

            // Enrollment mappings
            cfg.CreateMap<Enrollment, EnrollmentDTO>()
               .ForMember(dest => dest.CourseTitle,
                          opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : ""))
               .ForMember(dest => dest.CourseDescription,
                          opt => opt.MapFrom(src => src.Course != null ? src.Course.Description : ""))
               .ForMember(dest => dest.CourseDifficulty,
                          opt => opt.MapFrom(src => src.Course != null ? src.Course.Difficulty : ""))
               .ForMember(dest => dest.CourseContentLink,       
                          opt => opt.MapFrom(src => src.Course != null ? src.Course.ContentLink : null))
               .ForMember(dest => dest.InstructorName,
                          opt => opt.MapFrom(src => src.Course != null && src.Course.Instructor != null
                                                    ? src.Course.Instructor.Name : ""))
               .ReverseMap();
            
            // Quiz mappings
            cfg.CreateMap<Quiz, QuizDTO>()
               .ForMember(dest => dest.CourseTitle,
                          opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : ""))
               .ForMember(dest => dest.Questions,
                          opt => opt.MapFrom(src => src.Questions))
               .ReverseMap();

            cfg.CreateMap<Question, QuestionDTO>().ReverseMap();

            cfg.CreateMap<Question, QuestionForLearnerDTO>();

            // Notification mappings
            cfg.CreateMap<Notification, NotificationDTO>().ReverseMap();
        });

        public static Mapper GetMapper()
        {
            return new Mapper(config);
        }
    }
}