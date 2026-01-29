using UnityEngine;
public class RotateObject : MonoBehaviour {
    void Update() {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}
