#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverdrawTools
{
    public class GreenScreen : MonoBehaviour
    {
        public Material Mat;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, Mat);
        }
    }

}
#endif
