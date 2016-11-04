﻿using System;

namespace EA4S.Db
{

    [Serializable]
    public class MiniGameData : IData
    {
        // Source
        public string Title_En;
        public MiniGameCode Code;
        public bool Available;
        public MiniGameDataType Type;
        public string Main;
        public string Variation;
        public string Description;
        public string IntroArabic;
        public string Title_Ar;
        public string Scene;

        public string GetId()
        {
            return Code.ToString();
        }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, type={4}, available={1},  title_en={2}, title_ar={3}]", GetId(), Available, Title_En, Title_Ar, Type.ToString());
        }

        public string GetIconResourcePath()
        {
            return "Images/GameIcons/minigame_Ico_" + Main;
        }

    }
}