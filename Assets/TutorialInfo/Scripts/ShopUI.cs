
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShadowHunters
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Catalogue")]
        public List<CardData> catalogue = new List<CardData>(); // assign in Inspector or load from Resources
        public Transform gridRoot;
        public GameObject cardItemPrefab;

        [Header("UI")]
        public TMP_Text coinsText;
        public TMP_Text toastText;

        void OnEnable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnDataChanged += RefreshCoins;
            BuildGrid();
            RefreshCoins();
        }

        void OnDisable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnDataChanged -= RefreshCoins;
        }

        void RefreshCoins()
        {
            if (coinsText)
                coinsText.text = $"Coins: {InventoryManager.Instance.Data.coins}";
        }

        void BuildGrid()
        {
            foreach (Transform child in gridRoot) Destroy(child.gameObject);

            foreach (var card in catalogue)
            {
                var go = Instantiate(cardItemPrefab, gridRoot);
                var view = go.GetComponent<CardItemView>();
                var owned = InventoryManager.Instance.Owns(card.id);
                bool showPrice = card.type != CardType.Booster;
                view.onBuy = TryBuy;
                view.Bind(card, showPrice, owned);

                // Special case: Booster displayed as a single "Open Booster" button
                if (card.type == CardType.Booster)
                {
                    var btn = view.buyButton;
                    if (btn)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(OpenBooster);
                        btn.interactable = InventoryManager.Instance.Data.coins >= card.price;
                        if (view.priceText) view.priceText.text = $"{card.price} 💰";
                    }
                }
            }
        }

        void TryBuy(CardData card)
        {
            if (InventoryManager.Instance.Owns(card.id))
            {
                Toast($"{card.displayName} déjà possédée.");
                return;
            }
            if (!InventoryManager.Instance.TrySpend(card.price))
            {
                Toast("Pas assez de pièces.");
                return;
            }
            InventoryManager.Instance.AddCard(card.id);
            Toast($"Acheté: {card.displayName}");
            BuildGrid();
        }

        void OpenBooster()
        {
            // Example booster: spend 10 coins, get a random non-owned Character
            int boosterPrice = 10;
            if (!InventoryManager.Instance.TrySpend(boosterPrice))
            {
                Toast("Pas assez de pièces pour le booster.");
                return;
            }

            var pool = new List<CardData>();
            foreach (var c in catalogue)
            {
                if (c.type == CardType.Character && !InventoryManager.Instance.Owns(c.id))
                    pool.Add(c);
            }
            if (pool.Count == 0)
            {
                Toast("Aucune carte disponible dans le booster.");
                InventoryManager.Instance.AddCoins(boosterPrice); // refund
                return;
            }
            var reward = pool[Random.Range(0, pool.Count)];
            InventoryManager.Instance.AddCard(reward.id);
            Toast($"Booster → {reward.displayName} !");
            BuildGrid();
        }

        void Toast(string msg)
        {
            if (!toastText) return;
            toastText.gameObject.SetActive(true);
            toastText.text = msg;
            CancelInvoke(nameof(HideToast));
            Invoke(nameof(HideToast), 2.5f);
        }

        void HideToast()
        {
            if (toastText) toastText.gameObject.SetActive(false);
        }
    }
}
