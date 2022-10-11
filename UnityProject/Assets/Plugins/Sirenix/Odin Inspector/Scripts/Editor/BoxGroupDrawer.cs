/*
 *Copyright(C) 2020 by  GYYX All rights reserved.
 *Unity版本：2018.4.23f1 
 *作者:程一峰  
 *创建日期: 2021-01-26 
 *模块说明：
 *版本: 1.2
*/


using UnityEngine;
using UnityEditor;

namespace Sirenix.OdinInspector.Editor.Drawers
{
    /// <summary>
    /// 方框形状的操作。。
    /// </summary>
    public class BoxGroupDrawer : BoxGroupAttributeDrawer
    {

        GUIStyle mStyle;

        protected override void Initialize()
        {
            base.Initialize();
            mStyle = new GUIStyle();
            mStyle.alignment = TextAnchor.MiddleCenter;
            mStyle.fontSize = 15;
            mStyle.fontStyle = FontStyle.Bold;
            mStyle.normal.textColor = new Color(0, 0.8f, 1f, 1);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.Space(20);

            if (Attribute.ShowLabel)
                Attribute.ShowLabel = false;
            {
                string lableStr = Attribute.GroupName;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(lableStr, mStyle, new[]
                {
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(30),
                });
                EditorGUILayout.EndVertical();
            }

            base.DrawPropertyLayout(label);
        }

    }
}
