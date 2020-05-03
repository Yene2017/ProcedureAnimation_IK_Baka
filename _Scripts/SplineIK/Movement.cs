using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 1;
    public float angleSpeed = 10;
    public void Update()
    {
        var hori = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");
        if (hori != 0 || vert != 0)
        {
            var vector = new Vector3(hori, 0, vert);
            GetComponent<SplineIK.SplineIK>().Cache(vector);
            transform.position += vector * Time.deltaTime * speed;
            var angle = Vector3.Angle(vector.normalized, transform.forward);
            var dir = Vector3.Cross(vector.normalized, transform.forward).y > 0 ? -1 : 1;
            var euler = transform.eulerAngles;
            euler.y += dir * angle * Time.deltaTime * angleSpeed;
            transform.eulerAngles = euler;
        }
    }
}
