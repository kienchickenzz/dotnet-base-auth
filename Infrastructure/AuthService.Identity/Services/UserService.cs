namespace AuthService.Identity.Services;

using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Common.ApplicationServices.BackgroundJob;
using AuthService.Application.Common.ApplicationServices.Caching;
// using AuthService.Application.Common.ApplicationServices.Email;
// using AuthService.Application.Common.ApplicationServices.FileStorage;
using AuthService.Application.Common.Exceptions;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Application.Features.Identities.Users;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;
using AuthService.Identity.Events;


internal partial class UserService : IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationIdentityDbContext _db;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly IJobService _jobService;
    // private readonly IMailService _mailService;
    // private readonly IFileStorageService _fileStorage;
    // private readonly IEmailTemplateService _templateService;
    private readonly IPublisher _mediator;

    public UserService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationIdentityDbContext db,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        IJobService jobService,
        // IMailService mailService,
        // IFileStorageService fileStorageService,
        // IEmailTemplateService templateService,
        IPublisher mediator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _jobService = jobService;
        // _mailService = mailService;
        // _fileStorage = fileStorageService;
        // _templateService = templateService;
        _mediator = mediator;
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null)
    {
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null)
    {
        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<List<UserResponse>> GetListAsync(CancellationToken cancellationToken) =>
        (await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserResponse>>();

    public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        _userManager.Users.AsNoTracking().CountAsync(cancellationToken);

    public async Task<UserResponse> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException("User Not Found.");

        return user.Adapt<UserResponse>();
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException("User Not Found.");

        bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin)
        {
            throw new BadRequestException("Administrators Profile's Status cannot be toggled");
        }

        user.IsActive = request.IsActivateUser;

        await _userManager.UpdateAsync(user);

        await _mediator.Publish(new ApplicationUserUpdatedEvent(user.Id));
    }
}