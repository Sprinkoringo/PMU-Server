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
