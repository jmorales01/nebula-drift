using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // La nave del jugador que la cámara seguirá
    public Vector3 offset = new Vector3(0f, 5f, -15f); // Posición relativa de la cámara respecto al target
    public float smoothSpeed = 0.125f; // Qué tan rápido la cámara se "suaviza" hacia la posición deseada

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is not assigned!", this);
            return;
        }

        // Calcula la posición deseada de la cámara
        // Es la posición del target más el offset
        Vector3 desiredPosition = target.position + offset;

        // Suaviza la posición actual de la cámara hacia la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}