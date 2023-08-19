using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace BloodyShop.Server.Systems
{
    internal class UserUI
    {
        private static Dictionary<string, User> _users = new();


        public static void RegisterUserWithUI(User user)
        {
            User? entity = null;

            _users.TryAdd(user.CharacterName.ToString(), user);
        }

        public static Dictionary<string, User> GetUsersWithUI()
        {
            return _users;
        }
    }
}
