﻿using Api.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserGetDto>();
        CreateMap<User, CurrentUserDto>();
        CreateMap<Student, StudentGetDto>().ReverseMap();
        CreateMap<Student, UserGetDto>();
        CreateMap<Student, CurrentUserDto>();
        CreateMap<Student, UserSearchDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Student"));
        CreateMap<Teacher, UserGetDto>();
        CreateMap<Teacher, CurrentUserDto>();
        CreateMap<Teacher, UserSearchDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Teacher"));
        CreateMap<Teacher, TeacherGetDto>().ReverseMap();
        CreateMap<Administrator, AdministratorGetDto>().ReverseMap();
        CreateMap<Administrator, UserGetDto>();
        CreateMap<Administrator, CurrentUserDto>();
        CreateMap<Administrator, UserSearchDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Administrator"));
        CreateMap<Enrollment, EnrollmentGetDto>().ReverseMap();
        CreateMap<Course, CourseGetDto>().ReverseMap();
        CreateMap<Course, CourseWithFilesDto>();
        CreateMap<MediaMaterial, CourseFileDto>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt));

    }
}

