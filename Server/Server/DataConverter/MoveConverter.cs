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
using System.Text;

namespace Server.DataConverter {
    public class MoveConverter {
        public static void ConvertV1ToV2(int num) {
            DataConverter.Moves.V2.Move moveV2 = new Server.DataConverter.Moves.V2.Move();

            DataConverter.Moves.V1.Move moveV1 = Server.DataConverter.Moves.V1.MoveManager.LoadMove(num);

            moveV2.Name = moveV1.Name;
            moveV2.LevelReq = moveV1.LevelReq;
            moveV2.Range = (Enums.MoveRange)0;
            moveV2.TargetType = (Enums.MoveTarget)0;
            moveV2.MoveType = (Enums.PokemonType)0;
            moveV2.EffectType = moveV1.Type;
            moveV2.MaxPP = 1;
            moveV2.Data1 = moveV1.Data1;
            moveV2.Data2 = moveV1.Data2;
            moveV2.Data3 = moveV1.Data3;
            moveV2.Big = moveV1.Big;
            moveV2.Sound = moveV1.Sound;
            moveV2.SpellAnim = moveV1.SpellAnim;
            moveV2.SpellDone = moveV1.SpellDone;
            moveV2.SpellTime = moveV1.SpellTime;
            moveV2.IsKey = moveV1.IsKey;
            moveV2.KeyItem = moveV1.KeyItem;

            Moves.V2.MoveManager.SaveMove(moveV2, num);
        }

        public static void ConvertV2ToV3(int num) {
            DataConverter.Moves.V3.Move moveV3 = new Server.DataConverter.Moves.V3.Move();

            DataConverter.Moves.V2.Move moveV2 = Server.DataConverter.Moves.V2.MoveManager.LoadMove(num);

            moveV3.Name = moveV2.Name;
            moveV3.MaxPP = moveV2.MaxPP;
            moveV3.EffectType = moveV2.EffectType;
            moveV3.Element = moveV2.MoveType;
            moveV3.MoveCategory = Enums.MoveCategory.Physical;
            moveV3.RangeType = moveV2.Range;
            moveV3.Range = 1;
            moveV3.TargetType = Enums.MoveTarget.Foes;

            moveV3.Data1 = moveV2.Data1;
            moveV3.Data2 = moveV2.Data2;
            moveV3.Data3 = moveV2.Data3;
            moveV3.Accuracy = 100;
            moveV3.AdditionalEffectData1 = 0;
            moveV3.AdditionalEffectData2 = 0;
            moveV3.AdditionalEffectData3 = 0;
            moveV3.PerPlayer = true;
            moveV3.IsKey = moveV2.IsKey;
            moveV3.KeyItem = moveV2.KeyItem;

            moveV3.Sound = moveV2.Sound;
            moveV3.Big = moveV2.Big;
            moveV3.SpellAnim = moveV2.SpellAnim;
            moveV3.SpellDone = moveV2.SpellDone;
            moveV3.SpellTime = moveV2.SpellTime;

            Moves.V3.MoveManager.SaveMove(moveV3, num);
        }

        public static void ConvertV3ToV4(int num) {
            DataConverter.Moves.V4.Move moveV4 = new Server.DataConverter.Moves.V4.Move();

            DataConverter.Moves.V3.Move moveV3 = Server.DataConverter.Moves.V3.MoveManager.LoadMove(num);

            moveV4.Name = moveV3.Name;
            moveV4.MaxPP = moveV3.MaxPP;
            moveV4.EffectType = moveV3.EffectType;
            moveV4.Element = moveV3.Element;
            moveV4.MoveCategory = moveV3.MoveCategory;
            moveV4.RangeType = moveV3.RangeType;
            moveV4.Range = moveV3.Range;
            moveV4.TargetType = moveV3.TargetType;

            moveV4.Data1 = moveV3.Data1;
            moveV4.Data2 = moveV3.Data2;
            moveV4.Data3 = moveV3.Data3;
            moveV4.Accuracy = moveV3.Accuracy;
            moveV4.HitTime = 1000;
            moveV4.AdditionalEffectData1 = moveV3.AdditionalEffectData1;
            moveV4.AdditionalEffectData2 = moveV3.AdditionalEffectData2;
            moveV4.AdditionalEffectData3 = moveV3.AdditionalEffectData3;
            moveV4.PerPlayer = moveV3.PerPlayer;
            moveV4.KeyItem = moveV3.KeyItem;

            moveV4.Sound = moveV3.Sound;
            
            moveV4.AttackerAnim.AnimationType = Enums.MoveAnimationType.Normal;
            moveV4.AttackerAnim.AnimationIndex = -1;
            moveV4.AttackerAnim.FrameSpeed = 60;
            moveV4.AttackerAnim.Repetitions = 1;

            moveV4.TravelingAnim.AnimationType = Enums.MoveAnimationType.Normal;
            moveV4.TravelingAnim.AnimationIndex = -1;
            moveV4.TravelingAnim.FrameSpeed = 60;
            moveV4.TravelingAnim.Repetitions = 1;

            moveV4.DefenderAnim.AnimationType = Enums.MoveAnimationType.Normal;
            moveV4.DefenderAnim.AnimationIndex = moveV3.SpellAnim;
            moveV4.DefenderAnim.FrameSpeed = moveV3.SpellTime;
            moveV4.DefenderAnim.Repetitions = moveV3.SpellDone;

            Moves.V4.MoveManager.SaveMove(moveV4, num);
        }

        public static void ConvertV4ToV5(int num) {
            DataConverter.Moves.V5.Move moveV5 = new Server.DataConverter.Moves.V5.Move();

            DataConverter.Moves.V4.Move moveV4 = Server.DataConverter.Moves.V4.MoveManager.LoadMove(num);

            moveV5.Name = moveV4.Name;
            moveV5.MaxPP = moveV4.MaxPP;
            moveV5.EffectType = moveV4.EffectType;
            moveV5.Element = moveV4.Element;
            moveV5.MoveCategory = moveV4.MoveCategory;
            moveV5.RangeType = moveV4.RangeType;
            moveV5.Range = moveV4.Range;
            moveV5.TargetType = moveV4.TargetType;

            moveV5.Data1 = moveV4.Data1;
            moveV5.Data2 = moveV4.Data2;
            moveV5.Data3 = moveV4.Data3;
            moveV5.Accuracy = moveV4.Accuracy;
            moveV5.HitTime = moveV4.HitTime;
            moveV5.HitFreeze = true;
            moveV5.AdditionalEffectData1 = moveV4.AdditionalEffectData1;
            moveV5.AdditionalEffectData2 = moveV4.AdditionalEffectData2;
            moveV5.AdditionalEffectData3 = moveV4.AdditionalEffectData3;
            moveV5.PerPlayer = moveV4.PerPlayer;
            moveV5.KeyItem = moveV4.KeyItem;

            moveV5.Sound = moveV4.Sound;

            moveV5.AttackerAnim.AnimationType = moveV4.AttackerAnim.AnimationType;
            moveV5.AttackerAnim.AnimationIndex = moveV4.AttackerAnim.AnimationIndex;
            moveV5.AttackerAnim.FrameSpeed = moveV4.AttackerAnim.FrameSpeed;
            moveV5.AttackerAnim.Repetitions = moveV4.AttackerAnim.Repetitions;

            moveV5.TravelingAnim.AnimationType = moveV4.TravelingAnim.AnimationType;
            moveV5.TravelingAnim.AnimationIndex = moveV4.TravelingAnim.AnimationIndex;
            moveV5.TravelingAnim.FrameSpeed = moveV4.TravelingAnim.FrameSpeed;
            moveV5.TravelingAnim.Repetitions = moveV4.TravelingAnim.Repetitions;

            moveV5.DefenderAnim.AnimationType = moveV4.DefenderAnim.AnimationType;
            moveV5.DefenderAnim.AnimationIndex = moveV4.DefenderAnim.AnimationIndex;
            moveV5.DefenderAnim.FrameSpeed = moveV4.DefenderAnim.FrameSpeed;
            moveV5.DefenderAnim.Repetitions = moveV4.DefenderAnim.Repetitions;

            Moves.V5.MoveManager.SaveMove(moveV5, num);
        }
    }
}
