using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Items.V1
{
    public class Item
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Pic { get; set; }
        public Enums.ItemType Type { get; set; }
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int StrReq { get; set; }
        public int DefReq { get; set; }
        public int SpeedReq { get; set; }
        public int TypeReq { get; set; }
        public Enums.Rank AccessReq { get; set; }
        public int AddHP { get; set; }
        public int AddMP { get; set; }
        public int AddSP { get; set; }
        public int AddAtk { get; set; }
        public int AddDef { get; set; }
        public int AddSpclAtk { get; set; }
        public int AddSpeed { get; set; }
        public int AddEXP { get; set; }
        public int AttackSpeed { get; set; }
        public int Price { get; set; }
        public bool Stackable { get; set; }
        public bool Bound { get; set; }
        public bool Loseable { get; set; }

        public int RecruitBonus { get; set; }
    }
}
