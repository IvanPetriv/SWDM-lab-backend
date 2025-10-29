using Api.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// </summary>
public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<Student, StudentGetDto>().ReverseMap();
        CreateMap<Teacher, TeacherGetDto>().ReverseMap();
        CreateMap<Administrator, AdministratorGetDto>().ReverseMap();
        CreateMap<Enrollment, EnrollmentGetDto>().ReverseMap();
        CreateMap<Course, CourseGetDto>().ReverseMap();
    }
}
