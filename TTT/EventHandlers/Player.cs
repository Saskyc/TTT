using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using LiteNetLib4Mirror.Open.Nat;
using MEC;
using PlayerRoles;
using System.Security.Policy;
using System.Xml.Linq;
using TTT.Methods;
using UnityEngine;

namespace TTT.EventHandlers
{
    public static class Player
    {
        public static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.EnableEffect(EffectType.Scanned, 5, 5);

            ev.Player.DisplayNickname = $"{ev.Player.Nickname} {ev.Player.Id}";

            RoleTypeId Skin = ev.Player.GetRandomSkin();
            ev.Player.SetStatistics(Skin);

            if (TTT.Roles_Distributed)
                return;

            ev.Player.Role.Set(Skin);

            ev.Player.Position = Room.Get(RoomType.Hcz106).Position;
        }

        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!TTT.Roles_Distributed)
                return;

            /*
            if (!TTT.ReadSplashScreen[ev.Player])
                TTT.ReadSplashScreen[ev.Player] = true;
                return;
            */

            /*
            if (TTT.GameRole[ev.Player] == "Innocent")
                return;
            */

            Exiled.API.Features.Player PLR = ev.Player;

            if (!TTT.ConfirmAbility[PLR])
            {
                TTT.ConfirmAbility[PLR] = true;
                PLR.Broadcast(3, "<size=35><b>Safety lock\n<size=25>Click again to activate the ability</size></b>");

                Timing.CallDelayed(1.5F, () =>
                {
                    TTT.ConfirmAbility[PLR] = false;
                    return;
                });
            }

            if (TTT.NextAbilityUse[PLR] == null)
                return;

            if ((float)Exiled.API.Features.Round.ElapsedTime.TotalSeconds <= TTT.NextAbilityUse[PLR])
            {
                float MyVar = TTT.NextAbilityUse[PLR] - (float)Exiled.API.Features.Round.ElapsedTime.TotalSeconds;
                MyVar = Mathf.Ceil(MyVar);
                PLR.Broadcast(3, $"<size=35><b>Ability: <color=red>Failure</color>\n<size=25>Wait {MyVar} more seconds to use the ability</b></size>");
                return;
            }

            if (TTT.UsingAbility[PLR])
                return;

            TTT.UsingAbility[PLR] = true;
            //TTT.AbilityActivated(PLR);
            TTT.UsingAbility[PLR] = false;
        }

        public static void OnJumping(JumpingEventArgs ev)
        {
            ev.Player.Stamina = ev.Player.Stamina - 0.3F;
            ev.Player.EnableEffect(EffectType.Disabled, 1, 0.5f);
            return;
        }

        public static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {

            if (!$"{ev.Player.CurrentItem}".Contains("Keycard"))
                return;

            if (!(ev.Door.Type == DoorType.Scp106Primary || ev.Door.Type == DoorType.HID || ev.Door.Type == DoorType.HczArmory || ev.Door.Type == DoorType.Scp096))
                return;

            if(ev.Player.CurrentItem.Type == ItemType.KeycardJanitor)
                ev.Player.RemoveItem(ev.Player.CurrentItem);

            if (ev.Door is IDamageableDoor breaker && !breaker.IsDestroyed)
                breaker.Break();
        }

        public static void OnHurt(HurtEventArgs ev)
        {
            //ev.Player.SetCustomInfo();

            if (ev.Attacker == null)
                return;

            if(ev.Attacker.CurrentItem.Type == ItemType.GunCOM15)
            {
                ev.Player.EnableEffect(EffectType.AmnesiaVision, 1, 15);

                ev.Player.EnableEffect(EffectType.Poisoned, 1, 15);

                ev.Player.EnableEffect(EffectType.AmnesiaItems, 1, 10);

                return;
            }

            if (ev.Attacker.CurrentItem.Type == ItemType.GunCOM18)
            {
                ev.Player.EnableEffect(EffectType.Blinded, 1, 0.5f);

                ev.Player.EnableEffect(EffectType.Concussed, 1, 0.5f);

                return;
            }
        }

        public static void OnDying(DyingEventArgs ev)
        {
            if (!TTT.Roles_Distributed)
                return;

            if (TTT.GameRole[ev.Player] == "Dead")
                return;

            TTT.GameRole[ev.Player] = "Dead";
            TTT.TimeOfDeath[ev.Player] = (float)Exiled.API.Features.Round.ElapsedTime.TotalSeconds;
            
            foreach(Exiled.API.Features.Player plr in Exiled.API.Features.Player.List)
            {
                if (TTT.GameRole[plr] == "Detective")
                    plr.EnableEffect(EffectType.Scanned, 5, 5);
            }

            if(ev.Attacker == ev.Player || ev.Player == null)
            {
                //Suicide
                return;
            }

            if (TTT.GameTeam[ev.Player] == TTT.GameTeam[ev.Attacker])
            {
                //Teamkill
                Log.Info($"-------------\n{ev.Attacker}\n{TTT.OriginalRole[ev.Attacker]}\n<color=red>has TEAMKILLED</color>\n{ev.Player.Nickname}\n{TTT.OriginalRole[ev.Player]}\n-------------");
                TTT.Teamkills[ev.Attacker] = TTT.Teamkills[ev.Attacker] + 1;
                Log.Info($"{ev.Attacker.Nickname} has now {TTT.Teamkills[ev.Attacker]} teamkills");

                if (!(TTT.Teamkills[ev.Attacker] >= 3))
                    return;

                Log.Info($"{ev.Attacker.Nickname} {ev.Attacker.UserId} has teamkilled {TTT.Teamkills[ev.Attacker]} players. This player has been kicked.");
                foreach(Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
                {
                    ev.Player.Broadcast(5, $"<size=25><b>{ev.Attacker.DisplayNickname} has been kicked by the automatic teamkill prevention system.");
                    ev.Player.Kick("You have been kicked by the automatic teamkill prevention system. You have teamkilled 3 players. Our staff has been notified of this incident.");
                }

                return;
            }

            // Normal kill
            return;
            /*

IF {GET:EVATTACKER:Teamkills} = 2
    MAXHP {EVATTACKER} 70
    HP {EVATTACKER} 70 $IF {GET:EVATTACKER:HP} > 70
    BROADCAST {EVATTACKER} 5 <size=35><b>You have teamkilled 2 players.\n<size=25>If you teamkill once again, you will be <u>kicked from the server</u>.
ENDIF

TEMP {GET:EVATTACKER:HP} - 10
HP {EVATTACKER} {@}
DAMAGE {EVATTACKER} 10 You have teamkilled a player and lost 10 HP as result. Since you had less than 10 HP, you died.

GOTO FetchXp
APPEND {@} - 120
PDATA SET {EVATTACKER} XP {@}
STOP


NormalKill:
FASTCALL Log {GET:EVATTACKER} ({GET:EVATTACKER:OriginalRole}) has killed {GET:PLR} ({GET:PLR:OriginalRole})

TEMP {GET:EVATTACKER:Kills} + 1
PDATA SET {EVATTACKER} Kills {@}

GOTO FetchXp
APPEND {@} + 60
PDATA SET {EVATTACKER} XP {@}

FASTCALL Log {GET:EVATTACKER:NAME} has now {GET:EVATTACKER:Kills} kills.
STOP


Sucide:
FASTCALL Log {GET:PLR} ({GET:PLR:OriginalRole}) comitted suicide.
STOP


-> FetchXp
TEMP {GET:EVATTACKER:XP}
TEMP 0 $IF {@} = UNDEFINED
<-
            */
        }
    }
}
