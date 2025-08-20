
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShadowHunters
{
    public class InventoryUI : MonoBehaviour
    {
        public Transform gridRoot;
        public GameObject cardItemPrefab;

        [Header("Filters")]
        public TMP_Dropdown roleDropdown; // Any, Hunter, Shadow, Neutre, Favorites
        public TMP_InputField searchInput;

        List<CardData> allCards = new List<CardData>();

        void Start()
        {
            // Load all CardData from Resources
            allCards.AddRange(Resources.LoadAll<CardData>("CardData"));
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnDataChanged += Rebuild;
            Rebuild();
        }

        void OnDestroy()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnDataChanged -= Rebuild;
        }

        public void Rebuild()
        {
            foreach (Transform child in gridRoot) Destroy(child.gameObject);

            string search = (searchInput ? searchInput.text : "").ToLowerInvariant();
            int roleFilter = roleDropdown ? roleDropdown.value : 0; // map in UI

            foreach (var card in allCards)
            {
                bool owned = InventoryManager.Instance.Owns(card.id);
                if (!owned) continue;

                if (!string.IsNullOrEmpty(search) && !card.displayName.ToLowerInvariant().Contains(search))
                    continue;

                if (roleFilter > 0)
                {
                    var wanted = (CardRole)(roleFilter - 1); // 1..3 map to enum 0..2
                    if (roleFilter == 4) // Favorites special value
                    {
                        if (!InventoryManager.Instance.IsFavorite(card.id)) continue;
                    }
                    else if (card.role != wanted) continue;
                }

                var go = Instantiate(cardItemPrefab, gridRoot);
                var view = go.GetComponent<CardItemView>();
                view.Bind(card, false, owned);

                // Add toggle favorite on click
                var btn = view.buyButton;
                if (btn)
                {
                    btn.GetComponentInChildren<TMP_Text>().text = InventoryManager.Instance.IsFavorite(card.id) ? "★ Fav" : "☆ Fav";
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        InventoryManager.Instance.ToggleFavorite(card.id);
                        Rebuild();
                    });
                }
            }
        }
    }
}
