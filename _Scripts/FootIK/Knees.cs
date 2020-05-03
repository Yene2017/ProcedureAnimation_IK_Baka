using UnityEngine;

namespace FootIK
{
    public class Knees : MonoBehaviour
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