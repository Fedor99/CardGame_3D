using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ArrowVisuals : MonoBehaviour, IVisualEffect
    {
        CardChooser cardChooser;
        Camera camera;
        float coef = 2.1f;

        bool proceedAnimation = false;

        // Start is called before the first frame update
        void Awake()
        {
            cardChooser = GetComponent<CardChooser>();
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (proceedAnimation)
                return;

            if (cardChooser.hoverOnCardObject_WithCondition != null)
            {
                transform.LookAt(cardChooser.hoverOnCardObject_WithCondition.transform.position);
                SetLength(Vector3.Distance(transform.position, cardChooser.hoverOnCardObject_WithCondition.transform.position));
            }
            else
            {
                Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Table")
                    {
                        transform.LookAt(hit.point);
                        SetLength(Vector3.Distance(transform.position, hit.point));
                    }
                }
            }
        }

        void SetLength(float length)
        {
            transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            length / coef);
        }

        #region Launch

        public bool ProceedAnimation() { return false; }
        #endregion
    }
}