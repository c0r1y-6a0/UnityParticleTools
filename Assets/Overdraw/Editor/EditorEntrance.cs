using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace OverdrawTools
{
    public static class EditorEntrance
    {
        [MenuItem("Tools/删除Overdraw工具相关组件", false, 2)]
        public static void DeleteOverdraw()
        {
            GameObject.DestroyImmediate(GameObject.Find("TransparentCoverageCamera"));
            GameObject.DestroyImmediate(GameObject.Find("OverdrawCamera"));
        }

        private static void InitCamGo(Transform t)
        {
            t.localPosition = new Vector3(0, 40, 0);
            t.localRotation = Quaternion.Euler(90, 0, 0);
            t.localScale = Vector3.one;
        }
        [MenuItem("Tools/添加Overdraw工具相关组件", false, 2)]
        public static void BeginOverdrawProfile()
        {
            var overdrawCamGo = GameObject.Find("OverdrawCam");
            if (overdrawCamGo != null)
                return;

            //var camRoot = GameObject.Find("CameraFollowRoot");
            GameObject transparentGo = GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Overdraw/Scene/TransparentCoverageCamera.prefab"));
            transparentGo.name = "TransparentCoverageCamera";
            //transparentGo.transform.SetParent(camRoot.transform);
            InitCamGo(transparentGo.transform);
            GameObject overdrawGo = GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Overdraw/Scene/OverdrawCamera.prefab"));
            overdrawGo.name = "OverdrawCamera";
            //overdrawGo.transform.SetParent(camRoot.transform);
            InitCamGo(overdrawGo.transform);
        }

        [MenuItem("Assets/检查特效效率", false, 2)]
        public static void ParticleProfile()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("", "请退出Play模式", "OK");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Overdraw/Scene/ParticleProfileScene.unity");

            var guids = Selection.assetGUIDs;
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);


            EditorApplication.isPlaying = true;

            var go = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            GameObject root = GameObject.Find("Root");
            ParticleEffectProfiler pep = root.GetComponent<ParticleEffectProfiler>();
            if (pep == null)
            {
                pep = root.AddComponent<ParticleEffectProfiler>();
                pep.OD = GameObject.Find("OverdrawCamera").GetComponent<Overdraw>();
            }

            root.transform.DetachChildren();
            go.transform.parent = root.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);
            go.transform.localScale = Vector3.one;
        }
    }
}
