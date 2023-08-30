using AutoMapper;
using BLL.Models.Auth;
using BLL.Services.TimeService;
using Common.Exceptions.User;
using Common.Helpers;
using DAL;
using DAL.Entities;
using DAL.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;

namespace BLL.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ITimeService _timeService;

    public UserService(ApplicationDbContext db, IMapper mapper, ITimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _timeService = timeService;
    }

    public async Task<Guid> RegisterUser(RegisterModel model)
    {
        if (await CheckUserExistByNick(model.Nick)) throw new NickAlreadyExistException();
        if (await  CheckUserExistByEmail(model.Email)) throw new EmailAlreadyExistException();

        var user = _mapper.Map<User>(model);
        user.RegistrationDate = _timeService.GetCurrentDateTime();
        
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user.Id;
    }
    
    public async Task<User> GetUserById(Guid id)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) throw new UserNotFoundException();
        return user;
    }
    
    public IQueryable<User> GetUsersById(Guid id)
    {
        return _db.Users.AsNoTracking().Where(u => u.Id == id);
    }

    public async Task<bool> UserExistsByCredentials(CredentialModel model)
    {
        var passwordHash = HashHelper.GetHash(model.Password);
        return await _db.Users.AnyAsync(u => u.PasswordHash == passwordHash && u.Email == model.Email);
    }
    
    public async Task<User> GetUserByCredentials(CredentialModel model)
    {
        var passwordHash = HashHelper.GetHash(model.Password);
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PasswordHash == passwordHash && u.Email == model.Email);
        if (user == null) throw new UserNotFoundException();
        return user;
    }

    public async Task<bool> CheckUserExistByNick(string nick)
    {
        return await _db.Users.GetFirstOrDefaultByNickFilterAsync(nick) != null;
    }

    public async Task<bool> CheckUserExistByEmail(string email)
    {
        return await _db.Users.GetFirstOrDefaultByEmailFilterAsync(email) != null;
    }

    public async Task<bool> UserExistById(Guid id)
    {
        return await _db.Users.AnyAsync(u => u.Id == id);
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await GetUserById(userId);
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}
