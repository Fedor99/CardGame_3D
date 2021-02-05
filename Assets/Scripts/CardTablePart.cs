using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// 1/2 of a card table.
    /// </summary>
    public class CardTablePart
    {
        /// <summary>
        /// Stores all player cards.
        /// </summary>
        public Card[][] cardArray;

        public Card[] destroyedCards;

        public int height { get; private set; }
        int maxWidth;

        public CardTablePart(int height, int maxWidth)
        {
            this.maxWidth = maxWidth;
            this.height = height;

            cardArray = new Card[height][]; // [rowIndex][columnIndex]
            for (int i = 0; i < height; i++)
                cardArray[i] = new Card[0];
        }

        /// <summary>
        /// CardPosition by index from cardArray[][]
        /// </summary>
        /// <param name="index"></param>
        /// <returns>returns CardPosition by index from cardArray[][]</returns>
        public CardPosition GetCardByIndex(int index)
        {
            // From cardArray[][]
            for (int row = 0; row < cardArray.Length; row++)
            {
                for (int col = 0; col < cardArray[row].Length; col++)
                {
                    if (cardArray[row][col].cardIndex == index)
                        return new CardPosition(row, col);
                }
            }

            return null;
        }

        /// <summary>
        /// Adds card to player`s table part after 'addCard.collumn'.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="addAfter"></param>
        public void AddCard(Card card, CardPosition addAfter)
        {
            if (addAfter.column < -1 || addAfter.column > maxWidth
                || addAfter.row < 0 || addAfter.row >= height)
                throw new System.Exception("Incorrect Card Position: " + addAfter);

            cardArray[addAfter.row] = HelperFunctions.AddAfter<Card>(cardArray[addAfter.row], card, addAfter.column);
        }

        /// <summary>
        /// Maoves card from table to destryedCards[]
        /// </summary>
        /// <param card you want to remove="card"></param>
        /// <param its position on the table="cardPosition"></param>
        public void DestroyCard(Card card, CardPosition cardPosition)
        {
            // Remove
            cardArray[cardPosition.row] = 
                HelperFunctions.RemoveAtIndex<Card>(cardArray[cardPosition.row], cardPosition.column);
            // Add to destroyedCards[]
            destroyedCards = HelperFunctions.AddAfter<Card>(destroyedCards, card, destroyedCards.Length - 1);
        }

        public CardPosition CardPositionByIndex(int cardIndex)
        {
            for (int row = 0; row < cardArray.Length; row++)
            {
                for (int col = 0; col < cardArray[row].Length; col++)
                {
                    if (cardArray[row][col].cardIndex == cardIndex)
                        return new CardPosition(row, col);
                }
            }
            return null;
        }

        /// <summary>
        /// -1 if there is no such row
        /// </summary>
        /// <returns>returns random row that contains at least two cards, -1 if there is no such row</returns>
        public int GetRandomRowWithCards()
        {
            int numberOfRowsWithCards = 0;
            int[] rowIndexes = new int[cardArray.Length];
            for (int i = 0; i < cardArray.Length; i++)
            {
                if (cardArray[i].Length > 0)
                {
                    rowIndexes[numberOfRowsWithCards] = i;
                    numberOfRowsWithCards++;
                }
            }

            if (numberOfRowsWithCards > 0)
                return rowIndexes[UnityEngine.Random.Range(0, numberOfRowsWithCards)];

            return -1;
        }
    }

    /// <summary>
    /// Represents position of the card on a CardTablePart.
    /// </summary>
    [System.Serializable]
    public class CardPosition
    {
        public int row, column;
        public CardPosition(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public override string ToString()
        {
            return "CardPosition(row: " + row + ", column: " + column + ")";
        }
    }
}