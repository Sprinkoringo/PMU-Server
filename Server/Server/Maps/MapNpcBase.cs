namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Players;

    public class MapNpcBase
    {

        int mBaseAtk;
        int mAtkBoost;
        int mBaseDef;
        int mDefBoost;
        int mBaseMaxHP;
        int mMaxHPBoost;
        int mBaseSpclAtk;
        int mSpclAtkBoost;
        int mBaseSpclDef;
        int mSpclDefBoost;
        int mBaseSpd;
        int mSpdBoost;

        DataManager.Maps.MapNpc rawNpc;

        public DataManager.Maps.MapNpc RawNpc {
            get { return rawNpc; }
        }

        public MapNpcBase(DataManager.Maps.MapNpc rawNpc) {
            this.rawNpc = rawNpc;
            Darkness = -2;
            Mobility = new bool[16];
            TimeMultiplier = 1000;
            AttackTimer = new TickCount(Core.GetTickCount().Tick);
            PauseTimer = new TickCount(Core.GetTickCount().Tick);
        }

        #region Properties

        public string Name {
            get { return rawNpc.Name; }
            set { rawNpc.Name = value; }
        }
        public Enums.Coloration Shiny {
            get { return (Enums.Coloration)rawNpc.Shiny; }
            set { rawNpc.Shiny = (byte)value; }
        }
        public int Form {
            get { return rawNpc.Form; }
            set { rawNpc.Form = value; }
        }
        public int Level {
            get { return rawNpc.Level; }
            set { rawNpc.Level = value; }
        }

        public int Num {
            get { return rawNpc.Num; }
            set { rawNpc.Num = value; }
        }
        public int Sprite {
            get;
            set;
        }
        public Enums.Sex Sex {
            get { return (Enums.Sex)rawNpc.Sex; }
            set { rawNpc.Sex = (byte)value; }
        }
        public Enums.PokemonType Type1 {
            get;
            set;
        }
        public Enums.PokemonType Type2 {
            get;
            set;
        }
        public string Ability1 {
            get;
            set;
        }
        public string Ability2 {
            get;
            set;
        }
        public string Ability3 {
            get;
            set;
        }

        public bool[] Mobility {
            get;
            set;
        }
        public int TimeMultiplier {
            get;
            set;
        }
        
        public int Darkness { get; set; }


        public RecruitMove[] Moves { get; set; }

        public virtual int HP {
            get { return rawNpc.HP; }
            set { rawNpc.HP = value; }
        }
        public int HPRemainder {
            get { return rawNpc.HPRemainder; }
            set { rawNpc.HPRemainder = value; }
        }


        public TickCount AttackTimer {
            get;
            set;
        }
        public TickCount PauseTimer { get; set; }

        public Enums.Direction Direction {
            get { return (Enums.Direction)rawNpc.Direction; }
            set { rawNpc.Direction = (byte)value; }
        }



        public int X {
            get { return rawNpc.X; }
            set { rawNpc.X = value; }
        }

        public int Y {
            get { return rawNpc.Y; }
            set { rawNpc.Y = value; }
        }


        #region Stats

        // Final Stats
        int maxHP;
        public int MaxHP {
            get {

                return maxHP;
            }
        }

        int atk;
        public int Atk {
            get {
                return atk;

            }
        }

        int def;
        public int Def {
            get {
                return def;

            }
        }

        int spd;
        public int Spd {
            get {
                return spd;

            }
        }

        int spclAtk;
        public int SpclAtk {
            get {
                return spclAtk;

            }
        }

        int spclDef;
        public int SpclDef {
            get {
                return spclDef;

            }
        }

        #region Base Stats

        public int BaseMaxHP {
            get {
                return mBaseMaxHP;
            }
            set {
                mBaseMaxHP = value;
                maxHP = mBaseMaxHP + rawNpc.MaxHPBonus + mMaxHPBoost;
                if (HP > maxHP) {
                    HP = maxHP;
                }
            }
        }
        public int BaseAtk {
            get {
                return mBaseAtk;
            }
            set {
                mBaseAtk = value;
                atk = BaseAtk + rawNpc.AtkBonus + mAtkBoost;
            }
        }
        public int BaseDef {
            get {
                return mBaseDef;
            }
            set {
                mBaseDef = value;
                def = mBaseDef + rawNpc.DefBonus + mDefBoost;
            }
        }
        public int BaseSpd {
            get {
                return mBaseSpd;
            }
            set {
                mBaseSpd = value;
                spd = mBaseSpd + rawNpc.SpdBonus + mSpdBoost;
            }
        }
        public int BaseSpclAtk {
            get {
                return mBaseSpclAtk;
            }
            set {
                mBaseSpclAtk = value;
                spclAtk = mBaseSpclAtk + rawNpc.SpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int BaseSpclDef {
            get {
                return mBaseSpclDef;
            }
            set {
                mBaseSpclDef = value;
                spclDef = mBaseSpclDef + rawNpc.SpclDefBonus + mSpclDefBoost;
            }
        }

        public int IQ { get; set; }

        #endregion Base Stats

        #region From Vitamins
        public int MaxHPBonus {
            get {
                return rawNpc.MaxHPBonus;
            }
            set {
                rawNpc.MaxHPBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetMaxHPLimit();
                //if (mBaseMaxHP + rawNpc.MaxHPBonus > baseStat) rawNpc.MaxHPBonus = baseStat - mBaseMaxHP;
                if (mBaseMaxHP + rawNpc.MaxHPBonus < 1) rawNpc.MaxHPBonus = 1 - mBaseMaxHP;
                maxHP = mBaseMaxHP + rawNpc.MaxHPBonus + mMaxHPBoost;
                if (HP > maxHP) {
                    HP = maxHP;
                }
            }
        }
        public int AtkBonus {
            get {
                return rawNpc.AtkBonus;
            }
            set {
                rawNpc.AtkBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetAttLimit();
                //if (mBaseAtk + rawNpc.AtkBonus > baseStat) rawNpc.AtkBonus = baseStat - mBaseAtk;
                if (mBaseAtk + rawNpc.AtkBonus < 1) rawNpc.AtkBonus = 1 - mBaseAtk;
                atk = BaseAtk + rawNpc.AtkBonus + mAtkBoost;
            }
        }
        public int DefBonus {
            get {
                return rawNpc.DefBonus;
            }
            set {
                rawNpc.DefBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetDefLimit();
                //if (mBaseDef + rawNpc.DefBonus > baseStat) rawNpc.DefBonus = baseStat - mBaseDef;
                if (mBaseDef + rawNpc.DefBonus < 1) rawNpc.DefBonus = 1 - mBaseDef;
                def = mBaseDef + rawNpc.DefBonus + mDefBoost;
            }
        }
        public int SpdBonus {
            get {
                return rawNpc.SpdBonus;
            }
            set {
                rawNpc.SpdBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpdLimit();
                //if (mBaseSpd + rawNpc.SpdBonus > baseStat) rawNpc.SpdBonus = baseStat - mBaseSpd;
                if (mBaseSpd + rawNpc.SpdBonus < 1) rawNpc.SpdBonus = 1 - mBaseSpd;
                spd = mBaseSpd + rawNpc.SpdBonus + mSpdBoost;
            }
        }
        public int SpclAtkBonus {
            get {
                return rawNpc.SpclAtkBonus;
            }
            set {
                rawNpc.SpclAtkBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpAttLimit();
                //if (mBaseSpclAtk + rawNpc.SpclAtkBonus > baseStat) rawNpc.SpclAtkBonus = baseStat - mBaseSpclAtk;
                if (mBaseSpclAtk + rawNpc.SpclAtkBonus < 1) rawNpc.SpclAtkBonus = 1 - mBaseSpclAtk;
                spclAtk = mBaseSpclAtk + rawNpc.SpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int SpclDefBonus {
            get {
                return rawNpc.SpclDefBonus;
            }
            set {
                rawNpc.SpclDefBonus = value;
                //int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpDefLimit();
                //if (mBaseSpclDef + rawNpc.SpclDefBonus > baseStat) rawNpc.SpclDefBonus = baseStat - mBaseSpclDef;
                if (mBaseSpclDef + rawNpc.SpclDefBonus < 1) rawNpc.SpclDefBonus = 1 - mBaseSpclDef;
                spclDef = mBaseSpclDef + rawNpc.SpclDefBonus + mSpclDefBoost;
            }
        }
        //public int IQBonus { get; set; }
        #endregion From Vitamins

        #region From Items

        public int MaxHPBoost {
            get {
                return mMaxHPBoost;
            }
            set {
                mMaxHPBoost = value;
                maxHP = mBaseMaxHP + rawNpc.MaxHPBonus + mMaxHPBoost;
                if (HP > maxHP) {
                    HP = maxHP;
                }
            }
        }
        public int AtkBoost {
            get {
                return mAtkBoost;
            }
            set {
                mAtkBoost = value;
                atk = BaseAtk + rawNpc.AtkBonus + mAtkBoost;
            }
        }
        public int DefBoost {
            get {
                return mDefBoost;
            }
            set {
                mDefBoost = value;
                def = mBaseDef + rawNpc.DefBonus + mDefBoost;
            }
        }
        public int SpdBoost {
            get {
                return mSpdBoost;
            }
            set {
                mSpdBoost = value;
                spd = mBaseSpd + rawNpc.SpdBonus + mSpdBoost;
            }
        }
        public int SpclAtkBoost {
            get {
                return mSpclAtkBoost;
            }
            set {
                mSpclAtkBoost = value;
                spclAtk = mBaseSpclAtk + rawNpc.SpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int SpclDefBoost {
            get {
                return mSpclDefBoost;
            }
            set {
                mSpclDefBoost = value;
                spclDef = mBaseSpclDef + rawNpc.SpclDefBonus + mSpclDefBoost;
            }
        }

        public int EXPBoost { get; set; }
        public int RecruitBoost { get; set; }

        #endregion From Items

        #region Buffs

        public int AttackBuff { get; set; }
        public int DefenseBuff { get; set; }
        public int SpAtkBuff { get; set; }
        public int SpDefBuff { get; set; }
        public int SpeedBuff { get; set; }
        public int AccuracyBuff { get; set; }
        public int EvasionBuff { get; set; }

        #endregion Buffs

        #endregion Stats




        #endregion Properties






    }
}