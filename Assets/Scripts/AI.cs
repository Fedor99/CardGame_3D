using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// AI player.
    /// </summary>
    [System.Serializable]
    public class AI : AbstractPalyer, IPlayerFunctionality
    {
        public AI(PlayerVisuals playerVisuals) : base(playerVisuals)
        {

        }

        #region PlayersTurn
        override public void PlayCard(CardPosition cardPosition = null)
        {
            Debug.Log("\nPlayer #" + playerIndex + ":");

            playersTurn = true;
            if (cardPosition == null)
            {
                int row = UnityEngine.Random.Range(0, cardTablePart.cardArray.Length);
                cardPosition = new CardPosition(row, UnityEngine.Random.Range(-1, cardTablePart.cardArray[row].Length));
            }

            chosenCard = GetRandomCardFromHand();

            cardTablePart.AddCard(chosenCard, cardPosition);

            Debug.Log("     Card #" + chosenCard.cardIndex + " played.");

            // ***Animation   move chosen card to cardPosition
            playerVisuals.StartCoroutine(playerVisuals.PlayCard(chosenCard.cardIndex, cardPosition));

            OnUpdate();
        }

        override public void ApplyActive(CardPosition cardPosition)
        {
            int[] appliedToAllyCards = new int[0];
            int[] appliedToOpponentCards = new int[0];

            if (chosenCard.HasActiveAbilityToAlly())
            {
                int randomRow = cardTablePart.GetRandomRowWithCards();
                CardPosition randomPlayerCardPosition = new CardPosition(randomRow,
                                                    UnityEngine.Random.Range(0,
                                                                    cardTablePart.cardArray[randomRow].Length));
                appliedToAllyCards = chosenCard.ApplyActiveToAlly(cardTablePart, randomPlayerCardPosition);
            }

            if (chosenCard.HasActiveAbilityToOpponent())
            {
                int randomRow1 = opponent.cardTablePart.GetRandomRowWithCards();
                if (randomRow1 != -1)
                {
                    CardPosition randomOpponentCardPosition = new CardPosition(randomRow1,
                            UnityEngine.Random.Range(0,
                                                        opponent.cardTablePart.cardArray[randomRow1].Length));
                    appliedToOpponentCards = chosenCard.ApplyActiveToOpponent(opponent.cardTablePart, randomOpponentCardPosition);
                }
            }

            //   *** Active abilities animation
            playerVisuals.StartCoroutine(playerVisuals.ApplyActive(
                cardPosition,
                appliedToAllyCards,
                appliedToOpponentCards));

            OnUpdate();
        }
        #endregion
    }
}