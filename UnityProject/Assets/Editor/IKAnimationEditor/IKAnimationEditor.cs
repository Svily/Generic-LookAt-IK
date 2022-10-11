using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private AnimationCurve AnimaCurve = new AnimationCurve();

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
            
            AddIKComponet(rCurGo);
            //找头骨
            Transform mHeadTrans = IKTool.FindTransform(rCurGo, "Head");
            LookAtIK mLookAtIK = rCurGo.GetComponent<LookAtIK>();
            if (mHeadTrans == null || mLookAtIK == null)
            {
                Debug.LogError("找不到模型的Head骨骼或LookIK组件!!!");
                return;   
            }
            
            //找颈椎x段骨
            var rBoneList = IKTool.FindHeadBones(mHeadTrans, 3).ToArray();
            //初始化IK解算器
            mLookAtIK.solver.SetChain(rBoneList, mHeadTrans, null, rCurGo.transform);
        }

        public static void AddIKComponet(GameObject rGo)
        {
            var IKCtrl = rGo.AddComponent<HumanLookAtIKCtrl>();
            IKCtrl.LookAtIK = rGo.GetComponent<LookAtIK>();
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