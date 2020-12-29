#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OverdrawTools
{
    public class ParticleEffectProfiler : MonoBehaviour
    {
        public Overdraw OD;

        public AnimationCurve ScreenRatioCurve;
        public AnimationCurve OverdrawCurve;
        public AnimationCurve ParticleCountCurve;
        public AnimationCurve TransparentRatioCurve;
        public AnimationCurve VerticeCurve;

        public float AverateOverdraw;
        public float PeakOverdraw;
        public float MaxParticleCount;
        public float MaxScreenRatio;
        public float MaxTransparentRatio;
        public float AverageTransparentRatio;
        public float AverateVertices;
        public float MaxVertices;

        private float m_particleCount;//粒子数量

        private ParticleSystem[] m_allParticles;
        private float m_totalTime = 0;
        private float m_playTime = 0;
        private bool m_done = false;
        private int m_frameCount = 0;
        private int m_validOverdrawFrameCount = 0;

        public void Reset()
        {
            m_allParticles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            float maxDuration = 0;
            foreach(var p in m_allParticles)
            {
                float val = p.main.duration + p.main.startLifetime.constantMax;
                maxDuration = val > maxDuration ? val : maxDuration;
                //p.gameObject.SetActive(true);
            }
            m_totalTime = maxDuration;

            EditorUtility.DisplayProgressBar("测试中", string.Format("剩余{0}秒", m_totalTime), 0);
        }

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(!OD)
            {
                Debug.LogError("need Overdraw component");
                return;
            }

            if (m_done)
                return;

            m_frameCount += 1;
            m_playTime += Time.deltaTime;
            if (m_playTime > m_totalTime)
            {
                m_done = true;
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("测试完成", "测试数据收集完毕，请点击Root节点，在Inspector中查看测试数据", "OK");
                return;
            }

            if (m_allParticles == null)
                return;

            EditorUtility.DisplayProgressBar("测试中", string.Format("剩余{0}秒", m_totalTime - m_playTime), m_playTime / m_totalTime );

            m_particleCount = 0;
            foreach(var p in m_allParticles)
            {
                m_particleCount += p.particleCount;
            }

            ParticleCountCurve.AddKey(m_frameCount, m_particleCount);
            MaxParticleCount = m_particleCount > MaxParticleCount ? m_particleCount : MaxParticleCount;
            OverdrawCurve.AddKey(m_frameCount, OD.OverdrawCount);
            PeakOverdraw = OD.OverdrawCount > PeakOverdraw ? OD.OverdrawCount : PeakOverdraw;
            if (OD.OverdrawCount > 0)
            {
                m_validOverdrawFrameCount += 1;
                AverateOverdraw = (AverateOverdraw * (m_validOverdrawFrameCount - 1) + OD.OverdrawCount) / m_validOverdrawFrameCount;
            }

            float val = OD.DrawPixelCount / ((float)Screen.width*Screen.height);
            ScreenRatioCurve.AddKey(m_frameCount, val);

            MaxScreenRatio = val > MaxScreenRatio ? val : MaxScreenRatio;

            float transparentRatio = OD.DrawPixelCount > 0 ? (OD.DrawPixelCount - OD.RealDrawPixelCount) / (float)OD.DrawPixelCount : 0;
            TransparentRatioCurve.AddKey(m_frameCount, transparentRatio);

            MaxTransparentRatio = transparentRatio > MaxTransparentRatio ? transparentRatio : MaxTransparentRatio;
            AverageTransparentRatio = (AverageTransparentRatio * (m_frameCount - 1) + transparentRatio) / m_frameCount;

            MaxVertices = UnityStats.vertices > MaxVertices ? UnityStats.vertices : MaxVertices;
            AverateVertices = (AverateVertices * (m_frameCount - 1) + UnityStats.vertices) / m_frameCount;
            VerticeCurve.AddKey(m_frameCount, UnityStats.vertices);
        }
    }
}
#endif
