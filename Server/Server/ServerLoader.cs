using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Server.Network;

namespace Server
{
    public class ServerLoader
    {
        static bool loadingComplete = false;
        static ManualResetEvent resetEvent;
        public static event EventHandler LoadComplete;

        public static void LoadServer() {
            Globals.CommandLine = CommandProcessor.ParseCommand(Environment.CommandLine); 
            string startFolder;
            int overridePath = Globals.CommandLine.FindCommandArg("-overridepath");
            if (overridePath > -1) {
                startFolder = Globals.CommandLine[overridePath + 1];
            } else {
                startFolder = System.Windows.Forms.Application.StartupPath;
            }

            Globals.LiveTime = new System.Diagnostics.Stopwatch();
            Globals.LiveTime.Start();

            resetEvent = new ManualResetEvent(false);
            IO.IO.Initialize(startFolder);
            Settings.Initialize();
            Settings.LoadConfig();
            Settings.LoadNews();
            Players.PlayerID.Initialize();
            Players.PlayerID.LoadIDInfo();

            Forms.LoadingUI loading = new Forms.LoadingUI();
            loading.Text = "Loading...";

            loading.Show();

            Thread t = new Thread(new ParameterizedThreadStart(LoadServerBackground));
            t.Start(loading);
        }

        private static void LoadServerBackground(Object param) {
            Forms.LoadingUI loading = param as Forms.LoadingUI;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDatasBackground));
            t.Name = "Data Load Thread";
            t.Start(loading);

            resetEvent.WaitOne();

            Globals.MainUI = new Forms.MainUI();

            loading.UpdateStatus("Initializing TCP...");

            NetworkManager.Initialize();
            NetworkManager.TcpListener.Listen(System.Net.IPAddress.Any, Settings.GamePort);

            loading.Close(true);

            if (LoadComplete != null)
                LoadComplete(null, EventArgs.Empty);
        }

        private static void LoadDatasBackground(object data) {
            Forms.LoadingUI loading = data as Forms.LoadingUI;

            //ThreadManager.SetMaxThreads(Settings.MaxWorkerThreads, Settings.MaxCompletionThreads);
            // Load pokedex
            Pokedex.Pokedex.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Pokedex... " + e.Percent.ToString() + "%"); };
            Pokedex.Pokedex.Initialize();
            Pokedex.Pokedex.LoadAllPokemon();
            // Load emoticons
            //Emoticons.EmoticonManagerBase.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Emoticons... " + e.Percent.ToString() + "%"); };
            //Emoticons.EmoticonManagerBase.Initialize(Settings.MaxEmoticons);
            //Emoticons.EmoticonManagerBase.LoadEmotions(null);
            // Load exp
            Exp.ExpManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Experience values... " + e.Percent.ToString() + "%"); };
            Exp.ExpManager.Initialize();
            Exp.ExpManager.LoadExps(null);
            // Load maps
            Maps.MapManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Maps... " + e.Percent.ToString() + "%"); };
            Maps.MapManager.Initialize();
            //Maps.MapManager.LoadMaps(null);
            // Load items
            Items.ItemManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Items... " + e.Percent.ToString() + "%"); };
            Items.ItemManager.Initialize();
            Items.ItemManager.LoadItems(null);
            // Load npcs
            Npcs.NpcManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Npcs... " + e.Percent.ToString() + "%"); };
            Npcs.NpcManager.Initialize();
            Npcs.NpcManager.LoadNpcs(null);
            // Load shops
            Shops.ShopManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Shops... " + e.Percent.ToString() + "%"); };
            Shops.ShopManager.Initialize();
            Shops.ShopManager.LoadShops(null);
            // Load moves
            Moves.MoveManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Moves... " + e.Percent.ToString() + "%"); };
            Moves.MoveManager.Initialize();
            Moves.MoveManager.LoadMoves(null);
            // Load evos
            Evolutions.EvolutionManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Evolutions... " + e.Percent.ToString() + "%"); };
            Evolutions.EvolutionManager.Initialize();
            Evolutions.EvolutionManager.LoadEvos(null);
            // Load stories
            Stories.StoryManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Stories... " + e.Percent.ToString() + "%"); };
            Stories.StoryManager.Initialize();
            Stories.StoryManager.LoadStories(null);
            // Load rdungeons
            RDungeons.RDungeonManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading RDungeons... " + e.Percent.ToString() + "%"); };
            RDungeons.RDungeonManager.Initialize();
            RDungeons.RDungeonManager.LoadRDungeons(null);
            // Load dungeons
            Dungeons.DungeonManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading Dungeons... " + e.Percent.ToString() + "%"); };
            Dungeons.DungeonManager.Initialize();
            Dungeons.DungeonManager.LoadDungeons(null);
            //Load wonder mail
            WonderMails.WonderMailManager.LoadUpdate += delegate(System.Object o, Server.LoadingUpdateEventArgs e) { loading.UpdateStatus("Loading WonderMail... " + e.Percent.ToString() + "%"); };
            WonderMails.WonderMailManager.Initialize();
            WonderMails.WonderMailManager.LoadMissionPools(null);

            //WonderMails.WonderMailSystem.Initialize();

            // Initialize the tournament subsystem
            Tournaments.TournamentManager.Initialize();

            // Initialize the party subsystem
            Players.Parties.PartyManager.Initialize();

            // Initialize statistics
            Statistics.PacketStatistics.Initialize();
            // Initialize logs
            Logging.Logger.Initialize();

            // Load scripts
            loading.UpdateStatus("Loading scripts... ");
            Scripting.ScriptManager.Initialize();
            Scripting.ScriptManager.CompileScript(IO.Paths.ScriptsFolder, true);
            loading.UpdateStatus("Loading script editor data...");
            Scripting.Editor.EditorHelper.Initialize();
            Scripting.ScriptManager.InvokeSub("ServerInit");

            // Finalizing load
            loading.UpdateStatus("Starting game loop...");
            AI.AIProcessor.InitGameLoop();

            resetEvent.Set();
            loadingComplete = true;
        }
    }
}
