using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class VerticalMovement : MonoBehaviour
{
    
    
    public float amplitude = 1f;
    public float frequency = 1f;
    
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPosition.y + amplitude * Mathf.Sin(frequency * Time.time);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
