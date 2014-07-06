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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Moves;
using Server.Maps;

namespace Server.Combat
{
    public class BattleSetup
    {
        IMap attackerMap;
        IMap defenderMap;

        public ICharacter Attacker { get; set; }
        public ICharacter Defender { get; set; }

        public IMap AttackerMap {
            get {
                if (attackerMap == null) {
                    attackerMap = MapManager.RetrieveActiveMap(Attacker.MapID);
                }
                return attackerMap;
            }
        }
        public IMap DefenderMap {
            get {
                if (defenderMap == null) {
                    defenderMap = MapManager.RetrieveActiveMap(Defender.MapID);
                }
                return defenderMap;
            }
        }

        public int AttackStat { get; set; }
        public int DefenseStat { get; set; }

        //AttackerLevel (may not be needed)

        public int moveSlot { get; set; }
        public int moveIndex { get; set; }
        public Move Move { get; set; }

        public bool Cancel { get; set; }
        public int AttackerMultiplier { get; set; }
        public int Multiplier { get; set; }
        public bool Hit { get; set; }
        public int Damage { get; set; }
        public bool KnockedOut { get; set; }
        public ulong ExpGained { get; set; }
        internal Network.PacketHitList packetStack;
        public Network.PacketHitList PacketStack {
            get { return packetStack; }
        }
        public List<String> BattleTags { get; set; }


        public BattleSetup() {
            AttackerMultiplier = 1000;
            Multiplier = 1000;
            Move = new Move();
            Network.PacketHitList.MethodStart(ref packetStack);
            BattleTags = new List<string>();
        }

        public void SetupMove(Move move) {

            Move = new Move();
            Move.Name = move.Name;
            Move.MaxPP = move.MaxPP;
            Move.EffectType = move.EffectType;
            Move.Element = move.Element;
            Move.MoveCategory = move.MoveCategory;
            Move.RangeType = move.RangeType;
            Move.Range = move.Range;
            Move.TargetType = move.TargetType;
            Move.Data1 = move.Data1;
            Move.Data2 = move.Data2;
            Move.Data3 = move.Data3;
            Move.Accuracy = move.Accuracy;
            Move.HitTime = move.HitTime;
            Move.AdditionalEffectData1 = move.AdditionalEffectData1;
            Move.AdditionalEffectData2 = move.AdditionalEffectData2;
            Move.AdditionalEffectData3 = move.AdditionalEffectData3;
            Move.PerPlayer = move.PerPlayer;
            Move.KeyItem = move.KeyItem;
            Move.Sound = move.Sound;
            Move.AttackerAnim.AnimationType = move.AttackerAnim.AnimationType;
            Move.AttackerAnim.AnimationIndex = move.AttackerAnim.AnimationIndex;
            Move.AttackerAnim.FrameSpeed = move.AttackerAnim.FrameSpeed;
            Move.AttackerAnim.Repetitions = move.AttackerAnim.Repetitions;
            Move.TravelingAnim.AnimationType = move.TravelingAnim.AnimationType;
            Move.TravelingAnim.AnimationIndex = move.TravelingAnim.AnimationIndex;
            Move.TravelingAnim.FrameSpeed = move.TravelingAnim.FrameSpeed;
            Move.TravelingAnim.Repetitions = move.TravelingAnim.Repetitions;
            Move.DefenderAnim.AnimationType = move.DefenderAnim.AnimationType;
            Move.DefenderAnim.AnimationIndex = move.DefenderAnim.AnimationIndex;
            Move.DefenderAnim.FrameSpeed = move.DefenderAnim.FrameSpeed;
            Move.DefenderAnim.Repetitions = move.DefenderAnim.Repetitions;

        }

        public Move SetdownMove() {
            Move newMove = new Move();

            newMove.Name = Move.Name;
            newMove.MaxPP = Move.MaxPP;
            newMove.EffectType = Move.EffectType;
            newMove.Element = Move.Element;
            newMove.MoveCategory = Move.MoveCategory;
            newMove.RangeType = Move.RangeType;
            newMove.Range = Move.Range;
            newMove.TargetType = Move.TargetType;
            newMove.Data1 = Move.Data1;
            newMove.Data2 = Move.Data2;
            newMove.Data3 = Move.Data3;
            newMove.Accuracy = Move.Accuracy;
            newMove.HitTime = Move.HitTime;
            newMove.AdditionalEffectData1 = Move.AdditionalEffectData1;
            newMove.AdditionalEffectData2 = Move.AdditionalEffectData2;
            newMove.AdditionalEffectData3 = Move.AdditionalEffectData3;
            newMove.PerPlayer = Move.PerPlayer;
            newMove.KeyItem = Move.KeyItem;
            newMove.Sound = Move.Sound;
            newMove.AttackerAnim.AnimationType = Move.AttackerAnim.AnimationType;
            newMove.AttackerAnim.AnimationIndex = Move.AttackerAnim.AnimationIndex;
            newMove.AttackerAnim.FrameSpeed = Move.AttackerAnim.FrameSpeed;
            newMove.AttackerAnim.Repetitions = Move.AttackerAnim.Repetitions;
            newMove.TravelingAnim.AnimationType = Move.TravelingAnim.AnimationType;
            newMove.TravelingAnim.AnimationIndex = Move.TravelingAnim.AnimationIndex;
            newMove.TravelingAnim.FrameSpeed = Move.TravelingAnim.FrameSpeed;
            newMove.TravelingAnim.Repetitions = Move.TravelingAnim.Repetitions;
            newMove.DefenderAnim.AnimationType = Move.DefenderAnim.AnimationType;
            newMove.DefenderAnim.AnimationIndex = Move.DefenderAnim.AnimationIndex;
            newMove.DefenderAnim.FrameSpeed = Move.DefenderAnim.FrameSpeed;
            newMove.DefenderAnim.Repetitions = Move.DefenderAnim.Repetitions;
            return newMove;
        }
    }
}
