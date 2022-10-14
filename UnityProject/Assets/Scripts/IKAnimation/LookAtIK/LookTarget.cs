using System;
using UnityEngine;

namespace IKAnimation
{
    public class LookTarget : MonoBehaviour
    {
        
        Vector3 nowPos ;
        private void Awake()
        {
            nowPos = this.transform.position;
        }

        private void Update()
        {
            this.transform.position = new Vector3((nowPos.x +(float) Math.Sin(Time.realtimeSinceStartup)*20),
                nowPos.y + (float)Math.Sin(Time.realtimeSinceStartup)*20,
                nowPos.z + (float)Math.Sin(Time.realtimeSinceStartup)*20);
        }
    }
}