using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    /// <summary>
    /// Connects card in unity world to the Card struct.
    /// </summary>
    public class CardVisuals : MonoBehaviour
    {
        public TextMeshPro pointsText;

        public int cardIndex;
        public int playerIndex;
        public bool onTable = false;

        public void SetPointsText(string text)
        {
            pointsText.text = text;
        }
        public void Initialize(Card card, int playerIndex)
        {
            this.cardIndex = card.cardIndex;
            this.playerIndex = playerIndex;

            Renderer renderer = GetComponent<Renderer>();
            renderer.material = card.cardMaterial;
        }
        public static CardVisuals FindByIndex(CardVisuals[] cardVisuals, int cardIndex)
        {
            for (int i = 0; i < cardVisuals.Length; i++)
            {
                if (cardVisuals[i].cardIndex == cardIndex)
                    return cardVisuals[i];
            }
            return null;
        }
    }
}