using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FootIK
{
    public class FootIK : MonoBehaviour
    {
        public Spine spine;
        public Spine1[] spine1;
        public Spine2 spine2;

        public Transform flTarget;
        public Transform frTarget;
        public Transform blTarget;
        public Transform brTarget;

        private void OnEnable()
        {
            if (!spine)
            {
                spine = GetComponentInChildren<Spine>();
            }
            if (spine1 == null || spine1.Length < 1)
            {
                spine1 = GetComponentsInChildren<Spine1>();
            }
            if (!spine2)
            {
                spine2 = GetComponentInChildren<Spine2>();
            }
            spine.Collect();
            spine2.Collect();
        }

        private void LateUpdate()
        {
            if(spine && spine2 && flTarget && frTarget && blTarget && brTarget)
            {
                var spineOffset = spine.Retarget(blTarget.position, brTarget.position);
                var spine2Offset = spine2.Retarget(flTarget.position, frTarget.position);
                var spine2Position = spine2.transform.position;
                foreach (var bone in spine1)
                {
                    bone.Calculate(spine2Offset, spineOffset);
                }
                spine2.transform.position = spine2Position;
            }
        }
    }
}
