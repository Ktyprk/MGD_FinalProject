using UnityEngine;

public class Spinner : MonoBehaviour
{

public GameObject spinningObject;
[SerializeField] private float spinSpeed = 100f;

    // Update is called once per frame
    void Update()
    {
        spinningObject.GetComponent<Transform>().RotateAround(spinningObject.GetComponent<Transform>().position, Vector3.up, spinSpeed * Time.deltaTime);
    }
}
