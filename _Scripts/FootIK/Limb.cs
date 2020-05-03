using System;
using UnityEngine;

namespace FootIK
{
    public struct Length
    {
        public float min;
        public float max;
    }

    [Serializable]
    public struct Limb
    {
        public Leg leg;
        public Knees knees;
        public Foot foot;

        public Vector3 target;
        Bone[] bones;

        public void Collect(Transform t, bool left)
        {
            foreach (var leg in t.GetComponentsInChildren<Leg>())
            {
                if (leg.left == left)
                {
                    this.leg = leg;
                    break;
                }
            }
            this.knees = this.leg.GetComponentInChildren<Knees>();
            this.foot = this.leg.GetComponentInChildren<Foot>();
            bones = new Bone[3];
            bones[0] = new Bone();
            bones[1] = new Bone();
            bones[2] = new Bone();
        }

        public Vector3 VisualTarget(Vector3 target)
        {
            Vector3 offset = Vector3.zero;
            var divide = Mathf.Sqrt((target - foot.position).sqrMagnitude / (leg.position - foot.position).sqrMagnitude);
            var softDivide = leg.softThreshold;
            var hardDivide = leg.hardThreshold;
            if (divide > hardDivide)
            {
                var delta = target - foot.position;
                var holdTarget = foot.position + (delta / divide * hardDivide);
                offset = target - holdTarget;
                target = holdTarget;
            }
            else if (divide > softDivide)
            {
                var delta = target - foot.position;
                var proportion = softDivide + (divide - softDivide) * Mathf.InverseLerp(softDivide, hardDivide, divide);
                var holdTarget = foot.position + (delta / divide * proportion);
                offset = target - holdTarget;
                target = holdTarget;
            }
            this.target = target;
            return offset;
        }

        public void Calculate(Vector3 target, Vector3 offset)
        {
            leg.position += offset;
            if (!foot || !knees)
            {
                return;
            }
            SetBone();
            Curle(target);
            GetBone();
        }

        private void Curle(Vector3 target)
        {
            var normal = Vector3.Cross(knees.position - leg.position, foot.position - knees.position);
            DebugPoint(Color.white);
            Debug.DrawLine(knees.position, knees.position + normal * 2, Color.blue);
            IKSolverUtil.SolveTrigonometric(bones, 0, 1, 2, target, normal, 1);
            DebugPoint(Color.green);
        }

        void DebugPoint(Color color)
        {
            Debug.DrawLine(leg.position, knees.position, color);
            Debug.DrawLine(foot.position, knees.position, color);
            Debug.DrawLine(foot.position, leg.position, color);
        }

        void SetBone()
        {
            bones[0].position = leg.position;
            bones[0].rotation = leg.transform.rotation;
            bones[1].position = knees.position;
            bones[1].rotation = knees.transform.rotation;
            bones[2].position = foot.position;
            bones[2].rotation = foot.transform.rotation;
        }

        void GetBone()
        {
            leg.position = bones[0].position;
            leg.transform.rotation = bones[0].rotation;
            knees.position = bones[1].position;
            knees.transform.rotation = bones[1].rotation;
            foot.position = bones[2].position;
            foot.transform.rotation = bones[2].rotation;
        }
    }
}