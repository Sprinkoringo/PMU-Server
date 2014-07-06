/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


namespace Script {
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Server;
    using Server.Maps;
    using Server.Players;
    using Server.RDungeons;
    using Server.Combat;
    using Server.Pokedex;
    using Server.Items;
    using Server.Moves;
    using Server.Npcs;
    using Server.Stories;
    using Server.Exp;
    using Server.Network;
    using PMU.Sockets;
    using Server.Players.Parties;
    using Server.Logging;
    using Server.Missions;
    using Server.Events.Player.TriggerEvents;
    using Server.WonderMails;
    using Server.Tournaments;

    public partial class Main {

        public static void RemoveBuffs(ICharacter character) {
            character.AttackBuff = 0;
            character.DefenseBuff = 0;
            character.SpeedBuff = 0;
            character.SpAtkBuff = 0;
            character.SpDefBuff = 0;
            character.AccuracyBuff = 0;
            character.EvasionBuff = 0;
        }

        public static void ChangeAttackBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeAttackBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeAttackBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                
                //twist band
                if (character.HasActiveItem(172)) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Twist Band prevented its Attack stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Hyper Cutter") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Hyper Cutter prevented its Attack from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.AttackBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.AttackBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.AttackBuff + counter > 10) {
                counter = 10 - character.AttackBuff;
            } else if (character.AttackBuff + counter < -10) {
                counter = -10 - character.AttackBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Attack fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.AttackBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeDefenseBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeDefenseBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeDefenseBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                
                //spin band
                if (character.HasActiveItem(719)) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Spin Band prevented its Defense stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Big Pecks") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Big Pecks prevented its Defense from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.DefenseBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.DefenseBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.DefenseBuff + counter > 10) {
                counter = 10 - character.DefenseBuff;
            } else if (character.DefenseBuff + counter < -10) {
                counter = -10 - character.DefenseBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Defense fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.DefenseBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeSpAtkBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeSpAtkBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeSpAtkBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                
                //twist band
                if (character.HasActiveItem(172)) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Twist Band prevented its Attack stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }


            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.SpAtkBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.SpAtkBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.SpAtkBuff + counter > 10) {
                counter = 10 - character.SpAtkBuff;
            } else if (character.SpAtkBuff + counter < -10) {
                counter = -10 - character.SpAtkBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Attack fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.SpAtkBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeSpDefBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeSpDefBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeSpDefBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                
                //spin band
                if (character.HasActiveItem(719)) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Spin Band prevented its Defense stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.SpDefBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.SpDefBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.SpDefBuff + counter > 10) {
                counter = 10 - character.SpDefBuff;
            } else if (character.SpDefBuff + counter < -10) {
                counter = -10 - character.SpDefBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Special Defense fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.SpDefBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeSpeedBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeSpeedBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeSpeedBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.SpeedBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.SpeedBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.SpeedBuff + counter > 10) {
                counter = 10 - character.SpeedBuff;
            } else if (character.SpeedBuff + counter < -10) {
                counter = -10 - character.SpeedBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Speed fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.SpeedBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeAccuracyBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeAccuracyBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeAccuracyBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Keen Eye") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Keen Eye prevented its Accuracy from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.AccuracyBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.AccuracyBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.AccuracyBuff + counter > 10) {
                counter = 10 - character.AccuracyBuff;
            } else if (character.AccuracyBuff + counter < -10) {
                counter = -10 - character.AccuracyBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Accuracy fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.AccuracyBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }

        public static void ChangeEvasionBuff(ICharacter character, IMap map, int counter, PacketHitList hitlist) {
            ChangeEvasionBuff(character, null, map, counter, hitlist);
        }

        public static void ChangeEvasionBuff(ICharacter character, ICharacter attacker, IMap map, int counter, PacketHitList hitlist) {
            if (counter < 0) {
                if (character.VolatileStatus.GetStatus("Mist") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stats didn't drop due to the mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
                if (attacker != character && character.VolatileStatus.GetStatus("Substitute") != null) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "White Smoke") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s White Smoke prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }

                if (HasAbility(character, "Clear Body") && !HasAbility(attacker, "Mold Breaker")) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Clear Body prevented its stats from dropping!", Text.WhiteSmoke), character.X, character.Y, 10);
                    return;
                }
            }

            if (HasAbility(character, "Contrary") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= -1;
            }

            if (HasAbility(character, "Simple") && !HasAbility(attacker, "Mold Breaker")) {
                counter *= 2;
            }

            if (character.EvasionBuff >= 10 && counter > 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion won't go higher!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            } else if (character.EvasionBuff <= -10 && counter < 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion won't go lower!", Text.WhiteSmoke), character.X, character.Y, 10);
                return;
            }

            if (character.EvasionBuff + counter > 10) {
                counter = 10 - character.EvasionBuff;
            } else if (character.EvasionBuff + counter < -10) {
                counter = -10 - character.EvasionBuff;
            }

            if (counter == 0) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion didn't change.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion rose slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -1) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion fell slightly.", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion rose sharply!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter == -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion fell harshly!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter > 2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion rose drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else if (counter < -2) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Evasion fell drastically!", Text.WhiteSmoke), character.X, character.Y, 10);
            }
            character.EvasionBuff += counter;

            if (counter < 0 && HasAbility(character, "Defiant")) {
                //hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " defied the stat change!", Text.WhiteSmoke), character.X, character.Y, 10);
                ChangeAttackBuff(character, map, 2, hitlist);
            }

        }
        
        public static void Flinch(ICharacter character, IMap map, PacketHitList hitlist) {
        	Flinch(character, null, map, hitlist);
        }
        
        public static void Flinch(ICharacter character, ICharacter attacker, IMap map, PacketHitList hitlist) {
        	if (!CheckStatusProtection(character, attacker, map, "Cringe", true, hitlist)) {
                if (character.AttackTimer == null || character.AttackTimer.Tick < Core.GetTickCount().Tick) {
                    character.AttackTimer = new TickCount(Core.GetTickCount().Tick);
                }
                character.AttackTimer = new TickCount(character.AttackTimer.Tick + 2000);
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " cringed!", Text.BrightRed), character.X, character.Y, 10);
            }
            if (HasAbility(character, "Steadfast")) {
            	ChangeSpeedBuff(character, map, 1, hitlist);
            }
        }

        //public static void Confuse(ICharacter character, IMap map, int confusionSteps, PacketHitList hitlist) {
        //    Confuse(character, null, map, confusionSteps, hitlist);
        //}

        //public static void Confuse(ICharacter character, ICharacter attacker, IMap map, int confusionSteps, PacketHitList hitlist) {
        //    PacketHitList.MethodStart(ref hitlist);

		//	bool skip = false;
        //    if (CheckStatusProtection(character, attacker, map, "Confusion:" + confusionSteps, true, hitlist)) {
        //        skip = true;
        //    }
		//	if (!skip) {
	    //        IMap characterMap = MapManager.RetrieveActiveMap(character.MapID);
	    //        if (character.CharacterType == Enums.CharacterType.Recruit) {
	    //            if (confusionSteps <= 0) {
	    //                ((Recruit)character).Owner.Player.Confusion = 0;
	    //                PacketBuilder.AppendConfusion(((Recruit)character).Owner, hitlist);
	    //                hitlist.AddPacketToMap(characterMap, PacketBuilder.CreateBattleMsg(character.Name + " recovered from confusion!", Text.BrightCyan), character.X, character.Y, 10);
	
	    //            } else {
	    //                ((Recruit)character).Owner.Player.Confusion = confusionSteps;
	    //                PacketBuilder.AppendConfusion(((Recruit)character).Owner, hitlist);
	    //                hitlist.AddPacketToMap(characterMap, PacketBuilder.CreateBattleMsg(character.Name + " became confused!", Text.BrightRed), character.X, character.Y, 10);
	
	    //            }
	    //        } else {
	    //            if (confusionSteps <= 0) {
	    //                ((MapNpc)character).Confused = 0;
	    //                PacketBuilder.AppendNpcConfusion(characterMap, hitlist, ((MapNpc)character).MapSlot);
	    //                hitlist.AddPacketToMap(characterMap, PacketBuilder.CreateBattleMsg(character.Name + " recovered from confusion!", Text.BrightCyan), character.X, character.Y, 10);
	    //            } else {
	    //                ((MapNpc)character).Confused = confusionSteps;
	    //                PacketBuilder.AppendNpcConfusion(characterMap, hitlist, ((MapNpc)character).MapSlot);
	    //                hitlist.AddPacketToMap(characterMap, PacketBuilder.CreateBattleMsg(character.Name + " became confused!", Text.BrightRed), character.X, character.Y, 10);
	
	    //            }
	    //        }
        //    }
            
        //    if (confusionSteps > 0) {
            	
	            //if (HasAbility(character, "Tangled Feet")) {
	            //	ChangeEvasionBuff(character, map, 2, hitlist);
	            //}
        //    }

        //    PacketHitList.MethodEnded(ref hitlist);
        //}

        public static void SetStatusAilment(ICharacter character, IMap map, Enums.StatusAilment statusAilment, int counter, PacketHitList hitlist)//only for active recruits
        {
            SetStatusAilment(character, null, map, statusAilment, counter, hitlist);
        }

        public static void SetStatusAilment(ICharacter character, ICharacter attacker, IMap map, Enums.StatusAilment statusAilment, int counter, PacketHitList hitlist)//only for active recruits
        {
            PacketHitList.MethodStart(ref hitlist);


            //Check against status protection


            if (CheckStatusProtection(character, attacker, map, statusAilment.ToString() + ":" + counter, true, hitlist)) {
                PacketHitList.MethodEnded(ref hitlist);
                return;
            }


            switch (statusAilment) {
                case Enums.StatusAilment.OK: {
                        switch (character.StatusAilment) {
                            case Enums.StatusAilment.Burn: {

                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " recovered from the burn!", Text.BrightCyan), character.X, character.Y, 10);
                                }
                                break;
                            case Enums.StatusAilment.Freeze: {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " thawed out!", Text.BrightCyan), character.X, character.Y, 10);
                                }
                                break;
                            case Enums.StatusAilment.Paralyze: {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " recovered from paralysis!", Text.BrightCyan), character.X, character.Y, 10);
                                }
                                break;
                            case Enums.StatusAilment.Poison: {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " recovered from the poisoning!", Text.BrightCyan), character.X, character.Y, 10);
                                }
                                break;
                            case Enums.StatusAilment.Sleep: {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " woke up!", Text.BrightCyan), character.X, character.Y, 10);
                                }
                                break;
                        }
                        character.StatusAilment = Enums.StatusAilment.OK;
                        character.StatusAilmentCounter = 0;

                    }
                    break;
                case Enums.StatusAilment.Burn: {
                        character.StatusAilment = Enums.StatusAilment.Burn;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " sustained a burn!", Text.BrightRed), character.X, character.Y, 10);
                    }
                    break;
                case Enums.StatusAilment.Freeze: {
                        character.StatusAilment = Enums.StatusAilment.Freeze;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was frozen solid!", Text.BrightRed), character.X, character.Y, 10);
                    }
                    break;
                case Enums.StatusAilment.Paralyze: {
                        character.StatusAilment = Enums.StatusAilment.Paralyze;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became paralyzed!", Text.BrightRed), character.X, character.Y, 10);
                    }
                    break;
                case Enums.StatusAilment.Poison: {
                        if (counter > 1) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was badly poisoned!", Text.BrightRed), character.X, character.Y, 10);
                        } else if (counter == 1) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was poisoned!", Text.BrightRed), character.X, character.Y, 10);
                        }
                        character.StatusAilment = Enums.StatusAilment.Poison;
                        character.StatusAilmentCounter = counter;
                    }
                    break;
                case Enums.StatusAilment.Sleep: {
                        character.StatusAilment = Enums.StatusAilment.Sleep;
                        character.StatusAilmentCounter = counter;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " fell asleep!", Text.BrightRed), character.X, character.Y, 10);
                    }
                    break;
            }

            if (character.VolatileStatus.GetStatus("Yawn") != null) {
                RemoveExtraStatus(character, map, "Yawn", hitlist);
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendStatusAilment(((Recruit)character).Owner, hitlist);
            } else {
                PacketBuilder.AppendNpcStatusAilment((MapNpc)character, hitlist);
            }

            RefreshCharacterTraits(character, map, hitlist);
            
            if (attacker != null && statusAilment != Enums.StatusAilment.OK) {
	            if (HasAbility(character, "Synchronize")) {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Synchronize shared the status problem!", Text.BrightRed), character.X, character.Y, 10);
	                SetStatusAilment(attacker, map, statusAilment, counter, hitlist);
	            }
            }
            

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static bool CheckStatusProtection(ICharacter character, IMap map, string status, bool msg, PacketHitList hitlist) {
            return CheckStatusProtection(character, null, map, status, msg, hitlist);
        }
        
        public static bool CheckStatusProtection(ICharacter character, ICharacter attacker, IMap map, string status, bool msg, PacketHitList hitlist) {
        	return CheckStatusProtection(character, attacker, map, status, "", msg, hitlist);
        }

        public static bool CheckStatusProtection(ICharacter character, ICharacter attacker, IMap map, string status, string tag, bool msg, PacketHitList hitlist) {
            try {
            PacketHitList.MethodStart(ref hitlist);
            ExtraStatus checkStatus;
            string[] statusParam = status.Split(':');

            //if (statusParam[0] == "Confusion" && statusParam[1].ToInt() <= 0) {
            //    if (character.CharacterType == Enums.CharacterType.Recruit && ((Recruit)character).Owner.Player.Confusion <= 0) {
            //        PacketHitList.MethodEnded(ref hitlist);
            //        return true;
            //    } else if (character.CharacterType == Enums.CharacterType.MapNpc && !((MapNpc)character).Confused) {
            //        PacketHitList.MethodEnded(ref hitlist);
            //        return true;
            //    }

            //    if (character.HasActiveItem(347)) {
            //        if (msg) {
            //            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't recover from confusion due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
            //        }
            //        PacketHitList.MethodEnded(ref hitlist);
            //        return true;
            //    }
            //}
            
            if (IsStatusBad(status)) {
            	if (character.VolatileStatus.GetStatus("Safeguard") != null && (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0)))) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Safeguard!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                        //pokemon specific item
                        if (HasActiveBagItem(character, 4, -1, (int)GetCharacterWeather(character)) || HasActiveBagItem(character, 4, (int)GetCharacterWeather(character), -1)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by its item!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }

                        //Type Gems
                        if (character.HasActiveItem(595) && GetCharacterWeather(character) == Enums.Weather.Cloudy) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the White Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(596) && GetCharacterWeather(character) == Enums.Weather.Sunny) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Fiery Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(597) && (GetCharacterWeather(character) == Enums.Weather.Raining || GetCharacterWeather(character) == Enums.Weather.Thunder)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Aqua Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(600) && GetCharacterWeather(character) == Enums.Weather.Hail) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Icy Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(602) && GetCharacterWeather(character) == Enums.Weather.Cloudy) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Poison Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(603) && GetCharacterWeather(character) == Enums.Weather.Sandstorm) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Earth Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(605) && GetCharacterWeather(character) == Enums.Weather.Fog) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Psyche Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(606) && (GetCharacterWeather(character) == Enums.Weather.None || GetCharacterWeather(character) == Enums.Weather.Ambiguous)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Guard Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(609) && (GetCharacterWeather(character) == Enums.Weather.None || GetCharacterWeather(character) == Enums.Weather.Ambiguous)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Dragon Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(610) && GetCharacterWeather(character) == Enums.Weather.Fog) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Dark Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }
                        if (character.HasActiveItem(611) && GetCharacterWeather(character) == Enums.Weather.Sandstorm) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by the Metal Gem!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;

                        }

                        if (HasAbility(character, "Leaf Guard") && GetCharacterWeather(character) == Enums.Weather.Sunny && !HasAbility(attacker, "Mold Breaker")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Leaf Guard!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (attacker != character && statusParam[0] != "Pause" && statusParam[0] != "Taunt" &&
                            statusParam[0] != "Torment" && statusParam[0] != "Encore" && statusParam[0] != "MoveSeal" &&
                            statusParam[0] != "Disable" && statusParam[0] != "PerishCount" && statusParam[0] != "Curse" &&
                            statusParam[0] != "Attract" && statusParam[0] != "Grounded" && character.VolatileStatus.GetStatus("Substitute") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by Substitute!", Text.BrightCyan), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
            }


            switch (statusParam[0]) {
                case "OK": {
                        if (character.StatusAilment == Enums.StatusAilment.OK) {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(347)) {
                            if (msg) {
                                switch (character.StatusAilment) {
                                    case Enums.StatusAilment.Burn: {
                                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't recover from the burn due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                                        }
                                        break;
                                    case Enums.StatusAilment.Freeze: {
                                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't thaw out due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                                        }
                                        break;
                                    case Enums.StatusAilment.Paralyze: {
                                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't recover from paralysis due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                                        }
                                        break;
                                    case Enums.StatusAilment.Poison: {
                                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't recover from poison due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                                        }
                                        break;
                                    case Enums.StatusAilment.Sleep: {
                                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't wake up due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                                        }
                                        break;
                                }
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Burn": {
                        if (character.StatusAilment == Enums.StatusAilment.OK) {
                            if (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) {
                                checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                                if (checkStatus != null && checkStatus.Tag == "1") {
                                    if (msg) {
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from burns!", Text.BrightCyan), character.X, character.Y, 10);
                                    }
                                    PacketHitList.MethodEnded(ref hitlist);
                                    return true;
                                }
                            }
	                        
	                        
                            if (character.HasActiveItem(633)) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected by its Rawst Scarf!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
	                        
                            if (HasAbility(character, "Water Veil") && !HasAbility(character, "Flare Boost") && !HasAbility(attacker, "Mold Breaker")) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Water Veil protected it from a burn!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }

                            if (character.Type1 == Enums.PokemonType.Fire || character.Type2 == Enums.PokemonType.Fire) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s type doesn't get burned!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                        } else if (character.StatusAilment == Enums.StatusAilment.Burn) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already burned!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        } else {
                            //if (msg) {
                            //	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wasn't burned", Text.BrightRed), character.X, character.Y, 10);
                            //}
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Freeze": {
                        if (character.StatusAilment == Enums.StatusAilment.OK) {
                            if (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) {
                                checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                                if (checkStatus != null && checkStatus.Tag == "2") {
                                    if (msg) {
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from being frozen!", Text.BrightCyan), character.X, character.Y, 10);
                                    }
                                    PacketHitList.MethodEnded(ref hitlist);
                                    return true;
                                }
                            }
	                        
                            if (character.HasActiveItem(634)) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected by its Aspear Scarf!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                            if (HasAbility(character, "Magma Armor") && !HasAbility(attacker, "Mold Breaker")) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Magma Armor protected it from being frozen!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }

                            if (character.Type1 == Enums.PokemonType.Ice || character.Type2 == Enums.PokemonType.Ice) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s type doesn't get frozen!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                        } else if (character.StatusAilment == Enums.StatusAilment.Freeze) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already frozen!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        } else {
                            //if (msg) {
                            //	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wasn't burned", Text.BrightRed), character.X, character.Y, 10);
                            //}
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Paralyze": {
                        if (character.StatusAilment == Enums.StatusAilment.OK) {
                        	if (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) {
                                checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                                if (checkStatus != null && checkStatus.Tag == "3") {
                                    if (msg) {
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from paralysis!", Text.BrightCyan), character.X, character.Y, 10);
                                    }
                                    PacketHitList.MethodEnded(ref hitlist);
                                    return true;
                                }
                            }
	                        
	                        
                            if (character.HasActiveItem(632)) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected by its Cheri Scarf!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                            if (HasAbility(character, "Limber") && !HasAbility(attacker, "Mold Breaker")) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Limber prevented it from being paralyzed!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                        } else if (character.StatusAilment == Enums.StatusAilment.Paralyze) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already paralyzed!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        } else {
                            //if (msg) {
                            //	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wasn't burned", Text.BrightRed), character.X, character.Y, 10);
                            //}
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Poison": {
                        if (character.StatusAilment == Enums.StatusAilment.OK) {
                            if (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) {
                                checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                                if (checkStatus != null && checkStatus.Tag == "4") {
                                    if (msg) {
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from poison!", Text.BrightCyan), character.X, character.Y, 10);
                                    }
                                    PacketHitList.MethodEnded(ref hitlist);
                                    return true;
                                }
                            }
                            if (HasAbility(character, "Immunity") && !HasAbility(character, "Toxic Boost") && !HasAbility(attacker, "Mold Breaker")) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Immunity prevented it from being poisoned!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }

                            if (character.Type1 == Enums.PokemonType.Poison || character.Type2 == Enums.PokemonType.Poison || character.Type1 == Enums.PokemonType.Steel || character.Type2 == Enums.PokemonType.Steel) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s type doesn't get poisoned!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                            if (character.HasActiveItem(483)) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected by its Pecha Scarf!", Text.BrightRed), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                        } else if (character.StatusAilment == Enums.StatusAilment.Poison) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already poisoned!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        } else {
                            //if (msg) {
                            //	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wasn't burned", Text.BrightRed), character.X, character.Y, 10);
                            //}
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                    }
                    break;
                case "Sleep": {
                		if (character.StatusAilment == Enums.StatusAilment.OK) {
                            if (attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) {
                                checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                                if (checkStatus != null && checkStatus.Tag == "5") {
                                    if (msg) {
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from paralysis!", Text.BrightCyan), character.X, character.Y, 10);
                                    }
                                    PacketHitList.MethodEnded(ref hitlist);
                                    return true;
                                }
                            }
	                        if (character.HasActiveItem(59)) {
	                            if (msg) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Insomniscope prevented it from falling asleep!", Text.WhiteSmoke), character.X, character.Y, 10);
	                            }
	                            PacketHitList.MethodEnded(ref hitlist);
	                            return true;
	                        }
	
	                        if (character.VolatileStatus.GetStatus("Sleepless") != null) {
	                            if (msg) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't fall asleep!", Text.BrightRed), character.X, character.Y, 10);
	                            }
	                            PacketHitList.MethodEnded(ref hitlist);
	                            return true;
	                        }
	
	
	                        if (HasAbility(character, "Insomnia") && !HasAbility(attacker, "Mold Breaker")) {
	                            if (msg) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Insomnia prevented it from falling asleep!", Text.WhiteSmoke), character.X, character.Y, 10);
	                            }
	                            PacketHitList.MethodEnded(ref hitlist);
	                            return true;
	                        }
	
	                        if (HasAbility(character, "Vital Spirit") && !HasAbility(attacker, "Mold Breaker")) {
	                            if (msg) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Vital Spirit prevented it from falling asleep!", Text.WhiteSmoke), character.X, character.Y, 10);
	                            }
	                            PacketHitList.MethodEnded(ref hitlist);
	                            return true;
	                        }
	
	                        

                        } else if (character.StatusAilment == Enums.StatusAilment.Sleep) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already asleep!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        } else {
                            //if (msg) {
                            //	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wasn't burned", Text.BrightRed), character.X, character.Y, 10);
                            //}
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Confusion": {

                    if (character.VolatileStatus.GetStatus("Confusion") != null) {
                        if (msg) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already confused!", Text.BrightRed), character.X, character.Y, 10);
                        }
                        PacketHitList.MethodEnded(ref hitlist);
                        return true;
                    }
                    
                    if ((attacker == null || !(HasAbility(attacker, "Infiltrator") || HasActiveBagItem(attacker, 7, 0, 0))) && character.VolatileStatus.GetStatus("Confusion Guard") != null) {
	                    if (msg) {
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from confusion!", Text.BrightCyan), character.X, character.Y, 10);
	                    }
	                    PacketHitList.MethodEnded(ref hitlist);
	                	return true;
	                }



                    if (character.HasActiveItem(323)) {
                        if (msg) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Persim Band prevented confusion!", Text.BrightRed), character.X, character.Y, 10);
                        }
                        PacketHitList.MethodEnded(ref hitlist);
                        return true;
                    }
                        if (HasAbility(character, "Own Tempo") && !HasAbility(character, "Tangled Feet") && !HasAbility(attacker, "Mold Breaker")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Own Tempo prevented it from becoming confused!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        //if (character.CharacterType == Enums.CharacterType.Recruit) {
                        //    if (((Recruit)character).Owner.Player.Confusion > 0 && statusParam[1].ToInt() > 0) {
                        //        if (msg) {
                        //            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already confused!", Text.BrightRed), character.X, character.Y, 10);
                        //        }
                        //        PacketHitList.MethodEnded(ref hitlist);
                        //        return true;
                        //    }
                        //} else {
                        //    if (((MapNpc)character).Confused) {
                        //        if (msg) {
                        //            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already confused!", Text.BrightRed), character.X, character.Y, 10);
                        //        }
                        //        PacketHitList.MethodEnded(ref hitlist);
                        //        return true;
                        //    }
                        //}
                    }
                    break;
                case "Cringe": {
                        

                        if (HasAbility(character, "Inner Focus") && !HasAbility(attacker, "Mold Breaker")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Inner Focus prevented it from cringing!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Immobilize": {
                        if (character.VolatileStatus.GetStatus("Immobilize") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already Immobilized!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected from being immobilized by the Shed Shell!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided immobilization with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Nightmare": {
                        if (character.VolatileStatus.GetStatus("Nightmare") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already under the effects of Nightmare!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(498)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The Lunar Wing protected " + character.Name + " from the nightmare!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "GastroAcid": {
                        if (character.VolatileStatus.GetStatus("GastroAcid") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already under the effects of Gastro Acid!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Attract": {
                        if (character.VolatileStatus.GetStatus("Attract") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already infatuated!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                        if (HasAbility(character, "Oblivious") && !HasAbility(attacker, "Mold Breaker")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided infatuation with Oblivious!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                        if (HasActiveBagItem(character, 14, 0, 0)) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by its item!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        	return true;
                        }
                    }
                    break;
                case "Encore": {
                        if (HasActiveBagItem(character, 14, 0, 0)) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by its item!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        	return true;
                        }
                    }
                    break;
                case "Torment": {
                        if (HasActiveBagItem(character, 14, 0, 0)) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by its item!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        	return true;
                        }
                    }
                    break;
                case "Taunt": {
                        if (HasActiveBagItem(character, 14, 0, 0)) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected by its item!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        	return true;
                        }
                    }
                    break;
                case "Embargo": {
                        if (character.VolatileStatus.GetStatus("Embargo") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already under the effects of Embargo!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "HealBlock": {
                        if (character.VolatileStatus.GetStatus("HealBlock") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already under the effects of Heal Block!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Substitute": {
                        if (character.VolatileStatus.GetStatus("Substitute") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has a substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Ingrain": {
                        if (character.VolatileStatus.GetStatus("Ingrain") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has its roots planted!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "AquaRing": {
                        if (character.VolatileStatus.GetStatus("AquaRing") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has an Aqua Ring!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Wish": {
                        if (character.VolatileStatus.GetStatus("Wish") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already being wished upon!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Exposed": {
                        if (character.VolatileStatus.GetStatus("Exposed") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already exposed!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MiracleEye": {
                        if (character.VolatileStatus.GetStatus("MiracleEye") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can already hit Dark- types with Psychic- type moves!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Bind": {
                        if (character.VolatileStatus.GetStatus("Bind:0") != null && statusParam[1] == "0" ||
                            character.VolatileStatus.GetStatus("Bind:1") != null && statusParam[1] == "1") {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40) && statusParam[1] == "0") {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Clamp": {
                        if (character.VolatileStatus.GetStatus("Clamp:0") != null && statusParam[1] == "0" ||
                            character.VolatileStatus.GetStatus("Clamp:1") != null && statusParam[1] == "1") {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40) && statusParam[1] == "0") {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Wrap": {
                        if (character.VolatileStatus.GetStatus("Wrap:0") != null && statusParam[1] == "0" ||
                            character.VolatileStatus.GetStatus("Wrap:1") != null && statusParam[1] == "1") {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40) && statusParam[1] == "0") {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "FireSpin": {
                        if (character.VolatileStatus.GetStatus("FireSpin") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already trapped by Fire Spin!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Whirlpool": {
                        if (character.VolatileStatus.GetStatus("Whirlpool") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already trapped by Whirlpool!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "SandTomb": {
                        if (character.VolatileStatus.GetStatus("SandTomb") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already trapped by Sand Tomb!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MagmaStorm": {
                        if (character.VolatileStatus.GetStatus("MagmaStorm") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already trapped by Magma Storm!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.HasActiveItem(40)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Shed Shell protected it from being trapped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (HasAbility(character, "Run Away")) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " avoided being trapped with Run Away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "LeechSeed": {
                        if (statusParam[1] == "0") {

                            if (character.VolatileStatus.GetStatus("LeechSeed") != null) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has a Leech Seed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }

                            if (character.Type1 == Enums.PokemonType.Grass || character.Type2 == Enums.PokemonType.Grass) {
                                if (msg) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " didn't take on the Leech Seed.", Text.WhiteSmoke), character.X, character.Y, 10);
                                }
                                PacketHitList.MethodEnded(ref hitlist);
                                return true;
                            }
                        }
                    }
                    break;
                case "TypeReduce": {
                        checkStatus = character.VolatileStatus.GetStatus("TypeReduce");
                        if (checkStatus != null && statusParam[1].ToInt() == checkStatus.Counter) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already takes reduced damage from " + (Enums.PokemonType)statusParam[1].ToInt() + " -type moves!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "TypeBoost": {
                        checkStatus = character.VolatileStatus.GetStatus("TypeBoost");
                        if (checkStatus != null && statusParam[1].ToInt() == checkStatus.Counter) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + (Enums.PokemonType)statusParam[1].ToInt() + " -type moves are already boosted!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "AttackReturn": {
                        checkStatus = character.VolatileStatus.GetStatus("AttackReturn");
                        if (checkStatus != null && statusParam[1].ToInt() == checkStatus.Counter) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already set to counter " + (Enums.MoveCategory)statusParam[1].ToInt() + " moves!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "FlashFire": {
                        if (character.VolatileStatus.GetStatus("FlashFire") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already powered up by Flash Fire!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MoveSeal": {
                        if (character.Moves[statusParam[1].ToInt()].MoveNum <= 0) {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.VolatileStatus.GetStatus(status) != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + MoveManager.Moves[character.Moves[statusParam[1].ToInt()].MoveNum].Name + " is already sealed!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Disable": {
                        if (character.Moves[statusParam[1].ToInt()].MoveNum == 0) {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.VolatileStatus.GetStatus("Disable") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has a move disabled!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Stockpile": {
                    checkStatus = character.VolatileStatus.GetStatus("Stockpile");
                        if (checkStatus != null && checkStatus.Counter >= 3) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't stockpile any more!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Roost": {
                        if (map.Tile[character.X, character.Y].Type == Enums.TileType.MobileBlock) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't land here!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "PerishCount": {
                    checkStatus = character.VolatileStatus.GetStatus("PerishCount");
                        if (checkStatus != null && checkStatus.Counter <= statusParam[1].ToInt()) {
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "FutureSight": {
                        if (character.VolatileStatus.GetStatus("FutureSight") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already targeted by Future Sight!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "FocusEnergy": {
                        if (character.VolatileStatus.GetStatus("FocusEnergy") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already focused!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Yawn": {
	
                        if (character.VolatileStatus.GetStatus("Sleepless") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't become drowsy its sleeplessness!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                        
                        if (HasAbility(character, "Insomnia") && !HasAbility(attacker, "Mold Breaker")) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't become drowsy due to Insomnia!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                        
                        if (HasAbility(character, "Vital Spirit") && !HasAbility(attacker, "Mold Breaker")) {
                        	if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't become drowsy due to Vital Spirit!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.VolatileStatus.GetStatus("Yawn") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already yawning!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.StatusAilment != Enums.StatusAilment.OK) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " didn't yawn!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Sleepless": {
                        if (character.StatusAilment == Enums.StatusAilment.Sleep && character.HasActiveItem(347)) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can't wake up due to the Blight Clay!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }

                        if (character.VolatileStatus.GetStatus("Sleepless") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already sleepless!", Text.BrightRed), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Status Guard": {
                    checkStatus = character.VolatileStatus.GetStatus("Status Guard");
                        if (checkStatus != null && tag == checkStatus.Tag) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected from the " + (Enums.StatusAilment)tag.ToInt() + " status!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Confusion Guard": {
                        if (character.VolatileStatus.GetStatus("Confusion Guard") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected from confusion", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Wide Guard": {
                        if (character.VolatileStatus.GetStatus("WideGuard") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected by Wide Guard!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Quick Guard": {
                        if (character.VolatileStatus.GetStatus("QuickGuard") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected by Quick Guard!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Protect": {
                        if (character.VolatileStatus.GetStatus("Protect") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Endure": {
                        if (character.VolatileStatus.GetStatus("Endure") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already enduring!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MagicCoat": {
                        if (character.VolatileStatus.GetStatus("MagicCoat") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already protected by Magic Coat!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MagnetRise": {
                        if (character.VolatileStatus.GetStatus("MagnetRise") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already levitating with Magnet Rise!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Telekinesis": {
                        if (character.VolatileStatus.GetStatus("Telekinesis") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is already floating from Telekinesis!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "Counter": {
                        if (character.VolatileStatus.GetStatus("Counter") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has Counter up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MirrorCoat": {
                        if (character.VolatileStatus.GetStatus("MirrorCoat") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has Mirror Coat up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "MetalBurst": {
                        if (character.VolatileStatus.GetStatus("MetalBurst") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has Metal Burst up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;
                case "DestinyBond": {
                        if (character.VolatileStatus.GetStatus("DestinyBond") != null) {
                            if (msg) {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " already has Destiny Bond up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            PacketHitList.MethodEnded(ref hitlist);
                            return true;
                        }
                    }
                    break;

            }

            PacketHitList.MethodEnded(ref hitlist);
            return false;
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: CheckStatusProtection " + status + " " + tag, Text.Black);
                
                Messenger.AdminMsg(ex.ToString(), Text.Black);
                
                return false;
            }
        }
        
        public static bool IsStatusBad(string status) {
            

            string[] statusParam = status.Split(':');


            switch (statusParam[0]) {
                case "Burn":
                case "Freeze":
                case "Paralyze":
                case "Poison":
                case "Sleep":
                case "Confusion":
                case "Cringe":
                case "Pause":
                case "Yawn":
                case "Taunt":
                case "Torment":
                case "Encore":
                case "Immobilize":
                case "Nightmare":
                case "LeechSeed":
                case "MoveSeal":
                case "Disable":
                case "Exposed":
                case "PerishCount":
                case "Curse":
                case "Embargo":
                case "HealBlock":
                case "Attract":
                case "GastroAcid":
                case "Blind":
                case "Shocker": {
                	return true;
                }
                default: {
                	return false;
                }
                break;
            }
        
        }

        public static void AddMapStatus(IMap map, string name, int counter, string tag, int graphicEffect, PacketHitList hitlist) {
            AddMapStatus(map, name, counter, tag, graphicEffect, true, hitlist);
        }

        public static void AddMapStatus(IMap map, string name, int counter, string tag, int graphicEffect, bool msg, PacketHitList hitlist) {

            if (map.TempStatus.GetStatus(name) == null) {
                map.TempStatus.Add(new MapStatus(name, counter, tag, graphicEffect));

                if (msg) {
                    switch (name) {
                        case "TrickRoom": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The floor's dimensions were twisted!", Text.BrightBlue));
                            }
                            break;
                        case "Gravity": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The floor's gravity intensified!", Text.BrightBlue));
                        		TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, 0, 0, Enums.Direction.Up, true, true, false);
								for (int i = 0; i < targets.Count; i++) {
				                    DropToGround(targets[i], map, hitlist, false);
				                }
                            }
                            break;
                        case "WonderRoom": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("All physical and special attacks changed properties!", Text.BrightBlue));
                            }
                            break;
                        case "MagicRoom": {
                        		
                        		TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, 0, 0, Enums.Direction.Up, true, true, false);
								for (int i = 0; i < targets.Count; i++) {
				                    targets[i].RefreshActiveItemList();
				                }
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("All items lost their effects!", Text.BrightBlue));
                            }
                            break;
                        case "Luminous": {
                        		
                        		hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The entire floor was illuminated!", Text.BrightBlue));
                                
                            }
                            break;
                    }
                }
                RefreshMapTraits(map, hitlist);
            }
        }

        public static void RemoveMapStatus(IMap map, string name, PacketHitList hitlist) {
            RemoveMapStatus(map, name, true, hitlist);
        }

        public static void RemoveMapStatus(IMap map, string name, bool msg, PacketHitList hitlist) {

            if (map.TempStatus.GetStatus(name) != null) {
                map.TempStatus.Remove(name);

                if (msg) {
                    switch (name) {
                        case "TrickRoom": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The floor's dimensions returned to normal.", Text.BrightBlue));
                            }
                            break;
                        case "Gravity": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The floor's gravity returned to normal.", Text.BrightBlue));
                            }
                            break;
                        case "WonderRoom": {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("All physical and special attacks returned to normal.", Text.BrightBlue));
                            }
                            break;
                        case "MagicRoom": {
                        		TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, 0, 0, Enums.Direction.Up, true, true, false);
                                for (int i = 0; i < targets.Count; i++) {
                                    targets[i].RefreshActiveItemList();
                                }
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("All items regained their effects!", Text.BrightBlue));
                            }
                            break;
                    }
                }
                RefreshMapTraits(map, hitlist);
            }
        }
        
        public static void RefreshMapTraits(IMap map, PacketHitList hitlist) {
        	
        	
        	map.Darkness = map.OriginalDarkness;
        	
        	if (map.TempStatus.GetStatus("Luminous") != null) {
        		map.Darkness = -1;
        	}
        	
        	if (map.TempStatus.GetStatus("Flash") != null && map.Darkness != -1 && map.Darkness < 17) {
        		map.Darkness = 17;
        	}
        	
        	
        	foreach (Client client in map.GetClients()) {
        		PacketBuilder.AppendMapDarkness(client, hitlist, map);
        	}
        }


        public static void AddExtraStatus(ICharacter character, IMap map, string name, int counter, ICharacter target, string tag, PacketHitList hitlist) {
            AddExtraStatus(character, null, map, name, counter, target, tag, hitlist, true);
        }

        public static void AddExtraStatus(ICharacter character, IMap map, string name, int counter, ICharacter target, string tag, PacketHitList hitlist, bool msg) {
            AddExtraStatus(character, null, map, name, counter, target, tag, hitlist, msg);
        }

        public static void AddExtraStatus(ICharacter character, ICharacter attacker, IMap map, string name, int counter, ICharacter target, string tag, PacketHitList hitlist, bool msg) {
            PacketHitList.MethodStart(ref hitlist);


            if (CheckStatusProtection(character, attacker, map, name + ":" + counter, tag, true, hitlist)) {
                PacketHitList.MethodEnded(ref hitlist);
                return;
            }


            if (name == "MovementSpeed" && character.VolatileStatus.GetStatus(name) != null ||
                name == "TypeReduce" && character.VolatileStatus.GetStatus(name) != null ||
                name == "TypeBoost" && character.VolatileStatus.GetStatus(name) != null ||
                name == "AttackReturn" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Type1" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Type2" && character.VolatileStatus.GetStatus(name) != null ||
                name == "LastUsedMoveSlot" && character.VolatileStatus.GetStatus(name) != null ||
                name == "TimesLastMoveUsed" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Stockpile" && character.VolatileStatus.GetStatus(name) != null ||
                name == "LastHitBy" && character.VolatileStatus.GetStatus(name) != null ||
                name == "LastMoveHitBy" && character.VolatileStatus.GetStatus(name) != null ||
                name == "PerishCount" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Curse" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Charge" && character.VolatileStatus.GetStatus(name) != null ||
                name == "SureShot" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Follow" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Ability1" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Ability2" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Ability3" && character.VolatileStatus.GetStatus(name) != null ||
                name == "ProxyAttack" && character.VolatileStatus.GetStatus(name) != null ||
                name == "ProxyDefense" && character.VolatileStatus.GetStatus(name) != null ||
                name == "ProxySpAttack" && character.VolatileStatus.GetStatus(name) != null ||
                name == "ProxySpDefense" && character.VolatileStatus.GetStatus(name) != null) {
                RemoveExtraStatus(character, map, name, hitlist, false);
            }
            
            if (name == "TypeReduce" && character.VolatileStatus.GetStatus(name) != null ||
                name == "TypeBoost" && character.VolatileStatus.GetStatus(name) != null ||
                name == "AttackReturn" && character.VolatileStatus.GetStatus(name) != null ||
                name == "Status Guard" && character.VolatileStatus.GetStatus(name) != null) {
                RemoveExtraStatus(character, map, name, hitlist, true);
            }
                

            if (name == "LeechSeed" && character.VolatileStatus.GetStatus(name) != null && counter == 1) {
                RemoveBondedExtraStatus(character, map, "LeechSeed:1", hitlist, msg);
            }

            if (character.VolatileStatus.GetStatus(name) == null) {
                ExtraStatus newStatus = new ExtraStatus();
                newStatus.Name = name;
                newStatus.Emoticon = -1;
                newStatus.Counter = counter;
                newStatus.Target = target;
                newStatus.Tag = tag;
                character.VolatileStatus.Add(newStatus);
                string[] nameSegment = name.Split(':');

                //announce to map

                switch (nameSegment[0]) {

                    case "Confusion": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became confused!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 4;
                            RefreshCharacterConfusion(character, map, hitlist);
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Safeguard": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected with a veil!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 137;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Status Guard": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from " + (Enums.StatusAilment)(tag.ToInt()) + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 39;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Confusion Guard": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is protected from confusion!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 39;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Mist": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is shrouded in mist!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 179;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "LuckyChant": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Lucky Chant blocked it from critical hits!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 193;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Reflect": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Reflect slightly raised its Defense!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 25;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "LightScreen": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Light Screen slightly raised its Special Defense!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 67;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Slip": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can now travel on water!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterMobility(character, map, hitlist);
                        }
                        break;
                    case "SuperMobile": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can now travel on any terrain!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterMobility(character, map, hitlist);
                        }
                        break;
                    case "MovementSpeed": {
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            ExtraStatus addedStatus = character.VolatileStatus.GetStatus("MovementSpeed");
                            if (addedStatus.Counter > 2) addedStatus.Counter = 2;
                            if (addedStatus.Counter < -3) addedStatus.Counter = -3;


                            switch (GetSpeedLimit(character)) {
                                case Enums.Speed.SuperSlow: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is at quarter speed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case Enums.Speed.Slow: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is at half speed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case Enums.Speed.Walking: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was slowed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case Enums.Speed.Running: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is at normal speed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                        RemoveExtraStatus(character, map, name, hitlist, false);
                                    }
                                    break;
                                case Enums.Speed.Fast: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is at double speed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case Enums.Speed.SuperFast: {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is at quadruple speed!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                            }
                        }
                        break;
                    case "AquaRing": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is surrounded by a veil of water!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 22;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Wish": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("A wish was made for " + character.Name + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 187;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Substitute": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " created a substitute!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSprite(character, map, hitlist);
                        }
                        break;
                    case "Blind": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became blinded!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterDarkness(character, map, hitlist);
                            newStatus.Emoticon = 17;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Nausea": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became nauseated!  It can't eat anything!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 36;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Shocker": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became shocked!  It can't tell friend from foe!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterDarkness(character, map, hitlist);
                            newStatus.Emoticon = 64;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Immobilize": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became immobilized!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "GastroAcid": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s abilities were nullified!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 63;
                            RefreshCharacterTraits(character, map, hitlist, false, false, false, true, false, false, false, false, false, false, false, false, true);
                        }
                        break;
                    case "Ability1": {
                            if (tag != "None") {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " acquired the ability " + tag + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            if (tag == "Insomnia") {
                                if (character.StatusAilment == Enums.StatusAilment.Sleep) {
                                    SetStatusAilment(character, map, Enums.StatusAilment.OK, 0, hitlist);
                                }
                                RemoveExtraStatus(character, map, "Yawn", hitlist);
                            }
                            RefreshCharacterTraits(character, map, hitlist, false, false, false, true, false, false, false, false, false, false, false, false, false);
                        }
                        break;
                    case "Ability2": {
                            if (tag != "None") {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " acquired the ability " + tag + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            if (tag == "Insomnia") {
                                if (character.StatusAilment == Enums.StatusAilment.Sleep) {
                                    SetStatusAilment(character, map, Enums.StatusAilment.OK, 0, hitlist);
                                }
                                RemoveExtraStatus(character, map, "Yawn", hitlist);
                            }
                            RefreshCharacterTraits(character, map, hitlist, false, false, false, true, false, false, false, false, false, false, false, false, false);
                        }
                        break;
                    case "Ability3": {
                            if (tag != "None") {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " acquired the ability " + tag + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            if (tag == "Insomnia") {
                                if (character.StatusAilment == Enums.StatusAilment.Sleep) {
                                    SetStatusAilment(character, map, Enums.StatusAilment.OK, 0, hitlist);
                                }
                                RemoveExtraStatus(character, map, "Yawn", hitlist);
                            }
                            RefreshCharacterTraits(character, map, hitlist, false, false, false, true, false, false, false, false, false, false, false, false, false);
                        }
                        break;
                    case "Nightmare": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " will get nightmares when it sleeps!", Text.BrightRed), character.X, character.Y, 10);
                            newStatus.Emoticon = 85;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Attract": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became infatuated!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            if (character.HasActiveItem(704)) {
                            	if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " has a Destiny Knot!", Text.WhiteSmoke), character.X, character.Y, 10);
                            	TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, map, character, character.X, character.Y, Enums.Direction.Up, true, false, false);
                                for (int i = 0; i < targets.Count; i++) {
                            		AddExtraStatus(targets[i], null, map, "Attract", counter, null, "", hitlist, true);
                            	}
                            }
                        }
                        break;
                    case "Follow": {
                            if (target != null) {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can only attack " + target.Name + " now!", Text.WhiteSmoke), character.X, character.Y, 10);
                            } else {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "is focused on a single target now!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            newStatus.Emoticon = 75;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Embargo": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was prevented from using items!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 147;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "HealBlock": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was prevented from recovering HP!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 91;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Ingrain": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " planted its roots!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "Bind":
                    case "Clamp":
                    case "Wrap": {
                            if (nameSegment[1] == "0") {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was trapped by the foe!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "FireSpin":
                    case "Whirlpool":
                    case "SandTomb":
                    case "MagmaStorm": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was trapped in the vortex!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "LeechSeed": {
                            if (nameSegment[1] == "0") {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took on a Leech Seed!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        }
                        break;
                    case "Taunt": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " fell for the taunt!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 23;
                            RefreshCharacterMoves(character, map, hitlist);
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Snatch": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is ready to snatch moves away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 48;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Rage": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became enraged!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 136;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "FocusEnergy": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is getting pumped!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 94;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Exposed": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became exposed to Normal- and Fighting- type attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 16;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "MiracleEye": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can now hit Dark- types with Psychic- type moves!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 116;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Torment": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was subjected to Torment!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 149;
                            RefreshCharacterMoves(character, map, hitlist);
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Encore": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " received an encore!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 75;
                            RefreshCharacterMoves(character, map, hitlist);
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "TypeReduce": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " will take less damage from " + ((Enums.PokemonType)counter).ToString() + "-type attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 96;
                        RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Type1": {
                            if (counter > 0) {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s type changed to " + ((Enums.PokemonType)counter).ToString() + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            RefreshCharacterTraits(character, map, hitlist, false, false, true, false, false, false, false, false, false, false, false, false, false);
                        }
                        break;
                    case "Type2": {
                            if (counter > 0) {
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s second type changed to " + ((Enums.PokemonType)counter).ToString() + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            RefreshCharacterTraits(character, map, hitlist, false, false, true, false, false, false, false, false, false, false, false, false, false);
                        }
                        break;
                    case "TypeBoost": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + ((Enums.PokemonType)counter).ToString() + "-type attacks were boosted!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 111;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "AttackReturn": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is set to counter " + ((Enums.MoveCategory)counter).ToString() + " attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 117;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Pierce": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " acquired a Pierce status!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 192;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Longtoss": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s range increased for single-target attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 186;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "FlashFire": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Flash Fire increased the power of its Fire-type moves!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "MoveSeal": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + MoveManager.Moves[character.Moves[nameSegment[1].ToInt()].MoveNum].Name + " was sealed!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterMoves(character, map, hitlist);
                        }
                        break;
                    case "Disable": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + MoveManager.Moves[character.Moves[counter].MoveNum].Name + " was disabled!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 161;
                            RefreshCharacterMoves(character, map, hitlist);
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "SolarBeam": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took in sunlight!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "SkullBash": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " lowered its head!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "RazorWind": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " whipped up a whirlwind!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "SkyAttack": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became cloaked in a harsh light!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "FocusPunch": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is tightening its focus!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "SuperCharge": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is charged with power!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Charge": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is charging electrical power!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 69;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "SureShot": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took aim!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 122;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Bide": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " began storing energy!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "Avalanche": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " readied an avalanche!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "VitalThrow": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " readied Vital Throw!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "Stockpile": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stockpiled " + counter + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Roost": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " landed on the ground!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "SkyDrop": {
                    	RemoveExtraStatus(character, map, "Grounded", hitlist, false);
                        RefreshCharacterSprite(character, map, hitlist);    
                            if (msg && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was taken into the sky!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            RefreshCharacterSpeedLimit(character, map, hitlist);
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "SemiInvul": {
                        RefreshCharacterSprite(character, map, hitlist);    
                        switch (tag) {
                                case "Dig": {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " burrowed its way underground!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case "Bounce": {
                                		RemoveExtraStatus(character, map, "Grounded", hitlist, false);
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " sprang up!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case "Fly": {
                                		RemoveExtraStatus(character, map, "Grounded", hitlist, false);
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " flew up high!", Text.WhiteSmoke), character.X, character.Y, 10);
                                    }
                                    break;
                                case "ShadowForce": {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " vanished instantly!", Text.WhiteSmoke), character.X, character.Y, 10);
                                        RefreshCharacterSprite(character, map, hitlist);
                                    }
                                    break;
                                case "Dive": {
                                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " hid underwater!", Text.WhiteSmoke), character.X, character.Y, 10);
                                        
                                    }
                                    break;
                            }
                            
                            RefreshCharacterSwapLock(character, map, hitlist);
                        }
                        break;
                    case "Invisible": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became invisible!", Text.WhiteSmoke), character.X, character.Y, 10);
                            RefreshCharacterVisibility(character, map, hitlist);
                        }
                        break;
                    case "PerishCount": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s perish count fell to " + counter + "!", Text.BrightRed), character.X, character.Y, 10);
                        }
                        break;
                    case "Curse": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("A curse was placed on " + character.Name + "!", Text.BrightRed), character.X, character.Y, 10);
                            newStatus.Emoticon = 99;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Grudge": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " wants a foe to bear a grudge!", Text.BrightRed), character.X, character.Y, 10);
                            newStatus.Emoticon = 123;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "FutureSight": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was targeted by Future Sight!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 173;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Yawn": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became drowsy!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 14;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Sleepless": {
                            if (character.StatusAilment == Enums.StatusAilment.Sleep) {
                                character.StatusAilment = Enums.StatusAilment.OK;
                                character.StatusAilmentCounter = 0;
                                if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was awakened!", Text.WhiteSmoke), character.X, character.Y, 10);
                                if (character.CharacterType == Enums.CharacterType.Recruit) {
                                    PacketBuilder.AppendStatusAilment(((Recruit)character).Owner, hitlist);
                                } else {
                                    PacketBuilder.AppendNpcStatusAilment((MapNpc)character, hitlist);
                                }
                            }
                            RemoveExtraStatus(character, map, "Yawn", hitlist);
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became sleepless!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 88;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Grounded": {
                    		DropToGround(character, map, hitlist, false);
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " dropped to the ground!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Protect": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " protected itself!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 165;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "WideGuard": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became protected from wide-range attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 151;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "QuickGuard": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became protected from far-range attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 109;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Endure": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " braced itself!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 145;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "MagicCoat": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was protected by Magic Coat!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 95;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "MagnetRise": {
                    		RemoveExtraStatus(character, map, "Grounded", hitlist, false);
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " levitated using Magnet Rise!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 125;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Conversion": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " has Conversion up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 162;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Conversion2": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " has Conversion 2 up!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 176;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Telekinesis": {
                    		RemoveExtraStatus(character, map, "Grounded", hitlist, false);
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was hurled into the air!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 83;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Autotomize": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became lighter!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 181;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Counter": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is set to counter physical attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 191;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "MirrorCoat": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is set to reflect special attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 81;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "MetalBurst": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is set to reflect attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 53;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "DestinyBond": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is trying to take its foe with it!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 43;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Rebound": {
                            if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is set to counter attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            newStatus.Emoticon = 159;
                            RefreshCharacterVolatileStatus(character, map, hitlist);
                        }
                        break;
                    case "Illusion": {
                        if (msg) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " hid behind an illusion!", Text.WhiteSmoke), character.X, character.Y, 10);
                        RefreshCharacterSprite(character, map, hitlist);
                        }
                        break;
                    case "DefenseCurl":
                    case "Minimize":
                    case "Rampage":
                    case "Rolling":
                    case "FuryCutter":
                    case "EchoedVoice":
                    case "TimesLastMoveUsed":
                    case "LastHitBy":
                    case "LastMoveHitBy":
                    case "LastMoveHitByOther": {

                        }
                        break;
                    case "LastUSedMoveSlot": {
                        RefreshCharacterMoves(character, map, hitlist);
                        }
                        break;
                }

            }




            //if (character.CharacterType == Enums.CharacterType.Recruit) {
            //    PacketBuilder.AppendVolatileStatus(((Recruit)character).Owner, hitlist);
            //} else if (character.CharacterType == Enums.CharacterType.MapNpc) {
            //    PacketBuilder.AppendNpcVolatileStatus(map, hitlist, ((MapNpc)character).MapSlot);
            //}

            //RefreshCharacterTraits(character, map, hitlist);

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RemoveExtraStatus(ICharacter character, IMap map, string name, PacketHitList hitlist) {
            RemoveExtraStatus(character, map, name, hitlist, true);
        }

        public static void RemoveExtraStatus(ICharacter character, IMap map, string name, PacketHitList hitlist, bool sendInfo) {
            PacketHitList.MethodStart(ref hitlist);

            ExtraStatus statusToRemove = character.VolatileStatus.GetStatus(name);

            if (statusToRemove != null) {
                string[] nameSegment = name.Split(':');

                bool forme = false, sprite = false, type = false, ability = false, atkSpeed = false,
                    confusion = false, speedLimit = false, mobility = false, visibility = false,
                    darkness = false, swapLock = false, moves = false, extraStatus = false;

                switch (nameSegment[0]) {
                    case "Confusion": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer confused!", Text.WhiteSmoke), character.X, character.Y, 10);
                            confusion = true;
                            extraStatus = true;
                        }
                        break;
                    case "Safeguard": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected by Safeguard!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Status Guard": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected from " + (Enums.StatusAilment)(statusToRemove.Tag.ToInt()) + "!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Confusion Guard": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected from confusion!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Mist": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Mist wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "LuckyChant": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Lucky Chant wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Reflect": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Reflect wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "LightScreen": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Light Screen wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "TypeReduce": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " no longer takes less damage from " + ((Enums.PokemonType)statusToRemove.Counter).ToString() + "-type attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "TypeBoost": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + ((Enums.PokemonType)statusToRemove.Counter).ToString() + "-type attacks are no longer boosted!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "AttackReturn": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer set to counter " + ((Enums.MoveCategory)statusToRemove.Counter).ToString() + " attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Taunt": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s taunt wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            moves = true;
                            extraStatus = true;
                        }
                        break;
                    case "Snatch": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer waiting to snatch a move!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Rage": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s rage faded away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "FocusEnergy": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Focus Energy wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Torment": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Torment wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            moves = true;
                            extraStatus = true;
                        }
                        break;
                    case "Encore": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s encore ended!", Text.WhiteSmoke), character.X, character.Y, 10);
                            moves = true;
                            extraStatus = true;
                        }
                        break;
                    case "Immobilize": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer immobilized!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Blind": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can see again!", Text.WhiteSmoke), character.X, character.Y, 10);
                            darkness = true;
                        }
                        break;
                    case "Nausea": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer nauseated!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Shocker": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer shocked!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Invisible": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " became visible again!", Text.WhiteSmoke), character.X, character.Y, 10);
                            visibility = true;
                        }
                        break;
                    case "Telekinesis": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer floating!", Text.WhiteSmoke), character.X, character.Y, 10);

                            extraStatus = true;
                        }
                        break;
                    case "GastroAcid": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s abilities are no longer nullified!", Text.WhiteSmoke), character.X, character.Y, 10);
                            ability = true;
                            extraStatus = true;
                        }
                        break;
                    case "Attract": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer infatuated!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;    
                            extraStatus = true;
                        }
                        break;
                    case "Nightmare": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " will no longer have nightmares when it sleeps!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Follow": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer focused on a single target!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Embargo": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can use items again!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "HealBlock": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " can recover HP again!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Ingrain": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer under the effects of Ingrain!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Bind": {
                            if (sendInfo && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Bind!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Clamp": {
                            if (sendInfo && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Clamp!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Wrap": {
                            if (sendInfo && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Wrap!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "FireSpin": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Fire Spin!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Whirlpool": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Whirlpool!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "SandTomb": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Sand Tomb!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "MagmaStorm": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Magma Storm!", Text.WhiteSmoke), character.X, character.Y, 10);
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "LeechSeed": {
                            if (sendInfo && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer seeded!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                        }
                        break;
                    case "MoveSeal": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + MoveManager.Moves[character.Moves[nameSegment[1].ToInt()].MoveNum].Name + " is no longer sealed!", Text.WhiteSmoke), character.X, character.Y, 10);
                        moves = true;
                        }
                        break;
                    case "Disable": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer disabled!", Text.WhiteSmoke), character.X, character.Y, 10);
                            moves = true;
                        }
                        break;
                    case "FocusPunch": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s lost its focus and couldn't move!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        speedLimit = true;
                            swapLock = true;
                        break;
                    case "SuperCharge": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer charged with power!", Text.WhiteSmoke), character.X, character.Y, 10);
                        extraStatus = true;
                        }
                        break;
                    case "Charge": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer electrically charged!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "SureShot": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " lost its aim!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Substitute": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s substitute faded away!", Text.WhiteSmoke), character.X, character.Y, 10);
                            sprite = true;
                        }
                        break;
                    case "Bide": {
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "Stockpile": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s stockpiled effect wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Roost": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took flight again!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "PerishCount": {
                        if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s perish count wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                        }
                        break;
                    case "Curse": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The curse was lifted from " + character.Name + "!", Text.BrightRed), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Yawn": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer drowsy!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Sleepless": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer sleepless!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "WideGuard": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected from wide-range attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "QuickGuard": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected from far-range attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Protect": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Endure": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stopped enduring!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "MagicCoat": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer protected by Magic Coat!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Counter": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Counter wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "MirrorCoat": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Mirror Coat wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "MetalBurst": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Metal Burst wore off!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "DestinyBond": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " gave up on the Destiny Bond!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Rebound": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is no longer set to counter attacks!", Text.WhiteSmoke), character.X, character.Y, 10);
                            extraStatus = true;
                        }
                        break;
                    case "Illusion": {
                            if (sendInfo) hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Illusion was broken!", Text.WhiteSmoke), character.X, character.Y, 10);
                            sprite = true;    
                        extraStatus = true;
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(503, character.X, character.Y));
                        }
                        break;
                    case "SkyDrop": {
                            if (sendInfo && nameSegment[1] == "0") {
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was freed from Sky Drop!", Text.WhiteSmoke), character.X, character.Y, 10);
                            }
                            sprite = true;
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "SemiInvul": {
                        sprite = true;
                        speedLimit = true;
                        swapLock = true;
                        }
                        break;
                    case "Slip":
                    case "SuperMobile": {
                        mobility = true;
                        }
                        break;
                    case "Rampage":
                    case "Rolling":
                    case "FuryCutter":
                    case "EchoedVoice":
                    case "DefenseCurl":
                    case "Minimize":
                    case "FlashFire":
                    case "LastHitBy":
                    case "LastHitByOther":
                    case "TimesLastMoveUsed": {

                        }
                        break;
                    case "Ability1":
                    case "Ability2":
                    case "Ability3": {
                        ability = true;
                        }
                        break;
                    case "FutureSight":
                    case "Exposed":
                    case "MiracleEye":
                    case "MagnetRise":
                    case "Grudge":
                    case "AquaRing":
                    case "Wish":
                    case "Autotomize": {
                        extraStatus = true;
                        }
                        break;
                    case "Type1":
                    case "Type2": {
                        type = true;
                        }
                        break;
                    case "LastUsedMoveSlot": {
                        moves = true;
                        }
                        break;
                    case "MovementSpeed": {
                        speedLimit = true;
                        }
                        break;
                    case "SolarBeam":
                    case "RazorWind":
                    case "Avalanche":
                    case "VitalThrow": {
                            speedLimit = true;
                            swapLock = true;
                        }
                        break;
                    case "SkullBash":
                    case "SkyAttack": {
                            swapLock = true;
                        }
                        break;


                }

                character.VolatileStatus.Remove(statusToRemove);

                //if (character.CharacterType == Enums.CharacterType.Recruit) {
                //    PacketBuilder.AppendVolatileStatus(((Recruit)character).Owner, hitlist);
                //} else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                //    PacketBuilder.AppendNpcVolatileStatus(map, hitlist, ((MapNpc)character).MapSlot);
                //}

                RefreshCharacterTraits(character, map, hitlist, forme, sprite, type, ability, atkSpeed, confusion, speedLimit, mobility, visibility, darkness, swapLock, moves, extraStatus);

            }

            PacketHitList.MethodEnded(ref hitlist);
        }


        public static bool HasAbility(ICharacter character, string ability) {
			if (character == null) return false;
            if (character.Ability1 == ability || character.Ability2 == ability || character.Ability3 == ability) return true;

            return false;
        }

        public static void CheckAttackerModAbility(BattleSetup setup) {
            if (HasAbility(setup.Attacker, "Adaptability")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Move.Element == setup.Attacker.Type1 || setup.Move.Element == setup.Attacker.Type2) {
                    setup.AttackerMultiplier *= 6;
                    setup.AttackerMultiplier /= 5;
                }
            }
            if (HasAbility(setup.Attacker, "Blaze")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status
                && setup.Move.Element == Enums.PokemonType.Fire
                && setup.Attacker.HP <= setup.Attacker.MaxHP / 4) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Blaze boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }
            }
            if (HasAbility(setup.Attacker, "Compoundeyes")) {
                if (setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 13;
                    setup.Move.Accuracy /= 10;
                }
            }
            //Damp
            if (setup.moveIndex == 135 || setup.moveIndex == 196) {
                bool explode = true;
                TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, null, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, true, false);
                for (int i = 0; i < checkedTargets.Count; i++) {
                    if (HasAbility(checkedTargets[i], "Damp")) {
                        setup.Move.TravelingAnim.AnimationIndex = -1;
                        setup.Move.DefenderAnim.AnimationIndex = -1;
                        SetAsNeutralizedMove(setup.Move);
                        explode = false;
                        break;
                    }
                }
                if (explode) {
                    if (setup.moveIndex == 135) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic215.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                    } else if (setup.moveIndex == 196) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic216.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                    }
                } else {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic320.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(505, setup.Attacker.X, setup.Attacker.Y));
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The damp conditions prevented an explosion!", Text.Blue), setup.Attacker.X, setup.Attacker.Y, 10);
                }
            }
            
            if (HasAbility(setup.Attacker, "Defeatist")) {
				if (setup.Attacker.HP < setup.Attacker.MaxHP / 2) {
					setup.AttackStat /= 2;
				}
            }
            if (HasAbility(setup.Attacker, "Flare Boost")) {

                if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                    if (setup.Attacker.StatusAilment == Enums.StatusAilment.Burn) {
                        setup.AttackStat *= 3;
                        setup.AttackStat /= 2;
                    }
                }
            }
            if (HasAbility(setup.Attacker, "Guts")) {

                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    if (setup.Attacker.StatusAilment == Enums.StatusAilment.Burn) {
                        setup.AttackStat *= 3;
                    } else if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                        setup.AttackStat *= 3;
                        setup.AttackStat /= 2;
                    }
                }
            }
            if (HasAbility(setup.Attacker, "Huge Power")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    //setup.PacketStack.AddPacketToMap(setup.Attacker.MapID, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Huge Power boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

            }
            if (HasAbility(setup.Attacker, "Hustle")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    setup.AttackerMultiplier *= 5;
                    setup.AttackerMultiplier /= 4;
                }
            }


            if (HasAbility(setup.Attacker, "Iron Fist")) {

                if (setup.moveIndex == 331 ||
                    setup.moveIndex == 329 ||
                    setup.moveIndex == 368 ||
                    setup.moveIndex == 460 ||
                    setup.moveIndex == 387 ||
                    setup.moveIndex == 333 ||
                    setup.moveIndex == 441 ||
                    setup.moveIndex == 399 ||
                    setup.moveIndex == 165 ||
                    setup.moveIndex == 330 ||
                    setup.moveIndex == 335 ||
                    setup.moveIndex == 400 ||
                    setup.moveIndex == 90 ||
                    setup.moveIndex == 334 ||
                    setup.moveIndex == 33) {
                    setup.AttackerMultiplier *= 6;
                    setup.AttackerMultiplier /= 5;
                }

            }
            
            
            //Plus and Minus
            if (HasAbility(setup.Attacker, "Minus")) {
                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                for (int i = 0; i < targets.Friends.Count; i++) {
                    if (HasAbility(targets.Friends[i], "Plus")) {
                        setup.AttackerMultiplier *= 4;
                        setup.AttackerMultiplier /= 3;
                    }
                }
            }
            if (HasAbility(setup.Attacker, "Plus")) {
                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                for (int i = 0; i < targets.Friends.Count; i++) {
                    if (HasAbility(targets.Friends[i], "Minus")) {
                        setup.AttackerMultiplier *= 4;
                        setup.AttackerMultiplier /= 3;
                    }
                }
            }
            if (HasAbility(setup.Attacker, "Prankster")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Status) {
                    setup.Move.Range += 2;
                }
            }
            if (HasAbility(setup.Attacker, "Pure Power")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    //setup.PacketStack.AddPacketToMap(setup.Attacker.MapID, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Pure Power boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 4;
                    setup.AttackerMultiplier /= 3;
                }

            }
            if (HasAbility(setup.Attacker, "Overgrow")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status
                && setup.Move.Element == Enums.PokemonType.Grass
                && setup.Attacker.HP <= setup.Attacker.MaxHP / 4) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Overgrow boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

            }
            if (HasAbility(setup.Attacker, "Reckless")) {
                if (setup.Move.EffectType == Enums.MoveType.SubHP && (setup.Move.AdditionalEffectData1 == 28 || setup.Move.AdditionalEffectData1 == 196)) {
                    setup.AttackerMultiplier *= 11;
                    setup.AttackerMultiplier /= 10;
                }

            }
            if (HasAbility(setup.Attacker, "Sand Force") && GetCharacterWeather(setup.Attacker) == Enums.Weather.Sandstorm) {
                if (setup.Move.Element == Enums.PokemonType.Ground || setup.Move.Element == Enums.PokemonType.Rock || setup.Move.Element == Enums.PokemonType.Steel) {
                    setup.AttackerMultiplier *= 6;
                    setup.AttackerMultiplier /= 5;
                }
            }
            if (HasAbility(setup.Attacker, "Serene Grace")) {
                switch (setup.Move.AdditionalEffectData1) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 35:
                    case 37:
                    case 38:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                    case 181: {
                            setup.Move.AdditionalEffectData2 *= 2;
                        }
                        break;
                }
            }
            if (HasAbility(setup.Attacker, "Sheer Force")) {
                switch (setup.Move.AdditionalEffectData1) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 35:
                    case 37:
                    case 38:
                    case 48:
                    case 49:
                    case 50:
                    case 51: {
                            setup.Move.AdditionalEffectData2 = 0;
                            setup.AttackerMultiplier *= 6;
                            setup.AttackerMultiplier /= 5;
                        }
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20: {
                            if (setup.Move.AdditionalEffectData3 > 0) {
                                setup.Move.AdditionalEffectData2 = 0;
                                setup.AttackerMultiplier *= 6;
                                setup.AttackerMultiplier /= 5;
                            }
                        }
                        break;
                }
            }
            if (HasAbility(setup.Attacker, "Solar Power")) {

                if (GetCharacterWeather(setup.Attacker) == Enums.Weather.Sunny && setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                    setup.AttackStat *= 4;
                    setup.AttackStat /= 3;
                }
            }
            if (HasAbility(setup.Attacker, "Stall")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Move.Range > 2) {
                    setup.Move.Range = 2;
                }
            }
            if (HasAbility(setup.Attacker, "Swarm")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status
                && setup.Move.Element == Enums.PokemonType.Bug
                && setup.Attacker.HP <= setup.Attacker.MaxHP / 4) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Swarm boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

            }
            if (HasAbility(setup.Attacker, "Technician")) {
                if (setup.Move.EffectType == Enums.MoveType.SubHP && setup.Move.Data1 <= 60) {
                    setup.AttackerMultiplier *= 4;
                    setup.AttackerMultiplier /= 3;
                }

            }
            if (HasAbility(setup.Attacker, "Torrent")) {
                if (setup.Move.MoveCategory != Enums.MoveCategory.Status
                && setup.Move.Element == Enums.PokemonType.Water
                && setup.Attacker.HP <= setup.Attacker.MaxHP / 4) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Torrent boosted its power!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

            }
            if (HasAbility(setup.Attacker, "Toxic Boost")) {

                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    if (setup.Attacker.StatusAilment == Enums.StatusAilment.Poison) {
                        setup.AttackStat *= 3;
                        setup.AttackStat /= 2;
                    }
                }
            }
        }

        public static void CheckImmunityAbility(BattleSetup setup) {

            if (HasAbility(setup.Defender, "Dry Skin") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Water) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }
            if (HasAbility(setup.Defender, "Flash Fire") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Fire) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }

            //Lightningrod
            if (!HasAbility(setup.Attacker, "Mold Breaker")) {
	            if (setup.Move.Element == Enums.PokemonType.Electric && setup.Attacker != setup.Defender) {
	                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, false, true, true);
	                int highestSpeed = 0;
	                ICharacter singleTarget = null;
	                for (int i = 0; i < targets.Count; i++) {
	                    if (HasAbility(targets[i], "Lightningrod")) {
	                        if (targets[i].Spd > highestSpeed) {
	                            singleTarget = targets[i];
	                            highestSpeed = targets[i].Spd;
	                        }
	                    }
	                }
	
	                if (singleTarget != null) {
	                    setup.Defender = singleTarget;
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Lightningrod drew in the attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
	                    SetAsNeutralizedMove(setup.Move);
	                }
	            }
            }

            if (HasAbility(setup.Defender, "Motor Drive") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Electric) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }
            if (HasAbility(setup.Defender, "Pressure") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (setup.moveSlot > -1 && setup.moveSlot < 4 && setup.Attacker.Moves[setup.moveSlot].CurrentPP > 0) {
                    setup.Attacker.Moves[setup.moveSlot].CurrentPP--;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " is exerting its Pressure!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                        PacketBuilder.AppendMovePPUpdate(((Recruit)setup.Attacker).Owner, setup.PacketStack, setup.moveSlot);
                    }
                }
            }
            if (HasAbility(setup.Defender, "Sap Sipper") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Grass) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }

            if (HasAbility(setup.Defender, "Shield Dust") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                switch (setup.Move.AdditionalEffectData1) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 35:
                    case 37:
                    case 38:
                    case 48:
                    case 49:
                    case 50:
                    case 51: {
                            setup.Move.AdditionalEffectData2 = 0;
                        }
                        break;
                }
            }

            //Storm Drain
            if (!HasAbility(setup.Attacker, "Mold Breaker")) {
	            if (setup.Move.Element == Enums.PokemonType.Water && setup.Attacker != setup.Defender) {
	                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, false, true, true);
	                int highestSpeed = 0;
	                ICharacter singleTarget = null;
	                for (int i = 0; i < targets.Count; i++) {
	                	
	                    if (HasAbility(targets[i], "Storm Drain")) {
	                        if (targets[i].Spd > highestSpeed) {
	                            singleTarget = targets[i];
	                            highestSpeed = targets[i].Spd;
	                        }
	                    }
	                }
	
	                if (singleTarget != null) {
	                    setup.Defender = singleTarget;
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Storm Drain drew in the attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
	                    SetAsNeutralizedMove(setup.Move);
	                }
	            }
            }

            if (HasAbility(setup.Defender, "Telepathy") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Friend && setup.Move.EffectType == Enums.MoveType.SubHP) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " avoided it's ally's attack with Telepathy!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.Move.Accuracy = 0;
                }
            }
            if (HasAbility(setup.Defender, "Volt Absorb") && setup.Defender != setup.Attacker && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Electric) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }
            if (HasAbility(setup.Defender, "Water Absorb") && setup.Defender != setup.Attacker && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Water) {
                    SetAsNeutralizedMove(setup.Move);
                }
            }
            if (HasAbility(setup.Defender, "Wonder Guard") && setup.Defender != setup.Attacker && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.None) {
                    setup.Move.EffectType = Enums.MoveType.Scripted;
                    setup.Move.Data1 = 30;
                    setup.Move.Data2 = 1;
                    setup.Move.Data3 = 0;
                    setup.Move.AdditionalEffectData1 = 0;
                    setup.Move.AdditionalEffectData2 = 0;
                    setup.Move.AdditionalEffectData3 = 0;
                }
            }
        }

        public static void CheckDefenderModAbility(BattleSetup setup) {
            ExtraStatus status;
            
            if (HasAbility(setup.Attacker, "Analytic")) {
                status = setup.Attacker.VolatileStatus.GetStatus("LastHitBy");
                if (status != null && status.Target == setup.Defender) {
                    setup.Multiplier *= 5;
                    setup.Multiplier /= 4;
                }
            }
            if (HasAbility(setup.Defender, "Anticipation") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);
                if (effectiveness > 6 && setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 4;
                    setup.Move.Accuracy /= 5;
                }
            }
            if (HasAbility(setup.Defender, "Dry Skin") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Fire) {
                    setup.Multiplier *= 5;
                    setup.Multiplier /= 4;
                }
            }
            if (HasAbility(setup.Defender, "Filter") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);
                if (effectiveness > 6 && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                    setup.Multiplier *= 288;
                    setup.Multiplier /= 416;
                    //setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " avoided damage with Wonder Guard!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                }
            }
            if (HasAbility(setup.Defender, "Forewarn") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Data1 >= 80 && setup.Move.EffectType != Enums.MoveType.SubHP && setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 4;
                    setup.Move.Accuracy /= 5;
                }
            }

            //Friend Guard
            if (!HasAbility(setup.Attacker, "Mold Breaker")) {
	            if (setup.Move.EffectType == Enums.MoveType.SubHP) {
	                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, false, true, false);
	                for (int i = 0; i < targets.Friends.Count; i++) {
	                    if (HasAbility(targets.Friends[i], "Friend Guard")) {
	                        setup.Multiplier *= 4;
	                        setup.Multiplier /= 5;
	                    }
	                }
	            }
            }

            if (HasAbility(setup.Defender, "Heatproof") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Fire) {
                    setup.Multiplier /= 3;
                }
            }
            if (HasAbility(setup.Attacker, "Hustle")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical && setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 8;
                    setup.Move.Accuracy /= 10;
                }
            }
            if (HasAbility(setup.Defender, "Intimidate") && MoveProcessor.GetMatchupWith(setup.Defender, setup.Attacker) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 3) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Intimidate affected " + setup.Attacker.Name + "!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    ChangeAttackBuff(setup.Attacker, setup.Defender, setup.AttackerMap, -1, setup.PacketStack);
                }
            }
            
            if (HasAbility(setup.Defender, "Levitate") && IsGroundImmune(setup.Defender, setup.DefenderMap) && setup.Defender != setup.Attacker && !HasAbility(setup.Attacker, "Mold Breaker")) {
		        if (setup.Move.Element == Enums.PokemonType.Ground) {
		            setup.Move.Accuracy = 0;
		            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " avoided damage with Levitate!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
		        }
		    }
            
            if (HasAbility(setup.Defender, "Marvel Scale") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical
                && setup.Defender.StatusAilment != Enums.StatusAilment.OK) {
                    setup.Multiplier *= 2;
                    setup.Multiplier /= 3;
                }
            }
            
            if (HasAbility(setup.Defender, "Multiscale") && setup.Defender.HP == setup.Defender.MaxHP && !HasAbility(setup.Attacker, "Mold Breaker")) {
                setup.Multiplier /= 2;
            }
            
            if (HasAbility(setup.Defender, "Tangled Feet") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe
            	&& setup.Move.Accuracy != -1 && setup.Defender.VolatileStatus.GetStatus("Confusion") != null && !HasAbility(setup.Attacker, "Mold Breaker")) {
                    setup.Move.Accuracy /= 2;
            }
            if (HasAbility(setup.Attacker, "Rivalry")) {
                if (setup.Attacker.Sex == setup.Defender.Sex) {
                    setup.Multiplier *= 11;
                    setup.Multiplier /= 10;
                }
            }
            if (HasAbility(setup.Defender, "Sand Veil") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (GetCharacterWeather(setup.Defender) == Enums.Weather.Sandstorm && setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 4;
                    setup.Move.Accuracy /= 5;
                }
            }
            if (HasAbility(setup.Defender, "Snow Cloak") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (GetCharacterWeather(setup.Defender) == Enums.Weather.Hail && setup.Move.Accuracy != -1) {
                    setup.Move.Accuracy *= 4;
                    setup.Move.Accuracy /= 5;
                }
            }
            if (HasAbility(setup.Defender, "Solid Rock") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);
                if (effectiveness > 6 && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                    setup.Multiplier *= 288;
                    setup.Multiplier /= 416;
                }
            }
            if (HasAbility(setup.Defender, "Soundproof") && !HasAbility(setup.Attacker, "Mold Breaker")) {

                if (setup.moveIndex == 180 ||
                    setup.moveIndex == 447 ||
                    setup.moveIndex == 630 ||
                    setup.moveIndex == 99 ||
                    setup.moveIndex == 105 ||
                    setup.moveIndex == 225 ||
                    setup.moveIndex == 317 ||
                    setup.moveIndex == 266 ||
                    setup.moveIndex == 680 ||
                    setup.moveIndex == 193 ||
                    setup.moveIndex == 629 ||
                    setup.moveIndex == 157 ||
                    setup.moveIndex == 372 ||
                    setup.moveIndex == 688 ||
                    setup.moveIndex == 311 ||
                    setup.moveIndex == 222 ||
                    setup.moveIndex == 210 ||
                    setup.moveIndex == 733 ||
                    setup.moveIndex == 703 ||
                    setup.moveIndex == 705 ||
                    setup.moveIndex == 708 ||
                    setup.moveIndex == 737) {
                    setup.Move.Accuracy = 0;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " avoided damage with Soundproof!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                }

            }
            if (HasAbility(setup.Defender, "Sturdy") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 39) {
                    setup.Move.Data1 = 0;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Sturdy prevents 1-Hit KO moves!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                } else if (setup.Defender.HP == setup.Defender.MaxHP) {
                    setup.KnockedOut = false;
                }
            }
            if (HasAbility(setup.Defender, "Thick Fat") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                if (setup.Move.Element == Enums.PokemonType.Fire
                || setup.Move.Element == Enums.PokemonType.Ice) {
                    setup.Multiplier *= 2;
                    setup.Multiplier /= 3;
                }
            }
            if (HasAbility(setup.Attacker, "Tinted Lens")) {
                int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);
                if (effectiveness < 6 && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                    setup.Multiplier *= 224;
                    setup.Multiplier /= 160;
                }
            }
            if (HasAbility(setup.Defender, "Wonder Guard") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);
                if (effectiveness < 7 && setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Move.Element != Enums.PokemonType.None) {
                    setup.Move.Accuracy = 0;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " avoided damage with Wonder Guard!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                }
            }
        }

        public static void CheckAfterMoveHitAbility(BattleSetup setup) {
            if (HasAbility(setup.Defender, "Anger Point") && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                if (setup.BattleTags.Contains("Critical")) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Anger Point was triggered!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    ChangeAttackBuff(setup.Defender, setup.DefenderMap, 20, setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Arena Trap")/* && (setup.Defender.HP > 0 || !setup.KnockedOut)*/ && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (IsGroundImmune(setup.Attacker, setup.DefenderMap)) {

                } else {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Arena Trap prevented escape!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "Immobilize", 2, null, "", setup.PacketStack);
                }
            }
            if (HasAbility(setup.Attacker, "Arena Trap") && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (IsGroundImmune(setup.Defender, setup.DefenderMap)) {

                } else {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Arena Trap prevented escape!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    AddExtraStatus(setup.Defender, setup.DefenderMap, "Immobilize", 2, null, "", setup.PacketStack);
                }
            }
            
            
            if (HasAbility(setup.Defender, "Color Change") && setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Move.Element != Enums.PokemonType.None
            	&& (setup.Move.Element != setup.Defender.Type1 && setup.Defender.Type1 != Enums.PokemonType.None || setup.Move.Element != setup.Defender.Type2 && setup.Defender.Type2 != Enums.PokemonType.None)) {
                AddExtraStatus(setup.Defender, setup.DefenderMap, "Type1", (int)setup.Move.Element, null, "", setup.PacketStack);
                AddExtraStatus(setup.Defender, setup.DefenderMap, "Type2", 0, null, "", setup.PacketStack);
            }

            if (HasAbility(setup.Defender, "Cursed Body") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 5) == 0 && setup.moveSlot > -1 && setup.moveSlot < 4) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Cursed Body sealed a move!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:" + setup.moveSlot, 0, null, "", setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Cute Charm") && setup.Move.MoveCategory != Enums.MoveCategory.Status
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (setup.Attacker.Sex == Enums.Sex.Genderless && setup.Defender.Sex == Enums.Sex.Genderless
                    || setup.Attacker.Sex == Enums.Sex.Male && setup.Defender.Sex == Enums.Sex.Female
                    || setup.Defender.Sex == Enums.Sex.Male && setup.Attacker.Sex == Enums.Sex.Female) {
                    if (Server.Math.Rand(0, 5) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Cute Charm caused infatuation!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Attract", Server.Math.Rand(3, 5), null, "", setup.PacketStack);
                    }
                }
            }
            if (HasAbility(setup.Defender, "Effect Spore") && setup.Move.MoveCategory != Enums.MoveCategory.Status
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 5) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Effect Spore caused status problems!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    Enums.StatusAilment status = (Enums.StatusAilment)Server.Math.Rand(3, 6);
                    if (status == Enums.StatusAilment.Sleep) {
                        SetStatusAilment(setup.Attacker, setup.AttackerMap, status, Server.Math.Rand(2, 4), setup.PacketStack);
                    } else {
                        SetStatusAilment(setup.Attacker, setup.AttackerMap, status, 1, setup.PacketStack);
                    }
                }
            }
            if (HasAbility(setup.Defender, "Flame Body") && setup.Move.MoveCategory != Enums.MoveCategory.Status
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 5) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Flame Body caused a burn!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Iron Barbs") && setup.Move.MoveCategory != Enums.MoveCategory.Status && !HasAbility(setup.Attacker, "Magic Guard")
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Iron Barbs hurt to touch!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    int dmg = setup.Attacker.MaxHP / 16;
                    if (setup.Attacker.MaxHP / 16 >= setup.Attacker.HP) {
                        DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 16, Enums.KillType.Other, setup.PacketStack, true);
                        setup.Cancel = true;
                    } else {
                        DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 16, Enums.KillType.Other, setup.PacketStack, true);
                    }
                }
            }
            if (HasAbility(setup.Defender, "Justified") && setup.Move.Element == Enums.PokemonType.Dark && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                ChangeAttackBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
            }
            
            if (HasAbility(setup.Defender, "Magnet Pull")/* && (setup.Defender.HP > 0 || !setup.KnockedOut)*/ && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (setup.Attacker.Type1 == Enums.PokemonType.Steel || setup.Attacker.Type2 == Enums.PokemonType.Steel) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Magnet Pull prevented escape!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "Immobilize", 2, null, "", setup.PacketStack);
                }
            }
            if (HasAbility(setup.Attacker, "Magnet Pull") && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (setup.Defender.Type1 == Enums.PokemonType.Steel || setup.Defender.Type2 == Enums.PokemonType.Steel) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Magnet Pull prevented escape!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    AddExtraStatus(setup.Defender, setup.DefenderMap, "Immobilize", 2, null, "", setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Moody") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 3) == 0) {
                    switch (Server.Math.Rand(0, 7)) {
                        case 0: {
                                ChangeAttackBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 1: {
                                ChangeDefenseBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 2: {
                                ChangeSpAtkBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 3: {
                                ChangeSpDefBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 4: {
                                ChangeSpeedBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 5: {
                                ChangeAccuracyBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                        case 6: {
                                ChangeEvasionBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                            }
                            break;
                    }

                    switch (Server.Math.Rand(0, 7)) {
                        case 0: {
                                ChangeAttackBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 1: {
                                ChangeDefenseBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 2: {
                                ChangeSpAtkBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 3: {
                                ChangeSpDefBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 4: {
                                ChangeSpeedBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 5: {
                                ChangeAccuracyBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                        case 6: {
                                ChangeEvasionBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
                            }
                            break;
                    }
                }
            }

            if (HasAbility(setup.Attacker, "Mummy") && !HasAbility(setup.Defender, "Mummy") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) != Enums.CharacterMatchup.Self
            	 && MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, setup.Defender.X, setup.Defender.Y)) {
                AddExtraStatus(setup.Defender, setup.Attacker, setup.AttackerMap, "Ability1", 0, null, "Mummy", setup.PacketStack, true);
                AddExtraStatus(setup.Defender, setup.Attacker, setup.AttackerMap, "Ability2", 0, null, "None", setup.PacketStack, false);
                AddExtraStatus(setup.Defender, setup.Attacker, setup.AttackerMap, "Ability3", 0, null, "None", setup.PacketStack, false);
            }
            if (HasAbility(setup.Defender, "Mummy") && !HasAbility(setup.Attacker, "Mummy") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) != Enums.CharacterMatchup.Self
            	 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                AddExtraStatus(setup.Attacker, setup.Defender, setup.DefenderMap, "Ability1", 0, null, "Mummy", setup.PacketStack, true);
                AddExtraStatus(setup.Attacker, setup.Defender, setup.DefenderMap, "Ability2", 0, null, "None", setup.PacketStack, false);
                AddExtraStatus(setup.Attacker, setup.Defender, setup.DefenderMap, "Ability3", 0, null, "None", setup.PacketStack, false);
            }
            
            if (HasAbility(setup.Defender, "Poison Point") && setup.Move.MoveCategory != Enums.MoveCategory.Status
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 5) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Poison Point caused poisoning!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison, 1, setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Rattled")
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (setup.Move.Element == Enums.PokemonType.Dark || setup.Move.Element == Enums.PokemonType.Bug || setup.Move.Element == Enums.PokemonType.Ghost) {
                    ChangeSpeedBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                }
            }
            if (HasAbility(setup.Defender, "Rough Skin") && setup.Move.MoveCategory != Enums.MoveCategory.Status && !HasAbility(setup.Attacker, "Magic Guard")
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Rough Skin hurts to touch!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    int dmg = setup.Attacker.MaxHP / 32;
                    if (dmg >= setup.Attacker.HP) {
                        DamageCharacter(setup.Attacker, setup.AttackerMap, dmg, Enums.KillType.Other, setup.PacketStack, true);
                        setup.Cancel = true;
                    } else {
                        DamageCharacter(setup.Attacker, setup.AttackerMap, dmg, Enums.KillType.Other, setup.PacketStack, true);
                    }
                }
            }
            if (HasAbility(setup.Defender, "Shadow Tag")/* && (setup.Defender.HP > 0 || !setup.KnockedOut)*/ && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !HasAbility(setup.Attacker, "Shadow Tag")
                && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Shadow Tag prevented escape!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Immobilize", 2, null, "", setup.PacketStack);

            }
            if (HasAbility(setup.Attacker, "Shadow Tag") && MoveProcessor.IsInAreaRange(3, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                && !HasAbility(setup.Defender, "Shadow Tag")
                && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Immobilize", false, setup.PacketStack)
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Shadow Tag prevented escape!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                AddExtraStatus(setup.Defender, setup.DefenderMap, "Immobilize", 2, null, "", setup.PacketStack);

            }
            if (HasAbility(setup.Defender, "Static") && setup.Move.MoveCategory != Enums.MoveCategory.Status
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                if (Server.Math.Rand(0, 5) == 0 && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Static caused paralysis!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                }
            }
            
            if (HasAbility(setup.Defender, "Trace")
                && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) != Enums.CharacterMatchup.Self) {
                string ability = "None";
                switch (Server.Math.Rand(0,3)) {
                	case 0: {
                		ability = setup.Attacker.Ability1;
                	}
                	break;
                	case 1: {
                		ability = setup.Attacker.Ability2;
                	}
                	break;
                	case 2: {
                		ability = setup.Attacker.Ability3;
                	}
                	break;
                }
                if (setup.Defender.Ability1 == "Trace" && ability != "None") {
                	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " traced an ability!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                	AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability1", 0, null, ability, setup.PacketStack, true);
                }
                if (setup.Defender.Ability2 == "Trace" && ability != "None") {
                	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " traced an ability!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                	AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability2", 0, null, ability, setup.PacketStack, true);
                }
                if (setup.Defender.Ability3 == "Trace" && ability != "None") {
                	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " traced an ability!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                	AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability3", 0, null, ability, setup.PacketStack, true);
                }
                /*
                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability1", 0, null, setup.Attacker.Ability1, setup.PacketStack, true);
                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability2", 0, null, setup.Attacker.Ability2, setup.PacketStack, true);
                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability3", 0, null, setup.Attacker.Ability3, setup.PacketStack, true);
                */
            }
            
            if (HasAbility(setup.Defender, "Weak Armor") && setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Weak Armor broke off!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                ChangeDefenseBuff(setup.Defender, setup.DefenderMap, -1, setup.PacketStack);
                ChangeSpeedBuff(setup.Defender, setup.DefenderMap, 2, setup.PacketStack);
            }
        }

        public static void CheckAfterActionTakenAbility(BattleSetup setup) {
			
			//Bad Dreams
            if (setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep) {
                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, false, false);
                for (int i = 0; i < targets.Foes.Count; i++) {
                    if (HasAbility(targets.Foes[i], "Bad Dreams")) {
                        if (setup.Attacker.HasActiveItem(498)) {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Lunar Wing kept the bad dreams away!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                        } else {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is having bad dreams!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            int dmg = setup.Attacker.MaxHP / 16;
                            if (setup.Attacker.MaxHP / 16 >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 16, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 16, Enums.KillType.Other, setup.PacketStack, true);
                            }
                            break;
                        }
                    }
                }
            }
			
            //Early Bird
            if (HasAbility(setup.Attacker, "Early Bird") && setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
                setup.Attacker.StatusAilmentCounter = 0;

            }

            //Healer
            if (!CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                for (int i = 0; i < targets.Friends.Count; i++) {
                    if (HasAbility(targets.Friends[i], "Healer")) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(targets.Friends[i].Name + "'s Healer cured " + setup.Attacker.Name + "'s status problem!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                        SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                        break;
                    }
                }
            }

            if (HasAbility(setup.Attacker, "Hydration") && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
                if (GetCharacterWeather(setup.Attacker) == Enums.Weather.Raining || GetCharacterWeather(setup.Attacker) == Enums.Weather.Thunder) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Hydration cured its status problem!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                }
            }
            if (HasAbility(setup.Attacker, "Shed Skin") && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
                if (Server.Math.Rand(0, 3) == 0) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Shed Skin cured its status problem!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                }
            }
        }
        
        public static void OnMoveSwapped(Client client, int oldSlot, int newSlot) {
            ExtraStatus status;
            status = client.Player.GetActiveRecruit().VolatileStatus.GetStatus("LastUsedMoveSlot");
        	if (status != null) {
        		
        		if (status.Counter == oldSlot) {
        			status.Counter = newSlot;
        		} else if (status.Counter == newSlot) {
        			status.Counter = oldSlot;
        		}
        	}

            status = client.Player.GetActiveRecruit().VolatileStatus.GetStatus("Disable");
        	if (status != null) {
        		
        		if (status.Counter == oldSlot) {
        			status.Counter = newSlot;
        		} else if (status.Counter == newSlot) {
        			status.Counter = oldSlot;
        		}
        	}
        	
        	ExtraStatus oldSeal = client.Player.GetActiveRecruit().VolatileStatus.GetStatus("MoveSeal:" + oldSlot);
        	ExtraStatus newSeal = client.Player.GetActiveRecruit().VolatileStatus.GetStatus("MoveSeal:" + newSlot);
        	
        	if (oldSeal != null) {
        		oldSeal.Name = "MoveSeal:" + newSlot;
        	}
        	if (newSeal != null) {
        		newSeal.Name = "MoveSeal:" + oldSlot;
        	}
        	RefreshCharacterMoves(client.Player.GetActiveRecruit(), client.Player.Map, null);
        	
        }
        
        public static void DropToGround(ICharacter character, IMap map, PacketHitList hitlist, bool msg) {
        	RemoveExtraStatus(character, map, "Telekinesis", hitlist, msg);
            RemoveExtraStatus(character, map, "MagnetRise", hitlist, msg);
            ExtraStatus status = character.VolatileStatus.GetStatus("SemiInvul");
            if (status != null) {
            	if (status.Tag == "Fly" || status.Tag == "Bounce") {
            		RemoveExtraStatus(character, map, "SemiInvul", hitlist, msg);
            	}
            }
            RemoveBondedExtraStatus(character, map, "SkyDrop:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "SkyDrop:1", hitlist, msg);
        }
        
        public static bool IsGroundImmune(ICharacter character, IMap map) {
        	bool immune = false;
        	if (character.Type1 == Enums.PokemonType.Flying || character.Type2 == Enums.PokemonType.Flying) {
        		immune = true;
        	}
        	if (character.VolatileStatus.GetStatus("Roost") != null) {
        		immune = false;
        	}
        	if (HasAbility(character, "Levitate")) {
        		immune = true;
        	}
        	if (character.VolatileStatus.GetStatus("MagnetRise") != null) {
        		immune = true;
        	}
        	if (character.VolatileStatus.GetStatus("Telekinesis") != null) {
        		immune = true;
        	}
        	if (character.HasActiveItem(82)) {
        		immune = true;
        	}
        	
        	if (character.HasActiveItem(111)) {
        		immune = false;
        	}
        	
        	if (character.VolatileStatus.GetStatus("Grounded") != null) {
        		immune = false;
        	}
        	if (map != null && map.TempStatus.GetStatus("Gravity") != null) {
        		immune = false;
        	}
        	return immune;
        }
        
        public static bool IsBondedStatus(string status) {
        	switch (status) {
        		case "Wrap:0":
	            case "Wrap:1":
	            case "Bind:0":
	            case "Bind:1":
	            case "Clamp:0":
	            case "Clamp:1":
	            case "LeechSeed:0":
	            case "LeechSeed:1":
	            case "SkyDrop:0":
	            case "SkyDrop:1":
	            case "LastHitBy":
	            	return true;
        		default:
        			return false;
        	}
        }

        public static void RemoveAllBondedExtraStatus(ICharacter character, IMap map, PacketHitList hitlist, bool msg) {
            //if the status has no target
            //if the status target does not have the same map ID
            //if the status target is not active
            PacketHitList.MethodStart(ref hitlist);

            RemoveBondedExtraStatus(character, map, "Wrap:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "Wrap:1", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "Bind:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "Bind:1", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "Clamp:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "Clamp:1", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "LeechSeed:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "LeechSeed:1", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "SkyDrop:0", hitlist, msg);
            RemoveBondedExtraStatus(character, map, "SkyDrop:1", hitlist, msg);
            RemoveExtraStatus(character, map, "LastHitBy", hitlist, msg);

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RemoveBondedExtraStatus(ICharacter character, IMap map, string statusName, PacketHitList hitlist, bool msg) {
            //when removing a targeted status
            //remove it from both parties
            //do it here, otherwise recursion will call an infinite loop
            PacketHitList.MethodStart(ref hitlist);

            ExtraStatus status = character.VolatileStatus.GetStatus(statusName);
            if (status == null) {
                PacketHitList.MethodEnded(ref hitlist);
                return;
            }
            //hitlist.AddPacketToMap(character.MapID, PacketBuilder.CreateBattleMsg(status, Text.WhiteSmoke), character.X, character.Y, 10);
            if (status.Target != null) {
                //Messenger.MapMsg(character.MapID, (status.Split(':')[1] == "1" && character.VolatileStatus.GetStatus(status).Target.VolatileStatus.GetStatus(status.Split(':')[0] + ":0") != null).ToString(), Text.WhiteSmoke);
                if (statusName.Split(':')[1] == "1" && status.Target.VolatileStatus.GetStatus(statusName.Split(':')[0] + ":0") != null) {
                    //Messenger.MapMsg(character.MapID, status + "!1", Text.WhiteSmoke);
                    RemoveExtraStatus(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), statusName.Split(':')[0] + ":0", hitlist, msg);
                    //Messenger.MapMsg(character.MapID, status + "!1", Text.WhiteSmoke);
                } else if (statusName.Split(':')[1] == "0" && status.Target.VolatileStatus.GetStatus(statusName.Split(':')[0] + ":1") != null) {
                    //Messenger.MapMsg(character.MapID, status + "!0", Text.WhiteSmoke);
                    RemoveExtraStatus(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), statusName.Split(':')[0] + ":1", hitlist, msg);
                }
            }

            RemoveExtraStatus(character, map, statusName, hitlist, msg);
            PacketHitList.MethodEnded(ref hitlist);
        }
        
        public static bool IsTeamStatus(string status) {
	        switch (status) {
	        	case "LightScreen":
				case "Reflect":
				case "Mist":
				case "Safeguard":
				case "LuckyChant":
				case "FutureSight":
				case "Wish":
				case "Embargo":
				case "HealBlock":
					return true;
				default:
					return false;
			}
        }

        public static void RemoveTeamStatus(ICharacter character, IMap map, PacketHitList hitlist, bool msg) {
            List<ExtraStatus> teamStatus = new List<ExtraStatus>();

            //string[] nameSegment = status.Name.Split(':');
			
			RemoveExtraStatus(character, map, "LightScreen", hitlist, msg);
			RemoveExtraStatus(character, map, "Reflect", hitlist, msg);
			RemoveExtraStatus(character, map, "Mist", hitlist, msg);
			RemoveExtraStatus(character, map, "Safeguard", hitlist, msg);
			RemoveExtraStatus(character, map, "LuckyChant", hitlist, msg);
			RemoveExtraStatus(character, map, "FutureSight", hitlist, msg);
			RemoveExtraStatus(character, map, "Wish", hitlist, msg);
			RemoveExtraStatus(character, map, "Embargo", hitlist, msg);
			RemoveExtraStatus(character, map, "HealBlock", hitlist, msg);
			
        }

        public static List<ExtraStatus> GetTeamStatus(ICharacter character) {
            
            return character.VolatileStatus.GetStatuses("LightScreen", "Reflect", "Mist", "Safeguard",
                "LuckyChant", "FutureSight", "Wish", "Embargo", "HealBlock");
        }

        public static Enums.Speed GetSpeedLimit(ICharacter character) {

            if (character.CharacterType == Enums.CharacterType.Recruit) {

                return ((Recruit)character).Owner.Player.SpeedLimit;
            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {

                return ((MapNpc)character).SpeedLimit;
            }
            return Enums.Speed.Running;
        }

        //public static void ChangeSpeedLimit(ICharacter character, Enums.Speed speed) {
        
        //    if (character.CharacterType == Enums.CharacterType.Recruit) {

        //        ((Recruit)character).Owner.Player.ge.SpeedLimit = speed;
        //    } else if (character.CharacterType == Enums.CharacterType.MapNpc) {

        //        ((MapNpc)character).SpeedLimit = speed;
        //    }

        //}
        public static void RefreshCharacterTraits(ICharacter character, IMap map, PacketHitList hitlist) {
            RefreshCharacterTraits(character, map, hitlist, true, true, true, true, true, true, true, true, true, true, true, true, true);
        }

        public static void RefreshCharacterTraits(ICharacter character, IMap map, PacketHitList hitlist, bool forme, bool sprite, bool type, bool ability,
            bool atkSpeed, bool confusion, bool speedLimit, bool mobility, bool visibility, bool darkness, bool swapLock, bool moves, bool extraStatus) {
            int point = 0;
            PacketHitList.MethodStart(ref hitlist);
            try {

                if (forme) {
                    RefreshCharacterForme(character, map, hitlist);
                    sprite = true;
                    type = true;
                    ability = true;
                }

                if (sprite) {
                    RefreshCharacterSprite(character, map, hitlist);
                }

                if (type) {
                    RefreshCharacterType(character, map, hitlist);
                    atkSpeed = true;
                    speedLimit = true;
                    mobility = true;
                }

                point = 1;
                if (ability) {
                    RefreshCharacterAbility(character, map, hitlist);
                    atkSpeed = true;
                    confusion = true;
                    speedLimit = true;
                    mobility = true;
                    darkness = true;
                }

                point = 2;
                RefreshCharacterAttackSpeed(character, map, hitlist);

                point = 3;
                RefreshCharacterConfusion(character, map, hitlist);

                point = 4;
                RefreshCharacterSpeedLimit(character, map, hitlist);

                point = 5;
                RefreshCharacterMobility(character, map, hitlist);

                point = 6;
                RefreshCharacterVisibility(character, map, hitlist);

                point = 7;
                RefreshCharacterDarkness(character, map, hitlist);

                point = 8;
                RefreshCharacterSwapLock(character, map, hitlist);

                point = 9;
                RefreshCharacterMoves(character, map, hitlist);

                point = 10;
                RefreshCharacterVolatileStatus(character, map, hitlist);

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: RefreshCharacterTraits", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
                Messenger.AdminMsg(point.ToString(), Text.Black);
                Messenger.AdminMsg(character.ToString(), Text.Black);
                Messenger.AdminMsg("Species #" + character.Species + ", Form #" + character.Form, Text.Black);
                throw new Exception();
            }

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterForme(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //forme
            if (HasAbility(character, "Forecast") && character.Species == 351) {
                switch (map.Weather) {
                    case Enums.Weather.Sunny: {
                            character.Form = 1;
                        }
                        break;
                    case Enums.Weather.Thunder:
                    case Enums.Weather.Raining: {
                            character.Form = 2;
                        }
                        break;
                    case Enums.Weather.Snowing:
                    case Enums.Weather.Snowstorm:
                    case Enums.Weather.Hail: {
                            character.Form = 3;
                        }
                        break;
                    default: {

                            character.Form = 0;
                        }
                        break;
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterSprite(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //sprite
            ExtraStatus status;

            int sprite = character.Sprite;

            character.CalculateOriginalSprite();

            if (character.HasActiveItem(259) && character.HeldItem.Num == 259 && map != null && map.Moral == Enums.MapMoral.House) {
                character.Sprite = character.HeldItem.Tag.ToInt();
            }

            status = character.VolatileStatus.GetStatus("Illusion");
            if (status != null) {
                character.Sprite = status.Counter;
            }

            if (character.VolatileStatus.GetStatus("Substitute") != null) {
                character.Sprite = 0;
                character.Form = 3;
            }

            if (character.VolatileStatus.GetStatus("SemiInvul") != null ||
            	character.VolatileStatus.GetStatus("SkyDrop:0") != null ||
            	character.VolatileStatus.GetStatus("SkyDrop:1") != null) {
                character.Sprite = 0;
                character.Form = 2;
            }
            if (sprite != character.Sprite) {
                if (character.CharacterType == Enums.CharacterType.Recruit) {
                    PacketBuilder.AppendSprite(((Recruit)character).Owner, hitlist);
                } else {

                    PacketBuilder.AppendNpcSprite(map, hitlist, ((MapNpc)character).MapSlot);
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterType(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //types
            ExtraStatus status;
            character.CalculateOriginalType();
            status = character.VolatileStatus.GetStatus("Type1");
            if (status != null) {
                character.Type1 = (Enums.PokemonType)status.Counter;
            }
            status = character.VolatileStatus.GetStatus("Type2");
            if (status != null) {
                character.Type2 = (Enums.PokemonType)status.Counter;
            }

            if (character.VolatileStatus.GetStatus("Roost") != null) {
                if (character.Type1 == Enums.PokemonType.Flying) character.Type1 = Enums.PokemonType.None;
                if (character.Type2 == Enums.PokemonType.Flying) character.Type2 = Enums.PokemonType.None;
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterAbility(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //abilites
            ExtraStatus status;
            character.CalculateOriginalAbility();

            status = character.VolatileStatus.GetStatus("Ability1");
            if (status != null) {
                character.Ability1 = status.Tag;
            }
            status = character.VolatileStatus.GetStatus("Ability2");
            if (status != null) {
                character.Ability2 = status.Tag;
            }
            status = character.VolatileStatus.GetStatus("Ability3");
            if (status != null) {
                character.Ability3 = status.Tag;
            }

            if (character.VolatileStatus.GetStatus("GastroAcid") != null) {
                character.Ability1 = "None";
                character.Ability2 = "None";
                character.Ability3 = "None";
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterAttackSpeed(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //regular attack timer

            character.TimeMultiplier = 1000;

            if (HasAbility(character, "Truant")) {
                character.TimeMultiplier *= 2;
            }
            if (HasAbility(character, "Unburden") && character.HeldItem == null) {
                character.TimeMultiplier *= 3;
                character.TimeMultiplier /= 4;
            }

            if (HasAbility(character, "Chlorophyll") && GetCharacterWeather(character) == Enums.Weather.Sunny) {
                character.TimeMultiplier *= 6;
                character.TimeMultiplier /= 10;
            }
            if (HasAbility(character, "Sand Rush") && GetCharacterWeather(character) == Enums.Weather.Sandstorm) {
                character.TimeMultiplier *= 6;
                character.TimeMultiplier /= 10;
            }
            if (HasAbility(character, "Swift Swim") &&
            (GetCharacterWeather(character) == Enums.Weather.Raining || GetCharacterWeather(character) == Enums.Weather.Thunder)) {
                character.TimeMultiplier *= 6;
                character.TimeMultiplier /= 10;
            }

            if (map != null && map.MapType == Enums.MapType.Instanced && ((InstancedMap)map).MapBase == 1945) {
                character.TimeMultiplier = 750;
                // Snowball game time multiplier override
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendTimeMultiplier(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterConfusion(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //confusion movements
            if (character.VolatileStatus.GetStatus("Confusion") != null && !(HasAbility(character, "Tangled Feet") && HasAbility(character, "Own Tempo"))) {
                character.Confused = true;
            } else {
                character.Confused = false;
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendConfusion(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterSpeedLimit(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //Movement Speed
            ExtraStatus status;

            int speed = 4;
            
            if (character.Species == 492 && character.Form == 1) {
            	speed++;
            }

            if (character.HasActiveItem(118)) {
                speed -= 2;
            }
            
            if (character.HasActiveItem(111)) {
            	speed -=1;
            }

            if ((GetCharacterWeather(character) == Enums.Weather.Snowing || GetCharacterWeather(character) == Enums.Weather.Snowstorm) && (character.Type1 == Enums.PokemonType.Ice || character.Type2 == Enums.PokemonType.Ice)) {
                speed++;
            }
            
            if (HasActiveBagItem(character, 3, -1, (int)GetCharacterWeather(character)) || HasActiveBagItem(character, 3, (int)GetCharacterWeather(character), -1)) {
                speed++;
            }

            //Type Gems
            if (character.HasActiveItem(598) && GetCharacterWeather(character) == Enums.Weather.Sunny) {
                speed++;
            }
            if (character.HasActiveItem(599) && GetCharacterWeather(character) == Enums.Weather.Cloudy) {
                speed++;
            }
            if (character.HasActiveItem(601) && (GetCharacterWeather(character) == Enums.Weather.None || GetCharacterWeather(character) == Enums.Weather.Ambiguous)) {
                speed++;
            }
            if (character.HasActiveItem(604) && (GetCharacterWeather(character) == Enums.Weather.None || GetCharacterWeather(character) == Enums.Weather.Ambiguous)) {
                speed++;
            }
            if (character.HasActiveItem(607) && GetCharacterWeather(character) == Enums.Weather.Sandstorm) {
                speed++;
            }
            if (character.HasActiveItem(608) && GetCharacterWeather(character) == Enums.Weather.Fog) {
                speed++;
            }

            if (HasAbility(character, "Speed Boost")) {
                speed++;
            }
            status = character.VolatileStatus.GetStatus("MovementSpeed");
            if (status != null) {
                speed += status.Counter;
            }

            if (character.StatusAilment == Enums.StatusAilment.Paralyze) {
                if (HasAbility(character, "Quick Feet")) {
                    speed++;
                } else if (speed > 3) {

                    speed = 3;
                }
            } else if (character.StatusAilment != Enums.StatusAilment.OK) {
                if (HasAbility(character, "Quick Feet")) {
                    speed++;
                }
            }

            if (speed > 6) speed = 6;
            if (speed < 1) speed = 1;

            if (map != null
            && (map.Moral == Enums.MapMoral.None || map.Moral == Enums.MapMoral.NoPenalty
            || (character.X >= 0 && character.X <= map.MaxX
            && character.Y >= 0 && character.Y <= map.MaxY
            && map.Tile[character.X, character.Y].Type == Enums.TileType.Arena))) {
                character.SpeedLimit = (Enums.Speed)speed;

            } else {
                character.SpeedLimit = Enums.Speed.Running;
            }

            if (character.VolatileStatus.GetStatus("Immobilize") != null ||
                character.VolatileStatus.GetStatus("Ingrain") != null ||
                character.VolatileStatus.GetStatus("FireSpin") != null ||
                character.VolatileStatus.GetStatus("Whirlpool") != null ||
                character.VolatileStatus.GetStatus("SandTomb") != null ||
                character.VolatileStatus.GetStatus("MagmaStorm") != null ||
                character.VolatileStatus.GetStatus("Bind:0") != null ||
                character.VolatileStatus.GetStatus("Clamp:0") != null ||
                character.VolatileStatus.GetStatus("Wrap:0") != null ||
                character.VolatileStatus.GetStatus("SkyDrop:0") != null ||
                character.VolatileStatus.GetStatus("Bind:1") != null ||
                character.VolatileStatus.GetStatus("Clamp:1") != null ||
                character.VolatileStatus.GetStatus("Wrap:1") != null ||
                character.VolatileStatus.GetStatus("SkyDrop:1") != null ||
                character.VolatileStatus.GetStatus("SolarBeam") != null ||
                character.VolatileStatus.GetStatus("RazorWind") != null ||
                character.VolatileStatus.GetStatus("FocusPunch") != null ||
                character.VolatileStatus.GetStatus("Dig") != null ||
                character.VolatileStatus.GetStatus("Dive") != null ||
                character.VolatileStatus.GetStatus("Bide") != null ||
                character.VolatileStatus.GetStatus("Avalanche") != null ||
                character.VolatileStatus.GetStatus("VitalThrow") != null ||
                character.VolatileStatus.GetStatus("Attract") != null) {
                character.SpeedLimit = Enums.Speed.Standing;
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {

                PacketBuilder.AppendSpeedLimit(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterMobility(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //Mobility

            for (int i = 0; i < character.Mobility.Length; i++) {
                character.Mobility[i] = false;
            }

            if (character.Type1 == Enums.PokemonType.Ground || character.Type2 == Enums.PokemonType.Ground) {
                character.Mobility[8] = true;
            }

            if (character.Type1 == Enums.PokemonType.Water || character.Type2 == Enums.PokemonType.Water) {
                character.Mobility[1] = true;
            }
            if (character.Type1 == Enums.PokemonType.Fire || character.Type2 == Enums.PokemonType.Fire) {
                character.Mobility[3] = true;
            }
            if ((character.Type1 == Enums.PokemonType.Flying || character.Type2 == Enums.PokemonType.Flying)
            && IsGroundImmune(character, map)) {
                character.Mobility[1] = true;
                character.Mobility[2] = true;
                character.Mobility[3] = true;
                character.Mobility[8] = true;
                //character.Mobility[5] = true;
            }
            if (character.Type1 == Enums.PokemonType.Ghost || character.Type2 == Enums.PokemonType.Ghost) {
                character.Mobility[1] = true;
                character.Mobility[2] = true;
                character.Mobility[3] = true;
                character.Mobility[4] = true;
                //character.Mobility[6] = true;
                //character.Mobility[7] = true;
            }
            
            if (character.Type1 == Enums.PokemonType.Ice || character.Type2 == Enums.PokemonType.Ice) {
                character.Mobility[5] = true;
            }

            if (character.Type1 == Enums.PokemonType.Bug || character.Type2 == Enums.PokemonType.Bug) {
                character.Mobility[6] = true;
            }

            if (character.Type1 == Enums.PokemonType.Grass || character.Type2 == Enums.PokemonType.Grass) {
                character.Mobility[7] = true;
            }

            if (character.VolatileStatus.GetStatus("Slip") != null) {
                character.Mobility[1] = true;
            }
            if (character.HasActiveItem(146)) {
                for (int i = 0; i < character.Mobility.Length; i++) {
                    character.Mobility[i] = true;
                }
            }
            if (character.VolatileStatus.GetStatus("SuperMobile") != null) {
                for (int i = 0; i < character.Mobility.Length; i++) {
                    character.Mobility[i] = true;
                }
            }

            if (HasAbility(character, "Levitate") || character.HasActiveItem(82)) {
            	if (IsGroundImmune(character, map)) {
	                character.Mobility[1] = true;
	                character.Mobility[2] = true;
	                character.Mobility[3] = true;
                }
            }
            
            if (character.HasActiveItem(349)) {
            	character.Mobility[1] = true;
            }
            
            if (character.HasActiveItem(805)) {
            	character.Mobility[3] = true;
            }
            
            if (character.HasActiveItem(814)) {
            	for (int i = 0; i < character.Mobility.Length; i++) {
                	character.Mobility[i] = false;
            	}
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendMobility(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterVisibility(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //invisibility
            ExtraStatus status;
            bool visible = true;

            status = character.VolatileStatus.GetStatus("SemiInvul");
            if (status != null && status.Tag == "ShadowForce" ||
                character.VolatileStatus.GetStatus("Invisible") != null) {
                visible = false;
            }

            if (map != null) {
                if (character.CharacterType == Enums.CharacterType.Recruit) {
                    Client client = ((Recruit)character).Owner;
                    hitlist.AddPacketToOthers(character, map, PacketBuilder.CreatePlayerActivation(client, visible), Enums.OutdateType.Location);
                    PacketBuilder.AppendVisibility(client, hitlist, visible);
                } else {
                    hitlist.AddPacketToOthers(character, map, PacketBuilder.CreateNpcActivation((MapNpc)character, visible), Enums.OutdateType.Location);
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterDarkness(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //sight range
            character.Darkness = -2;

            if (map != null && map.Darkness >= 0 && map.Darkness < 7 && HasAbility(character, "Keen Eye")) {
                character.Darkness = 7;
            }

            if (character.HasActiveItem(66)) {
                character.Darkness = -1;
            }
            if (character.HasActiveItem(348)) {
                character.Darkness = 0;
            }
            
            if (character.VolatileStatus.GetStatus("Blind") != null) {
                character.Darkness = 0;
            }

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendDarkness(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterSwapLock(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //Switching (Recruit only)
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                ((Recruit)character).Owner.Player.CanSwapActiveRecruit = true;
                if (character.VolatileStatus.GetStatus("Immobilize") != null ||
                character.VolatileStatus.GetStatus("Ingrain") != null ||
                character.VolatileStatus.GetStatus("FireSpin") != null ||
                character.VolatileStatus.GetStatus("Whirlpool") != null ||
                character.VolatileStatus.GetStatus("SandTomb") != null ||
                character.VolatileStatus.GetStatus("MagmaStorm") != null ||
                character.VolatileStatus.GetStatus("Bind:0") != null ||
                character.VolatileStatus.GetStatus("Clamp:0") != null ||
                character.VolatileStatus.GetStatus("Wrap:0") != null ||
                character.VolatileStatus.GetStatus("SkyDrop:0") != null ||
                character.VolatileStatus.GetStatus("Bind:1") != null ||
                character.VolatileStatus.GetStatus("Clamp:1") != null ||
                character.VolatileStatus.GetStatus("Wrap:1") != null ||
                character.VolatileStatus.GetStatus("SkyDrop:1") != null ||
                character.VolatileStatus.GetStatus("SolarBeam") != null ||
                character.VolatileStatus.GetStatus("SkullBash") != null ||
                character.VolatileStatus.GetStatus("RazorWind") != null ||
                character.VolatileStatus.GetStatus("SkyAttack") != null ||
                character.VolatileStatus.GetStatus("FocusPunch") != null ||
                character.VolatileStatus.GetStatus("Bide") != null ||
                character.VolatileStatus.GetStatus("Avalanche") != null ||
                character.VolatileStatus.GetStatus("VitalThrow") != null ||
                character.VolatileStatus.GetStatus("SemiInvul") != null) {
                    ((Recruit)character).Owner.Player.CanSwapActiveRecruit = false;
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }


        public static void RefreshCharacterMoves(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //Move restrictions
            ExtraStatus status;

            for (int i = 0; i < 4; i++) {
                if (character.VolatileStatus.GetStatus("MoveSeal:" + i) != null) {
                    character.Moves[i].Sealed = true;
                    continue;
                }
                status = character.VolatileStatus.GetStatus("Disable");
                if (status != null && status.Counter == i) {
                    character.Moves[i].Sealed = true;
                    continue;
                }

                if (character.VolatileStatus.GetStatus("Taunt") != null && character.Moves[i].MoveNum > 0 && MoveManager.Moves[character.Moves[i].MoveNum].MoveCategory == Enums.MoveCategory.Status) {
                    character.Moves[i].Sealed = true;
                    continue;
                }
                status = character.VolatileStatus.GetStatus("Encore");
                ExtraStatus lastMoveStatus = character.VolatileStatus.GetStatus("LastUsedMoveSlot");
                if (status != null && lastMoveStatus != null && lastMoveStatus.Counter != i) {
                    character.Moves[i].Sealed = true;
                    continue;
                }
                status = character.VolatileStatus.GetStatus("Torment");
                if (status != null && lastMoveStatus != null && lastMoveStatus.Counter == i) {
                    character.Moves[i].Sealed = true;
                    continue;
                }

                character.Moves[i].Sealed = false;
            }

            //move attack timer
            //for (int i = 0; i < 4; i++) {
            //    if (character.Moves[i].MoveNum > -1) {
            //        character.Moves[i].AttackTime = MoveManager.Moves[character.Moves[i].MoveNum].HitTime;
            //    } else {
            //        character.Moves[i].AttackTime = 0;
            //    }
            //}

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendPlayerMoves(((Recruit)character).Owner, hitlist);
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RefreshCharacterVolatileStatus(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            //volatile status
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendVolatileStatus(((Recruit)character).Owner, hitlist);
            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                if (map != null) {
                    PacketBuilder.AppendNpcVolatileStatus(map, hitlist, ((MapNpc)character).MapSlot);
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

    }
}