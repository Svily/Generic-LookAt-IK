using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace IKAnimation
{
    public class IKAnimationEditor : OdinEditorWindow
    {
        private static IKAnimationEditor rWindow => GetWindow<IKAnimationEditor>();
        
        private const int Btn_Height = 30;
        
        [LabelText("是否为类人骨骼")]
        public bool IsHuman = true;

        [LabelText("类人脊颈骨数量"), Range(0, 5)]
        public int HumanBoneNum = 3;
        
        [LabelText("怪物脊颈骨数量"), Range(0, 10)]
        public int MonsterBoneNum = 4;

        [LabelText("IK注视配置")]
        public LookAtIKConfig IKConfig;
        
        [MenuItem("EditorTools/IK Editor Tool")]
        public static void OpenWindow()
        {
            rWindow.titleContent = new GUIContent("IK Editor Tool");
            rWindow.Show();
        }
        
        [Button("初始化选中物体LooAtIK", ButtonHeight = Btn_Height)]
        public static void OnekeyFindSpine()
        {
            var rCurGo = Selection.activeGameObject;
            if (rCurGo == null)
                return;
            ClearModel(rCurGo);

            
            rCurGo.RequireComponent<LookAtIKCtrl>().IKConfig = rWindow.IKConfig;
            
            //找头骨
            List<Transform> rHeadList = IKTool.FindTransform(rCurGo, "Head");
            if (rHeadList.Count <= 0)
            {
                EditorUtility.DisplayDialog("错误", "没找到Head骨骼，请检查模型骨骼命名是否正确！！", "OK");
                return;
            }
            
            // 类人模型有且只有一个头
            if (rWindow.IsHuman)
            {
                var mHeadTrans = rHeadList[0];
                LookAtIK mLookAtIK = rCurGo.RequireComponent<LookAtIK>();
                if (mHeadTrans == null || mLookAtIK == null)
                {
                    Debug.LogError("找不到模型的Head骨骼或LookIK组件!!!");
                    return;   
                }
            
                //找颈椎x段骨
                var rBoneList = IKTool.FindHeadBones(mHeadTrans, rWindow.HumanBoneNum).ToArray();
                //初始化IK解算器
                mLookAtIK.solver.SetChain(rBoneList, mHeadTrans, null, rCurGo.transform);

            }
            else // 多头奇行种
            {
                foreach (var rHead in rHeadList)
                {
                    LookAtIK mLookAtIK = rCurGo.RequireComponent<LookAtIK>();
                    //找颈椎x段骨
                    var rBoneList = IKTool.FindHeadBones(rHead, rWindow.MonsterBoneNum).ToArray();
                    //初始化IK解算器
                    mLookAtIK.solver.SetChain(rBoneList, rHead, null, rCurGo.transform);
                }
            }
            
        }


        public static void ClearModel(GameObject rCurGo)
        {
            //先清除物体上所有的LookAt组件
            GameObject.DestroyImmediate(rCurGo.GetComponent<LookAtIKCtrl>());
            List<LookAtIK> ikList = rCurGo.GetComponentsInChildren<LookAtIK>().ToList();
            for (int i = ikList.Count - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(ikList[i]);
            }
        }

        public static void AddIKComponet(GameObject rGo)
        {
            var IKCtrl = rGo.AddComponent<HumanLookAtIKCtrl>();
            // IKCtrl.LookAtIK = rGo.GetComponent<LookAtIK>();
        }
        
        [LabelText("Model Directory Path"), Sirenix.OdinInspector.FolderPath]
        public string DirectoryPath;

        [Button]
        public static void InitDirectoryFiles()
        {
            if (Directory.Exists(rWindow.DirectoryPath))
            {
                List<FileInfo> fileList = new List<FileInfo>();
                GetAllIKModel(rWindow.DirectoryPath, ref fileList);
            }
        }


        public static void GetAllIKModel(string dirPath, ref List<FileInfo> fileList)
        {
            var directory = new DirectoryInfo(dirPath);
            var files = directory.GetFiles();
            
            fileList.AddRange(files.Where(file => file.Name.EndsWith("prefab")));

            var dirs = Directory.GetDirectories(dirPath);
            foreach (var dir in dirs)
            {
                GetAllIKModel(dir, ref fileList);
            }
        }
        
    }
}