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
using Server.Network;
using System.Drawing;

namespace Server.Players
{
    public class Ranks
    {
        public static bool IsAllowed(Client client, Enums.Rank RankToCheck) {
            Enums.Rank PlayerRank = client.Player.Access;
            switch (RankToCheck) {
                case Enums.Rank.Normal:
                    return true;
                case Enums.Rank.Moniter:
                    if (PlayerRank == Enums.Rank.Moniter | PlayerRank == Enums.Rank.Mapper | PlayerRank == Enums.Rank.Developer | PlayerRank == Enums.Rank.Admin | PlayerRank == Enums.Rank.ServerHost | PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
                case Enums.Rank.Mapper:
                    if (PlayerRank == Enums.Rank.Mapper | PlayerRank == Enums.Rank.Developer | PlayerRank == Enums.Rank.Admin | PlayerRank == Enums.Rank.ServerHost | PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
                case Enums.Rank.Developer:
                    if (PlayerRank == Enums.Rank.Developer | PlayerRank == Enums.Rank.Admin | PlayerRank == Enums.Rank.ServerHost | PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
                case Enums.Rank.Admin:
                    if (PlayerRank == Enums.Rank.Admin | PlayerRank == Enums.Rank.ServerHost | PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
                case Enums.Rank.ServerHost:
                    if (PlayerRank == Enums.Rank.ServerHost | PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
                case Enums.Rank.Scripter:
                    if (PlayerRank == Enums.Rank.Scripter) {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public static bool IsDisallowed(Client client, Enums.Rank RankToCheck) {
            return !IsAllowed(client, RankToCheck);
        }

        public static Color GetRankColor(Enums.Rank rank) {
            switch (rank) {
                case Enums.Rank.Normal:
                    return Color.Brown;
                case Enums.Rank.Moniter:
                    return Color.FromArgb(255, 254, 150, 46);
                case Enums.Rank.Mapper:
                    return Color.Cyan;
                case Enums.Rank.Developer:
                    return Color.FromArgb(255, 0, 110, 210);
                case Enums.Rank.Admin:
                    return Color.Pink;
                case Enums.Rank.ServerHost:
                    return Color.Yellow;
                case Enums.Rank.Scripter:
                    return Color.LightCyan;
                default:
                    return Color.DarkRed;
            }
        }
    }
}
