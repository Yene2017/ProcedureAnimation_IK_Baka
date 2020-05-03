using UnityEngine;

namespace FootIK
{
    public class Spine1 : MonoBehaviour
    {
        [Range(0, 1)]
        public float chestWeight = 1;

        public void Calculate(Vector3 spine2, Vector3 spine)
        {
            transform.position += chestWeight * spine2 + (1 - chestWeight) * spine;
        }
    }
}