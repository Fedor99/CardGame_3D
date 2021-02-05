using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SphereVisuals : MonoBehaviour, IVisualEffect
    {
        TextMesh cardPoints;
        public bool ProceedAnimation()
        {
            Renderer renderer = GetComponent<Renderer>();
            Color color = renderer.material.color;
            color.a = Mathf.Lerp(color.a, 0.0f, 0.25f);
            renderer.material.color = color;

            // Use '< n' instead of '== n' because of scientific number notation
            if ((double)color.a < 0.01f)
                return false;

            return true;
        }
    }
}