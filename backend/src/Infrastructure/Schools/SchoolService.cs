using Application.Features.Schools;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Schools;

public class SchoolService : ISchoolService
{
    private readonly ApplicationDbContext _context;

    public SchoolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(School school, CancellationToken cancellationToken)
    {
        await _context.Schools.AddAsync(school, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return school.Id;
    }

    public async Task<int> DeleteAsync(School school, CancellationToken cancellationToken)
    {
        _context.Schools.Remove(school);
        await _context.SaveChangesAsync(cancellationToken);
        return school.Id;
    }

    public async Task<List<School>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Schools.ToListAsync(cancellationToken);
    }

    public async Task<School> GetByIdAsync(int schoolId, CancellationToken cancellationToken)
    {
        return await _context.Schools
            .Where(school => school.Id == schoolId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<School> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Schools
            .Where(school => school.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(School school, CancellationToken cancellationToken)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync(cancellationToken);
        return school.Id;
    }
}
