using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1.5f;
    public Vector3 floatDirection = Vector3.up;

    private float timer = 0f;

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Make it face camera directly
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
