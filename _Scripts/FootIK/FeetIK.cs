using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FootIK
{
    [Serializable]
    public class FootStruct
    {
        public Transform foot;
        public Transform leg;
        public Transform body;

        public Transform target;

        public void LateUpdate()
        {
            if (!target || !foot || !body)
            {
                return;
            }
            var delta = target.position - foot.position;
            body.position += delta;
        }
    }

    public class FeetIK : MonoBehaviour
    {
        public FootStruct front_l;
        public FootStruct front_r;
        public FootStruct back_l;
        public FootStruct back_r;

        private void LateUpdate()
        {
            front_l.LateUpdate();
            front_r.LateUpdate();
            back_l.LateUpdate();
            back_r.LateUpdate();
        }
    }
}
