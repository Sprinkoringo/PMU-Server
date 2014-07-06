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


namespace Server.Players {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Characters = DataManager.Characters;
    using DataManager.Players;

    using Server.Combat;
    using Server.Network;
    using Server.Items;
    using Server.Exp;
    using Server.Moves;
    using Server.Maps;
    using PMU.Sockets;
    using Server.Database;

    public class Recruit : RecruitBase, Server.Combat.ICharacter {
        #region Fields

        RecruitData recruitData;
        Client owner;
        public List<int> ActiveItems { get; set; }

        #endregion Fields

        #region Properties

        public int HeldItemSlot {
            get { return recruitData.HeldItemSlot; }
            set { recruitData.HeldItemSlot = value; }
        }


        public int Darkness { get; set; }


        public Enums.StatusAilment StatusAilment {
            get;
            set;
        }

        public int StatusAilmentCounter {
            get;
            set;
        }

        public bool Confused { get; set; }

        public ExtraStatusCollection VolatileStatus {
            get;
            set;
        }

        public Client Owner {
            get { return owner; }
            //set { owner = value; }
        }

        public InventoryItem HeldItem {
            get {
                if (HeldItemSlot > -1) {
                    return owner.Player.Inventory[HeldItemSlot];
                } else {
                    return null;
                }
            }
            set {//may not use this; buggy?
                owner.Player.Inventory[HeldItemSlot] = value;
            }
        }

        public ulong GetNextLevel() {
            try {
                return ExpManager.Exp[Level - 1];
            } catch (IndexOutOfRangeException) {
                return ulong.MaxValue;
            }
        }

        #endregion Properties

        #region Methods

        public Recruit(Client owner) {
            this.owner = owner;

            recruitData = new RecruitData();
            
            HeldItemSlot = -1;
            Mobility = new bool[16];
            Darkness = -2;
            TimeMultiplier = 1000;
            ActiveItems = new List<int>();
            VolatileStatus = new ExtraStatusCollection();
        }

        public void LoadFromRecruitData(RecruitData recruitData, int recruitIndex) {
            this.recruitData = recruitData;
            base.RecruitIndex = recruitIndex;

            CopyDataFromRecruitData();

            if (Level <= -1) {
                return;
            }
            // Do checks
            if (base.HP < 0)
                base.HP = 1;
            if (IQ < 0)
                IQ = 0;
            if (MaxBelly <= 0) {
                MaxBelly = 100;
                Belly = MaxBelly;
            }
            if (Belly < 0 || Belly > MaxBelly) {
                Belly = MaxBelly;
            }

            //if (SpeciesOverride > 0 && SpeciesOverride <= Constants.TOTAL_POKEMON) {
            //    Species = SpeciesOverride;
            //} else {
            //Pokedex.Pokemon pokemon = Pokedex.Pokedex.FindBySprite(Sprite);
            //if (pokemon != null) {
            //    Species = pokemon.ID;
            //} else {
            //    Species = 0;
            //}
            //}
            if (Species < 0 || Species > Constants.TOTAL_POKEMON) {
                Species = 0;
            }

            CalculateOriginalSprite();
            CalculateOriginalStats();
            CalculateOriginalType();
            CalculateOriginalAbility();

            //CalculateOriginalMobility();

            // Do some more checks
            if (base.HP > MaxHP) {
                base.HP = MaxHP;
            }

            Loaded = true;

            if (Loaded) {
                // Verify move PP
                for (int i = 0; i < Moves.Length; i++) {
                    if (Moves[i].MoveNum > 0) {
                        if (Moves[i].MaxPP == -1 || Moves[i].MaxPP != MoveManager.Moves[Moves[i].MoveNum].MaxPP) {
                            Moves[i].MaxPP = MoveManager.Moves[Moves[i].MoveNum].MaxPP;
                        }
                        if (Moves[i].CurrentPP == -1 || Moves[i].CurrentPP > Moves[i].MaxPP) {
                            Moves[i].CurrentPP = Moves[i].MaxPP;
                        }
                    }
                }

                // Add Active Items
                LoadActiveItemList();
            }
        }

        private void CopyDataFromRecruitData() {
            // General
            this.InTempMode = recruitData.UsingTempStats;
            this.Name = recruitData.Name;
            this.Nickname = recruitData.Nickname;
            this.NpcBase = recruitData.NpcBase;
            this.Species = recruitData.Species;
            this.Sex = (Enums.Sex)recruitData.Sex;
            this.Form = recruitData.Form;
            this.Shiny = (Enums.Coloration)recruitData.Shiny;
            this.Level = recruitData.Level;
            this.Exp = recruitData.Exp;
            base.HP = recruitData.HP;
            this.StatusAilment = (Enums.StatusAilment)recruitData.StatusAilment;
            this.StatusAilmentCounter = recruitData.StatusAilmentCounter;
            this.IQ = recruitData.IQ;
            this.Belly = recruitData.Belly;
            this.MaxBelly = recruitData.MaxBelly;
            this.AtkBonus = recruitData.AtkBonus;
            this.DefBonus = recruitData.DefBonus;
            this.SpeedBuff = recruitData.SpeedBonus;
            this.SpclAtkBonus = recruitData.SpclAtkBonus;
            this.SpclDefBonus = recruitData.SpclDefBonus;

            // Moves
            for (int i = 0; i < this.Moves.Length; i++) {
                RecruitMove move = new RecruitMove();

                move.MoveNum = recruitData.Moves[i].MoveNum;
                move.CurrentPP = recruitData.Moves[i].CurrentPP;
                move.MaxPP = recruitData.Moves[i].MaxPP;
                //move.Sealed = recruitData.Moves[i].Sealed;

                this.Moves[i] = move;
            }

            // Volatile status
            for (int i = 0; i < recruitData.VolatileStatus.Count; i++) {
                ExtraStatus volatileStatus = new ExtraStatus();

                volatileStatus.Name = recruitData.VolatileStatus[i].Name;
                volatileStatus.Emoticon = recruitData.VolatileStatus[i].Emoticon;
                volatileStatus.Counter = recruitData.VolatileStatus[i].Counter;

                this.VolatileStatus.Add(volatileStatus);
            }
        }

        private void CopyDataToRecruitData() {
            if (recruitData == null) {
                recruitData = new RecruitData();
            }

            // General
            recruitData.UsingTempStats = this.InTempMode;
            recruitData.Name = this.Name;
            recruitData.Nickname = this.Nickname;
            recruitData.NpcBase = this.NpcBase;
            recruitData.Species = this.Species;
            recruitData.Sex = (byte)this.Sex;
            recruitData.Form = this.Form;
            recruitData.Shiny = (int)this.Shiny;
            recruitData.Level = this.Level;
            recruitData.Exp = this.Exp;
            recruitData.HP = this.HP;
            recruitData.StatusAilment = (int)this.StatusAilment;
            recruitData.StatusAilmentCounter = this.StatusAilmentCounter;
            recruitData.IQ = this.IQ;
            recruitData.Belly = this.Belly;
            recruitData.MaxBelly = this.MaxBelly;
            recruitData.AtkBonus = this.AtkBonus;
            recruitData.DefBonus = this.DefBonus;
            recruitData.SpeedBonus = this.SpeedBuff;
            recruitData.SpclAtkBonus = this.SpclAtkBonus;
            recruitData.SpclDefBonus = this.SpclDefBonus;

            // Moves
            for (int i = 0; i < this.Moves.Length; i++) {
                RecruitMove move = this.Moves[i];

                recruitData.Moves[i].MoveNum = move.MoveNum;
                recruitData.Moves[i].CurrentPP = move.CurrentPP;
                recruitData.Moves[i].MaxPP = move.MaxPP;
                //recruitData.Moves[i].Sealed = move.Sealed;
            }

            // Volatile status
            recruitData.VolatileStatus.Clear();
            for (int i = 0; i < this.VolatileStatus.Count; i++) {
                Characters.VolatileStatus recruitDataVolatileStatus = new Characters.VolatileStatus();
                recruitDataVolatileStatus.Name = this.VolatileStatus[i].Name;
                recruitDataVolatileStatus.Emoticon = this.VolatileStatus[i].Emoticon;
                recruitDataVolatileStatus.Counter = this.VolatileStatus[i].Counter;

                recruitData.VolatileStatus.Add(recruitDataVolatileStatus);
            }
        }

        public void Save(DatabaseConnection dbConnection) {
            CopyDataToRecruitData();
            PlayerDataManager.SavePlayerRecruit(dbConnection.Database, owner.Player.CharID, this.RecruitIndex, this.recruitData);
        }

        public void CheckHeldItem() {
            if (HeldItemSlot > -1 && owner.Player.Inventory[HeldItemSlot].Num < 1) {
                HeldItemSlot = -1;
            }
        }

        public void RefreshActiveItemList() {
            MaxHPBoost = 0;
            //add PP
            AtkBoost = 0;
            DefBoost = 0;
            SpclAtkBoost = 0;
            SpclDefBoost = 0;
            SpdBoost = 0;
            EXPBoost = 0;
            RecruitBoost = 0;

            ActiveItems.Clear();

            LoadActiveItemList();
        }

        public void LoadActiveItemList() {
            //add held item, if applicable
            if (HeldItemSlot > -1 && (HeldItem.Num < 0 || HeldItem.Num >= ItemManager.Items.MaxItems)) {
                HeldItemSlot = -1;
            }
            if (HeldItemSlot > -1 && ItemManager.Items[HeldItem.Num].Type == Enums.ItemType.Held) {
                if (MeetsReqs(HeldItem.Num) && !HeldItem.Sticky) {

                    AddToActiveItemList(HeldItem.Num);
                }
            }

            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (owner.Player.Team[i].Loaded && owner.Player.Team[i].HeldItemSlot > -1 && ItemManager.Items[owner.Player.Team[i].HeldItem.Num].Type == Enums.ItemType.HeldByParty) {

                    if (MeetsReqs(owner.Player.Team[i].HeldItem.Num) && !owner.Player.Team[i].HeldItem.Sticky) {

                        AddToActiveItemList(owner.Player.Team[i].HeldItem.Num);
                    }

                }
            }

            for (int i = 1; i < owner.Player.MaxInv; i++) {

                //held-in-bag items

                if (owner.Player.Inventory[i].Num > 0 && ItemManager.Items[owner.Player.Inventory[i].Num].Type == Enums.ItemType.HeldInBag
                    && MeetsReqs(owner.Player.Inventory[i].Num) && !owner.Player.Inventory[i].Sticky) {
                    AddToActiveItemList(owner.Player.Inventory[i].Num);

                }
            }

        }

        public void AddToActiveItemList(int itemNum) {
            if (!ActiveItems.Contains(itemNum)) {
                //add to active items list
                ActiveItems.Add(itemNum);


                //add bonuses
                MaxHPBoost += ItemManager.Items[itemNum].AddHP;
                //add PP
                AtkBoost += ItemManager.Items[itemNum].AddAttack;
                DefBoost += ItemManager.Items[itemNum].AddDefense;
                SpclAtkBoost += ItemManager.Items[itemNum].AddSpAtk;
                SpclDefBoost += ItemManager.Items[itemNum].AddSpDef;
                SpdBoost += ItemManager.Items[itemNum].AddSpeed;
                EXPBoost += ItemManager.Items[itemNum].AddEXP;
                RecruitBoost += ItemManager.Items[itemNum].RecruitBonus;

                Scripting.ScriptManager.InvokeFunction("OnItemActivated", itemNum, this);

                if (owner.Player.GetActiveRecruit() == this) {
                    Messenger.SendStats(owner);
                }
                if (ItemManager.Items[itemNum].AddHP != 0) {
                    Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
                }
            }
        }

        public void RemoveFromActiveItemList(int itemNum) {
            if (ActiveItems.Contains(itemNum)) {
                //remove from active items list
                ActiveItems.Remove(itemNum);


                //subtract bonuses
                MaxHPBoost -= ItemManager.Items[itemNum].AddHP;
                //sub PP
                AtkBoost -= ItemManager.Items[itemNum].AddAttack;
                DefBoost -= ItemManager.Items[itemNum].AddDefense;
                SpclAtkBoost -= ItemManager.Items[itemNum].AddSpAtk;
                SpclDefBoost -= ItemManager.Items[itemNum].AddSpDef;
                SpdBoost -= ItemManager.Items[itemNum].AddSpeed;
                EXPBoost -= ItemManager.Items[itemNum].AddEXP;
                RecruitBoost -= ItemManager.Items[itemNum].RecruitBonus;

                Scripting.ScriptManager.InvokeFunction("OnItemDeactivated", itemNum, this);

                if (owner.Player.GetActiveRecruit() == this) {
                    Messenger.SendStats(owner);
                }

                if (ItemManager.Items[itemNum].AddHP != 0) {
                    Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
                }
            }

        }

        public bool MeetsReqs(int itemNum) {
            /*
            if (BaseAtk + AtkBonus < ItemManager.Items[itemNum].AttackReq) {
                return false;
            }

            if (BaseDef + DefBonus < ItemManager.Items[itemNum].DefenseReq) {
                return false;
            }

            if (BaseSpclAtk + SpclAtkBonus < ItemManager.Items[itemNum].SpAtkReq) {
                return false;
            }

            if (BaseSpclDef + SpclDefBonus < ItemManager.Items[itemNum].SpDefReq) {
                return false;
            }

            if (BaseSpd + SpdBonus < ItemManager.Items[itemNum].SpeedReq) {
                return false;
            }
            */
            //if (ItemManager.Items[itemNum].ScriptedReq > -1) {
                return Scripting.ScriptManager.InvokeFunction("ScriptedReq", this, itemNum).ToBool();
            //}

            //return true;
        }

        public bool HasActiveItem(int itemNum) {
            if (ActiveItems.Contains(itemNum)) {
                return true;
            }
            return false;
        }

        public bool CanLearnNewMove() {
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                if (Moves[i].MoveNum < 1) {
                    return true;
                }
            }
            return false;
        }

        public bool CanLearnTMMove(int moveNum) {
            Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(Species, Form);
            if (pokemon != null) {
                return pokemon.TMMoves.Contains(moveNum);
            } else {
                return false;
            }
        }

        #endregion Methods

        public Enums.CharacterType CharacterType {
            get { return Enums.CharacterType.Recruit; }
        }

        public TickCount AttackTimer {
            get {
                return Owner.Player.AttackTimer;
            }
            set {
                Owner.Player.AttackTimer = value;
            }

        }

        public TickCount PauseTimer {
            get {
                return Owner.Player.PauseTimer;
            }
            set {
                Owner.Player.PauseTimer = value;
            }

        }

        public new int HP {
            get { return base.HP; }
            set {
                if (value > MaxHP) {
                    value = MaxHP;
                }
                if (value < 0) {
                    value = 0;
                }
                base.HP = value;

                Messenger.SendHP(owner, Array.IndexOf(owner.Player.Team, this));
            }
        }

        public string MapID {
            get { return owner.Player.MapID; }
        }

        public int X {
            get { return owner.Player.X; }
            set { owner.Player.X = value; }
        }

        public int Y {
            get { return owner.Player.Y; }
            set { owner.Player.Y = value; }
        }

        public Enums.Direction Direction {
            get { return owner.Player.Direction; }
            set { owner.Player.Direction = value; }
        }

        public bool HasMove(int moveNum) {
            for (int i = 0; i < Moves.Length; i++) {
                if (Moves[i].MoveNum == moveNum) {
                    return true;
                }
            }
            return false;
        }

        public void LearnNewMove(int moveNum) {
            if (CanLearnNewMove() == true) {
                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                    if (Moves[i].MoveNum < 1) {
                        Moves[i].MoveNum = moveNum;
                        Moves[i].MaxPP = MoveManager.Moves[moveNum].MaxPP;
                        Moves[i].CurrentPP = Moves[i].MaxPP;
                        Messenger.PlayerMsg(owner, "You have learned " + MoveManager.Moves[moveNum].Name + "!", Text.BrightGreen);
                        return;
                    }
                }
            } else {
                LearningMove = moveNum;
                string questionText;
                if (Name != "") {
                    questionText = Name + " is trying to learn " + MoveManager.Moves[moveNum].Name + ", but " + Name + " can only know 4 moves. Would you like to forget one to make room for " + MoveManager.Moves[moveNum].Name + "?";
                } else {
                    questionText = "You are trying to learn " + MoveManager.Moves[moveNum].Name + ", but you can only know 4 moves. Would you like to forget one to make room for " + MoveManager.Moves[moveNum].Name + "?";
                }
                Owner.Player.Hunted = false;
                Messenger.AskQuestion(Owner, "MovesFull", questionText, -1);
            }
        }

        public void GenerateMoveset() {
            Pokedex.PokemonForm pokemon = null;
            List<int> validLevelUpMoves = new List<int>();
            if (Species < 0) {
                return;
            }
            pokemon = Pokedex.Pokedex.GetPokemonForm(Species, Form);
            if (pokemon != null) {
                if (validLevelUpMoves.Count == 0) {
                    for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                        if (pokemon.LevelUpMoves[n].Level <= this.Level) {
                            validLevelUpMoves.Add(n);
                        }
                    }
                }
            }

            for (int i = 0; i < Moves.Length; i++) {
                if (validLevelUpMoves.Count > 0) {
                    int levelUpMoveSelected = Server.Math.Rand(0, validLevelUpMoves.Count);
                    Moves[i].MoveNum = pokemon.LevelUpMoves[validLevelUpMoves[levelUpMoveSelected]].Move;
                    if (Moves[i].MoveNum > -1) {
                        Moves[i].MaxPP = Server.Moves.MoveManager.Moves[Moves[i].MoveNum].MaxPP;
                        Moves[i].CurrentPP = Moves[i].MaxPP;
                    }
                    validLevelUpMoves.RemoveAt(levelUpMoveSelected);
                }
            }
        }

        public void RestoreVitals() {
            HP = MaxHP;
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i].CurrentPP = Moves[i].MaxPP;
            }
        }

        public void RestoreBelly() {
            Belly = MaxBelly;
            Messenger.SendBelly(owner);
        }

        public void ConsumeBelly(int bellyToRemove) {
            Belly -= bellyToRemove;
            if (Belly < 0) {
                Belly = 0;
            }
            Messenger.SendBelly(owner);
        }

        public void SetSpecies(int species) {
            Species = species;

            if (Pokedex.Pokedex.GetPokemon(species).Forms.Count <= Form) {
                Form = 0;
            }

            CalculateOriginalSprite();
            CalculateOriginalStats();
            CalculateOriginalType();
            CalculateOriginalAbility();
            //CalculateOriginalMobility();
        }

        public void SetForm(int form) {
            Form = form;

            if (Pokedex.Pokedex.GetPokemon(Species).Forms.Count <= Form) {
                Form = 0;
            }


            CalculateOriginalSprite();
            CalculateOriginalStats();
            CalculateOriginalType();
            CalculateOriginalAbility();
            //CalculateOriginalMobility();
        }

        public void UseMove(int moveSlot) {
            if (owner.Player.Dead) {
                return;
            }


            owner.Player.Map.ProcessingPaused = false;


            if (Ranks.IsDisallowed(owner, Enums.Rank.Moniter) || owner.Player.ProtectionOff) {
                owner.Player.Hunted = true;
                Messenger.SendHunted(owner);
            }

            if (!Core.GetTickCount().Elapsed(owner.Player.PauseTimer, 0)) {
                return;
            }

            if (moveSlot > -1 && moveSlot < 4) {
                Move move = MoveManager.Moves[Moves[moveSlot].MoveNum];
                IMap currentMap = owner.Player.GetCurrentMap();
                if (move.KeyItem > 0) {
                    bool doorOpened = UseMoveKey(currentMap, move, moveSlot);
                    if (doorOpened) {
                        return;
                    }
                }
            }
            BattleSetup setup = new BattleSetup();
            setup.Attacker = this;
            setup.moveSlot = moveSlot;

            BattleProcessor.HandleAttack(setup);


            BattleProcessor.FinalizeAction(setup);
        }

        public bool UseMoveKey(IMap currentMap, Move move, int moveSlot) {
            bool doorOpened = false;
            int x = 0;
            int y = 0;
            switch (owner.Player.Direction) {
                case Enums.Direction.Up: {
                        if (owner.Player.Y > 0) {
                            x = owner.Player.X;
                            y = owner.Player.Y - 1;
                        } else {
                            return false;
                        }
                    }
                    break;
                case Enums.Direction.Down: {
                        if (owner.Player.Y < currentMap.MaxY) {
                            x = owner.Player.X;
                            y = owner.Player.Y + 1;
                        } else {
                            return false;
                        }
                    }
                    break;
                case Enums.Direction.Left: {
                        if (owner.Player.X > 0) {
                            x = owner.Player.X - 1;
                            y = owner.Player.Y;
                        } else {
                            return false;
                        }
                    }
                    break;
                case Enums.Direction.Right: {
                        if (owner.Player.X < currentMap.MaxX) {
                            x = owner.Player.X + 1;
                            y = owner.Player.Y;
                        } else {
                            return false;
                        }
                    }
                    break;
            }

            // Check if a key exists
            if (currentMap.Tile[x, y].Type == Enums.TileType.Key) {
                // Check if the key they are using matches the map key
                if (move.KeyItem == currentMap.Tile[x, y].Data1) {
                    currentMap.Tile[x, y].DoorOpen = true;
                    currentMap.Tile[x, y].DoorTimer = Core.GetTickCount();

                    Messenger.SendDataToMap(MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));

                    if (string.IsNullOrEmpty(currentMap.Tile[x, y].String1)) {
                        //Messenger.PlayerMsg(Owner, "A door has been unlocked!", Text.White);
                    } else {
                        Messenger.PlayerMsg(Owner, currentMap.Tile[x, y].String1.Trim(), Text.White);
                    }

                    Messenger.PlaySoundToMap(MapID, "key.wav");

                    Messenger.SpellAnim(Moves[moveSlot].MoveNum, currentMap.MapID, x, y);
                    doorOpened = true;
                }
            }

            if (currentMap.Tile[x, y].Type == Enums.TileType.Door) {
                currentMap.Tile[x, y].DoorOpen = true;
                currentMap.Tile[x, y].DoorTimer = Core.GetTickCount();

                Messenger.SendDataToMap(MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));
                Messenger.PlaySoundToMap(MapID, "key.wav");
            }
            return doorOpened;
        }


        public void UseMoveOnAllies(BattleSetup setup, TargetCollection targets) {
            Parties.Party party = null;
            if (owner.Player.IsInParty()) {
                party = Parties.PartyManager.FindPlayerParty(owner);
            }
            //List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, setup.AttackerMap, this, owner.Player.X, owner.Player.Y, owner.Player.Direction);
            Move move = setup.SetdownMove();
            // Attack any players that are on the map
            foreach (ICharacter i in targets.Friends) {
                if (i.CharacterType == Enums.CharacterType.Recruit && Ranks.IsAllowed(((Recruit)i).Owner, Enums.Rank.Moniter)
                        && !((Recruit)i).Owner.Player.Hunted && !((Recruit)i).Owner.Player.Dead) {
                } else {
                    setup.Defender = i;
                    BattleProcessor.MoveHitCharacter(setup);
                    setup.SetupMove(move);
                    if (setup.Cancel) {
                        return;
                    }
                }
            }
        }

        public void UseMoveOnFoes(BattleSetup setup, TargetCollection targets) {
            Parties.Party party = null;
            if (owner.Player.IsInParty()) {
                party = Parties.PartyManager.FindPlayerParty(owner);
            }
            
            //List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, setup.AttackerMap, this, owner.Player.X, owner.Player.Y, owner.Player.Direction);
            
            Move move = setup.SetdownMove();

            foreach (ICharacter i in targets.Foes) {
                // Don't attack allies
                if (i.CharacterType == Enums.CharacterType.Recruit) {
                    Recruit recruit = i as Recruit;
                    if (Ranks.IsAllowed(recruit.Owner, Enums.Rank.Moniter) && !recruit.Owner.Player.Hunted
                        || (recruit.Owner.Player.Map.Tile[recruit.Owner.Player.X, recruit.Owner.Player.Y].Type == Enums.TileType.Arena) != (Owner.Player.Map.Tile[X, Y].Type == Enums.TileType.Arena)) {

                    } else {
                        setup.Defender = i;
                        BattleProcessor.MoveHitCharacter(setup);
                        setup.SetupMove(move);
                        if (setup.Cancel) {
                            return;
                        }
                    }
                } else if (i.CharacterType == Enums.CharacterType.MapNpc && (((MapNpc)i).Num <= 0 || ((MapNpc)i).HP <= 0)) {
                } else if (i.CharacterType == Enums.CharacterType.MapNpc) {
                    MapNpc npc = i as MapNpc;
                    if (Npcs.NpcManager.Npcs[npc.Num].Behavior == Enums.NpcBehavior.Scripted) {
                        if (setup.moveIndex == -1) {
                            Scripting.ScriptManager.InvokeSub("ScriptedNpc", setup.Attacker, Npcs.NpcManager.Npcs[npc.Num].AIScript, npc.Num, owner.Player.Map, i);
                        }
                    } else if (!string.IsNullOrEmpty(Npcs.NpcManager.Npcs[npc.Num].AttackSay) && (Npcs.NpcManager.Npcs[npc.Num].Behavior == Enums.NpcBehavior.Friendly || Npcs.NpcManager.Npcs[npc.Num].Behavior == Enums.NpcBehavior.Shopkeeper)) {
                        if (setup.moveIndex == -1) {
                            Stories.Story story = new Stories.Story();
                            Stories.StoryBuilderSegment segment = Stories.StoryBuilder.BuildStory();
                            Stories.StoryBuilder.AppendSaySegment(segment, Npcs.NpcManager.Npcs[npc.Num].Name.Trim() + ": " + Npcs.NpcManager.Npcs[npc.Num].AttackSay.Trim(),
                                Npcs.NpcManager.Npcs[npc.Num].Species, 0, 0);
                            segment.AppendToStory(story);
                            Stories.StoryManager.PlayStory(owner, story);
                        }
                    } else {
                        setup.Defender = i;
                        BattleProcessor.MoveHitCharacter(setup);
                        setup.SetupMove(move);
                        if (setup.Cancel) {
                            return;
                        }
                    }
                } else {
                    setup.Defender = i;
                    BattleProcessor.MoveHitCharacter(setup);
                    setup.SetupMove(move);
                    if (setup.Cancel) {
                        return;
                    }
                }


            }
        }


        public void TakeHeldItem(int val) {
            if (HeldItemSlot > -1 && HeldItem != null) {
                owner.Player.TakeItemSlot(HeldItemSlot, val, true);
            }
        }

        public void UseHeldItem() {
            if (HeldItemSlot > -1 && HeldItem != null) {
                owner.Player.UseItem(HeldItem, HeldItemSlot);
            }
        }

        public void GiveHeldItem(int num, int val, string tag, bool sticky) {
            int slot = owner.Player.FindInvSlot(num, val);
            owner.Player.GiveItem(num, val, tag, sticky);
            owner.Player.HoldItem(slot);
        }

        public void MapGetItem() {
            owner.Player.PickupItem();
        }

        public void MapDropItem(int val, Client playerFor) {
            if (HeldItemSlot > -1 && HeldItem != null) {
                owner.Player.DropItem(HeldItemSlot, val, playerFor);
            }
        }

        public void ThrowHeldItem() {
            if (HeldItemSlot > -1 && HeldItem != null) {
                owner.Player.ThrowItem(HeldItem, HeldItemSlot);
            }
        }


    }
}