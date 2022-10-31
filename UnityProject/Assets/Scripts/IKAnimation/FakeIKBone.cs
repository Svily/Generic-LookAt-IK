using UnityEngine;

[System.Serializable]
public class FakeIKBone {
    
    public Transform Bone;

    private Transform boneParent;
    
    public Transform BoneParent {
        get{
            if (boneParent == null) {
                boneParent = Bone.parent;
            }
            return boneParent;
        }
    }
    
    // 骨骼转动速度
    public float Speed = 30;
    // 骨骼恢复速度
    public float RecoverSpeed = 20;
    // 世界坐标系的绿色轴
    private Vector3 axis => Vector3.up;

    // 世界坐标系的绿色轴方向
    protected Vector3 transformAxis => Bone.rotation * axis;

    // 本地坐标系的绿色轴方向
    protected Vector3 localTransformAxis => Bone.localRotation * axis;

    // 当前转动四元数
    protected Quaternion currentQuaternion = Quaternion.identity;
    // 旋转函数
    public virtual void RotateToTarget(Vector3 targetPosition) { }
}