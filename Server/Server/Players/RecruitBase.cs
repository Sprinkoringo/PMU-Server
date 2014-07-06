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
using System.Xml;

namespace Server.Players {
    public class RecruitBase {
        #region Fields

        int mBaseMaxHP;
        int mBaseAtk;
        int mBaseDef;
        int mBaseSpd;
        int mBaseSpclAtk;
        int mBaseSpclDef;

        int mMaxHPBonus;
        int mAtkBonus;
        int mDefBonus;
        int mSpdBonus;
        int mSpclAtkBonus;
        int mSpclDefBonus;

        int mMaxHPBoost;
        int mAtkBoost;
        int mDefBoost;
        int mSpdBoost;
        int mSpclAtkBoost;
        int mSpclDefBoost;



        #endregion Fields


        // General Data
        public string Name { get; set; }
        public bool Nickname { get; set; }
        public Enums.Coloration Shiny { get; set; }
        //public int[] Sprite { get; set; }
        public int Sprite { get; set; }
        public int Level { get; set; }
        public ulong Exp { get; set; }
        //public int[] Species { get; set; }
        public int Species { get; set; }
        public int Form { get; set; }
        public Enums.Sex Sex { get; set; }
        public Enums.PokemonType Type1 { get; set; }
        public Enums.PokemonType Type2 { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string Ability3 { get; set; }
        public Enums.Speed SpeedLimit { get; set; }
        public bool[] Mobility { get; set; }
        public int TimeMultiplier { get; set; }

        // Moves Data
        public RecruitMove[] Moves { get; set; }

        // Vitals Data
        public int HP { get; set; }
        public int HPRemainder { get; set; }

        public int Belly { get; set; }
        public int MaxBelly { get; set; }

        public bool InTempMode {
            get;
            set;
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
                maxHP = mBaseMaxHP + mMaxHPBonus + mMaxHPBoost;
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
                atk = BaseAtk + mAtkBonus + mAtkBoost;
            }
        }
        public int BaseDef {
            get {
                return mBaseDef;
            }
            set {
                mBaseDef = value;
                def = mBaseDef + mDefBonus + mDefBoost;
            }
        }
        public int BaseSpd {
            get {
                return mBaseSpd;
            }
            set {
                mBaseSpd = value;
                spd = mBaseSpd + mSpdBonus + mSpdBoost;
            }
        }
        public int BaseSpclAtk {
            get {
                return mBaseSpclAtk;
            }
            set {
                mBaseSpclAtk = value;
                spclAtk = mBaseSpclAtk + mSpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int BaseSpclDef {
            get {
                return mBaseSpclDef;
            }
            set {
                mBaseSpclDef = value;
                spclDef = mBaseSpclDef + mSpclDefBonus + mSpclDefBoost;
            }
        }

        public int IQ { get; set; }

        #endregion Base Stats

        #region From Vitamins
        public int MaxHPBonus {
            get {
                return mMaxHPBonus;
            }
            set {
                mMaxHPBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetMaxHPLimit();
                if (mBaseMaxHP + mMaxHPBonus > baseStat) mMaxHPBonus = baseStat - mBaseMaxHP;
                if (mBaseMaxHP + mMaxHPBonus < 1) mMaxHPBonus = 1 - mBaseMaxHP;
                maxHP = mBaseMaxHP + mMaxHPBonus + mMaxHPBoost;
                if (HP > maxHP) {
                    HP = maxHP;
                }
            }
        }
        public int AtkBonus {
            get {
                return mAtkBonus;
            }
            set {
                mAtkBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetAttLimit();
                if (mBaseAtk + mAtkBonus > baseStat) mAtkBonus = baseStat - mBaseAtk;
                if (mBaseAtk + mAtkBonus < 1) mAtkBonus = 1 - mBaseAtk;
                atk = BaseAtk + mAtkBonus + mAtkBoost;
            }
        }
        public int DefBonus {
            get {
                return mDefBonus;
            }
            set {
                mDefBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetDefLimit();
                if (mBaseDef + mDefBonus > baseStat) mDefBonus = baseStat - mBaseDef;
                if (mBaseDef + mDefBonus < 1) mDefBonus = 1 - mBaseDef;
                def = mBaseDef + mDefBonus + mDefBoost;
            }
        }
        public int SpdBonus {
            get {
                return mSpdBonus;
            }
            set {
                mSpdBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpdLimit();
                if (mBaseSpd + mSpdBonus > baseStat) mSpdBonus = baseStat - mBaseSpd;
                if (mBaseSpd + mSpdBonus < 1) mSpdBonus = 1 - mBaseSpd;
                spd = mBaseSpd + mSpdBonus + mSpdBoost;
            }
        }
        public int SpclAtkBonus {
            get {
                return mSpclAtkBonus;
            }
            set {
                mSpclAtkBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpAttLimit();
                if (mBaseSpclAtk + mSpclAtkBonus > baseStat) mSpclAtkBonus = baseStat - mBaseSpclAtk;
                if (mBaseSpclAtk + mSpclAtkBonus < 1) mSpclAtkBonus = 1 - mBaseSpclAtk;
                spclAtk = mBaseSpclAtk + mSpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int SpclDefBonus {
            get {
                return mSpclDefBonus;
            }
            set {
                mSpclDefBonus = value;
                int baseStat = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpDefLimit();
                if (mBaseSpclDef + mSpclDefBonus > baseStat) mSpclDefBonus = baseStat - mBaseSpclDef;
                if (mBaseSpclDef + mSpclDefBonus < 1) mSpclDefBonus = 1 - mBaseSpclDef;
                spclDef = mBaseSpclDef + mSpclDefBonus + mSpclDefBoost;
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
                maxHP = mBaseMaxHP + mMaxHPBonus + mMaxHPBoost;
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
                atk = BaseAtk + mAtkBonus + mAtkBoost;
            }
        }
        public int DefBoost {
            get {
                return mDefBoost;
            }
            set {
                mDefBoost = value;
                def = mBaseDef + mDefBonus + mDefBoost;
            }
        }
        public int SpdBoost {
            get {
                return mSpdBoost;
            }
            set {
                mSpdBoost = value;
                spd = mBaseSpd + mSpdBonus + mSpdBoost;
            }
        }
        public int SpclAtkBoost {
            get {
                return mSpclAtkBoost;
            }
            set {
                mSpclAtkBoost = value;
                spclAtk = mBaseSpclAtk + mSpclAtkBonus + mSpclAtkBoost;
            }
        }
        public int SpclDefBoost {
            get {
                return mSpclDefBoost;
            }
            set {
                mSpclDefBoost = value;
                spclDef = mBaseSpclDef + mSpclDefBonus + mSpclDefBoost;
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


        //public int IQBoost { get; set; }



        // Internal Data
        public bool Loaded { get; set; }
        public int NpcBase { get; set; }
        public int LearningMove { get; set; }
        public int MoveItem { get; set; }

        public int RecruitIndex { get; set; }

        public string FileVersion { get; set; }

        public RecruitBase() {
            Moves = new RecruitMove[Constants.MAX_PLAYER_MOVES];
            RecruitIndex = -1;
            MoveItem = -1;
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                Moves[i] = new RecruitMove();
            }
        }

        #region Loading/Saving

        /*public virtual void Load(string path, int recruitIndex) {
            this.RecruitIndex = recruitIndex;
            this.Belly = -1;
            this.MaxBelly = -1;
            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(path))) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        switch (reader.Name) {
                            case "FileVersion": {
                                    FileVersion = reader.ReadString();
                                }
                                break;
                            case "Sprite": {
                                    Sprite = reader.ReadString().ToInt();
                                }
                                break;
                            case "Name": {
                                    Name = reader.ReadString();
                                }
                                break;
                            case "Nickname": {
                                    Nickname = reader.ReadString().ToBool();
                                }
                                break;
                            case "NpcBase": {
                                    NpcBase = reader.ReadString().ToInt(-1);
                                }
                                break;
                            case "SpeciesOverride": {
                                    SpeciesOverride = reader.ReadString().ToInt();
                                }
                                break;
                            case "Sex": {
                                    Sex = (Enums.Sex)reader.ReadString().ToInt();
                                }
                                break;
                            case "InTempMode": {
                                    inTempMode = reader.ReadString().ToBool();
                                }
                                break;
                            case "HeldItemSlot": {
                                    HeldItemSlot = reader.ReadString().ToInt();
                                }
                                break;
                            case "Level": {
                                    Level = reader.ReadString().ToInt();
                                    if (Level > Exp.ExpManager.Exp.MaxLevels) {
                                        Level = Exp.ExpManager.Exp.MaxLevels;
                                    }
                                }
                                break;
                            case "Exp": {
                                    Exp = reader.ReadString().ToUlng();
                                }
                                break;
                            case "HP": {
                                    HP = reader.ReadString().ToInt();
                                }
                                break;
                            case "StatusAilment": {
                                    StatusAilment = (Enums.StatusAilment)reader.ReadString().ToInt();
                                }
                                break;
                            case "StatusAilmentCounter": {
                                    StatusAilmentCounter = reader.ReadString().ToInt();
                                }
                                break;
                            case "ExtraStatus":
                                {
                                    int num = reader["num"].ToInt(-1);
                                    if (num > -1 && num < Constants.MAX_PLAYER_MOVES)
                                    {
                                         = new ;
                                        using (XmlReader moveReader = reader.ReadSubtree())
                                        {
                                            while (moveReader.Read())
                                            {
                                                if (moveReader.IsStartElement())
                                                {
                                                    switch (moveReader.Name)
                                                    {
                                                        case "MoveNum":
                                                            {
                                                                Moves[num].MoveNum = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "CurrentPP":
                                                            {
                                                                Moves[num].CurrentPP = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "MaxPP":
                                                            {
                                                                Moves[num].MaxPP = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "Sealed":
                                                            {
                                                                Moves[num].Sealed = moveReader.ReadString().ToBool();
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "IQ": {
                                    IQ = reader.ReadString().ToInt();
                                }
                                break;
                            case "Belly": {
                                    Belly = reader.ReadString().ToInt(-1);
                                }
                                break;
                            case "MaxBelly": {
                                    MaxBelly = reader.ReadString().ToInt(-1);
                                }
                                break;
                            case "StrBonus": {
                                    AtkBonus = reader.ReadString().ToInt();
                                }
                                break;
                            case "DefBonus": {
                                    DefBonus = reader.ReadString().ToInt();
                                }
                                break;
                            case "SpeedBonus": {
                                    SpdBonus = reader.ReadString().ToInt();
                                }
                                break;
                            case "SpclAttackBonus": {
                                    SpclAtkBonus = reader.ReadString().ToInt();
                                }
                                break;
                            //case "IQBonus": {
                            //        IQBonus = reader.ReadString().ToInt();
                            //    }
                            //    break;
                            case "SpclDefBonus": {
                                    SpclDefBonus = reader.ReadString().ToInt();
                                }
                                break;

                            case "Move": {
                                    int num = reader["num"].ToInt(-1);
                                    if (num > -1 && num < Constants.MAX_PLAYER_MOVES) {
                                        Moves[num] = new RecruitMove();
                                        using (XmlReader moveReader = reader.ReadSubtree()) {
                                            while (moveReader.Read()) {
                                                if (moveReader.IsStartElement()) {
                                                    switch (moveReader.Name) {
                                                        case "MoveNum": {
                                                                Moves[num].MoveNum = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "CurrentPP": {
                                                                Moves[num].CurrentPP = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "MaxPP": {
                                                                Moves[num].MaxPP = moveReader.ReadString().ToInt(-1);
                                                            }
                                                            break;
                                                        case "Sealed":
                                                            {
                                                                Moves[num].Sealed = moveReader.ReadString().ToBool();
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            // Do checks
            if (HP < 0)
                HP = 1;
            if (IQ < 0)
                IQ = 0;
            if (MaxBelly <= 0) {
                MaxBelly = 100;
                Belly = MaxBelly;
            }
            if (Belly < 0 || Belly > MaxBelly) {
                Belly = MaxBelly;
            }

            if (SpeciesOverride > 0 && SpeciesOverride <= Constants.TOTAL_POKEMON) {
                Species = SpeciesOverride;
            } else {
                Pokedex.Pokemon pokemon = Pokedex.Pokedex.FindBySprite(Sprite);
                if (pokemon != null) {
                    Species = pokemon.ID;
                } else { // Oh no!
                    Species = 0; // We'll be MissingNo...
                }
            }
            if (Species < 0 || Species > Constants.TOTAL_POKEMON) {
                Species = 0;
            }

            CalculateOriginalStats();
            CalculateOriginalType();
            
            CalculateOriginalMobility();

            // Do some more checks
            if (HP > MaxHP) {
                HP = MaxHP;
            }



            Loaded = true;
        }

        public virtual void Save(string path) {
            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(path), Settings.XmlWriterSettings)) {
                writer.WriteStartDocument();
                writer.WriteStartElement("Recruit");

                writer.WriteStartElement("Generic");

                writer.WriteElementString("FileVersion", "V1");
                writer.WriteElementString("Sprite", Sprite.ToString());
                writer.WriteElementString("Name", Name);
                writer.WriteElementString("IsNickname", Nickname.ToString());
                writer.WriteElementString("NpcBase", NpcBase.ToString());
                writer.WriteElementString("SpeciesOverride", SpeciesOverride.ToString());
                writer.WriteElementString("Sex", ((int)Sex).ToString());
                writer.WriteElementString("InTempMode", inTempMode.ToString());

                writer.WriteEndElement();
                writer.WriteStartElement("Stats");
                writer.WriteElementString("HeldItemSlot", HeldItemSlot.ToString());
                writer.WriteElementString("Level", Level.ToString());
                writer.WriteElementString("Exp", Exp.ToString());
                writer.WriteElementString("HP", HP.ToString());
                writer.WriteElementString("StatusAilment", ((int)StatusAilment).ToString());
                writer.WriteElementString("StatusAilmentCounter", StatusAilmentCounter.ToString());
                writer.WriteElementString("IQ", IQ.ToString());
                writer.WriteElementString("Belly", Belly.ToString());
                writer.WriteElementString("MaxBelly", MaxBelly.ToString());

                writer.WriteEndElement();
                writer.WriteStartElement("StatBonus");

                writer.WriteElementString("StrBonus", AtkBonus.ToString());
                writer.WriteElementString("DefBonus", DefBonus.ToString());
                writer.WriteElementString("SpeedBonus", SpdBonus.ToString());
                writer.WriteElementString("SpclAttackBonus", SpclAtkBonus.ToString());
                writer.WriteElementString("SpclDefBonus", SpclDefBonus.ToString());
                //writer.WriteElementString("IQBonus", IQBonus.ToString());

                writer.WriteEndElement();
                writer.WriteStartElement("Moves");

                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                    writer.WriteStartElement("Move");
                    writer.WriteAttributeString("num", i.ToString());
                    writer.WriteElementString("MoveNum", Moves[i].MoveNum.ToString());
                    writer.WriteElementString("CurrentPP", Moves[i].CurrentPP.ToString());
                    writer.WriteElementString("MaxPP", Moves[i].MaxPP.ToString());
                    writer.WriteElementString("Sealed", Moves[i].Sealed.ToIntString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        */
        #endregion

        public void CalculateOriginalSpecies() {

        }

        public void CalculateOriginalSprite() {
            //Sprite = Pokedex.Pokedex.GetPokemonForm(Species, Form).Sprite[(int)Shiny, (int)Sex];
            Sprite = Species;
        }

        public void CalculateOriginalStats() {
            BaseMaxHP = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetMaxHP(Level);
            BaseAtk = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetAtt(Level);
            BaseDef = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetDef(Level);
            BaseSpclAtk = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpAtt(Level);
            BaseSpclDef = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpDef(Level);
            BaseSpd = Pokedex.Pokedex.GetPokemonForm(Species, Form).GetSpd(Level);
        }

        public void CalculateOriginalType() {
            Type1 = Pokedex.Pokedex.GetPokemonForm(Species, Form).Type1;
            Type2 = Pokedex.Pokedex.GetPokemonForm(Species, Form).Type2;

        }

        //public void CalculateOriginalMobility() {
        //    //beta mobility settings
        //    for (int i = 0; i < Mobility.Length; i++) {
        //        Mobility[i] = false;
        //    }

        //    if (Type1 == Enums.PokemonType.Water || Type2 == Enums.PokemonType.Water) {
        //        Mobility[1] = true;
        //    }
        //    if (Type1 == Enums.PokemonType.Fire || Type2 == Enums.PokemonType.Fire) {
        //        Mobility[3] = true;
        //    }
        //    if (Type1 == Enums.PokemonType.Flying || Type2 == Enums.PokemonType.Flying) {
        //        Mobility[1] = true;
        //        Mobility[2] = true;
        //        Mobility[3] = true;
        //    }
        //    if (Type1 == Enums.PokemonType.Ghost || Type2 == Enums.PokemonType.Ghost) {
        //        Mobility[1] = true;
        //        Mobility[2] = true;
        //        Mobility[3] = true;
        //        Mobility[4] = true;
        //    }
        //}

        public void CalculateOriginalAbility() {
            Ability1 = Pokedex.Pokedex.GetPokemonForm(Species, Form).Ability1;
            Ability2 = Pokedex.Pokedex.GetPokemonForm(Species, Form).Ability2;
            Ability3 = Pokedex.Pokedex.GetPokemonForm(Species, Form).Ability3;

        }

        public static int DetermineSpecies(int sprite) {
            int species;
            //if (speciesOverride > 0 && speciesOverride <= Constants.TOTAL_POKEMON) {
            //    species = speciesOverride;
            //} else {
            Pokedex.Pokemon pokemon = Pokedex.Pokedex.FindBySprite(sprite);
            if (pokemon != null) {
                species = pokemon.ID;
            } else { // MissingNo.
                species = 0;
            }
            //}
            if (species < 0 || species > Constants.TOTAL_POKEMON) {
                species = 0;
            }
            return species;
        }

    }
}
