using UnityEngine;

[System.Serializable]
public class FakeEyeIKBone : FakeIKBone {
    //眼睛水平最大转动角度
    public float AngleX = 4;
    //眼睛垂直最大转动角度
    public float AngleY = 1;

    // 依赖于头部是否恢复
    [HideInInspector]
    public bool NeedRecoverEye;
   
    public override void RotateToTarget(Vector3 targetPosition) {
        if (Bone == null) return;

        if (NeedRecoverEye) {
            currentQuaternion = Quaternion.RotateTowards(currentQuaternion, Quaternion.identity, RecoverSpeed * Time.deltaTime);
        }
        else {
            //世界坐标系的目标向量
            Vector3 baseTargetDir = targetPosition - Bone.position;
            //转到本地坐标系的目标向量，一定要用BoneParent转换。。。一开始没注意到这里，总是转不对，经常被模型翻白眼。。。
            var targetDir = BoneParent.InverseTransformDirection(baseTargetDir);
            //本地坐标系下的蓝色轴方向
            Vector3 forwardTemp = BoneParent.InverseTransformDirection(Bone.forward);
            //以下计算都在本地坐标系下计算
            //计算一个本地坐标系下，绿色轴到目标向量的旋转轴 rotateAxis
            Vector3 rotateAxis = Vector3.Cross(localTransformAxis, targetDir);
            //转动角度
            float targetAngle = Vector3.Angle(targetDir, localTransformAxis);
            //根据旋转轴 和 本地坐标系蓝色轴 计算一个权重，用于在AngleX和AngleY中做插值（很魔性的计算。。。）
            //这里一定要做先归一化，再点乘
            float cosAAA = Mathf.Abs(Vector3.Dot(rotateAxis.normalized, forwardTemp.normalized));
            //平滑过渡一下
            float angle = Mathf.SmoothStep(AngleX, AngleY, cosAAA);
         
            if (targetAngle > angle) {
                targetAngle = angle;
            }
      
            Quaternion targetQuaternion = Quaternion.AngleAxis(targetAngle, rotateAxis);
            currentQuaternion = Quaternion.RotateTowards(currentQuaternion, targetQuaternion, Speed * Time.deltaTime);
        }

        Bone.localRotation = currentQuaternion * Bone.localRotation;
    }
}