using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter
{
    public class ItemConverter
    {
        public static void ConvertV1ToV2(int num)
        {
            DataConverter.Items.V2.Item itemV2 = new Server.DataConverter.Items.V2.Item();

            DataConverter.Items.V1.Item itemV1 = Server.DataConverter.Items.V1.ItemManager.LoadItem(num);

            itemV2.Name = itemV1.Name;
            itemV2.Desc = itemV1.Desc;
            itemV2.Pic = itemV1.Pic;
            if ((int)itemV1.Type > 1 && (int)itemV1.Type < 8)
            {
                itemV2.Type = Enums.ItemType.Held;
            }
            else
            {
                itemV2.Type = itemV1.Type;
            }
            itemV2.Data1 = itemV1.Data1;
            itemV2.Data2 = itemV1.Data2;
            itemV2.Data3 = itemV1.Data3;
            itemV2.Price = itemV1.Price;
            itemV2.Stackable = itemV1.Stackable;
            itemV2.Bound = itemV1.Bound;
            itemV2.Loseable = itemV1.Loseable;
            itemV2.Rarity = 1;

            itemV2.AttackReq = itemV1.StrReq;
            itemV2.DefenseReq = itemV1.DefReq;
            itemV2.SpAtkReq = 0;
            itemV2.SpDefReq = 0;
            itemV2.SpeedReq = itemV1.SpeedReq;
            itemV2.ScriptedReq = -1;

            itemV2.AddHP = itemV1.AddHP;
            itemV2.AddPP = itemV1.AddMP;
            itemV2.AddAttack = itemV1.AddAtk;
            itemV2.AddDefense = itemV1.AddDef;
            itemV2.AddSpAtk = itemV1.AddSpclAtk;
            itemV2.AddSpDef = 0;
            itemV2.AddSpeed = itemV1.AddSpeed;
            itemV2.AddEXP = itemV1.AddEXP;
            if (itemV1.AttackSpeed < 1)
            {
                itemV2.AttackSpeed = 1000;
            }
            else
            {
                itemV2.AttackSpeed = itemV1.AttackSpeed;
            }
            itemV2.RecruitBonus = itemV1.RecruitBonus;


            Items.V2.ItemManager.SaveItem(itemV2, num);
        }
    }
}
