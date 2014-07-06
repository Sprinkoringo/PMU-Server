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


namespace DataManager.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MapNpc
    {
        #region Fields

        //readonly int MaxStat = 255;

        //int atk;
        //int def;
        bool hpChanged;
        int lastHP;
        int mapSlot;
        

        // Final Stats
        //int maxHP;
        //int mBaseAtk;
        //int mAtkBonus;
        //int mAtkBoost;
        //int mBaseDef;
        //int mBaseMaxHP;
        //int mBaseSpclAtk;
        //int mBaseSpclDef;
        //int mBaseSpd;
        //int mDefBonus;
        //int mDefBoost;
        //int mMaxHPBonus;
        //int mMaxHPBoost;
        //int mSpclAtkBonus;
        //int mSpclAtkBoost;
        //int mSpclDefBonus;
        //int mSpclDefBoost;
        //int mSpdBonus;
        //int mSpdBoost;
        //int spclAtk;
        //int spclDef;
        //int spd;

        #endregion Fields

        #region Constructors

        public MapNpc(int mapSlot) {
            this.mapSlot = mapSlot;
            //Mobility = new bool[16];
            //TimeMultiplier = 1000;
            Moves = new Characters.Move[4];
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i] = new Characters.Move();
            }

            VolatileStatus = new List<Characters.VolatileStatus>();
            HeldItem = new Characters.InventoryItem();
            HeldItem.Num = -1;
        }

        #endregion Constructors

        #region Properties

        //public string Ability1 {
        //    get;
        //    set;
        //}

        //public string Ability2 {
        //    get;
        //    set;
        //}

        //public string Ability3 {
        //    get;
        //    set;
        //}

        public int AccuracyBuff {
            get;
            set;
        }

        //public int Atk {
        //    get {
        //        if (atk > MaxStat) {
        //            return MaxStat;
        //        } else {
        //            return atk;
        //        }
        //    }
        //}

        public int AtkBonus {
            get;
            set;
        }

        //public int AtkBoost {
        //    get {
        //        return mAtkBoost;
        //    }
        //    set {
        //        mAtkBoost = value;
        //        atk = BaseAtk + mAtkBonus + mAtkBoost;
        //    }
        //}

        public int AttackBuff {
            get;
            set;
        }

        public int AttackTimer {
            get;
            set;
        }

        //public int BaseAtk {
        //    get {
        //        return mBaseAtk;
        //    }
        //    set {
        //        mBaseAtk = value;
        //        atk = BaseAtk + mAtkBonus + mAtkBoost;
        //    }
        //}

        //public int BaseDef {
        //    get {
        //        return mBaseDef;
        //    }
        //    set {
        //        mBaseDef = value;
        //        def = mBaseDef + mDefBonus + mDefBoost;
        //    }
        //}

        //public int BaseMaxHP {
        //    get {
        //        return mBaseMaxHP;
        //    }
        //    set {
        //        mBaseMaxHP = value;
        //        maxHP = mBaseMaxHP + mMaxHPBonus + mMaxHPBoost;
        //        if (HP > maxHP) {
        //            HP = maxHP;
        //        }
        //    }
        //}

        //public int BaseSpclAtk {
        //    get {
        //        return mBaseSpclAtk;
        //    }
        //    set {
        //        mBaseSpclAtk = value;
        //        spclAtk = mBaseSpclAtk + mSpclAtkBonus + mSpclAtkBoost;
        //    }
        //}

        //public int BaseSpclDef {
        //    get {
        //        return mBaseSpclDef;
        //    }
        //    set {
        //        mBaseSpclDef = value;
        //        spclDef = mBaseSpclDef + mSpclDefBonus + mSpclDefBoost;
        //    }
        //}

        //public int BaseSpd {
        //    get {
        //        return mBaseSpd;
        //    }
        //    set {
        //        mBaseSpd = value;
        //        spd = mBaseSpd + mSpdBonus + mSpdBoost;
        //    }
        //}

        //public int ConfusionStepCounter {
        //    get;
        //    set;
        //}

        //public int Def {
        //    get {
        //        if (def > MaxStat) {
        //            return MaxStat;
        //        } else {
        //            return def;
        //        }
        //    }
        //}

        public int DefBonus {
            get;
            set;
        }

        //public int DefBoost {
        //    get {
        //        return mDefBoost;
        //    }
        //    set {
        //        mDefBoost = value;
        //        def = mBaseDef + mDefBonus + mDefBoost;
        //    }
        //}

        public int DefenseBuff {
            get;
            set;
        }

        public byte Direction {
            get;
            set;
        }

        public int EvasionBuff {
            get;
            set;
        }

        public int Form {
            get;
            set;
        }

        public Characters.InventoryItem HeldItem {
            get;
            set;
        }

        public bool HitByMove {
            get;
            set;
        }

        public virtual int HP {
            get;
            set;
        }

        public bool HPChanged {
            get { return hpChanged; }
        }

        public int HPRemainder {
            get;
            set;
        }

        public int HPStepCounter {
            get;
            set;
        }

        public int IQ {
            get;
            set;
        }

        public int Level {
            get;
            set;
        }

        //public int Species { get { return Npcs.NpcManager.Npcs[Num].Species; } }

        public int MapSlot {
            get { return mapSlot; }
        }

        //public int MaxHP {
        //    get {
        //        return maxHP;
        //    }
        //}

        public int MaxHPBonus {
            get;
            set;
        }

        //public int MaxHPBoost {
        //    get {
        //        return mMaxHPBoost;
        //    }
        //    set {
        //        mMaxHPBoost = value;
        //        maxHP = mBaseMaxHP + mMaxHPBonus + mMaxHPBoost;
        //        if (HP > maxHP) {
        //            HP = maxHP;
        //        }
        //    }
        //}

        //public bool[] Mobility {
        //    get;
        //    set;
        //}

        public Characters.Move[] Moves {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public int Num {
            get;
            set;
        }

        public int PauseTimer {
            get;
            set;
        }

        public byte Sex {
            get;
            set;
        }

        public byte Shiny {
            get;
            set;
        }

        public int SpAtkBuff {
            get;
            set;
        }

        //public int SpclAtk {
        //    get {
        //        if (spclAtk > MaxStat) {
        //            return MaxStat;
        //        } else {
        //            return spclAtk;
        //        }
        //    }
        //}

        public int SpclAtkBonus {
            get;
            set;
        }

        //public int SpclAtkBoost {
        //    get {
        //        return mSpclAtkBoost;
        //    }
        //    set {
        //        mSpclAtkBoost = value;
        //        spclAtk = mBaseSpclAtk + mSpclAtkBonus + mSpclAtkBoost;
        //    }
        //}

        //public int SpclDef {
        //    get {
        //        if (spclDef > MaxStat) {
        //            return MaxStat;
        //        } else {
        //            return spclDef;
        //        }
        //    }
        //}

        public int SpclDefBonus {
            get;
            set;
        }

        //public int SpclDefBoost {
        //    get {
        //        return mSpclDefBoost;
        //    }
        //    set {
        //        mSpclDefBoost = value;
        //        spclDef = mBaseSpclDef + mSpclDefBonus + mSpclDefBoost;
        //    }
        //}

        //public int Spd {
        //    get {
        //        if (spd > MaxStat) {
        //            return MaxStat;
        //        } else {
        //            return spd;
        //        }
        //    }
        //}

        public int SpdBonus {
            get;
            set;
        }

        //public int SpdBoost {
        //    get {
        //        return mSpdBoost;
        //    }
        //    set {
        //        mSpdBoost = value;
        //        spd = mBaseSpd + mSpdBonus + mSpdBoost;
        //    }
        //}

        public int SpDefBuff {
            get;
            set;
        }

        public int SpeedBuff {
            get;
            set;
        }

        //public byte SpeedLimit {
        //    get;
        //    set;
        //}

        //public int Sprite {
        //    get;
        //    set;
        //}

        public byte StatusAilment {
            get;
            set;
        }

        public int StatusAilmentCounter {
            get;
            set;
        }

        public string Target {
            get;
            set;
        }

        //public int TimeMultiplier {
        //    get;
        //    set;
        //}

        //public byte Type1 {
        //    get;
        //    set;
        //}

        //public byte Type2 {
        //    get;
        //    set;
        //}

        public List<Characters.VolatileStatus> VolatileStatus {
            get;
            set;
        }

        public int X {
            get;
            set;
        }

        public int Y {
            get;
            set;
        }

        #endregion Properties
    }
}