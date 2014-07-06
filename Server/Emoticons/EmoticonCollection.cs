namespace Server.Emoticons
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EmoticonCollection
    {
        #region Fields

        Emoticon[] emoticons;
        int maxEmoticons;

        #endregion Fields

        #region Constructors

        public EmoticonCollection(int maxEmoticons)
        {
            if (maxEmoticons == 0)
                maxEmoticons = 10;
            this.maxEmoticons = maxEmoticons;
            emoticons = new Emoticon[maxEmoticons + 1];
        }

        #endregion Constructors

        #region Properties

        public int MaxEmoticons
        {
            get { return maxEmoticons; }
        }

        #endregion Properties

        #region Indexers

        public Emoticon this[int index]
        {
            get { return emoticons[index]; }
            set { emoticons[index] = value; }
        }

        #endregion Indexers
    }
}