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


//namespace Server.Players
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//    using System.Xml;

//    using Exp;

//    using Items;

//    using Moves;

//    using Server.AI;
//    using Server.Combat;
//    using Server.Maps;

//    using Server.Network;
//    using PMU.Sockets;

//    public class RecruitOld : Players.RecruitBase, Combat.ICharacter
//    {
//        Client owner;

//        #region Constructors

//        public RecruitOld(Client owner) {
//            this.owner = owner;
//            HeldItemSlot = -1;
//            Mobility = new bool[16];
//            ActiveItems = new List<int>();
//            VolatileStatus = new List<ExtraStatus>();
//        }

//        #endregion Constructors

//        #region Properties

//        public Enums.CharacterType CharacterType {
//            get { return Enums.CharacterType.Recruit; }
//        }

//        public InventoryItem HeldItem {
//            get {
//                if (HeldItemSlot > -1) {

//                    return owner.Player.Inventory[HeldItemSlot];

//                } else {

//                    return null;
//                }


//            }
            
//            set//may not use this; buggy?
//            {

//                owner.Player.Inventory[HeldItemSlot] = value;

//            }

//        }

//        public TickCount AttackTimer {
//            get {
//                return Owner.Player.AttackTimer;
//            }
//            set {
//                Owner.Player.AttackTimer = value;
//            }

//        }

//        public TickCount PauseTimer {
//            get {
//                return Owner.Player.PauseTimer;
//            }
//            set {
//                Owner.Player.PauseTimer = value;
//            }

//        }

//        public new int HP {
//            get { return base.HP; }
//            set {
//                if (value > MaxHP) {
//                    value = MaxHP;
//                }
//                if (value < 0) {
//                    value = 0;
//                }
//                base.HP = value;

//                Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
//            }
//        }

//        public string MapID {
//            get { return owner.Player.MapID; }
//        }

//        public int X {
//            get { return owner.Player.X; }
//            set { owner.Player.X = value; }
//        }

//        public int Y {
//            get { return owner.Player.Y; }
//            set { owner.Player.Y = value; }
//        }

//        public Enums.Direction Direction
//        {
//            get { return owner.Player.Direction; }
//            set { owner.Player.Direction = value; }
//        }


//        public Enums.StatusAilment StatusAilment { get; set; }//TODO: make sure these are reset to 0 when deposit/withdrawn, sendplayerdata when switched
//        public int StatusAilmentCounter { get; set; }
//        public List<ExtraStatus> VolatileStatus { get; set; }

//        public Client Owner {
//            get { return owner; }
//        }


//        public List<int> ActiveItems { get; set; }

//        #endregion Properties

//        #region Methods


//        public void Load(string playerFolder, int recruitIndex, int teamSlot) {
//            if (recruitIndex != -2) {
//                LoadDirect(playerFolder + "Recruits/" + recruitIndex + ".xml", recruitIndex);
//            } else {
//                LoadDirect(playerFolder + "Recruits/TempTeam/" + teamSlot + ".xml", -2);
//            }
//        }

//        public void LoadDirect(string path, int recruitIndex) {
//            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)) == false) {
//                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
//            }
            
//            this.RecruitIndex = recruitIndex;
//            this.Belly = -1;
//            this.MaxBelly = -1;
//            using (XmlReader reader = XmlReader.Create(IO.IO.ProcessPath(path))) {
//                while (reader.Read()) {
//                    if (reader.IsStartElement()) {
//                        switch (reader.Name) {
//                            case "FileVersion": {
//                                    FileVersion = reader.ReadString();
//                                }
//                                break;
//                            case "Sprite": {
//                                    Sprite = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "Name": {
//                                    Name = reader.ReadString();
//                                }
//                                break;
//                            case "Nickname": {
//                                    Nickname = reader.ReadString().ToBool();
//                                }
//                                break;
//                            case "NpcBase": {
//                                    NpcBase = reader.ReadString().ToInt(-1);
//                                }
//                                break;
//                            case "SpeciesOverride": {
//                                    SpeciesOverride = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "Sex": {
//                                    Sex = (Enums.Sex)reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "InTempMode": {
//                                    InTempMode = reader.ReadString().ToBool();
//                                }
//                                break;
//                            case "HeldItemSlot": {
//                                    HeldItemSlot = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "Level": {
//                                    Level = reader.ReadString().ToInt();
//                                    if (Level > Exp.ExpManager.Exp.MaxLevels) {
//                                        Level = Exp.ExpManager.Exp.MaxLevels;
//                                    }
//                                }
//                                break;
//                            case "Exp": {
//                                    Exp = reader.ReadString().ToUlng();
//                                }
//                                break;
//                            case "HP": {
//                                    base.HP = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "StatusAilment": {
//                                    StatusAilment = (Enums.StatusAilment)reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "StatusAilmentCounter": {
//                                    StatusAilmentCounter = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "VolatileStatus":
//                                {
//                                    int num = reader["num"].ToInt(-1);
//                                    if (VolatileStatus.Count <= num)
//                                    {
//                                        VolatileStatus.Add(new ExtraStatus());
//                                    }
//                                        using (XmlReader statusReader = reader.ReadSubtree())
//                                        {
//                                            while (statusReader.Read())
//                                            {
//                                                if (statusReader.IsStartElement())
//                                                {
//                                                    switch (statusReader.Name)
//                                                    {
//                                                        case "VolatileStatusName":
//                                                            {
//                                                                VolatileStatus[num].Name = statusReader.ReadString();
//                                                            }
//                                                            break;
//                                                        case "VolatileStatusEmoticon":
//                                                            {
//                                                                VolatileStatus[num].Emoticon = statusReader.ReadString().ToInt(-1);
//                                                            }
//                                                            break;
//                                                        case "VolatileStatusCounter":
//                                                            {
//                                                                VolatileStatus[num].Counter = statusReader.ReadString().ToInt(-1);
//                                                            }
//                                                            break;
//                                                    }
//                                                }
//                                            }
//                                        }
                                    
//                                }
//                                break;
//                            case "IQ": {
//                                    IQ = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "Belly": {
//                                    Belly = reader.ReadString().ToInt(-1);
//                                }
//                                break;
//                            case "MaxBelly": {
//                                    MaxBelly = reader.ReadString().ToInt(-1);
//                                }
//                                break;
//                            case "StrBonus": {
//                                    AtkBonus = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "DefBonus": {
//                                    DefBonus = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "SpeedBonus": {
//                                    SpdBonus = reader.ReadString().ToInt();
//                                }
//                                break;
//                            case "SpclAttackBonus": {
//                                    SpclAtkBonus = reader.ReadString().ToInt();
//                                }
//                                break;
//                            //case "IQBonus": {
//                            //        IQBonus = reader.ReadString().ToInt();
//                            //    }
//                            //    break;
//                            case "SpclDefBonus": {
//                                    SpclDefBonus = reader.ReadString().ToInt();
//                                }
//                                break;

//                            case "Move": {
//                                    int num = reader["num"].ToInt(-1);
//                                    if (num > -1 && num < Constants.MAX_PLAYER_MOVES) {
//                                        Moves[num] = new RecruitMove();
//                                        using (XmlReader moveReader = reader.ReadSubtree()) {
//                                            while (moveReader.Read()) {
//                                                if (moveReader.IsStartElement()) {
//                                                    switch (moveReader.Name) {
//                                                        case "MoveNum": {
//                                                                Moves[num].MoveNum = moveReader.ReadString().ToInt(-1);
//                                                            }
//                                                            break;
//                                                        case "CurrentPP": {
//                                                                Moves[num].CurrentPP = moveReader.ReadString().ToInt(-1);
//                                                            }
//                                                            break;
//                                                        case "MaxPP": {
//                                                                Moves[num].MaxPP = moveReader.ReadString().ToInt(-1);
//                                                            }
//                                                            break;
//                                                        case "Sealed":
//                                                            {
//                                                                Moves[num].Sealed = moveReader.ReadString().ToBool();
//                                                            }
//                                                            break;
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//            // Do checks
//            if (base.HP < 0)
//                base.HP = 1;
//            if (IQ < 0)
//                IQ = 0;
//            if (MaxBelly <= 0) {
//                MaxBelly = 100;
//                Belly = MaxBelly;
//            }
//            if (Belly < 0 || Belly > MaxBelly) {
//                Belly = MaxBelly;
//            }

//            if (SpeciesOverride > 0 && SpeciesOverride <= Constants.TOTAL_POKEMON) {
//                Species = SpeciesOverride;
//            } else {
//                Pokedex.Pokemon pokemon = Pokedex.Pokedex.FindBySprite(Sprite);
//                if (pokemon != null) {
//                    Species = pokemon.ID;
//                } else { 
//                    Species = 0;
//                }
//            }
//            if (Species < 0 || Species > Constants.TOTAL_POKEMON) {
//                Species = 0;
//            }

//            CalculateOriginalStats();
//            CalculateOriginalType();
            
//            CalculateOriginalMobility();

//            // Do some more checks
//            if (base.HP > MaxHP)
//            {
//                base.HP = MaxHP;
//            }



//            Loaded = true;


//            if (Loaded) {
//                // Verify move PP
//                for (int i = 0; i < Moves.Length; i++) {
//                    if (Moves[i].MoveNum > 0) {
//                        if (Moves[i].MaxPP == -1 || Moves[i].MaxPP != MoveManager.Moves[Moves[i].MoveNum].MaxPP) {
//                            Moves[i].MaxPP = MoveManager.Moves[Moves[i].MoveNum].MaxPP;
//                        }
//                        if (Moves[i].CurrentPP == -1 || Moves[i].CurrentPP > Moves[i].MaxPP) {
//                            Moves[i].CurrentPP = Moves[i].MaxPP;
//                        }
//                    }
//                }

//                // Add Active Items

//                LoadActiveItemList();
//            }
//        }

//        public void Save(string folder, int teamSlot) {
//            if (RecruitIndex != -2) {
//                SaveDirect(folder + "Recruits/" + RecruitIndex + ".xml");
//            } else {
//                SaveDirect(folder + "Recruits/TempTeam/" + teamSlot + ".xml");
//            }
//        }

//        public void SaveDirect(string path) {
//            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)) == false) {
//                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
//            }

//            using (XmlWriter writer = XmlWriter.Create(IO.IO.ProcessPath(path), Settings.XmlWriterSettings))
//            {
//                writer.WriteStartDocument();
//                writer.WriteStartElement("Recruit");

//                writer.WriteStartElement("Generic");

//                writer.WriteElementString("FileVersion", "V1");
//                writer.WriteElementString("Sprite", Sprite.ToString());
//                writer.WriteElementString("Name", Name);
//                writer.WriteElementString("IsNickname", Nickname.ToString());
//                writer.WriteElementString("NpcBase", NpcBase.ToString());
//                writer.WriteElementString("SpeciesOverride", SpeciesOverride.ToString());
//                writer.WriteElementString("Sex", ((int)Sex).ToString());
//                writer.WriteElementString("InTempMode", InTempMode.ToString());

//                writer.WriteEndElement();
//                writer.WriteStartElement("Stats");
//                writer.WriteElementString("HeldItemSlot", HeldItemSlot.ToString());
//                writer.WriteElementString("Level", Level.ToString());
//                writer.WriteElementString("Exp", Exp.ToString());
//                writer.WriteElementString("HP", HP.ToString());
//                writer.WriteElementString("StatusAilment", ((int)StatusAilment).ToString());
//                writer.WriteElementString("StatusAilmentCounter", StatusAilmentCounter.ToString());

//                writer.WriteStartElement("VolatileStatusList");

//                for (int i = 0; i < VolatileStatus.Count; i++)
//                {
//                    writer.WriteStartElement("VolatileStatus");
//                    writer.WriteAttributeString("num", i.ToString());
//                    writer.WriteElementString("VolatileStatusName", VolatileStatus[i].Name);
//                    writer.WriteElementString("VolatileStatusEmoticon", VolatileStatus[i].Emoticon.ToString());
//                    writer.WriteElementString("VolatileStatusCounter", VolatileStatus[i].Counter.ToString());
//                    writer.WriteEndElement();
//                }

//                writer.WriteEndElement();

//                writer.WriteElementString("IQ", IQ.ToString());
//                writer.WriteElementString("Belly", Belly.ToString());
//                writer.WriteElementString("MaxBelly", MaxBelly.ToString());

//                writer.WriteEndElement();
//                writer.WriteStartElement("StatBonus");

//                writer.WriteElementString("StrBonus", AtkBonus.ToString());
//                writer.WriteElementString("DefBonus", DefBonus.ToString());
//                writer.WriteElementString("SpeedBonus", SpdBonus.ToString());
//                writer.WriteElementString("SpclAttackBonus", SpclAtkBonus.ToString());
//                writer.WriteElementString("SpclDefBonus", SpclDefBonus.ToString());
//                //writer.WriteElementString("IQBonus", IQBonus.ToString());

//                writer.WriteEndElement();
//                writer.WriteStartElement("Moves");

//                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++)
//                {
//                    writer.WriteStartElement("Move");
//                    writer.WriteAttributeString("num", i.ToString());
//                    writer.WriteElementString("MoveNum", Moves[i].MoveNum.ToString());
//                    writer.WriteElementString("CurrentPP", Moves[i].CurrentPP.ToString());
//                    writer.WriteElementString("MaxPP", Moves[i].MaxPP.ToString());
//                    writer.WriteElementString("Sealed", Moves[i].Sealed.ToIntString());
//                    writer.WriteEndElement();
//                }

//                writer.WriteEndElement();

//                writer.WriteEndElement();
//                writer.WriteEndDocument();
//            }
//        }

//        //internal void SetHP(int hp) { ~unneeded due to properties
//        //    base.HP = hp;
//        //}


//        public void CheckHeldItem() {
//            if (HeldItemSlot > -1 && owner.Player.Inventory[HeldItemSlot].Num < 1) {
//                HeldItemSlot = -1;
//            }
//        }

//        public void RefreshActiveItemList() {
//            MaxHPBoost = 0;
//            //add PP
//            AtkBoost = 0;
//            DefBoost = 0;
//            SpclAtkBoost = 0;
//            SpclDefBoost = 0;
//            SpdBoost = 0;
//            EXPBoost = 0;
//            RecruitBoost = 0;

//            ActiveItems.Clear();

//            LoadActiveItemList();
//        }

//        public void LoadActiveItemList() {
//            //add held item, if applicable
//            if (HeldItemSlot > -1 && (HeldItem.Num < 1 || HeldItem.Num > ItemManager.Items.MaxItems)) {
//                HeldItemSlot = -1;
//            }
//            if (HeldItemSlot > -1 && ItemManager.Items[HeldItem.Num].Type == Enums.ItemType.Held) {
//                if (MeetsReqs(HeldItem.Num)) {

//                    AddToActiveItemList(HeldItem.Num);
//                }
//            }

//            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
//                if (owner.Player.Team[i] != null && owner.Player.Team[i].HeldItemSlot > -1 && ItemManager.Items[owner.Player.Team[i].HeldItem.Num].Type == Enums.ItemType.HeldByParty) {

//                    if (MeetsReqs(owner.Player.Team[i].HeldItem.Num)) {

//                        AddToActiveItemList(owner.Player.Team[i].HeldItem.Num);
//                    }

//                }
//            }

//            for (int i = 1; i < owner.Player.MaxInv; i++) {

//                //held-in-bag items

//                if (owner.Player.Inventory[i].Num > 0 && ItemManager.Items[owner.Player.Inventory[i].Num].Type == Enums.ItemType.HeldInBag && MeetsReqs(owner.Player.Inventory[i].Num)) {
//                    AddToActiveItemList(owner.Player.Inventory[i].Num);

//                }
//            }

//        }

//        public void AddToActiveItemList(int itemNum) {
//            if (!ActiveItems.Contains(itemNum)) {
//                //add to active items list
//                ActiveItems.Add(itemNum);


//                //add bonuses
//                MaxHPBoost += ItemManager.Items[itemNum].AddHP;
//                //add PP
//                AtkBoost += ItemManager.Items[itemNum].AddAttack;
//                DefBoost += ItemManager.Items[itemNum].AddDefense;
//                SpclAtkBoost += ItemManager.Items[itemNum].AddSpAtk;
//                SpclDefBoost += ItemManager.Items[itemNum].AddSpDef;
//                SpdBoost += ItemManager.Items[itemNum].AddSpeed;
//                EXPBoost += ItemManager.Items[itemNum].AddEXP;
//                RecruitBoost += ItemManager.Items[itemNum].RecruitBonus;

//                Scripting.ScriptManager.InvokeFunction("OnItemActivated", itemNum, this);

//                if (owner.Player.GetActiveRecruit() == this) {
//                    Messenger.SendStats(owner);
//                }
//                if (ItemManager.Items[itemNum].AddHP != 0) {
//                    Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
//                }
//            }
//        }

//        public void RemoveFromActiveItemList(int itemNum) {
//            if (ActiveItems.Contains(itemNum)) {
//                //remove from active items list
//                ActiveItems.Remove(itemNum);


//                //subtract bonuses
//                MaxHPBoost -= ItemManager.Items[itemNum].AddHP;
//                //sub PP
//                AtkBoost -= ItemManager.Items[itemNum].AddAttack;
//                DefBoost -= ItemManager.Items[itemNum].AddDefense;
//                SpclAtkBoost -= ItemManager.Items[itemNum].AddSpAtk;
//                SpclDefBoost -= ItemManager.Items[itemNum].AddSpDef;
//                SpdBoost -= ItemManager.Items[itemNum].AddSpeed;
//                EXPBoost -= ItemManager.Items[itemNum].AddEXP;
//                RecruitBoost -= ItemManager.Items[itemNum].RecruitBonus;

//                Scripting.ScriptManager.InvokeFunction("OnItemDeactivated", itemNum, this);

//                if (owner.Player.GetActiveRecruit() == this) {
//                    Messenger.SendStats(owner);
//                }

//                if (ItemManager.Items[itemNum].AddHP != 0) {
//                    Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
//                }
//            }

//        }

//        public bool MeetsReqs(int itemNum) {
//            if (BaseAtk + AtkBonus < ItemManager.Items[itemNum].AttackReq) {
//                return false;
//            }

//            if (BaseDef + DefBonus < ItemManager.Items[itemNum].DefenseReq) {
//                return false;
//            }

//            if (BaseSpclAtk + SpclAtkBonus < ItemManager.Items[itemNum].SpAtkReq) {
//                return false;
//            }

//            if (BaseSpclDef + SpclDefBonus < ItemManager.Items[itemNum].SpDefReq) {
//                return false;
//            }

//            if (BaseSpd + SpdBonus < ItemManager.Items[itemNum].SpeedReq) {
//                return false;
//            }
//            if (ItemManager.Items[itemNum].ScriptedReq > -1) {
//                return Scripting.ScriptManager.InvokeFunction("ScriptedReq", this, ItemManager.Items[itemNum].ScriptedReq, ItemManager.Items[itemNum].Data1, ItemManager.Items[itemNum].Data2, ItemManager.Items[itemNum].Data3).ToBool();
//            }

//            return true;
//        }
        
//        public bool HasActiveItem(int itemNum) {
//            if (ActiveItems.Contains(itemNum)) {
//                    return true;
//                }
//                return false;
//        }

        //public void TakeHeldItem(int val) {
        //    if (HeldItemSlot > -1 && HeldItem != null) {
        //        owner.Player.TakeItemSlot(HeldItemSlot, val, true);
        //    }
        //}


        //public void UseHeldItem(PacketHitList hitlist) {
        //    if (HeldItemSlot > -1 && HeldItem != null) {
        //        owner.Player.UseItem(HeldItemSlot, hitlist);
        //    }
        //}
        //public void MapGetItem() {
        //    owner.Player.MapGetItem();
        //}

        //public void MapDropItem(int val, Client playerFor) {
        //    if (HeldItemSlot > -1 && HeldItem != null) {
        //        owner.Player.MapDropItem(HeldItemSlot, val, playerFor);
        //    }
        //}

        //public void ThrowHeldItem() {

        //}

        //public bool CanLearnNewMove() {
        //    for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
        //        if (Moves[i].MoveNum < 1) {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

//        public bool CanLearnTMMove(int moveNum) {
//            Pokedex.Pokemon pokemon = Pokedex.Pokedex.GetPokemon(Species);
//            if (pokemon != null) {
//                return pokemon.CanLearnTMMove(MoveManager.Moves[moveNum].Name.Replace("`", ""));
//            } else {
//                return false;
//            }
//        }

//        //public int GetHPRegen()
//        //{
//        //    if (Settings.HPRegen)
//        //    {
//        //        int i;
//        //        i = GetSpd() / 2;
//        //        if (i < 2) i = 2;
//        //        return i;
//        //    }
//        //    return 0;
//        //}

//        public ulong GetNextLevel() {
//            try {
//                return ExpManager.Exp[Level - 1];
//            } catch (IndexOutOfRangeException) {
//                return ulong.MaxValue;
//            }
//        }



//        public bool HasMove(int moveNum) {
//            for (int i = 0; i < Moves.Length; i++) {
//                if (Moves[i].MoveNum == moveNum) {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public void LearnNewMove(int moveNum) {
//            if (CanLearnNewMove() == true) {
//                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
//                    if (Moves[i].MoveNum < 1) {
//                        Moves[i].MoveNum = moveNum;
//                        Moves[i].MaxPP = MoveManager.Moves[moveNum].MaxPP;
//                        Moves[i].CurrentPP = Moves[i].MaxPP;
//                        Messenger.PlayerMsg(owner, "You have learned " + MoveManager.Moves[moveNum].Name + "!", Text.BrightGreen);
//                        return;
//                    }
//                }
//            } else {
//                LearningMove = moveNum;
//                string questionText;
//                if (Name != "") {
//                    questionText = Name + " is trying to learn " + MoveManager.Moves[moveNum].Name + ", but " + Name + " can only know 4 moves. Would you like to forget one to make room for " + MoveManager.Moves[moveNum].Name + "?";
//                } else {
//                    questionText = "You are trying to learn " + MoveManager.Moves[moveNum].Name + ", but you can only know 4 moves. Would you like to forget one to make room for " + MoveManager.Moves[moveNum].Name + "?";
//                }
//                Messenger.AskQuestion(Owner, "MovesFull", questionText, -1);
//            }
//        }

//        public void GenerateMoveset()
//        {
//            Pokedex.Pokemon pokemon = null;
//            List<int> validLevelUpMoves = new List<int>();
//                    if (Species > -1)
//                    {
//                        pokemon = Pokedex.Pokedex.GetPokemon(Species);
//                    }
//                    else
//                    {
//                        pokemon = Pokedex.Pokedex.FindBySprite(Sprite);
//                    }
//                    if (pokemon != null)
//                    {
//                        if (validLevelUpMoves.Count == 0)
//                        {
//                            for (int n = 0; n < pokemon.LevelUpMoves.Count; n++)
//                            {
//                                if (pokemon.LevelUpMoves[n].Level <= this.Level)
//                                {
//                                    validLevelUpMoves.Add(n);
//                                }
//                            }
//                        }
//                    }

//            for (int i = 0; i < Moves.Length; i++)
//            {
//                    if (validLevelUpMoves.Count > 0)
//                    {
//                        int levelUpMoveSelected = Server.Math.Rand(0, validLevelUpMoves.Count);
//                        Moves[i].MoveNum = Server.Moves.MoveManager.FindMoveName(pokemon.LevelUpMoves[validLevelUpMoves[levelUpMoveSelected]].Move);
//                        if (Moves[i].MoveNum > -1)
//                        {
//                            Moves[i].MaxPP = Server.Moves.MoveManager.Moves[Moves[i].MoveNum].MaxPP;
//                            Moves[i].CurrentPP = Moves[i].MaxPP;
//                        }
//                        validLevelUpMoves.RemoveAt(levelUpMoveSelected);
//                    }
//            }
//        }

//        public void RestoreVitals() {
//            HP = MaxHP;
//            for (int i = 0; i < Moves.Length; i++) {
//                Moves[i].CurrentPP = Moves[i].MaxPP;
//            }
//        }

//        public void RestoreBelly() {
//            Belly = MaxBelly;
//            Messenger.SendBelly(owner);
//        }

        

//            CalculateOriginalStats();
//            CalculateOriginalType();
//            CalculateOriginalMobility();
//        }

//        public void UseMove(int moveSlot) {
//            if (Ranks.IsDisallowed(owner, Enums.Rank.Moniter)) {
//                    owner.Player.Hunted = true;
//            }
//            if (moveSlot > -1 && moveSlot < 4)
//            {
//                Move move = MoveManager.Moves[Moves[moveSlot].MoveNum];
//                IMap currentMap = owner.Player.GetCurrentMap();
//                if (move.IsKey) {
//                    bool doorOpened = UseMoveKey(currentMap, move, moveSlot);
//                    if (doorOpened) {
//                        return;
//                    }
//                }
//            }
//            BattleProcessor.HandleAttack(this, moveSlot);
//            //    //Confusion check
//            //    if (Moves[moveSlot].CurrentPP > 0) {
//            //        Moves[moveSlot].CurrentPP--;
//            //        // Attack any players that are in range
//            //        switch (move.TargetType) {
//            //            case Enums.MoveTarget.Foes: {
//            //                    UseMoveOnFoes(currentMap, move, moveSlot);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.User: {
//            //                    // Attack the user
//            //                    BattleProcessor.MoveHitCharacter(this, this, Moves[moveSlot].MoveNum);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.UserAndAllies: {
//            //                    UseMoveOnAllies(currentMap, move, moveSlot);
//            //                    // Attack the user
//            //                    BattleProcessor.MoveHitCharacter(this, this, Moves[moveSlot].MoveNum);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.UserAndFoe: {
//            //                    // Attack the user
//            //                    BattleProcessor.MoveHitCharacter(this, this, Moves[moveSlot].MoveNum);
//            //                    UseMoveOnFoes(currentMap, move, moveSlot);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.All: {
//            //                    // Attack allies
//            //                    UseMoveOnAllies(currentMap, move, moveSlot);
//            //                    // Attack foes
//            //                    UseMoveOnFoes(currentMap, move, moveSlot);
//            //                    // Attack the user
//            //                    BattleProcessor.MoveHitCharacter(this, this, Moves[moveSlot].MoveNum);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.AllAlliesButUser: {
//            //                    UseMoveOnAllies(currentMap, move, moveSlot);
//            //                }
//            //                break;
//            //            case Enums.MoveTarget.AllButUser: {
//            //                    UseMoveOnAllies(currentMap, move, moveSlot);
//            //                    UseMoveOnFoes(currentMap, move, moveSlot);
//            //                }
//            //                break;
//            //        }
//            //        Messenger.SendMovePPUpdate(owner, moveSlot);
//            //    }

//        }

//        public bool UseMoveKey(IMap currentMap, Move move, int moveSlot) {
//            bool doorOpened = false;
//            int x = 0;
//            int y = 0;
//            switch (owner.Player.Direction) {
//                case Enums.Direction.Up: {
//                        if (owner.Player.Y > 0) {
//                            x = owner.Player.X;
//                            y = owner.Player.Y - 1;
//                        } else {
//                            return false;
//                        }
//                    }
//                    break;
//                case Enums.Direction.Down: {
//                        if (owner.Player.Y < currentMap.MaxY) {
//                            x = owner.Player.X;
//                            y = owner.Player.Y + 1;
//                        } else {
//                            return false;
//                        }
//                    }
//                    break;
//                case Enums.Direction.Left: {
//                        if (owner.Player.X > 0) {
//                            x = owner.Player.X - 1;
//                            y = owner.Player.Y;
//                        } else {
//                            return false;
//                        }
//                    }
//                    break;
//                case Enums.Direction.Right: {
//                        if (owner.Player.X < currentMap.MaxX) {
//                            x = owner.Player.X + 1;
//                            y = owner.Player.Y;
//                        } else {
//                            return false;
//                        }
//                    }
//                    break;
//            }

//            // Check if a key exists
//            if (currentMap.Tile[x, y].Type == Enums.TileType.Key) {
//                // Check if the key they are using matches the map key
//                if (move.KeyItem == currentMap.Tile[x, y].Data1) {
//                    currentMap.Tile[x, y].DoorOpen = true;
//                    currentMap.Tile[x, y].DoorTimer = Core.GetTickCount();

//                    Messenger.SendDataToMap(MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));

//                    if (string.IsNullOrEmpty(currentMap.Tile[x, y].String1)) {
//                        //Messenger.PlayerMsg(Owner, "A door has been unlocked!", Text.White);
//                    } else {
//                        Messenger.PlayerMsg(Owner, currentMap.Tile[x, y].String1.Trim(), Text.White);
//                    }

//                    Messenger.PlaySoundToMap(MapID, "key.wav");

//                    Messenger.SpellAnim(Moves[moveSlot].MoveNum, currentMap.MapID, x, y);
//                    doorOpened = true;
//                }
//            }

//            if (currentMap.Tile[x, y].Type == Enums.TileType.Door) {
//                currentMap.Tile[x, y].DoorOpen = true;
//                currentMap.Tile[x, y].DoorTimer = Core.GetTickCount();

//                Messenger.SendDataToMap(MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));
//                Messenger.PlaySoundToMap(MapID, "key.wav");
//            }
//            return doorOpened;
//        }


//        public void UseMoveOnAllies(BattleSetup setup) {
//            Parties.Party party = null;
//            if (owner.Player.IsInParty()) {
//                party = Parties.PartyManager.FindPlayerParty(owner);
//            }
//                List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, MapID, this, owner.Player.X, owner.Player.Y, owner.Player.Direction);
//                Move move = setup.SetdownMove();
//            // Attack any players that are on the map
//            foreach (ICharacter i in targets) {
//                    if (i == null) {
//                        setup.Defender = i;
//                     BattleProcessor.MoveHitCharacter(setup);
//                     setup.SetupMove(move);
//                     if (setup.Cancel) {
//                        return;
//                        }
//                    } else if (i.CharacterType == Enums.CharacterType.Recruit) {
//                        Recruit recruit = i as Recruit;
                		
//                        bool attackPlayer = false;
//                        if (owner.Player.Map.Moral == Enums.MapMoral.Safe || owner.Player.Map.Moral == Enums.MapMoral.House) {
//                            attackPlayer = true;
//                        }
                
                
//                    if (!string.IsNullOrEmpty(owner.Player.GuildName)) {
//                        if (!string.IsNullOrEmpty(recruit.Owner.Player.GuildName)) {
//                            if (recruit.Owner.Player.GuildName == owner.Player.GuildName) {
//                                attackPlayer = true;
//                            }
//                        }
//                    }
                
                
//                     if (recruit.Owner.Player.Map.Tile[recruit.Owner.Player.X, recruit.Owner.Player.Y].Type == Enums.TileType.Arena) {
//                        attackPlayer = false;
//                     }
                
//                     if (recruit.Owner.Player.PK) {
//                        attackPlayer = false;
//                     }	
                	
//                     if (Ranks.IsAllowed(recruit.Owner, Enums.Rank.Moniter) && !recruit.Owner.Player.Hunted) {
//                            attackPlayer = false;
//                     }
                
//                     if (party != null) {
//                         if (party.Members.IsPlayerInParty(recruit.Owner.Player.CharID)) {
//                             attackPlayer = true;
//                         }
//                     }
                
//                     if (i == this) {
//                        attackPlayer = false;
//                     }
                
//                     if (attackPlayer) {
//                         setup.Defender = i;
//                         BattleProcessor.MoveHitCharacter(setup);
//                         setup.SetupMove(move);
//                         if (setup.Cancel) {
//                            return;
//                            }
   		             
//                     }
                		
//                    }
//            }
            	
            
                
            
//        }

//        public void UseMoveOnFoes(BattleSetup setup) {
//            Parties.Party party = null;
//            if (owner.Player.IsInParty()) {
//                party = Parties.PartyManager.FindPlayerParty(owner);
//            }
//            // Attack any players that are on the map
//            List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, MapID, this, owner.Player.X, owner.Player.Y, owner.Player.Direction);
//            Move move = setup.SetdownMove();
            
//            foreach (ICharacter i in targets) {
            		
//                // Don't attack allies
//                if (i == null) {
//                        setup.Defender = i;
//                     BattleProcessor.MoveHitCharacter(setup);
//                     setup.SetupMove(move);
//                     if (setup.Cancel) {
//                        return;
//                        }
//                    } else if (i.CharacterType == Enums.CharacterType.Recruit) {
//                        Recruit recruit = i as Recruit;
//                     bool attackPlayer = true;
//                     if (owner.Player.Map.Moral == Enums.MapMoral.Safe || owner.Player.Map.Moral == Enums.MapMoral.House) {
//                            attackPlayer = false;
//                     }
   	             
//                         if (!string.IsNullOrEmpty(owner.Player.GuildName)) {
//                             if (!string.IsNullOrEmpty(recruit.Owner.Player.GuildName)) {
//                                 if (recruit.Owner.Player.GuildName == owner.Player.GuildName) {
//                                     attackPlayer = false;
//                                 }
//                             }
//                         }

//                         if (Owner.Player.Map.Tile[X, Y].Type == Enums.TileType.Arena && recruit.Owner.Player.Map.Tile[recruit.Owner.Player.X, recruit.Owner.Player.Y].Type == Enums.TileType.Arena)
//                         {
//                            attackPlayer = true;
//                         }
   	             
//                     if (recruit.Owner.Player.PK) {
//                            attackPlayer = true;
//                     }
   	             
//                     if (Ranks.IsAllowed(recruit.Owner, Enums.Rank.Moniter) && !recruit.Owner.Player.Hunted) {
//                            attackPlayer = false;
//                     }
   	             
//                     if (party != null) {
//                         if (party.Members.IsPlayerInParty(recruit.Owner.Player.CharID)) {
//                             attackPlayer = false;
//                         }
//                     }
   	             
//                     if (i == this) {
//                            attackPlayer = false;
//                     }
   	             
//                     if (attackPlayer) {
//                        setup.Defender = i;
//                        BattleProcessor.MoveHitCharacter(setup);
//                        setup.SetupMove(move);
//                        if (setup.Cancel) {
//                                return;
//                            }
//                    }	
//                } else if (i.CharacterType == Enums.CharacterType.MapNpc) {
//                        MapNpc npc = i as MapNpc;
//                // Check if the npc is alive

//                    if (npc.Num > 0 && npc.HP > 0) {
   	                 
   	                     
//                         if (Npcs.NpcManager.Npcs[npc.Num].Behavior != Enums.NpcBehavior.Friendly && Npcs.NpcManager.Npcs[npc.Num].Behavior != Enums.NpcBehavior.Shopkeeper && Npcs.NpcManager.Npcs[npc.Num].Behavior != Enums.NpcBehavior.Scripted) {
//                             setup.Defender = npc;
//                             BattleProcessor.MoveHitCharacter(setup);
//                             setup.SetupMove(move);
//                             if (setup.Cancel) {
//                                        return;
//                                    }
//                         } else {
//                             if (setup.moveSlot == 4) {
//                                 if (Npcs.NpcManager.Npcs[npc.Num].Behavior == Enums.NpcBehavior.Scripted && Settings.Scripting == true) {
//                                     Scripting.ScriptManager.InvokeSub("ScriptedNpc", setup.Attacker, Npcs.NpcManager.Npcs[npc.Num].AIScript, npc.Num, owner.Player.Map, i);
//                                 } else {
//                                     if (!string.IsNullOrEmpty(Npcs.NpcManager.Npcs[npc.Num].AttackSay)) {
//                                         Stories.Story story = new Stories.Story();
//                                         Stories.StoryBuilderSegment segment = Stories.StoryBuilder.BuildStory();
//                                         Stories.StoryBuilder.AppendSaySegment(segment, Npcs.NpcManager.Npcs[npc.Num].Name.Trim() + ": " + Npcs.NpcManager.Npcs[npc.Num].AttackSay.Trim(),
//                                             Pokedex.Pokedex.GetPokemon(Npcs.NpcManager.Npcs[npc.Num].Species).Mugshot, 0, 0);
//                                         segment.AppendToStory(story);
//                                         Stories.StoryManager.PlayStory(owner, story);
   	                                     
//                                     }
//                                 }
//                             }
//                         }
   	                 
//                     }
//                }
                
//            }
//        }

//        //public bool IsInRangeOfAllies(IMap currentMap, Move move, int moveSlot) {
//        //    if (owner.Player.IsInParty()) {
//                // Attack the party members that are in range
//        //        Parties.Party party = Parties.PartyManager.FindPlayerParty(owner);
//        //        if (party != null) {
//        //            foreach (Client i in party.Members.GetMemberClients()) {
//        //                if (MoveProcessor.IsInRange(move.RangeType, move.Range, owner.Player.X, owner.Player.Y,
//        //                         owner.Player.Direction, i.Player.X, i.Player.Y)) {
//        //                    return true;
//        //                }
//        //            }
//        //        }
//        //    } else {
//                // Attack the guild members that are in range
//        //        if (!string.IsNullOrEmpty(owner.Player.GuildName)) {
//        //            foreach (Client i in currentMap.GetClients()) {
//        //                if (!string.IsNullOrEmpty(i.Player.GuildName)) {
//        //                    if (i.Player.GuildName == owner.Player.GuildName) {
//        //                        if (MoveProcessor.IsInRange(move.RangeType, move.Range, owner.Player.X, owner.Player.Y,
//        //                                owner.Player.Direction, i.Player.X, i.Player.Y)) {
//        //                            return true;
//        //                        }
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    }
//        //    return false;
//        //}

//        //public bool IsInRangeOfFoes(IMap currentMap, Move move, int moveSlot) {
//        //    Parties.Party party = null;
//        //    if (owner.Player.IsInParty()) {
//        //        party = Parties.PartyManager.FindPlayerParty(owner);
//        //    }
//            // Attack any players that are on the map
//        //    foreach (Client i in currentMap.GetClients()) {
//                // Don't attack allies
//        //        bool attackPlayer = false;
//        //        if (party != null) {
//        //            if (party.Members.IsPlayerInParty(i.Player.CharID)) {
//        //                attackPlayer = false;
//        //            }
//        //        } else {
//        //            if (!string.IsNullOrEmpty(owner.Player.GuildName)) {
//        //                if (!string.IsNullOrEmpty(i.Player.GuildName)) {
//        //                    if (i.Player.GuildName == owner.Player.GuildName) {
//        //                        attackPlayer = false;
//        //                    }
//        //                }
//        //            }
//        //        }
//        //        if (attackPlayer) {
//                    // Check if the target player is in range
//        //            if (MoveProcessor.IsInRange(move.RangeType, move.Range, owner.Player.X, owner.Player.Y,
//        //                owner.Player.Direction,
//        //                i.Player.X, i.Player.Y)) {
//                        // They are in range, attack them
//        //                return true;
//        //            }
//        //        }
//        //    }
//            // Attack any npcs that are in range
//        //    for (int i = 0; i < currentMap.ActiveNpc.Length; i++) {
//        //        MapNpc npc = currentMap.ActiveNpc[i];
//                // Check if the npc is alive
//        //        if (npc.Num > 0 && npc.HP > 0) {
//        //            if (MoveProcessor.IsInRange(move.RangeType, move.Range, owner.Player.X, owner.Player.Y,
//        //                owner.Player.Direction,
//        //                npc.X, npc.Y)) {

//        //                return true;
//        //            }
//        //        }
//        //    }
//        //    return false;
//        //}

//        #endregion Methods
//    }
//}