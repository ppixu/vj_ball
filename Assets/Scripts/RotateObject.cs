using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float angularSpeed;
    [SerializeField] Vector3 axis;
    void Update() {
        Quaternion rotation = Quaternion.AngleAxis(this.angularSpeed, this.axis);
        this.gameObject.transform.rotation = this.gameObject.transform.rotation * rotation;
    }
}
