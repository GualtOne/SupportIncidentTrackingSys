using System.ComponentModel;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.Role
{
    public enum Role
    {
        Заявитель,
        Согласующий,
        [Description ("Администратор Маршрутов")]
        АдминистраторМаршрутов,
    }
    public static class AuthService
    {
        public static User? CurrentUser { get; set; }

        public static User? Login(string login, string password)
        {
            User user = DBService.GetUserByLogin(login);
            if ((user.IsActive ?? false) || user != null)
            {
                if (VerifyPassword(password, user.PasswordHash ?? string.Empty))
                    return user;
            }
            return null;
        }

        private static bool VerifyPassword(string pass, string realpass)
        {
            string h = Hash.MakeHash(pass);
            return h == realpass;
        }



        public static bool HasPermission(Permission permission)
        {
            if (CurrentUser == null) return false;
            return permission switch
            {
                Permission.Read => CurrentUser.Role == Role.Согласующий,
                Permission.Write => CurrentUser.Role == Role.Заявитель,
                Permission.Edit => CurrentUser.Role == Role.АдминистраторМаршрутов,
                Permission.Delete => CurrentUser.Role == Role.АдминистраторМаршрутов,
                Permission.Admin => CurrentUser.Role == Role.АдминистраторМаршрутов,
                Permission.FullAccess => CurrentUser.Role == Role.АдминистраторМаршрутов,
                _ => true // по умолчанию разрешено
            };
        }
    }

    public enum Permission
    {
        Read,
        Write,
        FullAccess,
        Edit,
        Admin,
        Delete
    }
}
