using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using UnityEngine;

namespace BloodyShop.Server.Systems
{
    public static class ChatSystem
    {
        public static bool isEnabled = true;
        public static string Prefix = "!bloodyShop";

        public static float Command_Delay = 5;

        public static void HandleCommands(VChatEvent ev)
        {
            try
            {
                string[] args = { };
                if (ev.Message.Contains(' ')) args = ev.Message.Split(' ').Skip(2).ToArray();
                

                var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToArray();
                float getCurrentTime = Time.realtimeSinceStartup;

                foreach (Type type in types)
                {
                    string command = ev.Message.Split(' ')[1].ToLower();
                    if (!NameExists(type, command, out var primary)) continue;
                    var cmd = type.GetMethod("Initialize");
                    cmd.Invoke(null, new[] { new Context(Prefix, ev, args) });
                    return;
                }
                Output.InvalidCommand(ev);
            } catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error {ex.Message}");
                Output.InvalidCommand(ev);
            }
            
        }

        public static void SetPrefix(string StoreName)
        {
            Prefix = "!" + StoreName.ToLower().Replace(" ", "");
        }

        private static bool NameExists(Type type, string command, out string primary)
        {
            primary = "invalid";
            List<string> aliases = type.GetAttributeValue((CommandAttribute cmd) => cmd.Aliases);
            if (aliases.Any(x => x.ToLower() == command.ToLower()))
            {
                primary = aliases.First().ToLower();
                return true;
            }
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public List<string> Aliases;

        public string Name { get; set; }
        public string Usage { get; set; }
        public string Description { get; set; }
        public int ReqPermission { get; set; }

        public CommandAttribute(string name, string usage = "", string description = "None", int reqPermission = 100)
        {
            Name = name;
            Usage = usage;
            Description = description;
            ReqPermission = reqPermission;

            Aliases = new List<string>();
            Aliases.AddRange(Name.ToLower().Split(", "));
        }
    }

    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default;
        }
    }
}
