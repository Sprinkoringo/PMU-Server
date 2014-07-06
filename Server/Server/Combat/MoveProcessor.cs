using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;
using Server.Network;

namespace Server.Combat
{
    public class MoveProcessor
    {
        public static bool IsInAreaRange(int range, int X1, int Y1, int X2, int Y2) {
            int DistanceX = System.Math.Abs(X1 - X2);
            int DistanceY = System.Math.Abs(Y1 - Y2);

            // Are they in range?
            if (DistanceX <= range && DistanceY <= range) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsInFront(int range, Enums.Direction userDir, int userX, int userY, int targetX, int targetY) {
            switch (userDir) {
                case Enums.Direction.Down: {
                        if (userX == targetX && targetY >= userY && targetY <= userY + range) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                case Enums.Direction.Up: {
                    if (userX == targetX && targetY >= userY - range && targetY <= userY) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                case Enums.Direction.Right: {
                        if (targetX >= userX && targetX <= userX + range && userY == targetY) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                case Enums.Direction.Left: {
                        if (targetX >= userX - range  && targetX <= userX && userY == targetY) {
                            return true;
                        } else {
                            return false;
                        }
                    }
            }
            return false;
        }

        public static Enums.CharacterMatchup GetMatchupWith(ICharacter attacker, ICharacter defender) {
            if (attacker == null) return Enums.CharacterMatchup.Foe;

            if (attacker.CharacterType == Enums.CharacterType.Recruit) {
                Players.Recruit attackerRecruit = (Players.Recruit)attacker;
                //if the character is null, it is neither friend, foe, nor self
                if (defender == null) return Enums.CharacterMatchup.None;

                //if it's an NPC, it's automatically considered an enemy
                if (defender.CharacterType == Enums.CharacterType.MapNpc) return Enums.CharacterMatchup.Foe;

                //we can assume the target is a recruit now.
                Players.Recruit recruit = defender as Players.Recruit;

                //if the character is dead, it is neither friend, foe, nor self
                if (defender.CharacterType == Enums.CharacterType.Recruit && ((Players.Recruit)defender).Owner.Player.Dead) return Enums.CharacterMatchup.None;

                //if the character is oneself, then return self.  A dead self will still be considered "none"
                if (defender == attacker) return Enums.CharacterMatchup.Self;

                bool targetInArena = (recruit.Owner.Player.X > 0 && recruit.Owner.Player.X <= recruit.Owner.Player.Map.MaxX
                            && recruit.Owner.Player.Y > 0 && recruit.Owner.Player.Y <= recruit.Owner.Player.Map.MaxY
                            && recruit.Owner.Player.Map.Tile[recruit.Owner.Player.X, recruit.Owner.Player.Y].Type == Enums.TileType.Arena);

                bool selfInArena = (attackerRecruit.Owner.Player.X > 0 && attackerRecruit.Owner.Player.X <= attackerRecruit.Owner.Player.Map.MaxX
                            && attackerRecruit.Owner.Player.Y > 0 && attackerRecruit.Owner.Player.Y <= attackerRecruit.Owner.Player.Map.MaxY
                            && attackerRecruit.Owner.Player.Map.Tile[attackerRecruit.Owner.Player.X, attackerRecruit.Owner.Player.Y].Type == Enums.TileType.Arena);

                //if one character is in the arena, and the other one is not, then they can't affect each other
                if (targetInArena != selfInArena) {
                    return Enums.CharacterMatchup.None;
                }

                Players.Parties.Party party = null;
                if (attackerRecruit.Owner.Player.IsInParty()) {
                    party = Players.Parties.PartyManager.FindPlayerParty(attackerRecruit.Owner);
                }

                //if players are in the same party, they are friends, regardless of what the area or guild says
                if (party != null) {
                    if (party.IsPlayerInParty(recruit.Owner.Player.CharID)) {
                        return Enums.CharacterMatchup.Friend;
                    }
                }

                //if both players are in the arena, they are enemies, regardless of map or guild
                if (targetInArena) {
                    return Enums.CharacterMatchup.Foe;
                }

                //if both players are in the same guild, they are friends, regardless of mapmoral
                if (!string.IsNullOrEmpty(attackerRecruit.Owner.Player.GuildName)) {
                    if (!string.IsNullOrEmpty(recruit.Owner.Player.GuildName)) {
                        if (recruit.Owner.Player.GuildName == attackerRecruit.Owner.Player.GuildName) {
                            return Enums.CharacterMatchup.Friend;
                        }
                    }
                }

                //if mapmoral is safe, they are friends.  if not, they are enemies.
                if (attackerRecruit.Owner.Player.Map.Moral == Enums.MapMoral.Safe || attackerRecruit.Owner.Player.Map.Moral == Enums.MapMoral.House) {
                    return Enums.CharacterMatchup.Friend;
                } else {
                    return Enums.CharacterMatchup.Foe;
                }
            } else {

                if (defender != null) {
                    if (defender.CharacterType == Enums.CharacterType.Recruit && ((Players.Recruit)defender).Owner.Player.Dead) return Enums.CharacterMatchup.None;

                    if (defender == attacker) return Enums.CharacterMatchup.Self;

                    if (defender.CharacterType == Enums.CharacterType.Recruit && ((Players.Recruit)defender).Owner.Player.Hunted) {
                        return Enums.CharacterMatchup.Foe;
                    } else if (defender.CharacterType == Enums.CharacterType.MapNpc) {
                        return Enums.CharacterMatchup.Friend;
                    }
                }

                return Enums.CharacterMatchup.None;
            }
        }

        public static TargetCollection GetTargetsInRange(Enums.MoveRange rangeType, int range, IMap map, ICharacter user, int userX, int userY, Enums.Direction userDir, bool hitsFoes, bool hitsAllies, bool hitsSelf) {
            TargetCollection targetlist = new TargetCollection();
            
            switch (rangeType) {
                case Enums.MoveRange.Floor: {
                        #region Floor
                        foreach (Client i in map.GetClients()) {
                            if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                            }
                        }
                        for (int i = 0; i < map.ActiveNpc.Length; i++) {
                            if (map.ActiveNpc[i].Num > 0) {
                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                }
                            }
                        }
                        #endregion
                    }
                    break;
                case Enums.MoveRange.FrontOfUser: {
                        #region FrontOfUser
                        foreach (Client i in map.GetClients()) {
                            if (IsInFront(range, userDir, userX, userY, i.Player.X, i.Player.Y)) {
                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                }
                            }
                        }
                        for (int i = 0; i < map.ActiveNpc.Length; i++) {
                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                }
                            }
                        }
                        #endregion
                    }
                    break;
                case Enums.MoveRange.FrontOfUserUntil: {
                        #region FrontOfUserUntil
                        bool stopattile = false;
                        for (int r = 0; r <= range; r++) {
                            foreach (Client i in map.GetClients()) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                if (user != i.Player.GetActiveRecruit() && IsInFront(r, userDir, userX, userY, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                    stopattile = true;
                                }
                            }
                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                if (map.ActiveNpc[i].Num > 0 && user != map.ActiveNpc[i] && IsInFront(r, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                    stopattile = true;
                                }
                            }
                            if (stopattile) {
                                break;
                            }
                        }

#endregion
                    }
                    break;
                case Enums.MoveRange.Room: {
                        #region Room
                        foreach (Client i in map.GetClients()) {
                            if (IsInAreaRange(range, userX, userY, i.Player.X, i.Player.Y)) {
                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                }
                            }
                        }
                    for (int i = 0; i < map.ActiveNpc.Length; i++) {
                        if (map.ActiveNpc[i].Num > 0 && IsInAreaRange(range, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                            if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                            }
                            }
                        }
#endregion
                    }
                    break;
                case Enums.MoveRange.ArcThrow: {
                        #region ArcThrow
                        bool stopAtTile = false;
                        for (int r = 0; r <= range; r++) {
                            //check directly forward
                            foreach (Client i in map.GetClients()) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                if (user != i.Player.GetActiveRecruit() && IsInFront(r, userDir, userX, userY, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                    stopAtTile = true;
                                }
                            }

                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                if (map.ActiveNpc[i].Num > 0 && user != map.ActiveNpc[i] && IsInFront(r, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                    stopAtTile = true;
                                }
                            }
                            if (stopAtTile) {
                                break;
                            }

                            //check adjacent tiles
                            switch (userDir) {
                                case Enums.Direction.Down: {
                                        for (int s = 1; s <= r; s++) {
                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX - s, userY + r, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX - s, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }
                                            if (stopAtTile) {
                                                break;
                                            }

                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX + s, userY + r, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX + s, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            if (stopAtTile) {
                                                break;
                                            }

                                        }

                                    }
                                    break;
                                case Enums.Direction.Up: {
                                        for (int s = 1; s <= r; s++) {
                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX - s, userY - r, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX - s, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }
                                            if (stopAtTile) {
                                                break;
                                            }

                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX + s, userY - r, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX + s, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            if (stopAtTile) {
                                                break;
                                            }

                                        }

                                    }
                                    break;
                                case Enums.Direction.Left: {
                                        for (int s = 1; s <= r; s++) {
                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX - r, userY - s, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX - r, userY - s, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }
                                            if (stopAtTile) {
                                                break;
                                            }

                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX - r, userY + s, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX - r, userY + s, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            if (stopAtTile) {
                                                break;
                                            }

                                        }
                                    }
                                    break;
                                case Enums.Direction.Right: {
                                        for (int s = 1; s <= r; s++) {
                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX + r, userY - s, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX + r, userY - s, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }
                                            if (stopAtTile) {
                                                break;
                                            }

                                            foreach (Client i in map.GetClients()) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                                if (IsInFront(0, userDir, userX + r, userY + s, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                                if (map.ActiveNpc[i].Num > 0 && IsInFront(0, userDir, userX + r, userY + s, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                                    stopAtTile = true;
                                                }
                                            }

                                            if (stopAtTile) {
                                                break;
                                            }

                                        }
                                    }
                                    break;
                            }

                            if (stopAtTile) {
                                break;
                            }

                        }
                        #endregion
                    }
                    break;
                case Enums.MoveRange.StraightLine: {
                        #region StraightLine
                        foreach (Client i in map.GetClients()) {
                            if (IsInFront(range, userDir, userX, userY, i.Player.X, i.Player.Y)) {
                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                }
                            }
                        }
                        for (int i = 0; i < map.ActiveNpc.Length; i++) {
                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                }
                            }
                        }
#endregion
                    }
                    break;
                case Enums.MoveRange.FrontAndSides: {
                        #region FrontAndSides
                        foreach (Client i in map.GetClients()) {
                            if (IsInFront(range, userDir, userX, userY, i.Player.X, i.Player.Y)) {
                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                }
                            }
                            switch (userDir) {
                                case Enums.Direction.Down: {
                                        for (int r = 1; r <= range; r++) {
                                            if (IsInFront(range - r, userDir, userX + r, userY + r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                            if (IsInFront(range - r, userDir, userX - r, userY + r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                        }

                                    }
                                    break;
                                case Enums.Direction.Up: {
                                        for (int r = 1; r <= range; r++) {
                                            if (IsInFront(range - r, userDir, userX + r, userY - r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                            if (IsInFront(range - r, userDir, userX - r, userY - r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                        }

                                    }
                                    break;
                                case Enums.Direction.Left: {
                                        for (int r = 1; r <= range; r++) {
                                            if (IsInFront(range - r, userDir, userX - r, userY + r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                            if (IsInFront(range - r, userDir, userX - r, userY - r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case Enums.Direction.Right: {
                                        for (int r = 1; r <= range; r++) {
                                            if (IsInFront(range - r, userDir, userX + r, userY + r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                            if (IsInFront(range - r, userDir, userX + r, userY - r, i.Player.X, i.Player.Y)) {
                                                if (user == i.Player.GetActiveRecruit() && hitsSelf || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, i.Player.GetActiveRecruit()) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(i.Player.GetActiveRecruit(), GetMatchupWith(user, i.Player.GetActiveRecruit()));
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        for (int i = 0; i < map.ActiveNpc.Length; i++) {
                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                }
                            }
                            switch (userDir) {
                                case Enums.Direction.Down: {
                                        for (int r = 1; r <= range; r++) {
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX + r, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX - r, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                        }

                                    }
                                    break;
                                case Enums.Direction.Up: {
                                        for (int r = 1; r <= range; r++) {
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX + r, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX - r, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                        }

                                    }
                                    break;
                                case Enums.Direction.Left: {
                                        for (int r = 1; r <= range; r++) {
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX - r, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX - r, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case Enums.Direction.Right: {
                                        for (int r = 1; r <= range; r++) {
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX + r, userY + r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                            if (map.ActiveNpc[i].Num > 0 && IsInFront(range - r, userDir, userX + r, userY - r, map.ActiveNpc[i].X, map.ActiveNpc[i].Y)) {
                                                if (user == map.ActiveNpc[i] && hitsSelf || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Friend && hitsAllies || GetMatchupWith(user, map.ActiveNpc[i]) == Enums.CharacterMatchup.Foe && hitsFoes) {
                                                    targetlist.Add(map.ActiveNpc[i], GetMatchupWith(user, map.ActiveNpc[i]));
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

#endregion
                    }
                    break;
                case Enums.MoveRange.LineUntilHit: {
                        #region LineUntilHit
                        bool stopattile = false;
                        for (int r = 0; r <= range; r++) {
                            foreach (Client i in map.GetClients()) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, i.Player.GetActiveRecruit());
                                if (user != i.Player.GetActiveRecruit() && IsInFront(r, userDir, userX, userY, i.Player.X, i.Player.Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(i.Player.GetActiveRecruit(), matchup);
                                    stopattile = true;
                                }
                            }
                            for (int i = 0; i < map.ActiveNpc.Length; i++) {
                                Enums.CharacterMatchup matchup = GetMatchupWith(user, map.ActiveNpc[i]);
                                if (map.ActiveNpc[i].Num > 0 && user != map.ActiveNpc[i] && IsInFront(r, userDir, userX, userY, map.ActiveNpc[i].X, map.ActiveNpc[i].Y) &&
                                    (matchup == Enums.CharacterMatchup.Friend && hitsAllies || matchup == Enums.CharacterMatchup.Foe && hitsFoes)) {
                                    targetlist.Add(map.ActiveNpc[i], matchup);
                                    stopattile = true;
                                }
                            }
                            if (stopattile) {
                                break;
                            }
                        }

                        #endregion
                    }
                    break;
                case Enums.MoveRange.User: {
                        targetlist.Add(user, Enums.CharacterMatchup.Self);
                    }
                    break;
                case Enums.MoveRange.Special: {
                        targetlist.Add(null, Enums.CharacterMatchup.None);
                    }
                    break;
            }

            return targetlist;
        }

        public static bool IsInRange(Enums.MoveRange rangeType, int range, int userX, int userY, Enums.Direction userDir, int targetX, int targetY) {
            switch (rangeType) {
                case Enums.MoveRange.Floor: {
                        return true;
                    }
                case Enums.MoveRange.FrontOfUser:
                // We don't have diagnols, so we can't cut corners. Ignore corners for this range type
                case Enums.MoveRange.FrontOfUserUntil: {
                        return IsInFront(range, userDir, userX, userY, targetX, targetY);
                    }
                case Enums.MoveRange.Room: {
                        return IsInAreaRange(range, userX, userY, targetX, targetY);
                    }
                case Enums.MoveRange.ArcThrow: {
                        switch (userDir) {
                            case Enums.Direction.Down:
                            case Enums.Direction.Up: {
                                    if (IsInFront(range, userDir, userX, userY, targetX, targetY)) {
                                        return true;
                                    }
                                    for (int i = 1; i <= range; i++) {
                                        if (IsInFront(range - i, userDir, userX + i, userY, targetX, targetY)) {
                                            return true;
                                        }
                                        if (IsInFront(range - i, userDir, userX - i, userY, targetX, targetY)) {
                                            return true;
                                        }
                                    }

                                }
                                break;
                            case Enums.Direction.Left:
                            case Enums.Direction.Right: {
                                    if (IsInFront(range, userDir, userX, userY, targetX, targetY)) {
                                        return true;
                                    }
                                    for (int i = 1; i <= range; i++) {
                                        if (IsInFront(range - i, userDir, userX, userY + i, targetX, targetY)) {
                                            return true;
                                        }
                                        if (IsInFront(range - i, userDir, userX, userY - i, targetX, targetY)) {
                                            return true;
                                        }
                                    }
                                }
                                break;
                        }
                        return false;
                    }
                case Enums.MoveRange.StraightLine: {
                        return IsInFront(range, userDir, userX, userY, targetX, targetY);
                    }
                case Enums.MoveRange.FrontAndSides: {
                        switch (userDir) {
                            case Enums.Direction.Down:
                            case Enums.Direction.Up: {
                                    if (IsInFront(range, userDir, userX, userY, targetX, targetY)) {
                                        return true;
                                    }
                                    for (int i = 1; i <= range; i++) {
                                        if (IsInFront(range - i, userDir, userX + i, userY, targetX, targetY)) {
                                            return true;
                                        }
                                        if (IsInFront(range - i, userDir, userX - i, userY, targetX, targetY)) {
                                            return true;
                                        }
                                    }

                                }
                                break;
                            case Enums.Direction.Left:
                            case Enums.Direction.Right: {
                                    if (IsInFront(range, userDir, userX, userY, targetX, targetY)) {
                                        return true;
                                    }
                                    for (int i = 1; i <= range; i++) {
                                        if (IsInFront(range - i, userDir, userX, userY + i, targetX, targetY)) {
                                            return true;
                                        }
                                        if (IsInFront(range - i, userDir, userX, userY - i, targetX, targetY)) {
                                            return true;
                                        }
                                    }
                                }
                                break;
                        }
                        return false;
                    }
                case Enums.MoveRange.LineUntilHit: {
                        return IsInFront(2, userDir, userX, userY, targetX, targetY);
                    }
                case Enums.MoveRange.User: {
                        return true;
                    }
                case Enums.MoveRange.Special: {
                        return true;
                    }
                default: {
                        return false;
                    }
            }
        }

        public static RDungeons.RDungeonRoom GetTargetRoom(Maps.IMap map, int targetX, int targetY) {
            int x1 = targetX;
            int y1 = targetY;
            int blockedCount = 0;

            int leftXDistance = -1;
            int rightXDistance = -1;

            int upYDistance = -1;
            int downYDistance = -1;

            int roomStartX = -1;
            int roomWidth = -1;

            int roomStartY = -1;
            int roomHeight = -1;
            while (true) {
                // Keep going left until we've hit a wall...
                x1--;
                blockedCount = 0;
                if (x1 < 0) {
                    roomStartX = 0;
                    break;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY - 1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY + 1)) {
                    blockedCount++;
                }
                if (blockedCount == 3 || blockedCount == 2) {
                    // This means that a hallway was found between blocks!
                    leftXDistance = x1;
                    roomStartX = x1;
                    break;
                }
            }
            x1 = targetX;
            while (true) {
                // Keep going right until we've hit a wall...
                x1++;
                blockedCount = 0;
                if (x1 > map.MaxX) {
                    roomWidth = map.MaxX - roomStartX;
                    break;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY - 1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, x1, targetY + 1)) {
                    blockedCount++;
                }
                if (blockedCount == 3 || blockedCount == 2) {
                    // This means that a hallway was found between blocks!
                    rightXDistance = x1;
                    roomWidth = (x1 - targetX) + (targetX - roomStartX);
                    break;
                }
            }

            while (true) {
                // Keep going up until we've hit a wall...
                y1--;
                blockedCount = 0;
                if (y1 < 0) {
                    roomStartY = 0;
                    break;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX, y1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX - 1, y1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX + 1, y1)) {
                    blockedCount++;
                }
                if (blockedCount == 3 || blockedCount == 2) {
                    // This means that a hallway was found between blocks!
                    upYDistance = y1;
                    roomStartY = y1;
                    break;
                }
            }
            y1 = targetY;
            while (true) {
                // Keep going down until we've hit a wall...
                y1++;
                blockedCount = 0;
                if (y1 > map.MaxY) {
                    roomHeight = map.MaxY - roomStartY;
                    break;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX, y1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX - 1, y1)) {
                    blockedCount++;
                }
                if (AI.MovementProcessor.IsBlocked(map, targetX + 1, y1)) {
                    blockedCount++;
                }
                if (blockedCount == 3 || blockedCount == 2) {
                    // This means that a hallway was found between blocks!
                    downYDistance = y1;
                    roomHeight = (y1 - targetY) + (targetY - roomStartY);
                    break;
                }
            }

            return new RDungeons.RDungeonRoom(roomStartX, roomStartY, roomWidth, roomHeight);
        }

    }
}
