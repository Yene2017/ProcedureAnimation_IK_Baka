using UnityEngine;

namespace FootIK
{
    public class Foot : MonoBehaviour
    {
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