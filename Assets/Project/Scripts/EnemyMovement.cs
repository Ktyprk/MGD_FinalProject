using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > 3f)
            {
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(target.transform.position);
            }
        }
    }
}
