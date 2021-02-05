using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MainTest : MonoBehaviour
    {
        public int[] i;

        // Start is called before the first frame update
        void Start()
        {
            i = HelperFunctions.AddAfter<int>(i, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}