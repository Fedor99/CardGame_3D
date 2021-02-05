using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CardChooser : MonoBehaviour
    {
        // Should be set in inspector
        public GameObject arrowObject;

        public bool isVisible = true;
        public bool onTable = false;
        public int playerIndex = -1;
        public int cardIndexException = -1;
        //public PlayerVisuals playerVisuals;
        public GameObject hoverOnCardObject;
        public GameObject hoverOnCardObject_WithCondition;
        public GameObject chosenCardObject;
        public GameObject applyToCardObject;

        GameObject createdArrow;

        Camera camera;
        // Start is called before the first frame update
        void Start()
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.tag == "Card")
                {
                    //CardVisuals hitCardVisuals = hitObject.GetComponent<CardVisuals>();
                    {
                        hoverOnCardObject = hitObject;

                        CardVisuals cv = hoverOnCardObject.GetComponent<CardVisuals>();
                        if (playerIndex == -1
                            || (playerIndex == cv.playerIndex)
                                                            && onTable == cv.onTable
                                                            && cv.cardIndex != cardIndexException)
                        {
                            //Debug.Log("playerIndex = " + playerIndex);

                            hoverOnCardObject_WithCondition = hitObject;

                            if (Input.GetKeyDown(KeyCode.Mouse0))
                            {
                                if (applyToCardObject == null && chosenCardObject != null)
                                {
                                    applyToCardObject = hoverOnCardObject;
                                    Destroy(createdArrow);
                                }
                                if (chosenCardObject == null && applyToCardObject == null)
                                {
                                    SetChosenCardObject(hoverOnCardObject_WithCondition);
                                }
                            }
                        }
                        else {
                            hoverOnCardObject_WithCondition = null;
                        }
                    }
                }
                else
                {
                    hoverOnCardObject = null;
                    hoverOnCardObject_WithCondition = null;
                }
            }
        }

        public void SetChosenCardObject(GameObject chosenCardObject)
        {
            this.chosenCardObject = chosenCardObject;
            createdArrow = Instantiate(arrowObject, this.chosenCardObject.transform.position,
                this.chosenCardObject.transform.rotation);
            createdArrow.SetActive(isVisible);
        }
    }
}