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
        CreateMap<Teacher, UserGetDto>();
        CreateMap<Teacher, CurrentUserDto>();
        CreateMap<Teacher, TeacherGetDto>().ReverseMap();
        CreateMap<Administrator, AdministratorGetDto>().ReverseMap();
        CreateMap<Administrator, UserGetDto>();
        CreateMap<Administrator, CurrentUserDto>();
        CreateMap<Enrollment, EnrollmentGetDto>().ReverseMap();
        CreateMap<Course, CourseGetDto>().ReverseMap();

    }
}

