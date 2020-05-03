using UnityEngine;

namespace FootIK
{
    public class Spine : MonoBehaviour
    {
        public Limb left;
        public Limb right;
        [Range(0, 1)]
        public float offsetRange = 0.3f;

        public void Collect()
        {
            left = new Limb();
            right = new Limb();
            left.Collect(transform, true);
            right.Collect(transform, false);
        }

        public Vector3 Retarget(Vector3 lTarget, Vector3 rTarget)
        {
            var lOffset = left.VisualTarget(lTarget);
            var rOffset = right.VisualTarget(rTarget);
            var mainOffset = (lOffset + rOffset) / 2;
            if (mainOffset.magnitude > offsetRange)
            {
                mainOffset = mainOffset.normalized* offsetRange;
            }
            transform.position += mainOffset;
            left.Calculate(lTarget, lOffset - mainOffset);
            right.Calculate(rTarget, rOffset - mainOffset);
            return mainOffset;
        }
    }
}