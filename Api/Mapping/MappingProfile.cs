﻿using Api.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// </summary>
public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<User, UserGetDto>();
        CreateMap<Student, StudentGetDto>().ReverseMap();
        CreateMap<Student, UserGetDto>();
        CreateMap<Student, UserGetDto>();
        CreateMap<Teacher, UserGetDto>();
        CreateMap<Teacher, TeacherGetDto>().ReverseMap();
        CreateMap<Teacher, UserGetDto>();
        CreateMap<Administrator, AdministratorGetDto>().ReverseMap();
        CreateMap<Administrator, UserGetDto>();
        CreateMap<Enrollment, EnrollmentGetDto>().ReverseMap();
        CreateMap<Course, CourseGetDto>().ReverseMap();

    }
}

