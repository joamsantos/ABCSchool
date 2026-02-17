using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries;

public class GetSchoolsQuery : IRequest<IResponseWrapper> { }

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;
    
    public GetSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
    
    public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schoolsInDb = await _schoolService.GetAllAsync(cancellationToken);
        if (schoolsInDb.Any())
        {
            return await ResponseWrapper<IReadOnlyCollection<SchoolResponse>>
                .SuccessAsync(data: schoolsInDb.Adapt<IReadOnlyCollection<SchoolResponse>>());
        }
        return await ResponseWrapper<IReadOnlyCollection<SchoolResponse>>
            .FailAsync(message: "No Schools were found.");
    }
}
