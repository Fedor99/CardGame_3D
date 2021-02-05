using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class HelperFunctions
    {
        /// <summary>
        /// Incerts an element into an array after specified element index.
        /// 'after = -1' means the position before the first element.
        /// </summary>
        /// <typeparam type ="T"></typeparam>
        /// <param array tou want to incert element in="arr"></param>
        /// <param new element that is to be added="newElement"></param>
        /// <param specified element index="after"></param>
        /// <returns>returns an array with incerted element</returns>
        public static T[] AddAfter<T>(T[] arr, T newElement, int after)
        {
            if (arr.Length == 0)
                return new T[1] { newElement };

            T[] newArr = new T[arr.Length + 1];
            for (int i = 0; i < newArr.Length; i++)
            {
                if (i == after + 1)
                    continue;

                int index = i;
                int arrIndex = i;
                if (i > after + 1)
                {
                    arrIndex--;
                }

                newArr[index] = 
                    arr[arrIndex];
            }
            newArr[after + 1] = newElement;

            return newArr;
        }

        /// <summary>
        /// Adds an element to an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="newElement"></param>
        /// <returns></returns>
        public static T[] Add<T>(T[] arr, T newElement)
        {
            return AddAfter<T>(arr, newElement, arr.Length - 1);
        }

        /// <summary>
        /// Removes element from an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param element index="index"></param>
        /// <returns>returns an array at specified index</returns>
        public static T[] RemoveAtIndex<T>(T[] arr, int index)
        {
            T[] newArr = new T[arr.Length - 1];

            int counter = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == index)
                    continue;
                newArr[counter] = arr[i];
                counter++;
            }
            return newArr;
        }

        public static int RandomExcept(int from, int to, int except)
        {
            if (from == 0 && to == 1)
                return -1;
            int result = UnityEngine.Random.Range(from, to - 1);
            if (result >= except)
                result += 1;
            return result;
        }
    }
}