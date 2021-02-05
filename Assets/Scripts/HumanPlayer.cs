using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Local player
    /// </summary>
    [System.Serializable]
    public class HumanPlayer : AbstractPalyer, IPlayerFunctionality
    {
        public HumanPlayer(PlayerVisuals playerVisuals) : base(playerVisuals)
        {

        }

#region PlayersTurn
        override public void PlayersTurn()
        {
            playersTurn = true;
            playerVisuals.StartCoroutine(playerVisuals.ChooseCard());
        }

        /// <summary>
        /// Plays card onto a table
        /// </summary>
        /// <param Position of a card on a table you want it push to="cardPosition"></param>
        override public void PlayCard(CardPosition cardPosition = null)
        {
            if (cardPosition == null)
            {
                int row = UnityEngine.Random.Range(0, cardTablePart.cardArray.Length);
                cardPosition = new CardPosition(row, UnityEngine.Random.Range(-1, cardTablePart.cardArray[row].Length));
            }

            cardTablePart.AddCard(chosenCard, cardPosition);

            // ***Animation   move chosen card to cardPosition
            playerVisuals.StartCoroutine(playerVisuals.PlayCard(chosenCard.cardIndex, cardPosition));

            OnUpdate();
        }

        override public void ApplyActive(CardPosition cardPosition)
        {
            playerVisuals.StartCoroutine(playerVisuals.ApplyActive(cardPosition, chosenCard));
        }
        #endregion
    }
}