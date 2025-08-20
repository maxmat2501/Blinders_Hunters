
using UnityEngine;

namespace ShadowHunters
{
    public enum CardRole { Hunter, Shadow, Neutre, Booster }
    public enum CardType { Character, Equipment, Booster }

    [CreateAssetMenu(menuName = "Shadow Hunters/Card Data", fileName = "CardData")]
    public class CardData : ScriptableObject
    {
        public string id;            // unique key (e.g., "Artcade")
        public string displayName;   // shown to players
        public CardRole role;
        public CardType type = CardType.Character;
        public int price = 100;
        public bool fullArt = false;
        public Sprite art;           // assign in Inspector (or load via Resources by id)
    }
}
