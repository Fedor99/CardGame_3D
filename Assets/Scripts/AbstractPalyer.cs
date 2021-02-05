using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    [System.Serializable]
    public abstract class AbstractPalyer
    {
        public bool useGraphics;

        public bool playersTurn;

        public int playerIndex;

        int initialNumberOfCards;
        public Card[] playerCards;
        public int numberOfPlayerCardsInHand;

        int playerPoints = 0;

        public AbstractPalyer opponent;
        public CardManager cardManager;
        public CardTablePart cardTablePart;
        public PlayerVisuals playerVisuals;

        public AbstractPalyer(PlayerVisuals playerVisuals)
        {
            this.playerVisuals = playerVisuals;
        }


        #region PlayCard() variables
        public Card chosenCard;
        #endregion
        #region Methods to be overriden
        virtual public void PlayersTurn()
        {
            this.PlayCard();
        }
        public abstract void PlayCard(CardPosition cardPosition = null);
        public void ApplyPassive(CardPosition cardPosition)
        {
            int[] appliedToAllyCards =
                chosenCard.ApplyPassiveToAlly(cardTablePart, new CardPosition(cardPosition.row, cardPosition.column + 1));
            int[] appliedToOpponentCards =
                chosenCard.ApplyPassiveToOpponent(opponent.cardTablePart, new CardPosition(cardPosition.row, cardPosition.column + 1));

            //   *** Passive ability animation
            playerVisuals.StartCoroutine(playerVisuals.ApplyPassive(
                cardPosition,
                appliedToAllyCards,
                appliedToOpponentCards));

            OnUpdate();
        }
        public abstract void ApplyActive(CardPosition cardPosition);
        public void EndTurn()
        {
            playersTurn = false;

            OnUpdate();
        }
        #endregion;


        protected Card GetRandomCardFromHand()
        {
            int index = UnityEngine.Random.Range(0, playerCards.Length);
            Card card = playerCards[index];
            playerCards = HelperFunctions.RemoveAtIndex<Card>(playerCards, index);
            numberOfPlayerCardsInHand--;
            return card;
        }

        /// <summary>
        /// Call it from Awake() or Start()
        /// </summary>
        public void Initialize()
        {
            cardManager = GameObject.FindObjectOfType<CardManager>();
            initialNumberOfCards = cardManager.initialNumberOfCards;
            playerCards = GetInitialCards(initialNumberOfCards);
            numberOfPlayerCardsInHand = playerCards.Length;

            playerVisuals.player = this;
            for (int c = 0; c < playerCards.Length; c++)
            {
                playerVisuals.AddCardToHand(playerCards[c]);
            }

            // Max 2 rows and 9 cards in a row
            cardTablePart = new CardTablePart(2, 9);
        }

        Card[] GetInitialCards(int numberOfCards)
        {
            Card[] allTypesOfCards = cardManager.allTypesOfCards;

            Card[] result = new Card[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                Card newCard = allTypesOfCards[UnityEngine.Random.Range(0, allTypesOfCards.Length)];
                newCard.cardIndex = i;
                newCard.playerIndex = playerIndex;
                result[i] = newCard;
            }
            return result;
        }

        /// <summary>
        /// Sums up player points.
        /// </summary>
        /// <returns>returns sum of all card points on player`s table part</returns>
        public int CountPlayerPoints()
        {
            int result = 0;
            for (int row = 0; row < cardTablePart.cardArray.Length; row++)
            {
                for (int col = 0; col < cardTablePart.cardArray[row].Length; col++)
                {
                    result += cardTablePart.cardArray[row][col].cardPoints;
                }
            }
            return result;
        }

        /// <summary>
        /// Removes specified card from hand and returns the specified card.
        /// </summary>
        /// <returns>returns player card with specified index from hand</returns>
        public Card GetCard(int cardIndex)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                Card result = playerCards[i];
                if (result.cardIndex == cardIndex)
                {
                    playerCards = HelperFunctions.RemoveAtIndex<Card>(playerCards, i);
                    return result;
                }
            }

            throw new Exception("Card not found! card number: " + cardIndex);
        }

        public Card FindCardByIndex(int cardIndex)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                Card result = playerCards[i];
                if (result.cardIndex == cardIndex)
                {
                    return result;
                }
            }

            for (int row = 0; row < cardTablePart.cardArray.Length; row++)
            {
                for (int col = 0; col < cardTablePart.cardArray[row].Length; col++)
                {
                    Card result = cardTablePart.cardArray[row][col];
                    if (result.cardIndex == cardIndex)
                        return result;
                }
            }

            throw new Exception("Card not found! card number: " + cardIndex);
        }

        public void OnUpdate()
        {
            playerPoints = CountPlayerPoints();
        }

        public override string ToString()
        {
            string result = "\n Player " + playerIndex + ":";
            result += "\n ---PlayerCards in hand:";
            for (int c = 0; c < playerCards.Length; c++)
            {
                result += "\n" + playerCards[c];
            }

            result += "\n\n ---PlayerPoints: " + playerPoints;

            result += "\n\n ---PlayerTablePart Cards: ";
            for (int row = 0; row < cardTablePart.cardArray.Length; row++)
            {
                result += "\n\n Row " + row + ":";
                for (int col = 0; col < cardTablePart.cardArray[row].Length; col++)
                {
                    result += cardTablePart.cardArray[row][col];
                }
            }

            return result;
        }
    }
}