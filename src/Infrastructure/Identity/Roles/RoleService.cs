using Application.Exceptions;
using Application.Features.Identity.Roles;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Helpers;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Roles;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<string> CreateAsync(CreateRoleRequest request)
    {
        var newRole = new ApplicationRole
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await _roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return newRole.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var roleInDb = await _roleManager.FindByIdAsync(id)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role."]);
        }

        if ((await _userManager.GetUsersInRoleAsync(roleInDb.Name)).Count > 0)
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role as it is currently assigned to users."]);
        }

        var result = await _roleManager.DeleteAsync(roleInDb);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return roleInDb.Name;
    }

    public async Task<bool> DoesItExistsAsync(string name)
    {
        return await _roleManager.RoleExistsAsync(name);
    }

    public async Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rolesInDb = await _roleManager
            .Roles
            .ToListAsync(cancellationToken);

        return rolesInDb.Adapt<IReadOnlyCollection<RoleResponse>>();
    }

    public async Task<RoleResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var roleInDb = await _context.Roles
            .FirstOrDefaultAsync(role => role.Id == id, cancellationToken)
            ?? throw new NotFoundException(["Role does not exist."]);

        return roleInDb.Adapt<RoleResponse>();
    }

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(id, cancellationToken);

        role.Permissions = await _context.RoleClaims
            .Where(rc => rc.RoleId == id && rc.ClaimType == ClaimConstants.Permission)
            .Select(rc => rc.ClaimValue)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Changes not allowed on system role '{roleInDb.Name}'."]);
        }

        roleInDb.Name = request.Name;
        roleInDb.Description = request.Description;
        roleInDb.NormalizedName = request.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(roleInDb);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return roleInDb.Name;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (roleInDb.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"Not allowed to change permissions for '{roleInDb.Name}' role."]);
        }

        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            request.NewPermissions.RemoveAll(p => p.StartsWith("Permission.Tenants."));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);

        foreach (var claim in currentClaims.Where(c => !request.NewPermissions.Any(p => p == c.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);

            if (!result.Succeeded)
            {
                throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
            }
        }

        foreach (var newPermission in request.NewPermissions.Where(p => !currentClaims.Any(c => c.Value == p)))
        {
            await _context
                .RoleClaims
                .AddAsync(new ApplicationRoleClaim
                {
                    RoleId = roleInDb.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = newPermission,
                    Description = "",
                    Group = ""
                });
        }

        await _context.SaveChangesAsync();

        return "Permissions Updated Successfully.";
    }
}
