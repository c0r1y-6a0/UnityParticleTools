#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace OverdrawTools
{
    public class Overdraw : MonoBehaviour
    {

        public float OverdrawCount { get; private set; }
        public float DrawPixelCount{ get; private set; }
        public float RealDrawPixelCount{ get; private set; }

        public Material GreenScreen;

        private Camera TransparentCoverageCamera;
        private RenderTexture m_sceneRT;

        private static int ComponentCount = 0;

        private ComputeShader m_CS;

        private int m_threadGroupX;
        private int m_threadGroupY;
        private int m_kernel;
        private bool m_inited = false;
        private RenderTexture m_overdrawRT;

        private int[] m_drawTime;
        private int[] m_drawPixelCount;
        private int[] m_realDrawPixelCount;
        private ComputeBuffer m_buffer1;
        private ComputeBuffer m_buffer2;
        private ComputeBuffer m_buffer3;

        private GUIStyle m_overdrawLabelStyle;
        // Use this for initialization
        void Start()
        {
            ComponentCount += 1;

            m_CS = AssetDatabase.LoadAssetAtPath("Assets/Overdraw/Shader/Overdraw.compute", typeof(ComputeShader)) as ComputeShader;

            m_overdrawRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_overdrawRT.enableRandomWrite = true;
            m_overdrawRT.Create();

            m_threadGroupX = Mathf.RoundToInt(Screen.width / 32.0f);
            m_threadGroupY = Mathf.RoundToInt(Screen.height / 32.0f);

            m_drawTime = new int[m_threadGroupX * m_threadGroupY];
            m_drawPixelCount = new int[m_threadGroupX * m_threadGroupY];
            m_realDrawPixelCount = new int[m_threadGroupX * m_threadGroupY];

            var c = GetComponent<Camera>();
            c.SetReplacementShader(Shader.Find("Replace/Replacement"), "OverdrawTag");
            c.targetTexture = m_overdrawRT;

            if (GreenScreen != null)
            {
                GreenScreen.SetTexture("_OverdrawRT", m_overdrawRT);
            }

            m_overdrawLabelStyle = new GUIStyle();
            m_overdrawLabelStyle.normal.textColor = Color.red;
            m_overdrawLabelStyle.fontSize = 20;

            var go = GameObject.Find("TransparentCoverageCamera");
            if(!go)
            {
                Debug.LogError("需要TransparentCovertageCamera才能测量透明渲染比例");
            }
            else
            {
                TransparentCoverageCamera = go.GetComponent<Camera>();
                if (TransparentCoverageCamera.targetTexture == null)
                {
                    m_sceneRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                    m_sceneRT.enableRandomWrite = true;
                    m_sceneRT.Create();
                    TransparentCoverageCamera.targetTexture = m_sceneRT;
                }
            }
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(50, (ComponentCount - 1) * 80 + 50, 200, 50), string.Format("Overdraw: {0}", OverdrawCount), m_overdrawLabelStyle);
        }

        private void OnDestroy()
        {
            ComponentCount -= 1;

            if (m_overdrawRT != null)
            {
                m_overdrawRT.Release();
            }
            if(m_sceneRT != null)
            {
                m_sceneRT.Release();
            }
        }

        private void OnPostRender()
        {
            if (m_overdrawRT == null || m_CS == null)
                return;

            if (!m_inited)
            {
                m_inited = true;


                m_kernel = m_CS.FindKernel("CSMain");
                m_CS.SetInt("GroupXCount", m_threadGroupX);
                m_CS.SetTexture(m_kernel, "OverdrawRT", m_overdrawRT);
                m_CS.SetTexture(m_kernel, "SceneRT", m_sceneRT);

                m_buffer1 = new ComputeBuffer(m_drawTime.Length, 4);
                m_buffer1.SetData(m_drawTime);
                m_CS.SetBuffer(m_kernel, "DrawTime", m_buffer1);

                m_buffer2 = new ComputeBuffer(m_drawPixelCount.Length, 4);
                m_buffer2.SetData(m_drawPixelCount);
                m_CS.SetBuffer(m_kernel, "DrawPixelCount", m_buffer2);

                m_buffer3 = new ComputeBuffer(m_realDrawPixelCount.Length, 4);
                m_buffer3.SetData(m_realDrawPixelCount);
                m_CS.SetBuffer(m_kernel, "RealDrawPixelCount", m_buffer3);
            }

            m_CS.Dispatch(m_kernel, m_threadGroupX, m_threadGroupY, 1);
            m_buffer1.GetData(m_drawTime);
            m_buffer2.GetData(m_drawPixelCount);
            m_buffer3.GetData(m_realDrawPixelCount);

            int drawTime = 0;
            DrawPixelCount = 0;
            RealDrawPixelCount = 0;
            for (int j = 0; j < m_threadGroupY; j++)
            {
                for (int i = 0; i < m_threadGroupX; i++)
                {
                    int index = j * m_threadGroupX + i;
                    drawTime += m_drawTime[index];
                    DrawPixelCount += m_drawPixelCount[index];
                    RealDrawPixelCount += m_realDrawPixelCount[index];
                }
            }

            OverdrawCount = (DrawPixelCount == 0) ? 0 : drawTime / (float)(DrawPixelCount);
        }
    }
}
#endif
