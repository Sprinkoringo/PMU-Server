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



namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class Settings
    {
        public static string GameName { get; set; }
        public static string MOTD { get; set; }
        
        public static List<string> News { get; set; }

        public static int StartMap { get; set; }
        public static int StartX { get; set; }
        public static int StartY { get; set; }
        public static int NewCharForm { get; set; }
        public static int NewCharSpecies { get; set; }
        public static int Crossroads { get; set; }

        public static int GamePort { get; set; }
        public static string DatabaseIP { get; set; }
        public static int DatabasePort { get; set; }
        public static string DatabaseUser { get; set; }
        public static string DatabasePassword { get; set; }

        public static XmlWriterSettings XmlWriterSettings { get; set; }

        public static void Initialize() {
            XmlWriterSettings = new System.Xml.XmlWriterSettings();
            XmlWriterSettings.OmitXmlDeclaration = false;
            XmlWriterSettings.IndentChars = "  ";
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.NewLineChars = Environment.NewLine;

            News = new List<string>();
        }

        public static void LoadConfig() {
            using (XmlReader reader = XmlReader.Create(IO.Paths.DataFolder + "config.xml")) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        switch (reader.Name) {
                            case "GamePort":
                                if (reader.Read()) {
                                    GamePort = reader.ReadString().ToInt();
                                }
                                break;
                            case "DatabaseIP":
                                if (reader.Read()) {
                                    DatabaseIP = reader.ReadString();
                                }
                                break;
                            case "DatabasePort":
                                if (reader.Read())
                                {
                                    DatabasePort = reader.ReadString().ToInt();
                                }
                                break;
                            case "DatabaseUser":
                                if (reader.Read())
                                {
                                    DatabaseUser = reader.ReadString();
                                }
                                break;
                            case "DatabasePassword":
                                if (reader.Read())
                                {
                                    DatabasePassword = reader.ReadString();
                                }
                                break;
                        }
                    }
                }
            }


            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                //load most recent news
                string query = "SELECT id, message " +
                    "FROM title WHERE title.id = 'GameName' OR title.id = 'MOTD'";

                foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
                {
                    switch (columnCollection["id"].ValueString)
                    {
                        case "GameName":
                            {
                                GameName = columnCollection["message"].ValueString;
                            }
                            break;
                        case "MOTD":
                            {
                                MOTD = columnCollection["message"].ValueString;
                            }
                            break;
                    }

                }

                query = "SELECT id, val " +
                   "FROM start_value "+
                   "WHERE start_value.id = 'Crossroads' "+
                   "OR start_value.id = 'NewCharForm' "+
                   "OR start_value.id = 'NewCharSpecies' "+
                   "OR start_value.id = 'StartMap' "+
                   "OR start_value.id = 'StartX' "+
                   "OR start_value.id = 'StartY'";

                foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
                {
                    switch (columnCollection["id"].ValueString)
                    {
                        case "Crossroads":
                            {
                                Crossroads = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                        case "NewCharForm":
                            {
                                NewCharForm = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                        case "NewCharSpecies":
                            {
                                NewCharSpecies = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                        case "StartMap":
                            {
                                StartMap = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                        case "StartX":
                            {
                                StartX = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                        case "StartY":
                            {
                                StartY = columnCollection["val"].ValueString.ToInt();
                            }
                            break;
                    }
                }
            }
        }

        public static void LoadNews() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                //load most recent news
                string query = "SELECT num, news_time, message " +
                    "FROM news WHERE news.num > (SELECT COUNT(num) FROM news) - 10";

                foreach (DataColumnCollection columnCollection in database.RetrieveRowsEnumerable(query))
                {
                    int num = columnCollection["num"].ValueString.ToInt();

                    News.Add("[" + columnCollection["news_time"].ValueString + "] " + columnCollection["message"].ValueString);
                }
            }
        }

        public static void SaveMOTD()
        {
            //save motd
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;

                database.UpdateOrInsert("title", new IDataColumn[] {
                    database.CreateColumn(true, "id", "MOTD"),
                    database.CreateColumn(false, "message", MOTD)
                });

            }
        }

        public static void AddNews(string newNews) {
            string date = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");

            News.Add("[" + date + "] " + newNews);

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;
                
                database.AddRow("news", new IDataColumn[] {
                    database.CreateColumn(false, "message", newNews)
                });

            }
        }
    }
}
