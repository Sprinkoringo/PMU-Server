#region Header

/*
 * Created by SharpDevelop.
 * User: Pikachu
 * Date: 17/10/2009
 * Time: 11:07 PM
 *
 */

#endregion Header

namespace Server.WonderMails
{
    using System;

    /// <summary>
    /// Description of RewardItem.
    /// </summary>
    public class MissionRewardData
    {

        public int ItemNum
        {
            get;
            set;
        }

        public int Amount
        {
            get;
            set;
        }

        public string Tag
        {
            get;
            set;
        }

        public MissionRewardData() {
        }
        
    }
}