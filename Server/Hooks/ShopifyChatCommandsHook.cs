using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using BloodyShop.Server.Systems;
using Unity.Collections;
using Unity.Entities;
using BloodyShop.Server.Utils;

namespace BloodyShop.Server.Hooks
{

    [HarmonyPatch(typeof(ChatMessageSystem), nameof(ChatMessageSystem.OnUpdate))]
    public class ChatMessageSystem_Patch
    {
        public static void Prefix(ChatMessageSystem __instance)
        {
            if (ChatSystem.isEnabled)
            {
                if (__instance.__ChatMessageJob_entityQuery != null)
                {
                    NativeArray<Entity> entities = __instance.__ChatMessageJob_entityQuery.ToEntityArray(Allocator.Temp);
                    foreach (var entity in entities)
                    {
                        var fromData = __instance.EntityManager.GetComponentData<FromCharacter>(entity);
                        var userData = __instance.EntityManager.GetComponentData<User>(fromData.User);
                        var chatEventData = __instance.EntityManager.GetComponentData<ChatMessageEvent>(entity);

                        var messageText = chatEventData.MessageText.ToString();
                        if(messageText != "!reload")
                        {
                            if (messageText.StartsWith(ChatSystem.Prefix, System.StringComparison.Ordinal))
                            {
                                VChatEvent ev = new VChatEvent(fromData.User, fromData.Character, messageText, chatEventData.MessageType, userData);
                                ChatSystem.HandleCommands(ev);
                                __instance.EntityManager.AddComponent<DestroyTag>(entity);
                            }
                        }
                    }
                }
            }
        }
    }
}
