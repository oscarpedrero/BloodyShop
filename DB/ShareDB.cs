using VRising.GameData;
using VRising.GameData.Models;
using ProjectM;
using BloodyShop.DB.Models;
using System.Collections.Generic;
using System.Linq;
using BloodyShop.Utils;
using System;

namespace BloodyShop.DB
{
    public class ShareDB
    {

        public static List<CurrencyModel> currenciesList = new();

        public static List<CurrencyModel> getCurrencyList()
        {
            return currenciesList;
        }

        public static CurrencyModel getCurrency(int guid)
        {
            return currenciesList.FirstOrDefault(currency => currency.guid == guid);
        }

        public static CurrencyModel getCurrencyByName(string name)
        {
            return currenciesList.FirstOrDefault(currency => currency.name == name);
        }

        public static bool setCurrencyList(List<CurrencyModel> currencies)
        {

            currenciesList = currencies;

            return true;
        }

        public static bool addCurrencyList(string name, int guid, bool drop)
        {
            var currency = new CurrencyModel();
            var id = getCurrencyList().Last().id +1;
            currency.name = name;
            currency.guid = guid;
            currency.id = id;
            currency.drop = drop;

            currenciesList.Add(currency);

            return true;
        }

        public static List<string> GetCurrencyListMessage()
        {
            var listCurrency = new List<string>();

            foreach (CurrencyModel item in currenciesList)
            {
                listCurrency.Add($"{FontColorChat.White("[")}{FontColorChat.Yellow(item.id.ToString())}{FontColorChat.White("]")} " +
                        $"{FontColorChat.Yellow(item.name)} ");
            }

            return listCurrency;

        }

        public static bool SearchCurrencyByCommand(int index, out CurrencyModel currencyModel)
        {
           
            currencyModel = currenciesList.FirstOrDefault(currency => currency.id == index);
            if (currencyModel == null)
                return false;

            return true;
        }

        public static bool RemoveCurrencyyByCommand(int index)
        {
            try
            {
                var currencies = currenciesList.RemoveAll(currency => currency.id == index);
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static List<CurrencyModel> searchCurrencyByNameForShop(string text)
        {

            var result = new List<CurrencyModel>();
            foreach (var currency in currenciesList)
            {
                if (currency.name.Contains(text))
                {
                    result.Add(currency);
                }
            }

            return result;
        }

        public static int searchIdForCurrency(int GUID)
        {
            foreach (var currency in currenciesList)
            {
                if (currency.guid == GUID)
                {
                    return currency.id;
                }
            }

            return -1;
        }
    }
}
