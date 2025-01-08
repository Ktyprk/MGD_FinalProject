using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject target;
    public int damage = 10;  // Verilen hasar
    public float attackRate = 1f; // Saniyede kaç kez hasar
    private bool isAttacking = false;

    
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (!isAttacking)
            {
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(target.transform.position);
                
                gameObject.GetComponent<VerticalMovement>().frequency = 5f;
                if (gameObject.CompareTag("Bat"))
                {
                    gameObject.GetComponent<Spinner>().spinSpeed = 300f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            StopCoroutine(Attack());
            isAttacking = false;
            target.GetComponent<Spinner>().spinSpeed = 0f;
        }
    }
    
    
    IEnumerator Attack()
    {
        
        while (isAttacking)
        {
            if (gameObject.CompareTag("Bat"))
            {
                gameObject.GetComponent<Spinner>().spinSpeed = 900f;
            }
            gameObject.GetComponent<VerticalMovement>().frequency = 20f;
            target.GetComponent<Spinner>().spinSpeed = 200f;
            target.GetComponent<TargetHealthController>().TargetTakeDamage(damage);
            yield return new WaitForSeconds(attackRate);
            
        }
    }
}
