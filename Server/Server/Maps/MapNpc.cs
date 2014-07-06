namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using PMU.Sockets;

    using Server.Combat;
    using Server.Items;
    using Server.Moves;
    using Server.Network;
    using Server.Npcs;
    using Server.Players;

    public class MapNpc : MapNpcBase, Combat.ICharacter
    {
        #region Fields

        bool hpChanged;
        int lastHP;
        InventoryItem heldItem;

        #endregion Fields

        #region Constructors

        public MapNpc(string mapID, DataManager.Maps.MapNpc rawNpc)
            : base(rawNpc) {
            

            SpeedLimit = Enums.Speed.Running;

            Moves = new Players.RecruitMove[4];
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i] = new Players.RecruitMove(RawNpc.Moves[i]);
            }

            //if (RawNpc.HeldItem == null) {
            //    HeldItem = null;
            //} else {
            //    HeldItem = new InventoryItem(RawNpc.HeldItem);
            //}
            heldItem = new InventoryItem(RawNpc.HeldItem);

            VolatileStatus = new ExtraStatusCollection();
            for (int i = 0; i < RawNpc.VolatileStatus.Count; i++) {
                VolatileStatus.Add(new ExtraStatus(RawNpc.VolatileStatus[i]));
            }
            MapID = mapID;
            ActiveItems = new List<int>();

            //CalculateOriginalSprite();
            //CalculateOriginalStats();
            //CalculateOriginalType();
            //CalculateOriginalAbility();
            //CalculateOriginalMobility();
            if (Num > 0) {
                LoadActiveItemList();
            }
        }

        public MapNpc(string mapID, int mapSlot)
            : base(new DataManager.Maps.MapNpc(mapSlot)) {
            

            SpeedLimit = Enums.Speed.Running;

            Moves = new Players.RecruitMove[4];
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i] = new Players.RecruitMove(RawNpc.Moves[i]);
            }

            //if (RawNpc.HeldItem == null) {
            //    HeldItem = null;
            //} else {
            //    HeldItem = new InventoryItem(RawNpc.HeldItem);
            //}
            heldItem = new InventoryItem(RawNpc.HeldItem);

            VolatileStatus = new ExtraStatusCollection();
            for (int i = 0; i < RawNpc.VolatileStatus.Count; i++) {
                VolatileStatus.Add(new ExtraStatus(RawNpc.VolatileStatus[i]));
            }
            MapID = mapID;
            ActiveItems = new List<int>();

            SpeedLimit = Enums.Speed.Running;
        }

        #endregion Constructors

        #region Properties

        public Enums.CharacterType CharacterType {
            get { return Enums.CharacterType.MapNpc; }
        }

        public bool Confused {
            //get { return rawNpc.ConfusionStepCounter; }
            //set { rawNpc.ConfusionStepCounter = value; }
            get;
            set;
        }

        public InventoryItem HeldItem {
            get {
                if (heldItem.Num < 0) {
                    return null;
                } else {
                    return heldItem;
                }
            }
            set {
                if (value == null) {
                    heldItem.Num = -1;
                    heldItem.Amount = 0;
                    heldItem.Sticky = false;
                    heldItem.Tag = "";
                } else {
                    heldItem.Num = value.Num;
                    heldItem.Amount = value.Amount;
                    heldItem.Sticky = value.Sticky;
                    heldItem.Tag = value.Tag;
                }
            }
        }

        public bool HitByMove {
            get;
            set;
        }

        public override int HP {
            get {
                return base.HP;
            }
            set {
                if (base.HP != lastHP) {
                    lastHP = base.HP;
                    hpChanged = true;
                }
                if (value > MaxHP) {
                    value = MaxHP;
                }
                base.HP = value;

            }
        }

        public bool HPChanged {
            get { return hpChanged; }
        }

        public int HPStepCounter {
            get { return RawNpc.HPStepCounter; }
            set { RawNpc.HPStepCounter = value; }
        }


        public List<int> ActiveItems {
            get;
            set;
        }


        public string MapID {
            get;
            set;
        }

        public int MapSlot {
            get { return RawNpc.MapSlot; }
        }

        public int Species {
            get { return Npcs.NpcManager.Npcs[Num].Species; }
        }

        public Enums.Speed SpeedLimit {
            get;
            set;
        }

        public Enums.StatusAilment StatusAilment {
            get { return (Enums.StatusAilment)RawNpc.StatusAilment; }
            set { RawNpc.StatusAilment = (byte)value; }
        }

        public int StatusAilmentCounter {
            get { return RawNpc.StatusAilmentCounter; }
            set { RawNpc.StatusAilmentCounter = value; }
        }

        public Client Target {
            get;
            set;
        }

        public int UseableMoveCount {
            get {
                int counter = 0;
                for (int i = 0; i < Moves.Length; i++) {
                    if (Moves[i].MoveNum > -1 && Moves[i].CurrentPP > 0) {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public ExtraStatusCollection VolatileStatus {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Save() {
            RawNpc.VolatileStatus = new List<DataManager.Characters.VolatileStatus>();
            for (int i = 0; i < VolatileStatus.Count; i++) {
                RawNpc.VolatileStatus.Add(VolatileStatus[i].RawVolatileStatus);
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

            ActiveItems.Clear();

            LoadActiveItemList();
        }

        public void LoadActiveItemList() {
            //add held item, if applicable

            if (HeldItem != null && !HeldItem.Sticky && MeetsReqs(HeldItem.Num)) {
                AddToActiveItemList(HeldItem.Num);
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

                Scripting.ScriptManager.InvokeFunction("OnItemActivated", itemNum, this);

                if (ItemManager.Items[HeldItem.Num].AddHP != 0) {
                    //SendHPToMap(TODO
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

                Scripting.ScriptManager.InvokeFunction("OnItemDeactivated", itemNum, this);


                if (ItemManager.Items[HeldItem.Num].AddHP != 0) {
                    //SendHPToMap(TODO
                }
            }

        }


        public void CalculateOriginalAbility() {
            Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(Species, Form);
            Ability1 = pokemon.Ability1;
            Ability2 = pokemon.Ability2;
            Ability3 = pokemon.Ability3;
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


        public void CalculateOriginalSpecies() {

        }

        public void CalculateOriginalSprite() {
            //Sprite = Pokedex.Pokedex.GetPokemonForm(Species, Form).Sprite[(int)Shiny, (int)Sex];
            Sprite = Species;
        }

        public void CalculateOriginalStats() {
            Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(Species, Form);
            BaseMaxHP = pokemon.GetMaxHP(Level);
            BaseAtk = pokemon.GetAtt(Level);
            BaseDef = pokemon.GetDef(Level);
            BaseSpclAtk = pokemon.GetSpAtt(Level);
            BaseSpclDef = pokemon.GetSpDef(Level);
            BaseSpd = pokemon.GetSpd(Level);
        }

        public void CalculateOriginalType() {
            Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(Species, Form);
            Type1 = pokemon.Type1;
            Type2 = pokemon.Type2;
        }

        public void GenerateHeldItem() {
            //add sticky?
            List<InventoryItem> possibleDrops = new List<InventoryItem>();
            for (int i = 0; i < Constants.MAX_NPC_DROPS; i++) {
                if (NpcManager.Npcs[Num].Drops[i].ItemNum > 0) {
                    if (NpcManager.Npcs[Num].Drops[i].Chance == 1) {
                        InventoryItem drop = new InventoryItem();
                        drop.Num = NpcManager.Npcs[Num].Drops[i].ItemNum;
                        drop.Amount = NpcManager.Npcs[Num].Drops[i].ItemValue;
                        drop.Tag = NpcManager.Npcs[Num].Drops[i].Tag;
                        possibleDrops.Add(drop);
                    } else {
                        if (NpcManager.Npcs[Num].Drops[i].Chance > 0) {

                            if (Server.Math.Rand(0, NpcManager.Npcs[Num].Drops[i].Chance + 1) == 0) {
                                InventoryItem drop = new InventoryItem();
                                drop.Num = NpcManager.Npcs[Num].Drops[i].ItemNum;
                                drop.Amount = NpcManager.Npcs[Num].Drops[i].ItemValue;
                                drop.Tag = NpcManager.Npcs[Num].Drops[i].Tag;
                                possibleDrops.Add(drop);
                            }
                        }
                    }
                }
            }
            if (possibleDrops.Count > 0) {
                GiveItem(possibleDrops[Server.Math.Rand(0, possibleDrops.Count)]);
            }
        }

        public void GenerateMoveset() {
            Npcs.Npc npc = Npcs.NpcManager.Npcs[Num];
            Pokedex.PokemonForm pokemon = null;
            List<int> validLevelUpMoves = new List<int>();
            for (int i = 0; i < Moves.Length; i++) {
                if (npc.Moves[i] == -1) {
                    if (npc.Species > -1) {
                        pokemon = Pokedex.Pokedex.GetPokemonForm(npc.Species, Form);
                    } else {
                        return;
                    }
                    if (pokemon != null) {
                        if (validLevelUpMoves.Count == 0) {
                            for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                                if (pokemon.LevelUpMoves[n].Level <= this.Level) {
                                    validLevelUpMoves.Add(n);
                                }
                            }
                        }
                    }
                    break;
                }
            }
            for (int i = 0; i < Moves.Length; i++) {
                if (npc.Moves[i] == -1) {
                    if (validLevelUpMoves.Count > 0) {
                        int levelUpMoveSelected = Server.Math.Rand(0, validLevelUpMoves.Count);
                        Moves[i].MoveNum = pokemon.LevelUpMoves[validLevelUpMoves[levelUpMoveSelected]].Move;
                        if (Moves[i].MoveNum > -1) {
                            Moves[i].MaxPP = Server.Moves.MoveManager.Moves[Moves[i].MoveNum].MaxPP;
                            Moves[i].CurrentPP = Moves[i].MaxPP;
                        }
                        validLevelUpMoves.RemoveAt(levelUpMoveSelected);
                    }
                } else if (npc.Moves[i] > 0) {
                    // Move is preset in the npc editor
                    Moves[i].MoveNum = npc.Moves[i];
                    Moves[i].MaxPP = Server.Moves.MoveManager.Moves[Moves[i].MoveNum].MaxPP;
                    Moves[i].CurrentPP = Moves[i].MaxPP;
                }
            }
        }

        public bool HasActiveItem(int itemNum) {
            if (HeldItem != null && HeldItem.Num == itemNum && ActiveItems.Contains(itemNum)) {
                return true;
            }
            return false;
        }

        public bool IsInRangeOfAllies(IMap currentMap, Move move, int moveSlot) {
            for (int i = 0; i < currentMap.ActiveNpc.Length; i++) {
                if (currentMap.ActiveNpc[i].Num > 0) {
                    if (MoveProcessor.IsInRange(move.RangeType, move.Range, this.X, this.Y,
                                     this.Direction,
                                     currentMap.ActiveNpc[i].X, currentMap.ActiveNpc[i].Y)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsInRangeOfFoes(IMap currentMap, Move move, int moveSlot) {
            // Check if any players are in range
            foreach (Client i in currentMap.GetClients()) {
                // Check if the target player is in range
                if (MoveProcessor.IsInRange(move.RangeType, move.Range, this.X, this.Y,
                    this.Direction,
                    i.Player.X, i.Player.Y)) {
                    // They are in range, attack them
                    return true;
                }
            }
            return false;
        }

        public void MapDropItem(int val, Client playerFor) {
            if (HeldItem != null) {
                IMap currentMap = MapManager.RetrieveActiveMap(MapID);
                int i = currentMap.FindOpenItemSlot();

                if (i != -1) {
                    
                    if (currentMap != null) {
                        if (val >= HeldItem.Amount) {
                            currentMap.SpawnItemSlot(i, HeldItem.Num, HeldItem.Amount, HeldItem.Sticky, false, HeldItem.Tag, X, Y, playerFor);
                            RemoveFromActiveItemList(HeldItem.Num);
                            HeldItem = null;
                        } else {
                            currentMap.SpawnItemSlot(i, HeldItem.Num, val, false, false, HeldItem.Tag, X, Y, playerFor);
                            HeldItem.Amount -= val;
                        }
                        Scripting.ScriptManager.InvokeSub("OnDropItem", this, -1, currentMap.ActiveItem[i]);
                    }
                }
            }
        }

        public void MapGetItem() {
            int i = 0;

            IMap map = MapManager.RetrieveActiveMap(MapID);

            for (i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                // See if theres even an item here
                if ((map.ActiveItem[i].Num >= 0) & (map.ActiveItem[i].Num < Items.ItemManager.Items.MaxItems)) {
                    // Check if item is at the same location as the player
                    if ((map.ActiveItem[i].X == X) & (map.ActiveItem[i].Y == Y)) {
                        // Open slot available?
                        if (HeldItem == null) {
                            // Set item in players inventory
                            InventoryItem newItem = new InventoryItem();
                            newItem.Num = map.ActiveItem[i].Num;
                            newItem.Sticky = map.ActiveItem[i].Sticky;
                            newItem.Tag = map.ActiveItem[i].Tag;

                            if (ItemManager.Items[map.ActiveItem[i].Num].Type == Enums.ItemType.Currency || ItemManager.Items[map.ActiveItem[i].Num].StackCap > 0) {
                                newItem.Amount = map.ActiveItem[i].Value;
                                if (newItem.Amount > ItemManager.Items[map.ActiveItem[i].Num].StackCap) {
                                    newItem.Amount = ItemManager.Items[map.ActiveItem[i].Num].StackCap;
                                }
                            } else {
                                newItem.Amount = 0;
                            }

                            GiveItem(newItem);

                            // Erase item from the map ~ done in spawnitemslot

                            map.SpawnItemSlot(i, -1, 0, false, false, "", X, Y, null);

                            Scripting.ScriptManager.InvokeSub("OnPickupItem", this, -1, HeldItem);
                            return;
                        } else {
                            if (map.ActiveItem[i].Num == HeldItem.Num && (ItemManager.Items[map.ActiveItem[i].Num].Type == Enums.ItemType.Currency || ItemManager.Items[map.ActiveItem[i].Num].StackCap > 0)) {
                                HeldItem.Amount += map.ActiveItem[i].Value;
                                if (HeldItem.Amount > ItemManager.Items[map.ActiveItem[i].Num].StackCap) {
                                    HeldItem.Amount = ItemManager.Items[map.ActiveItem[i].Num].StackCap;
                                }
                                HeldItem.Sticky = map.ActiveItem[i].Sticky;
                            }
                        }

                    }
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

        public void SendHPToMap(PacketHitList packetList, IMap map, int mapNpcNum) {
            PacketBuilder.AppendNpcHP(map, packetList, mapNpcNum);
            hpChanged = false;
        }

        public void GiveHeldItem(int num, int val, string tag, bool sticky) {
            InventoryItem newItem = new InventoryItem();
            newItem.Num = num;
            newItem.Amount = val;
            newItem.Sticky = sticky;
            newItem.Tag = tag;
            GiveItem(newItem);
        }




        public void GiveItem(InventoryItem item) {
            HeldItem = item;
            if (!HeldItem.Sticky && MeetsReqs(HeldItem.Num)) {
                AddToActiveItemList(HeldItem.Num);
            }
        }

        public void TakeHeldItem(int val) {
            bool takeItem = false;
            if (HeldItem != null) {
                if (ItemManager.Items[HeldItem.Num].Type == Enums.ItemType.Currency || ItemManager.Items[HeldItem.Num].StackCap > 0) {
                    // Is what we are trying to take away more then what they have?  If so just set it to zero
                    if (val >= HeldItem.Amount) {
                        takeItem = true;
                    } else {
                        HeldItem.Amount = HeldItem.Amount - val;
                    }
                } else {
                    takeItem = true;
                }

                if (takeItem) {
                    RemoveFromActiveItemList(HeldItem.Num);
                    HeldItem = null;
                }
            }
        }

        public void ThrowHeldItem() {
            BattleSetup setup = new BattleSetup();

            setup.Attacker = this;

            Scripting.ScriptManager.InvokeSub("ThrewItem", setup, HeldItem, 0);

            BattleProcessor.FinalizeAction(setup);
        }

        public void UseHeldItem() {
            if (HeldItem == null) {
                return;
            }

            BattleSetup setup = new BattleSetup();

            setup.Attacker = this;

            BattleProcessor.HandleItemUse(HeldItem, 0, setup);

            BattleProcessor.FinalizeAction(setup);
        }

        //public void UseMoveOnSelf(int moveNum) {
        //    BattleProcessor.MoveHitCharacter(this, this, moveNum);
        //    Messenger.SpellAnim(moveNum, this.MapID, this.X, this.Y);
        //}
        public void UseMove(int moveSlot) {
            BattleSetup setup = new BattleSetup();

            setup.Attacker = this;
            setup.moveSlot = moveSlot;

            BattleProcessor.HandleAttack(setup);

            BattleProcessor.FinalizeAction(setup);
        }

        public void UseMoveOnAllies(BattleSetup setup, TargetCollection targets) {
            //List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, setup.AttackerMap, this, X, Y, Direction);
            Move move = setup.SetdownMove();

            foreach (ICharacter i in targets.Friends) {
                if (i.CharacterType == Enums.CharacterType.MapNpc && ((MapNpc)i).Num <= 0) {

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
            // Attack any players that are on the map
            //List<ICharacter> targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, setup.AttackerMap, this, X, Y, Direction);
            Move move = setup.SetdownMove();

            foreach (ICharacter i in targets.Foes) {
                setup.Defender = i;
                BattleProcessor.MoveHitCharacter(setup);
                setup.SetupMove(move);
                if (setup.Cancel) {
                    return;
                }
            }
        }

        #endregion Methods
    }
}