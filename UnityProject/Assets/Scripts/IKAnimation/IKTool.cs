using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IKAnimation
{
    public static class IKTool
    {
        public static Transform FindTransform(GameObject rGo, string targetName)
        {
            Transform rHeadBone = FindBones(rGo.transform, targetName);
            return rHeadBone;
        }

        public static Transform FindBones(Transform parent, string targetName)
        {
            Transform target = null;
            if (parent.name.Contains(targetName))
            {
                target = parent;
                return target;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                var curNode = parent.GetChild(i);
                if (curNode.name.Contains(targetName))
                    return curNode;
                
                target = FindBones(curNode, targetName);
                if (target != null)
                    return target;
                
            }

            return target;
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