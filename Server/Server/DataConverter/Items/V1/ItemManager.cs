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

namespace Server.DataConverter.Items.V1
{
    class ItemManager
    {
        public static Item LoadItem(int itemNum)
        {
            Item item = new Item();
            string[] parse = null;
            using (System.IO.StreamReader read = new System.IO.StreamReader(IO.Paths.ItemsFolder + "item" + itemNum + ".dat"))
            {
                while (!(read.EndOfStream))
                {
                    parse = read.ReadLine().Split('|');
                    switch (parse[0].ToLower())
                    {
                        case "itemdata":
                            if (parse[1].ToLower() != "v1")
                            {
                                read.Close();
                                return null;
                            }
                            break;
                        case "data":
                            item.Name = parse[1].Trim();
                            item.Desc = parse[2];
                            item.Pic = parse[3].ToInt();
                            item.Type = (Enums.ItemType)parse[4].ToInt();
                            item.Data1 = parse[5].ToInt();
                            item.Data2 = parse[6].ToInt();
                            item.Data3 = parse[7].ToInt();
                            item.StrReq = parse[8].ToInt();
                            item.DefReq = parse[9].ToInt();
                            item.SpeedReq = parse[10].ToInt();
                            item.TypeReq = parse[11].ToInt();
                            item.AccessReq = (Enums.Rank)parse[12].ToInt();
                            item.AddHP = parse[13].ToInt();
                            item.AddMP = parse[14].ToInt();
                            item.AddSP = parse[15].ToInt();
                            item.AddAtk = parse[16].ToInt();
                            item.AddDef = parse[17].ToInt();
                            item.AddSpclAtk = parse[18].ToInt();
                            item.AddSpeed = parse[19].ToInt();
                            item.AddEXP = parse[20].ToInt();
                            item.AttackSpeed = parse[21].ToInt();
                            item.Price = parse[22].ToInt();
                            item.Stackable = parse[23].ToBool();
                            item.Bound = parse[24].ToBool();
                            if (parse.Length > 25)
                            {
                                item.Loseable = parse[25].ToBool();
                            }
                            break;
                        case "recruit":
                            item.RecruitBonus = parse[1].ToInt();
                            break;
                    }
                }
            }
            return item;
        }

        public static void SaveItem(Item item, int itemNum)
        {
            string FileName = IO.Paths.ItemsFolder + "item" + itemNum + ".dat";
            using (System.IO.StreamWriter Write = new System.IO.StreamWriter(FileName))
            {
                Write.WriteLine("ItemData|V1");
                Write.WriteLine("Data" + "|" + item.Name + "|" + item.Desc + "|" + item.Pic + "|" + (int)item.Type + "|" + item.Data1 + "|" + item.Data2 + "|" + item.Data3 + "|" + item.StrReq + "|" + item.DefReq + "|" + item.SpeedReq + "|" + item.TypeReq + "|" + (int)item.AccessReq + "|" + item.AddHP + "|" + item.AddMP + "|" + item.AddSP + "|" + item.AddAtk + "|" + item.AddDef + "|" + item.AddSpclAtk + "|" + item.AddSpeed + "|" + item.AddEXP + "|" + item.AttackSpeed + "|" + item.Price + "|" + item.Stackable + "|" + item.Bound + "|");
                Write.WriteLine("recruit" + "|" + item.RecruitBonus + "|");
            }
        }
    }
}
