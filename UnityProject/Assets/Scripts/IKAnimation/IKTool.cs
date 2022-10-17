using System.Collections.Generic;
using UnityEngine;

namespace IKAnimation
{
    public static class IKTool
    {
        public static T RequireComponent<T>(this GameObject rGo) where T : Component
        {
            T component = rGo.GetComponent<T>();
            return component != null ? component : rGo.AddComponent<T>();
        }
        
        public static List<Transform> FindTransform(GameObject rGo, string targetName)
        {
            List<Transform> headList = new List<Transform>();
            FindBones(rGo.transform, targetName, ref headList);
            return headList;
        }

        public static void FindBones(Transform parent, string targetName, ref List<Transform> headList)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var curNode = parent.GetChild(i);
                if (curNode.name.Contains(targetName))
                    headList.Add(curNode);
                FindBones(curNode, targetName, ref headList);
            }
        }

        public static List<Transform> FindHeadBones(Transform rTrans, int nBoneCount)
        {
            List<Transform> rBoneList = new List<Transform>();
            for (int i = 0; i < nBoneCount; i++)
            {
                var rParent = rTrans.parent;
                if (rParent == null)
                {
                    break;
                }

                rTrans = rParent;
                rBoneList.Add(rParent);
            }

            return rBoneList;
        }
    }
}