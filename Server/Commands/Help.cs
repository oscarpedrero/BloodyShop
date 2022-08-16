using BloodyShop.Server.Systems;
using BloodyShop.Server.Utils;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BloodyShop.Server.Commands
{
    [Command("help, h", Usage = "help [<command>]", Description = "Shows a list of commands, or details about a command.")]
    public static class Help
    {
        public static void Initialize(Context ctx)
        {
            List<string> commands = new List<string>();
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0).ToArray();
            try
            {
                if (types.Any(x => x.GetAttributeValue((CommandAttribute cmd) => cmd.Aliases.First() == ctx.Args[0].ToLower())))
                {

                    
                    var type = types.First(x => x.GetAttributeValue((CommandAttribute cmd) => cmd.Aliases.First() == ctx.Args[0].ToLower()));

                    List<string> aliases = type.GetAttributeValue((CommandAttribute cmd) => cmd.Aliases);
                    if (aliases.First() == "add" && ctx.Event.User.IsAdmin)
                    {
                        string usage = type.GetAttributeValue((CommandAttribute cmd) => cmd.Usage);
                        string description = type.GetAttributeValue((CommandAttribute cmd) => cmd.Description);
                        Output.SendSystemMessage(ctx, $"Help for {FontColorChat.Green(ctx.Prefix + " " + aliases.First())}");
                        Output.SendSystemMessage(ctx, FontColorChat.White($"Aliases: {string.Join(", ", aliases)}"));
                        Output.SendSystemMessage(ctx, FontColorChat.White($"Description: {description}"));
                        Output.SendSystemMessage(ctx, FontColorChat.White($"Usage: {ctx.Prefix} {usage}"));
                    }
                    else
                    {
                        Output.CustomErrorMessage(ctx, $"Specified command not found.");
                    }
                    
                    return;
                }
                else
                {
                    Output.CustomErrorMessage(ctx, $"Specified command not found.");
                    return;
                }
            }
            catch
            {
                Output.SendSystemMessage(ctx, "List of all commands:");
                foreach (Type type in types)
                {
                    List<string> aliases = type.GetAttributeValue((CommandAttribute cmd) => cmd.Aliases);
                    string description = type.GetAttributeValue((CommandAttribute cmd) => cmd.Description);
                    

                    string s = "";
                    bool send = false;
                    if(aliases.First() == "add" && ctx.Event.User.IsAdmin)
                    {
                        s = $"{FontColorChat.Red("[ADMIN]")} {FontColorChat.Green(ctx.Prefix + " " + string.Join(", ", aliases))} - {FontColorChat.White(description)}";
                        send = true;
                    } else
                    {
                        s = $"{FontColorChat.Green(ctx.Prefix + " " + string.Join(", ", aliases))} - {FontColorChat.White(description)}";
                        send = true;
                    }
                    
                    if (send) Output.SendSystemMessage(ctx, s);
                }
            }
        }
    }
}
