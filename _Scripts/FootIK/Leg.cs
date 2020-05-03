using UnityEngine;

namespace FootIK
{
    public class Leg : MonoBehaviour
    {
        public bool left;
        [Range(0, 1)]
        public float softThreshold = 0.1f;
        [Range(0, 1)]
        public float hardThreshold = 0.6f;


        public Vector3 position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }
    }
}