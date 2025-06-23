using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Mueve el láser hacia adelante en su propia dirección (eje Z local)
            // Usamos transform.forward para la dirección local del láser
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody component not found on Laser! Please add one.", this);
            enabled = false; // Deshabilita el script si no hay Rigidbody
        }

        Destroy(gameObject, lifeTime);
    }
}