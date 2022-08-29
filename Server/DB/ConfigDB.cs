using BloodyShop.DB.Models;
using BloodyShop.Server.DB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB
{
    public class ConfigDB
    {
        public static bool ShopEnabled = true;

        public static string StoreName = "BloodyShop";

        public static bool DropEnabled = true;

        public static int DropNpcPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsNpc { get; set; } = 0;
        public static int DropdNpcCoinsMin { get; set; } = 0;
        public static int DropNpcCoinsMax { get; set; } = 0;
        public static int MaxCoinsPerDayPerPlayerNpc { get; set; } = 0;

        public static int DropdVBloodPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsVBlood { get; set; } = 0;
        public static int DropVBloodCoinsMin { get; set; } = 0;
        public static int DropVBloodCoinsMax { get; set; } = 0;
        public static int MaxCoinsPerDayPerPlayerVBlood { get; set; } = 0;

        public static int DropPvpPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsPvp { get; set; } = 0;
        public static int DropPvpCoinsMin { get; set; } = 0;
        public static int DropPvpCoinsMax { get; set; } = 0;
        public static int MaxCoinsPerDayPerPlayerPvp { get; set; } = 0;
        public static List<UserCoinsPerDayModel> UsersCoinsPerDay { get; set; } = new List<UserCoinsPerDayModel>();

        public static List<(string name, DateTime date, UserCoinsPerDayModel model)> _normalizedUsersCoinsPerDay = new();

        public static bool setUsersCoinsPerDay(List<UserCoinsPerDayModel> listUsersCoinsPerDay)
        {
            foreach (UserCoinsPerDayModel userCoinsPerDay in listUsersCoinsPerDay)
            {
                DateTime oDate = DateTime.Parse(userCoinsPerDay.date);
                if (oDate != DateTime.Today)
                {
                    listUsersCoinsPerDay.Remove(userCoinsPerDay);
                    continue;
                }
                _normalizedUsersCoinsPerDay.Add((userCoinsPerDay.CharacterName, oDate, userCoinsPerDay));
            }
            UsersCoinsPerDay = listUsersCoinsPerDay;

            return true;
        }

        public static void addUserCoinsPerDayToList(UserCoinsPerDayModel userCoinsPerDay)
        {
            foreach (var (name, date, model) in _normalizedUsersCoinsPerDay)
            {
                if (name == userCoinsPerDay.CharacterName)
                {
                    UsersCoinsPerDay.Remove(model);
                    _normalizedUsersCoinsPerDay.Remove((name, date, model));
                    UsersCoinsPerDay.Add(userCoinsPerDay);
                    _normalizedUsersCoinsPerDay.Add((userCoinsPerDay.CharacterName, DateTime.Parse(userCoinsPerDay.date), userCoinsPerDay));
                    break;
                }
            }
        }

        public static bool searchUserCoinPerDay(string characterName, out UserCoinsPerDayModel userCoinsPerDayModel)
        {

            userCoinsPerDayModel = null;
            if (characterName == "")
            {
                return false;
            }

            var today = DateTime.Today;

            var todayString = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

            foreach (var (name, date, model) in _normalizedUsersCoinsPerDay)
            {
                if (name == characterName)
                {
                    if(today == date)
                    {
                        userCoinsPerDayModel = model;
                        break;
                    } else
                    {
                        UsersCoinsPerDay.Remove(model);
                        _normalizedUsersCoinsPerDay.Remove((name, date, model));
                        userCoinsPerDayModel = new UserCoinsPerDayModel
                        {
                            CharacterName = model.CharacterName,
                            date = todayString
                        };
                        UsersCoinsPerDay.Add(model);
                        _normalizedUsersCoinsPerDay.Add((characterName, today, userCoinsPerDayModel));
                        break;
                    }
                }
            }

            if(userCoinsPerDayModel == null)
            {
                userCoinsPerDayModel = new UserCoinsPerDayModel
                {
                    CharacterName = characterName,
                    date = todayString
                };
                UsersCoinsPerDay.Add(userCoinsPerDayModel);
                _normalizedUsersCoinsPerDay.Add((characterName, today, userCoinsPerDayModel));
                return true;

            } else
            {
                return true;
            }
        }
    }
}
