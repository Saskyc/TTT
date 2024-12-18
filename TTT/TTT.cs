using Exiled.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;

namespace TTT
{
    public class TTT : Plugin<Config>
    {
        public static TTT Instance;

        public static Dictionary<Player, int> Teamkills = new();

        public static Dictionary<Player, int> Kills = new();

        public static Dictionary<Player, int> SecondsInWarhead = new();

        public static Dictionary<Player, RoleTypeId> Skin = new();

        public static Dictionary<Player, string> GameRole = new();

        public static Dictionary<Player, bool> ConfirmAbility = new();

        public static Dictionary<Player, float> NextAbilityUse = new();

        public static Dictionary<Player, bool> UsingAbility = new();

        public static Dictionary<Player, float> TimeOfDeath = new();

        public static Dictionary<Player, string> GameTeam = new();

        public static Dictionary<Player, string> OriginalRole = new();
        
        public static bool Roles_Distributed = false;

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.Verified += EventHandlers.Player.OnVerified;

            Exiled.Events.Handlers.Player.TogglingNoClip += EventHandlers.Player.OnTogglingNoClip;

            Exiled.Events.Handlers.Player.Jumping += EventHandlers.Player.OnJumping;

            Exiled.Events.Handlers.Player.InteractingDoor += EventHandlers.Player.OnInteractingDoor;

            Exiled.Events.Handlers.Player.Hurt += EventHandlers.Player.OnHurt;

            Exiled.Events.Handlers.Player.Dying += EventHandlers.Player.OnDying;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.Verified -= EventHandlers.Player.OnVerified;

            Exiled.Events.Handlers.Player.TogglingNoClip -= EventHandlers.Player.OnTogglingNoClip;

            Exiled.Events.Handlers.Player.Jumping -= EventHandlers.Player.OnJumping;

            Exiled.Events.Handlers.Player.InteractingDoor -= EventHandlers.Player.OnInteractingDoor;

            Exiled.Events.Handlers.Player.Hurt -= EventHandlers.Player.OnHurt;

            Exiled.Events.Handlers.Player.Dying -= EventHandlers.Player.OnDying;

            base.OnDisabled();
        }
    }
}
