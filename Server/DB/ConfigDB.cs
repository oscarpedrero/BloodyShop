using BloodyShop.DB.Models;
using BloodyShop.Server.DB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB
{
    public class ConfigDB
    {
        public static bool ShopEnabled { get; set; } = true;
        public static bool AnnounceAddRemovePublic { get; set; } = true;
        public static bool AnnounceBuyPublic { get; set; } = true;
        public static string StoreName { get; set; } = "Bloody Shop";
        public static bool DropEnabled { get; set; } = true;
        public static int DropNpcPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsNpc { get; set; } = 0;
        public static int DropdNpcCurrenciesMin { get; set; } = 0;
        public static int DropNpcCurrenciesMax { get; set; } = 0;
        public static int MaxCurrenciesPerDayPerPlayerNpc { get; set; } = 0;
        public static int DropdVBloodPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsVBlood { get; set; } = 0;
        public static int DropVBloodCurrenciesMin { get; set; } = 0;
        public static int DropVBloodCurrenciesMax { get; set; } = 0;
        public static int MaxCurrenciesPerDayPerPlayerVBlood { get; set; } = 0;
        public static int DropPvpPercentage { get; set; } = 0;
        public static int IncrementPercentageDropEveryTenLevelsPvp { get; set; } = 0;
        public static int DropPvpCurrenciesMin { get; set; } = 0;
        public static int DropPvpCurrenciesMax { get; set; } = 0;
        public static int MaxCurrenciesPerDayPerPlayerPvp { get; set; } = 0;
        public static List<UserCurrenciesPerDayModel> UsersCurrenciesPerDay { get; set; } = new List<UserCurrenciesPerDayModel>();

        public static List<(string name, DateTime date, UserCurrenciesPerDayModel model)> _normalizedUsersCurrenciesPerDay = new();

        public static bool setUsersCurrenciesPerDay(List<UserCurrenciesPerDayModel> listUsersCurrenciesPerDay)
        {

            UsersCurrenciesPerDay = new();

            foreach (UserCurrenciesPerDayModel userCurrenciesPerDay in listUsersCurrenciesPerDay)
            {
                DateTime oDate = DateTime.Parse(userCurrenciesPerDay.date);
                if (oDate != DateTime.Today)
                {
                    continue;
                }
                _normalizedUsersCurrenciesPerDay.Add((userCurrenciesPerDay.CharacterName, oDate, userCurrenciesPerDay));
                UsersCurrenciesPerDay.Add(userCurrenciesPerDay);
            }

            return true;
        }

        public static void addUserCurrenciesPerDayToList(UserCurrenciesPerDayModel userCurrenciesPerDay)
        {
            foreach (var (name, date, model) in _normalizedUsersCurrenciesPerDay)
            {
                if (name == userCurrenciesPerDay.CharacterName)
                {
                    UsersCurrenciesPerDay.Remove(model);
                    _normalizedUsersCurrenciesPerDay.Remove((name, date, model));
                    UsersCurrenciesPerDay.Add(userCurrenciesPerDay);
                    _normalizedUsersCurrenciesPerDay.Add((userCurrenciesPerDay.CharacterName, DateTime.Parse(userCurrenciesPerDay.date), userCurrenciesPerDay));
                    break;
                }
            }
        }

        public static bool searchUserCurrencyPerDay(string characterName, out UserCurrenciesPerDayModel userCurrenciesPerDayModel)
        {

            userCurrenciesPerDayModel = null;
            if (characterName == "")
            {
                return false;
            }

            var today = DateTime.Today;

            var todayString = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

            foreach (var (name, date, model) in _normalizedUsersCurrenciesPerDay)
            {
                if (name == characterName)
                {
                    if(today == date)
                    {
                        userCurrenciesPerDayModel = model;
                        break;
                    } else
                    {
                        UsersCurrenciesPerDay.Remove(model);
                        _normalizedUsersCurrenciesPerDay.Remove((name, date, model));
                        userCurrenciesPerDayModel = new UserCurrenciesPerDayModel
                        {
                            CharacterName = model.CharacterName,
                            date = todayString
                        };
                        UsersCurrenciesPerDay.Add(model);
                        _normalizedUsersCurrenciesPerDay.Add((characterName, today, userCurrenciesPerDayModel));
                        break;
                    }
                }
            }

            if(userCurrenciesPerDayModel == null)
            {
                userCurrenciesPerDayModel = new UserCurrenciesPerDayModel
                {
                    CharacterName = characterName,
                    date = todayString
                };
                UsersCurrenciesPerDay.Add(userCurrenciesPerDayModel);
                _normalizedUsersCurrenciesPerDay.Add((characterName, today, userCurrenciesPerDayModel));
                return true;

            } else
            {
                return true;
            }
        }
    }
}
