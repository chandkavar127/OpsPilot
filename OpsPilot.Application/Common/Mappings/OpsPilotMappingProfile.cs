using AutoMapper;
using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Common.Mappings;

public class OpsPilotMappingProfile : Profile
{
    public OpsPilotMappingProfile()
    {
        CreateMap<Request, RequestSummaryDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.RequestType != null ? src.RequestType.Name : string.Empty))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeProfile != null ? src.EmployeeProfile.FullName : string.Empty))
            .ForMember(dest => dest.HasAttachment, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AttachmentPath)));
    }
}
