
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShadowHunters
{
    public class CardItemView : MonoBehaviour
    {
        public Image artImage;
        public TMP_Text titleText;
        public TMP_Text priceText;
        public Button buyButton;

        [HideInInspector] public CardData data;
        public System.Action<CardData> onBuy;

        public void Bind(CardData card, bool showPrice = true, bool owned = false)
        {
            data = card;
            if (titleText) titleText.text = card.displayName;
            if (priceText) priceText.text = showPrice ? $"{card.price} 💰" : (owned ? "Owned" : "");
            if (artImage) artImage.sprite = card.art;
            if (buyButton)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => onBuy?.Invoke(card));
                buyButton.interactable = showPrice && !owned && card.type != CardType.Booster;
            }
        }
    }
}
