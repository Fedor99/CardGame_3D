using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    public class CardTooltip : MonoBehaviour
    {
        private bool launched = false;

        public PlayerVisuals playerVisuals;
        public CardChooser cardChooser;
        public CardChooser cardChooser1;
        public GameObject tooltip;
        public TextMeshPro tooltipText;

        public void Launch()
        {
            launched = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (launched)
            {
                if (playerVisuals.player.playerIndex == 1)
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                cardChooser.playerIndex = playerVisuals.player.playerIndex;
                cardChooser1.playerIndex = playerVisuals.player.playerIndex;

                if (cardChooser.hoverOnCardObject_WithCondition != null ||
                    cardChooser1.hoverOnCardObject_WithCondition != null)
                {
                    if (cardChooser.hoverOnCardObject_WithCondition != null)
                        SetTooltipText(cardChooser.hoverOnCardObject_WithCondition);
                    if (cardChooser1.hoverOnCardObject_WithCondition != null)
                        SetTooltipText(cardChooser1.hoverOnCardObject_WithCondition);
                    tooltip.SetActive(true);
                }
                else
                    tooltip.SetActive(false);
            }
        }

        void SetTooltipText(GameObject hoverOnCardObject_WithCondition)
        {
            tooltipText.text = "";
            try
            {
                transform.position = hoverOnCardObject_WithCondition.transform.position + new Vector3(3, 0, 0);
                Card card = playerVisuals.player.FindCardByIndex(
                                hoverOnCardObject_WithCondition.GetComponent<CardVisuals>().cardIndex);
                tooltipText.text = "Name: " + card.CardName + "\n + Ability: " + card.Description;
            }
            catch (System.Exception) { }
        }
    }
}