using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OverdrawTools.ParticleEffectProfiler))]
public class ParticleEffectProfilerInspector : Editor
{
    private void ShowLabel(string label, bool red)
    {
        if (red)
        {
            GUIStyle s = new GUIStyle();
            s.normal.textColor = Color.red;
            EditorGUILayout.LabelField(label, s); 
        }
        else
        {
            EditorGUILayout.LabelField(label);
        }

    }

    public override void OnInspectorGUI()
    {
        OverdrawTools.ParticleEffectProfiler inspectorObj = (OverdrawTools.ParticleEffectProfiler)target;
        EditorGUILayout.CurveField("屏占比", inspectorObj.ScreenRatioCurve);
        EditorGUILayout.CurveField("Overdraw", inspectorObj.OverdrawCurve);
        EditorGUILayout.CurveField("粒子数量", inspectorObj.ParticleCountCurve);
        EditorGUILayout.CurveField("透明像素百分比", inspectorObj.TransparentRatioCurve);
        EditorGUILayout.CurveField("顶点数", inspectorObj.VerticeCurve);

        ShowLabel(string.Format("所有实际画到的像素的平均overdraw为{0}, 建议小于3", inspectorObj.AverateOverdraw), inspectorObj.AverateOverdraw >= 3);
        ShowLabel(string.Format("所有实际画到的像素的最大overdraw为{0}, 建议小于4", inspectorObj.PeakOverdraw), inspectorObj.PeakOverdraw >= 4);
        ShowLabel(string.Format("最大粒子数量为{0}，建议小于20", inspectorObj.MaxParticleCount), inspectorObj.MaxParticleCount >= 20);
        ShowLabel(string.Format("最大屏占比为{0}，过大时请考虑是否有不必要的大范围透明渲染", inspectorObj.MaxScreenRatio), inspectorObj.MaxScreenRatio >= 0.2);
        ShowLabel(string.Format("透明像素占实际渲染像素的比例 平均为:{0}，建议小于50%", inspectorObj.AverageTransparentRatio), inspectorObj.AverageTransparentRatio>= 0.5);
        ShowLabel(string.Format("透明像素占实际渲染像素的比例 峰值为:{0}，建议小于80%", inspectorObj.MaxTransparentRatio), inspectorObj.MaxTransparentRatio>= 0.8);
        ShowLabel(string.Format("顶点数 平均为:{0}，建议小于100", inspectorObj.AverateVertices), inspectorObj.AverateVertices>= 100);
        ShowLabel(string.Format("顶点数 峰值为:{0}，建议小于200", inspectorObj.MaxVertices), inspectorObj.MaxVertices >= 500);
    }

}