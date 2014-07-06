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


namespace Server.Stories
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.DatabaseConnector.MySql;
    using PMU.DatabaseConnector;
    using Server.Database;

    public class StoryManagerBase
    {
        #region Fields

        static StoryCollection stories;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static StoryCollection Stories {
            get { return stories; }
        }

        #endregion Properties

        #region Methods


        public static void Initialize() {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                //method for getting count
                string query = "SELECT COUNT(num) FROM story";
                DataColumnCollection row = dbConnection.Database.RetrieveRow(query);

                int count = row["COUNT(num)"].ValueString.ToInt();

                stories = new StoryCollection(count);
            }
        }

        public static void LoadStories(object object1)
        {
            try
            {

                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
                {
                    for (int i = 0; i <= stories.MaxStories; i++)
                    {
                        LoadStory(i, dbConnection.Database);
                        if (LoadUpdate != null)
                            LoadUpdate(null, new LoadingUpdateEventArgs(i, stories.MaxStories));
                    }
                    if (LoadComplete != null)
                        LoadComplete(null, null);

                }
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex);
            }
        }

        public static void LoadStory(int storyNum, MySql database)
        {
            if (stories.Stories.ContainsKey(storyNum) == false)
                stories.Stories.Add(storyNum, new Story(storyNum.ToString()));
            Story story = new Story(storyNum.ToString());

            string query = "SELECT revision, " +
                "name, " +
                "story_start " +
                "FROM story WHERE story.num = \'" + storyNum + "\'";

            DataColumnCollection row = database.RetrieveRow(query);
            if (row != null)
            {
                story.Revision = row["revision"].ValueString.ToInt();
                story.Name = row["name"].ValueString;
                story.StoryStart = row["story_start"].ValueString.ToInt();
            }

            query = "SELECT segment, " +
                "action, " +
                "checkpoint " +
                "FROM story_segment WHERE story_segment.num = \'" + storyNum + "\'";

            List<DataColumnCollection> columnCollections = database.RetrieveRows(query);
            if (columnCollections == null) columnCollections = new List<DataColumnCollection>();
            foreach (DataColumnCollection columnCollection in columnCollections)
            {

                StorySegment segment = new StorySegment();

                int segmentNum = columnCollection["segment"].ValueString.ToInt();

                segment.Action = (Enums.StoryAction)columnCollection["action"].ValueString.ToInt();
                bool isCheckpoint = columnCollection["checkpoint"].ValueString.ToBool();

                string query2 = "SELECT param_key, " +
                    "param_val " +
                    "FROM story_param WHERE story_param.num = \'" + storyNum + "\' AND story_param.segment = \'" + segmentNum + "\'";

                List<DataColumnCollection> columnCollections2 = database.RetrieveRows(query2);
                if (columnCollections2 == null) columnCollections2 = new List<DataColumnCollection>();
                foreach (DataColumnCollection columnCollection2 in columnCollections2)
                {
                    string paramKey = columnCollection2["param_key"].ValueString;
                    string paramVal = columnCollection2["param_val"].ValueString;
                    segment.Parameters.Add(paramKey, paramVal);
                }
                story.Segments.Add(segment);

                if (isCheckpoint)
                {
                    story.ExitAndContinue.Add(segmentNum);
                }
            }

            stories.Stories[storyNum] = story;
        }


        public static void SaveStory(int storyNum)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data))
            {
                MySql database = dbConnection.Database;

                database.BeginTransaction();

                database.ExecuteNonQuery("DELETE FROM story WHERE num = \'" + storyNum + "\'");
                database.ExecuteNonQuery("DELETE FROM story_segment WHERE num = \'" + storyNum + "\'");
                database.ExecuteNonQuery("DELETE FROM story_param WHERE num = \'" + storyNum + "\'");

                database.UpdateOrInsert("story", new IDataColumn[] {
                    database.CreateColumn(false, "num", storyNum.ToString()),
                    database.CreateColumn(false, "revision", stories[storyNum].Revision.ToString()),
                    database.CreateColumn(false, "name", stories[storyNum].Name),
                    database.CreateColumn(false, "story_start", stories[storyNum].StoryStart.ToString())
                });

                for (int i = 0; i < stories[storyNum].Segments.Count; i++)
                {
                    bool isCheckPoint = false;
                    for (int j = 0; j < stories[storyNum].ExitAndContinue.Count; j++)
                    {
                        if (stories[storyNum].ExitAndContinue[j] == i)
                        {
                            isCheckPoint = true;
                            break;
                        }
                    }
                    database.UpdateOrInsert("story_segment", new IDataColumn[] {
                        database.CreateColumn(false, "num", storyNum.ToString()),
                        database.CreateColumn(false, "segment", i.ToString()),
                        database.CreateColumn(false, "action", ((int)stories[storyNum].Segments[i].Action).ToString()),
                        database.CreateColumn(false, "checkpoint", isCheckPoint.ToIntString())
                    });

                    for (int j = 0; j < stories[storyNum].Segments[i].Parameters.Count; j++)
                    {
                        database.UpdateOrInsert("story_param", new IDataColumn[] {
                        database.CreateColumn(false, "num", storyNum.ToString()),
                        database.CreateColumn(false, "segment", i.ToString()),
                        database.CreateColumn(false, "param_key", stories[storyNum].Segments[i].Parameters.KeyByIndex(j)),
                        database.CreateColumn(false, "param_val", stories[storyNum].Segments[i].Parameters.ValueByIndex(j))
                    });
                    }
                }
                database.EndTransaction();
            }
        }

        #endregion Methods
    }
}