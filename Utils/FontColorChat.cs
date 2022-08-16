using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Utils
{
    internal class FontColorChat
    {
        public static string Color(string hexColor, string text)
        {
            return $"<color={hexColor}>{text}</color>";
        }
        public static string Red(string text)
        {
            return Color("#E90000", text);
        }
        public static string Blue(string text)
        {
            return Color("#0000ff", text);
        }
        public static string Green(string text)
        {
            return Color("#7FE030", text);
        }
        public static string Yellow(string text)
        {
            return Color("#FBC01E", text);
        }
        public static string White(string text)
        {
            return Color("#FFFFFF", text);
        }
        
    }
}
