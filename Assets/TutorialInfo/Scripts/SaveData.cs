
using System;
using System.Collections.Generic;

namespace ShadowHunters
{
    [Serializable]
    public class SaveData
    {
        public int coins = 0;
        public List<string> owned = new List<string>();     // card ids
        public List<string> favorites = new List<string>(); // card ids
    }
}
