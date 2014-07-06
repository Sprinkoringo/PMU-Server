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
using PMU.DatabaseConnector.MySql;
using Server.Database;

namespace Server.Pokedex {
    public class PokemonForm {

        #region Constructors

        public PokemonForm() {
            //PreferredGummy = new List<Gummy>();
            LevelUpMoves = new List<LevelUpMove>();
            TMMoves = new List<int>();
            EggMoves = new List<int>();
            TutorMoves = new List<int>();
            DWMoves = new List<int>();
            EventMoves = new List<int>();
            //Sprite = new int[2, 3];
            //Mugshot = new int[2, 3];


            //Quotes = new string[4];
        }

        #endregion Constructors

        #region Properties

        public string Ability1 { get; set; }

        public string Ability2 { get; set; }

        public string Ability3 { get; set; }

        public int BaseAtt { get; set; }

        public int BaseSpd { get; set; }

        public int BaseDef { get; set; }

        public int BaseSpAtt { get; set; }

        public int BaseSpDef { get; set; }

        public int BaseHP { get; set; }

        public int BaseRewardExp { get; set; }

        public int FormIndex { get; set; }

        public List<LevelUpMove> LevelUpMoves { get; set; }

        public string FormName { get; set; }

        //public string[] Quotes { get; set; }

        //public double RecruitRate { get; set; }


        public List<int> TMMoves { get; set; }

        public List<int> EggMoves { get; set; }

        public List<int> TutorMoves { get; set; }

        public List<int> DWMoves { get; set; }

        public List<int> EventMoves { get; set; }

        public Enums.PokemonType Type1 { get; set; }

        public Enums.PokemonType Type2 { get; set; }

        //public int[,] Sprite { get; set; }

        //public int[,] Mugshot { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public int MaleRatio { get; set; }

        public int FemaleRatio { get; set; }


        #endregion Properties

        #region Methods


        /// <summary>
        /// Returns the level that this Pokémon learns a certain move.
        /// </summary>
        /// <param name="moveToFind">The move to look for.</param>
        /// <returns></returns>
        public int FindMoveLevel(int moveToFind) {
            for (int i = 0; i < LevelUpMoves.Count; i++) {
                if (LevelUpMoves[i].Move == moveToFind) {
                    return LevelUpMoves[i].Level;
                }
            }
            return -1;
        }

        public int FindLevelMove(int levelLearned) {
            for (int i = 0; i < LevelUpMoves.Count; i++)
            {
                if (LevelUpMoves[i].Level == levelLearned)
                {
                    return LevelUpMoves[i].Move;
                }
            }
            return -1;
        }

        public bool CanRelearnLevelUpMove(int moveToTest, int level) {
            int moveLevel = FindMoveLevel(moveToTest);
            if (moveLevel > -1 && moveLevel <= level) {
                return true;
            } else {
                return false;
            }
        }

        
        public int GetMaxHP(int level)
        {
        	if (BaseHP > 1) {
        		//return BaseHP*(level + 6)/50 + level *3/2 + 6;
                return (BaseHP + 100) * (level + 1) / 70 + 10;
        	} else {
        		return (level / 5 + 1);
        	}
        }

        public int GetMaxHPLimit() {
            if (BaseHP > 1) {
            int scaledStat = 1530 * BaseHP / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                return 41;
            }
        }

        public int GetAtt(int level) {
            //return ((((BaseAtt / 2) + 45) * level / 100) + 5);
            return (BaseAtt + 100) * (level + 1) / 70 + 10;
        }

        public int GetAttLimit() {
            if (BaseHP > 1) {
                int scaledStat = 1530 * BaseAtt / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                int scaledStat = 1325 * BaseAtt / (BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            }
        }

        public int GetDef(int level) {
            //return (((BaseDef / 2) + 20) * level / 100) + 3;
            return (BaseDef + 100) * (level + 1) / 70 + 10;
        }

        public int GetDefLimit() {
            if (BaseHP > 1) {
                int scaledStat = 1530 * BaseDef / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                int scaledStat = 1325 * BaseDef / (BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            }
        }

        public int GetSpAtt(int level)
        {
            //return ((((BaseSpAtt / 2) + 45) * level / 100) + 5);
            return (BaseSpAtt + 100) * (level + 1) / 70 + 10;
        }

        public int GetSpAttLimit() {
            if (BaseHP > 1) {
                int scaledStat = 1530 * BaseSpAtt / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                int scaledStat = 1325 * BaseSpAtt / (BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            }
        }

        public int GetSpDef(int level)
        {
            //return (((BaseSpDef / 2) + 20) * level / 100) + 3;
            return (BaseSpDef + 100) * (level + 1) / 70 + 10;
        }

        public int GetSpDefLimit() {
            if (BaseHP > 1) {
                int scaledStat = 1530 * BaseSpDef / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                int scaledStat = 1325 * BaseSpDef / (BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            }
        }

        public int GetSpd(int level)
        {
            //return (((BaseSpd / 2 + 40) * level / 100) + 5);
            return (BaseSpd + 100) * (level + 1) / 70 + 10;
        }

        public int GetSpdLimit() {
            if (BaseHP > 1) {
                int scaledStat = 1530 * BaseSpd / (BaseHP + BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            } else {
                int scaledStat = 1325 * BaseSpd / (BaseAtt + BaseDef + BaseSpAtt + BaseSpDef + BaseSpd);
                return (scaledStat + 100) * (100 + 1) / 70 + 10;
            }
        }

        public int GetRewardExp(int level) {
            return (((BaseRewardExp * 3 / 5) * (level - 1) / 10) + (BaseRewardExp * 3 / 5)) * 1;
        }

        public bool HasLevelUpMove(int level) {
            for (int i = 0; i < LevelUpMoves.Count; i++) {
                if (LevelUpMoves[i].Level == level) {
                    return true;
                }
            }
            return false;
        }

        public Enums.Sex GenerateLegalSex() {
            if (MaleRatio + FemaleRatio <= 0) {
                return Enums.Sex.Genderless;
            } else if (Server.Math.Rand(0, MaleRatio + FemaleRatio) < MaleRatio) {
                return Enums.Sex.Male;
            } else {
                return Enums.Sex.Female;
            }

        }

        public void LoadAppearance(DatabaseConnection dbConnection, int ID, int formNum) {
            //MySql database = dbConnection.Database;

            
            //for (int i = 0; i < 3; i++) {
                
            //    string query = "SELECT pokedex_pokemonappearance.Sprite, pokedex_pokemonappearance.Mugshot " +
            //    "FROM pokedex_pokemonappearance " +
            //    "WHERE pokedex_pokemonappearance.DexNum = \'" + ID + "\' " +
            //    "AND pokedex_pokemonappearance.FormNum = \'" + formNum + "\' " +
            //    "AND pokedex_pokemonappearance.Shiny = \'0\' " +
            //    "AND pokedex_pokemonappearance.Gender = \'" + i + "\'";

            //    DataColumnCollection row = database.RetrieveRow(query);
            //    if (row != null)
            //    {
            //        Sprite[0, i] = row["Sprite"].ValueString.ToInt();
            //        Mugshot[0, i] = row["Mugshot"].ValueString.ToInt();
            //    }

            //    query = "SELECT pokedex_pokemonappearance.Sprite, pokedex_pokemonappearance.Mugshot " +
            //    "FROM pokedex_pokemonappearance " +
            //    "WHERE pokedex_pokemonappearance.DexNum = \'" + ID + "\' " +
            //    "AND pokedex_pokemonappearance.FormNum = \'" + formNum + "\' " +
            //    "AND pokedex_pokemonappearance.Shiny = \'1\' " +
            //    "AND pokedex_pokemonappearance.Gender = \'" + i + "\'";

            //    row = database.RetrieveRow(query);
            //    if (row != null)
            //    {
            //        Sprite[1, i] = row["Sprite"].ValueString.ToInt();
            //        Mugshot[1, i] = row["Mugshot"].ValueString.ToInt();
            //    }
            //}
        }


        public void LoadMoves(DatabaseConnection dbConnection, int ID, int formNum)
        {
            MySql database = dbConnection.Database;

            string query = "SELECT pokedex_pokemonlevelmove.LevelNum, pokedex_pokemonlevelmove.Move " +
                "FROM pokedex_pokemonlevelmove " +
                "WHERE pokedex_pokemonlevelmove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemonlevelmove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemonlevelmove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                int levelNum = column["LevelNum"].ValueString.ToInt();
                int moveNum = column["Move"].ValueString.ToInt();
                LevelUpMove move = new LevelUpMove(moveNum, levelNum);
                LevelUpMoves.Add(move);
            }

            query = "SELECT pokedex_pokemontmmove.Move " +
                "FROM pokedex_pokemontmmove " +
                "WHERE pokedex_pokemontmmove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemontmmove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemontmmove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                TMMoves.Add(column["Move"].ValueString.ToInt());
            }

            query = "SELECT pokedex_pokemoneggmove.Move " +
                "FROM pokedex_pokemoneggmove " +
                "WHERE pokedex_pokemoneggmove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemoneggmove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemoneggmove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                EggMoves.Add(column["Move"].ValueString.ToInt());
            }

            query = "SELECT pokedex_pokemontutormove.Move " +
                "FROM pokedex_pokemontutormove " +
                "WHERE pokedex_pokemontutormove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemontutormove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemontutormove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                TutorMoves.Add(column["Move"].ValueString.ToInt());
            }

            query = "SELECT pokedex_pokemondwmove.Move " +
                "FROM pokedex_pokemondwmove " +
                "WHERE pokedex_pokemondwmove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemondwmove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemondwmove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                DWMoves.Add(column["Move"].ValueString.ToInt());
            }

            query = "SELECT pokedex_pokemoneventmove.Move " +
                "FROM pokedex_pokemoneventmove " +
                "WHERE pokedex_pokemoneventmove.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemoneventmove.FormNum = \'" + formNum + "\' " +
                "ORDER BY pokedex_pokemoneventmove.MoveIndex";

            foreach (DataColumnCollection column in database.RetrieveRowsEnumerable(query))
            {
                EventMoves.Add(column["Move"].ValueString.ToInt());
            }
        }

        #endregion Methods
    }
}
