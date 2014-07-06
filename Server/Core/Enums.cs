using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
    public class Enums {
        public enum MapID {
            Active = 0,
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            TopLeft = 5,
            BottomLeft = 6,
            TopRight = 7,
            BottomRight = 8,
            TempActive = 9,
            TempUp = 10,
            TempDown = 11,
            TempLeft = 12,
            TempRight = 13,
            TempTopLeft = 14,
            TempBottomLeft = 15,
            TempTopRight = 16,
            TempBottomRight = 17
        }

        public enum JobDifficulty {
            E = 1,
            D = 2,
            C = 3,
            B = 4,
            A = 5,
            S = 6,
            Star = 7,
            TwoStar = 8,
            ThreeStar = 9,
            FourStar = 10,
            FiveStar = 11,
            SixStar = 12,
            SevenStar = 13,
            EightStar = 14,
            NineStar = 15
        }

        public enum JobStatus {
            Obtained = 0,
            Taken = 1,
            Suspended = 2,
            Finished = 3,
            Failed = 4
        }

        public enum ChatMessageType {
            Global,
            Map,
            MeMsg,
            Announcement,
            Staff,
            Guild,
            PM
        }

        public enum OutdateType {
            None,
            Location,
            Condition
        }

        public enum ExpKitModules {
            Debug,
            Chat,
            Counter,
            Party,
            FriendsList,
            MapReport
        }

        public enum MapMoral {
            None = 0,
            Safe = 1,
            NoPenalty = 2,
            House = 3,
        }

        public enum MapType {
            Standard,
            Instanced,
            House,
            RDungeonMap,
            Void
        }

        public enum MoveRange {
            FrontOfUser = 0,
            FrontAndSides = 1,
            ArcThrow = 2,
            Room = 3,
            LineUntilHit = 4,
            StraightLine = 5,
            Floor = 6,
            User = 7,
            FrontOfUserUntil = 8,
            Special = 9
        }



        public enum CharacterMatchup {
            None,
            Self,
            Friend,
            Foe
        }

        public enum MoveTarget   //S,A,F
        {
            Foes = 0,            //N,N,Y
            UserAndAllies = 1,   //Y,Y,N
            All = 2,             //Y,Y,Y
            User = 3,            //Y,N,N
            UserAndFoe = 4,      //Y,N,Y
            AllButUser = 5,      //N,Y,Y
            AllAlliesButUser = 6,//N,Y,N
            NoOne = 7            //N,N,N
        }

        public enum MoveCategory {
            Physical = 0,
            Special = 1,
            Status = 2
        }

        public enum NpcBehavior {
            AttackOnSight = 0,
            AttackWhenAttacked = 1,
            Friendly = 2,
            Shopkeeper = 3,
            Guard = 4,
            Scripted = 5,
            FullyScriptedAI = 6,
            FleeOnsight = 7
        }

        public enum Rank {
            Normal = 0,
            Moniter = 1,
            Mapper = 2,
            Developer = 3,
            Admin = 4,
            ServerHost = 5,
            Scripter = 6
        }

        public enum GuildRank {
            None = 0,
            Member = 1,
            Admin = 2,
            Founder = 3
        }

        public enum ExplorerRank {
            Normal = 0,
            Bronze = 1,
            Silver = 2,
            Gold = 3,
            Diamond = 4,
            Super = 5,
            Ultra = 6,
            Hyper = 7,
            Master = 8,
            MasterX = 9,
            MasterXX = 10,
            MasterXXX = 11,
            Guildmaster = 12
        }

        public enum MissionType {
            Rescue,
            ItemRetrieval,
            Escort,
            Outlaw
        }

        public enum StoryAction {
            Say = 0,
            Pause = 1,
            Padlock = 2,
            MapVisibility = 3,
            PlayMusic = 4,
            StopMusic = 5,
            ShowImage = 6,
            HideImage = 7,
            Warp = 8,
            PlayerPadlock = 9,
            ShowBackground = 10,
            HideBackground = 11,
            CreateFNPC = 12,
            MoveFNPC = 13,
            WarpFNPC = 14,
            ChangeFNPCDir = 15,
            DeleteFNPC = 16,
            RunScript = 17,
            HidePlayers = 18,
            ShowPlayers = 19,
            FNPCEmotion = 20,
            ChangeWeather = 21,
            HideNPCs = 22,
            ShowNPCs = 23,
            WaitForMap = 24,
            WaitForLoc = 25,
            AskQuestion = 26,
            GoToSegment = 27,
            ScrollCamera = 28,
            ResetCamera = 29,
            AddTriggerEvent = 30,
            MovePlayer = 31,
            ChangePlayerDir = 32
        }

        public enum ItemType {
            None = 0,
            Held = 1,
            HeldByParty = 2,
            HeldInBag = 3,
            [Obsolete()]
            Shield = 4,
            [Obsolete()]
            Legs = 5,
            [Obsolete()]
            Ring = 6,
            [Obsolete()]
            Necklace = 7,
            PotionAddHP = 8,
            PotionAddMP = 9,
            PotionAddBelly = 10,
            [Obsolete()]
            PotionSubHP = 11,
            [Obsolete()]
            PotionSubMP = 12,
            [Obsolete()]
            PotionSubBelly = 13,
            Key = 14,
            Currency = 15,
            TM = 16,
            Scripted = 17
        }

        public enum MoveType {
            AddHP = 0,
            AddMP = 1,
            SubHP = 3,
            SubMP = 4,
            //GiveItem=6,
            Scripted = 6,
        }

        public enum MoveAnimationType {
            Normal,
            Arrow,
            Throw,
            Beam,
            Overlay,
            Tile,
            ItemArrow,
            ItemThrow
        }

        public enum CharacterType {
            Recruit,
            MapNpc
        }

        public enum PokedexStat {
            Unknown,
            Defeated,
            Joined
        }

        public enum Weather {
            Ambiguous = 0,//for server weather.  counts as "none" in every other case.
            None = 1,
            Raining = 2,
            Snowing = 3,
            Thunder = 4,
            Hail = 5,
            DiamondDust = 6,
            Cloudy = 7,
            Fog = 8,
            Sunny = 9,
            Sandstorm = 10,
            Snowstorm = 11,
            Ashfall = 12
        }

        public enum Overlay {
            None = 0,
            Night
        }

        public enum TileType {
            Walkable = 0,
            Blocked = 1,
            Warp = 2,
            Item = 3,
            NPCAvoid = 4,
            Key = 5,
            KeyOpen = 6,
            Heal = 7,
            Kill = 8,
            Shop = 9,
            MobileBlock = 10,
            Arena = 11,
            Sound = 12,
            SpriteChange = 13,
            Sign = 14,
            Door = 15,
            Notice = 16,
            Chest = 17,
            LinkShop = 18,
            Scripted = 19,
            NpcSpawn = 20,
            House = 21,
            Bank = 22,
            Guild = 23,
            SpriteBlock = 24,
            LevelBlock = 25,
            Assembly = 26,
            Evolution = 27,
            Story = 28,
            MissionBoard = 29,
            RDungeonGoal = 30,
            ScriptedSign = 31,
            SpeciesChange = 32,
            Hallway = 33,
            HouseRoomWarp = 34,
            HouseOwnerBlock = 35,
            Ambiguous = 36,
            Slippery = 37,
            Slow = 38,
            DropShop = 39
        }

        public enum LoadingType {
            Arrows,
            Emotions,
            Exp,
            Classes,
            Maps,
            Items,
            Npcs,
            Shops,
            Moves,
            Evos,
            Stories,
            Missions,
            RDungeons,
            Dungeon
        }

        public enum PlainMsgType {
            NewAccount = 1,
            AccountOptions = 2,
            MainMenu = 3,
            NewChar = 4,
            Chars = 5,
        }

        public enum Sex {
            Genderless = 0,
            Male = 1,
            Female = 2
        }

        public enum BanType {
            None = 0,
            Mute = 1,
            Ban = 2
        }

        public enum Coloration {
            Normal,
            Shiny
        }

        public enum Direction {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }

        public enum Speed {
            Standing = 0,
            SuperSlow = 1,
            Slow = 2,
            Walking = 3,
            Running = 4,
            Fast = 5,
            SuperFast = 6,
            Slip = 7

        }

        public enum Time {
            Day = 0,
            Night = 1,
            Dawn = 2,
            Dusk = 3
        }

        public enum TargetType {
            Player = 0,
            Npc = 1
        }

        public enum Priority {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public enum Acceptance {
            Never,
            Maybe,
            Always
        }

        public enum TicketCategory {
            Support = 0,
            Request = 1,
            Bug = 2
        }

        public enum TicketStatus {
            Open = 0,
            Closed = 1
        }

        public enum Emotion {
            Excited,
            Fist,
            TearDrop,
            LoadTalking,
            Confused,
            Nervous,
            Angry,
            Shocked,
            TripleDots,
            Exclamation,
            Skull
        }

        public enum KillType {
            Player,
            Tile,
            Npc,
            Other
        }

        public enum WonderMailObtainType {
            Generator = 0,
            Dungeon,
            NPC,
            Friend,
            Manual,
            Event,
            Story,
            Premade
        }

        public enum RFloorGoalType {
            NextFloor,
            Map,
            Scripted
        }

        public enum PokemonType {
            None,
            Bug,
            Dark,
            Dragon,
            Electric,
            Fairy,
            Fighting,
            Fire,
            Flying,
            Ghost,
            Grass,
            Ground,
            Ice,
            Normal,
            Poison,
            Psychic,
            Rock,
            Steel,
            Water
        }

        public enum GrowthGroup {
            None = 0,
            Erratic = 1,
            Fast = 2,
            MediumFast = 3,
            MediumSlow = 4,
            Slow = 5,
            Fluctuating = 6
        }

        public enum StatusAilment {
            OK,
            Burn,
            Freeze,
            Paralyze,
            Poison,
            Sleep

        }
    }
}
