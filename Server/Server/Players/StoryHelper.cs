using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMU.Core;
using System.Xml;
using System.IO;

namespace Server.Players
{
    public class StoryHelper
    {
        Player owner;

        public StoryHelper(Player owner) {
            this.owner = owner;
        }

        private void VerifyDirectory(string directory) {
            if (Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        public void SaveSetting(string key, string value) {
            if (owner.PlayerData.StoryHelperStateSettings.ContainsKey(key) == false) {
                owner.PlayerData.StoryHelperStateSettings.Add(key, value);
            } else {
                owner.PlayerData.StoryHelperStateSettings.SetValue(key, value);
            }
        }

        public string ReadSetting(string key) {
            int index = owner.PlayerData.StoryHelperStateSettings.IndexOfKey(key);
            if (index > -1) {
                return owner.PlayerData.StoryHelperStateSettings.ValueByIndex(index);
            } else {
                return null;
            }
        }
    }
}
