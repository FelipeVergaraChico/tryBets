using TryBets.Users.Models;
using TryBets.Users.DTO;

namespace TryBets.Users.Repository;

public class UserRepository : IUserRepository
{
    protected readonly ITryBetsContext _context;
    public UserRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public User Post(User user)
    {
        var userF = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();
        if (userF != null) {
            throw new Exception("E-mail already used");
        }
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }
    public User Login(AuthDTORequest login)
    {
        var userF = _context.Users.Where(u => u.Email == login.Email);
        if (userF.Count() == 0) {
            throw new Exception("Authentication failed");
        }
        if (userF.First().Password != login.Password) {
            throw new Exception("Authentication failed");
        }
        return userF.FirstOrDefault();
    }
}