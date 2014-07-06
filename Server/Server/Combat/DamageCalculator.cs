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
using Server.Players;
using Server.Npcs;
using Server.Pokedex;
using Server.Maps;

namespace Server.Combat
{
    public class DamageCalculator
    {
        //public static int CalculateCriticalDamagePVP(int attacker, int victim) {
        //    int baseDamage = PlayerManager.Players[attacker].GetDamage();
        //    return baseDamage + Math.Rand(0, (baseDamage / 2)) - PlayerManager.Players[victim].GetProtection();
        //}

        //public static int CalculateDamagePVP(int attacker, int victim) {
        //    return PlayerManager.Players[attacker].GetDamage() - PlayerManager.Players[victim].GetProtection();
        //}

        //public static int CalculateDamagePVN(int attacker, Map map, int mapNpcNum) {
        //    return PlayerManager.Players[attacker].GetDamage() - Convert.ToInt32(map.ActiveNpc[mapNpcNum].Def / 2);
        //}

        //public static int CalculateCriticalDamagePVN(int attacker, Map map, int mapNpcNum) {
        //    int baseDamage = PlayerManager.Players[attacker].GetDamage();
        //    return baseDamage + Math.Rand(0, baseDamage / 2) - Convert.ToInt32(map.ActiveNpc[mapNpcNum].Def / 2);
        //}

        //public static int CalculateDamageNVP(Map map, int mapNpcNum, int victim) {
        //    return (map.ActiveNpc[mapNpcNum].Str /* / 2 */) - PlayerManager.Players[victim].GetProtection();
        //}
        //attacker: row
        //defender: column
        static int[,] TypeMatchup = new int[19, 19] {{3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
                                                     {3,3,4,3,3,2,2,2,2,2,4,3,3,3,2,4,3,2,3},
                                                     {3,3,2,3,3,2,2,3,3,4,3,3,3,3,3,4,3,3,3},
                                                     {3,3,3,4,3,0,3,3,3,3,3,3,3,3,3,3,3,2,3},
                                                     {3,3,3,2,2,3,3,3,4,3,2,0,3,3,3,3,3,3,4},
                                                     {3,3,4,4,3,3,4,2,3,3,3,3,3,3,2,3,3,2,3},
                                                     {3,2,4,3,3,2,3,3,2,0,3,3,4,4,2,2,4,4,3},
                                                     {3,4,3,2,3,3,3,2,3,3,4,3,4,3,3,3,2,4,2},
                                                     {3,4,3,3,2,3,4,3,3,3,4,3,3,3,3,3,2,2,3},
                                                     {3,3,2,3,3,3,3,3,3,4,3,3,3,0,3,4,3,3,3},
                                                     {3,2,3,2,3,3,3,2,2,3,2,4,3,3,2,3,4,2,4},
                                                     {3,2,3,3,4,3,3,4,0,3,2,3,3,3,4,3,4,4,3},
                                                     {3,3,3,4,3,3,3,2,4,3,4,4,2,3,3,3,3,2,2},
                                                     {3,3,3,3,3,3,3,3,3,0,3,3,3,3,3,3,2,2,3},
                                                     {3,3,3,3,3,4,3,3,3,2,4,2,3,3,2,3,2,0,3},
                                                     {3,3,0,3,3,3,4,3,3,3,3,3,3,3,4,2,3,2,3},
                                                     {3,4,3,3,3,3,2,4,4,3,3,2,4,3,3,3,3,2,3},
                                                     {3,3,3,3,2,4,3,2,3,3,3,3,4,3,3,3,4,2,2},
                                                     {3,3,3,2,3,3,3,4,3,3,2,4,3,3,3,3,4,3,2}};

        //static int[] AttackBuffLevels = new int[21] { 128, 133, 138, 143, 148, 153, 161, 171, 179, 204, 256, 307, 332, 358, 384, 409, 422, 435, 448, 460, 473 };
        static int[] AttackBuffLevels = new int[21] {128,133,138,143,148,153,161,171,179,204,256,307,358,409,460,512,563,614,665,716,768};
        //static int[] DefenseBuffLevels = new int[21] {7,12,25,38,51,64,76,102,128,179,256,332,409,486,537,588,640,691,742,793,844};
        static int[] DefenseBuffLevels = new int[21] { 7, 12, 25, 38, 51, 64, 76, 102, 128, 179, 256, 307, 358, 409, 460, 512, 563, 614, 665, 716, 768 };
        static int[] SpeedBuffLevels = new int[21] { 7, 12, 25, 38, 51, 64, 76, 102, 128, 179, 256, 332, 409, 486, 537, 588, 640, 691, 742, 793, 844 };
        static int[] AccuracyBuffLevels = new int[21] {84,89,94,102,110,115,140,153,179,204,256,320,384,409,422,435,448,460,473,486,512};
        static int[] EvasionBuffLevels = new int[21] {512,486,473,460,448,435,422,409,384,345,263,204,179,153,128,102,89,76,64,51,38};
        public static int[] Effectiveness = new int[9] {2, 8, 16, 64, 96, 160, 256, 416, 640};


        public static int CalculateDamage(int moveBasePower, int attackerStr, int attackerLevel, int victimDef) {
            //int A = attackerStr + (moveBasePower) / 5;
            //int B = attackerLevel;
            //int C = victimDef;
            //int D = ((A - C) / 8) + (B * 43690 / 65536);
            //int X = Math.Rand(0, 16384);
            //return ((((D * 2) - C) + 10) + ((D * D) * 3276 / 65536));//* (57344 + X) / 65536;


            return ((((((attackerLevel * 2 / 3) + 2) * (25 + moveBasePower / 2) * attackerStr / 50) / victimDef) + 2) * Math.Rand(90, 101) / 100);

        }

        //public static int CalculateDamageCritical(int moveBasePower, int attackerStr, int attackerLevel, int victimDef) {
        //    int A = attackerStr + (moveBasePower) / 5;
        //    int B = attackerLevel;
        //    int C = victimDef;
        //    int D = ((A - C) / 8) + (B * 43690 / 65536);
        //    int X = Math.Rand(0, 16384);
        //    return (int)((((D * 2) - C) + 10) + ((D * D) * 3276 / 65536) * 1.5) * (57344 + X) / 65536;
        //}

        public static int CalculateTypeMatchup(Enums.PokemonType attackerType, Enums.PokemonType targetType)
        {
            return TypeMatchup[(int)attackerType, (int)targetType];

        }

        public static int ApplyAttackBuff(int baseStat, int buffLevel)
        {
            if (buffLevel < 0) buffLevel = 0;
            if (buffLevel > 20) buffLevel = 20;
            return baseStat * AttackBuffLevels[buffLevel + 10];
        }

        public static int ApplyDefenseBuff(int baseStat, int buffLevel)
        {
            if (buffLevel < 0) buffLevel = 0;
            if (buffLevel > 20) buffLevel = 20;
            return baseStat * DefenseBuffLevels[buffLevel + 10];
        }
        
        public static int ApplySpeedBuff(int baseStat, int buffLevel)
        {
            if (buffLevel < 0) buffLevel = 0;
            if (buffLevel > 20) buffLevel = 20;
            return baseStat * SpeedBuffLevels[buffLevel + 10];
        }

        public static int ApplyAccuracyBuff(int baseAcc, int buffLevel)
        {
            if (buffLevel < 0) buffLevel = 0;
            if (buffLevel > 20) buffLevel = 20;
            return baseAcc * AccuracyBuffLevels[buffLevel + 10];
        }

        public static int ApplyEvasionBuff(int baseAcc, int buffLevel)
        {
            if (buffLevel < 0) buffLevel = 0;
            if (buffLevel > 20) buffLevel = 20;
            return baseAcc * EvasionBuffLevels[buffLevel + 10];
        }
    }
}
