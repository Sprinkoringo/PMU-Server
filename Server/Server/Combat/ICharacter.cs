using System;
using System.Collections.Generic;
using System.Text;
using Server.Players;
using Server.Maps;
using Server.Moves;

namespace Server.Combat {
    public interface ICharacter {
        Enums.CharacterType CharacterType { get; }
        Enums.Sex Sex { get; set; }
        Enums.Coloration Shiny { get; set; }
        int Species { get; }
        int Form { get; set; }
        int Sprite { get; set; }

        //PokemonData BaseSpecies { get; set; }
        //PokemonData Species { get; }
        //PokemonData Sprite { get; set; }

        Enums.PokemonType Type1 { get; set; }
        Enums.PokemonType Type2 { get; set; }
        Enums.StatusAilment StatusAilment { get; set; }
        int StatusAilmentCounter { get; set; }
        bool Confused { get; set; }
        ExtraStatusCollection VolatileStatus { get; set; }
        

        string Ability1 { get; set; }
        string Ability2 { get; set; }
        string Ability3 { get; set; }

        Enums.Speed SpeedLimit { get; set; }
        bool[] Mobility { get; set; }
        int TimeMultiplier { get; set; }

        int HP { get; set; }
        int HPRemainder { get; set; }
        InventoryItem HeldItem { get; set; }
        List<int> ActiveItems { get; set; }

        RecruitMove[] Moves { get; set; }

        TickCount AttackTimer { get; set; }
        TickCount PauseTimer { get; set; }

        //stat boosts from vitamins
        int MaxHPBonus { get; set; }
        int AtkBonus { get; set; }
        int DefBonus { get; set; }
        int SpclAtkBonus { get; set; }
        int SpclDefBonus { get; set; }
        int SpdBonus { get; set; }

        //final stats (without buffing)
        int MaxHP { get; }
        int Atk { get; }
        int Def { get; }
        int SpclAtk { get; }
        int SpclDef { get; }
        int Spd { get; }

        //stat buffs
        int AttackBuff { get; set; }
        int DefenseBuff { get; set; }
        int SpAtkBuff { get; set; }
        int SpDefBuff { get; set; }
        int SpeedBuff { get; set; }
        int AccuracyBuff { get; set; }
        int EvasionBuff { get; set; }

        int Level { get; set; }
        string MapID { get; }
        string Name { get; }
        int X { get; set; }
        int Y { get; set; }
        Enums.Direction Direction { get; set; }
        int Darkness { get; set; }

        void CalculateOriginalSpecies();
        void CalculateOriginalSprite();
        void CalculateOriginalStats();
        void CalculateOriginalType();
        void CalculateOriginalAbility();

        //void UseMoveOnSelf(int moveNum);
        void UseMoveOnAllies(BattleSetup setup, TargetCollection targets);
        void UseMoveOnFoes(BattleSetup setup, TargetCollection targets);
        bool HasActiveItem(int itemNum);
        void AddToActiveItemList(int itemNum);
        void RemoveFromActiveItemList(int itemNum);
        void RefreshActiveItemList();
        void LoadActiveItemList();
        bool MeetsReqs(int itemNum);
        void GiveHeldItem(int num, int val, string tag, bool sticky);
        void TakeHeldItem(int val);
        void UseHeldItem();
        void MapGetItem();
        void MapDropItem(int val, Server.Network.Client playerFor);
        void ThrowHeldItem();
        //Enums.CharacterMatchup GetMatchupWith(ICharacter character);
        //bool IsInRangeOfAllies(IMap currentMap, Move move, int moveSlot);
        //bool IsInRangeOfFoes(IMap currentMap, Move move, int moveSlot);

    }
}
