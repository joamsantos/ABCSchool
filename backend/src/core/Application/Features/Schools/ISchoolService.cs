using Domain.Entities;

namespace Application.Features.Schools;


public interface ISchoolService
{
    Task<int> CreateAsync(School school, CancellationToken cancellationToken);
    Task<int> UpdateAsync(School school, CancellationToken cancellationToken);
    Task<int> DeleteAsync(School school, CancellationToken cancellationToken);
    Task<School> GetByIdAsync(int schoolId, CancellationToken cancellationToken);
    Task<List<School>> GetAllAsync(CancellationToken cancellationToken);
    Task<School> GetByNameAsync(string name, CancellationToken cancellationToken);
}

