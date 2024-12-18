using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT.Methods
{
    public static class Methods
    {
        public static RoleTypeId GetRandomSkin(this Player p)
        {
            List<RoleTypeId> mySkins = [RoleTypeId.Tutorial, RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.NtfCaptain, RoleTypeId.ChaosMarauder, RoleTypeId.FacilityGuard];
            Random r = new Random();
            int index = r.Next(mySkins.Count);
            RoleTypeId Skin = mySkins[index];
            return Skin;
        }

        public static void SetStatistics(this Player p, RoleTypeId Skin)
        {
            TTT.Teamkills[p] = 0;
            TTT.Kills[p] = 0;
            TTT.SecondsInWarhead[p] = 0;
            TTT.Skin[p] = Skin;
        }
    }
}
