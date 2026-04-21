using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
}
