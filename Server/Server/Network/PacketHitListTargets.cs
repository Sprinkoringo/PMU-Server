using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Network
{
    public enum PacketHitListTargets
    {
        Client,
        All,
        Map,
        RangedMap,
        MapBut,
        RangedMapBut,
        SurroundingPlayersBut,
        SurroundingPlayers,
        PlayersInSightBut,
        Party,
        Others
    }
}
