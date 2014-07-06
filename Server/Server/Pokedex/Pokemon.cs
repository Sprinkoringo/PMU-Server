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


namespace Server.Pokedex
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using PMU.DatabaseConnector.MySql;
    using Server.Database;
    
    public class Pokemon
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public Enums.GrowthGroup GrowthGroup { get; set; }

        public string EggGroup1 { get; set; }

        public string EggGroup2 { get; set; }

        public string SpeciesName { get; set; }

        public List<PokemonForm> Forms { get; set; }

        public Pokemon() {
            Forms = new List<PokemonForm>();
        }

        /*
        public void Load() {
            int formNum = 0;
            while (IO.IO.FileExists(IO.Paths.PokedexFolder + ID + "-" + formNum + ".xml")) {
                PokemonForm form = new PokemonForm();
                form.FormIndex = formNum;
                form.Load(ID);
                Forms.Add(form);
                formNum++;
            }
        }

        
        public void Save() {
            foreach(PokemonForm form in Forms) {
                form.Save(ID);
            }
        }
        */

        public void Load(DatabaseConnection dbConnection) {
            MySql database = dbConnection.Database;

            string query = "SELECT pokedex_pokemon.PokemonName, pokedex_pokemon.SpeciesName, " +
                "pokedex_pokemon.GrowthGroup, pokedex_pokemon.EggGroup1, pokedex_pokemon.EggGroup2 " +
                "FROM pokedex_pokemon " +
                "WHERE pokedex_pokemon.DexNum = \'" + ID + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                Name = row["PokemonName"].ValueString;
                SpeciesName = row["SpeciesName"].ValueString;
                GrowthGroup = (Enums.GrowthGroup)row["GrowthGroup"].ValueString.ToInt();
                EggGroup1 = row["EggGroup1"].ValueString;
                EggGroup2 = row["EggGroup2"].ValueString;
            }


            int formNum = 0;
            query = "SELECT pokedex_pokemonform.FormName, pokedex_pokemonform.HP, " +
                "pokedex_pokemonform.Attack, pokedex_pokemonform.Defense, " +
                "pokedex_pokemonform.SpecialAttack, pokedex_pokemonform.SpecialDefense, " +
                "pokedex_pokemonform.Speed, pokedex_pokemonform.Male, pokedex_pokemonform.Female, " +
                "pokedex_pokemonform.Height, pokedex_pokemonform.Weight, " +
                "pokedex_pokemonform.Type1, pokedex_pokemonform.Type2, " +
                "pokedex_pokemonform.Ability1, pokedex_pokemonform.Ability2, pokedex_pokemonform.Ability3, " +
                "pokedex_pokemonform.ExpYield " +
                "FROM pokedex_pokemonform " +
                "WHERE pokedex_pokemonform.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemonform.FormNum = \'" + formNum + "\'";

            row = database.RetrieveRow(query);
            while (row != null)
            {
                PokemonForm form = new PokemonForm();
                form.FormName = row["FormName"].ValueString;
                form.BaseHP = row["HP"].ValueString.ToInt();
                form.BaseAtt = row["Attack"].ValueString.ToInt();
                form.BaseDef = row["Defense"].ValueString.ToInt();
                form.BaseSpAtt = row["SpecialAttack"].ValueString.ToInt();
                form.BaseSpDef = row["SpecialDefense"].ValueString.ToInt();
                form.BaseSpd = row["Speed"].ValueString.ToInt();
                form.MaleRatio = row["Male"].ValueString.ToInt();
                form.FemaleRatio = row["Female"].ValueString.ToInt();
                form.Height = row["Height"].ValueString.ToDbl();
                form.Weight = row["Weight"].ValueString.ToDbl();
                form.Type1 = (Enums.PokemonType)row["Type1"].ValueString.ToInt();
                form.Type2 = (Enums.PokemonType)row["Type2"].ValueString.ToInt();
                form.Ability1 = row["Ability1"].ValueString;
                form.Ability2 = row["Ability2"].ValueString;
                form.Ability3 = row["Ability3"].ValueString;
                form.BaseRewardExp = row["ExpYield"].ValueString.ToInt();

                form.LoadAppearance(dbConnection, ID, formNum);
                form.LoadMoves(dbConnection, ID, formNum);
                Forms.Add(form);

                formNum++;
                query = "SELECT pokedex_pokemonform.FormName, pokedex_pokemonform.HP, " +
                "pokedex_pokemonform.Attack, pokedex_pokemonform.Defense, " +
                "pokedex_pokemonform.SpecialAttack, pokedex_pokemonform.SpecialDefense, " +
                "pokedex_pokemonform.Speed, pokedex_pokemonform.Male, pokedex_pokemonform.Female, " +
                "pokedex_pokemonform.Height, pokedex_pokemonform.Weight, " +
                "pokedex_pokemonform.Type1, pokedex_pokemonform.Type2, " +
                "pokedex_pokemonform.Ability1, pokedex_pokemonform.Ability2, pokedex_pokemonform.Ability3, " +
                "pokedex_pokemonform.ExpYield " +
                "FROM pokedex_pokemonform " +
                "WHERE pokedex_pokemonform.DexNum = \'" + ID + "\' " +
                "AND pokedex_pokemonform.FormNum = \'" + formNum + "\'";
                row = database.RetrieveRow(query);
            }
        }

    }
}