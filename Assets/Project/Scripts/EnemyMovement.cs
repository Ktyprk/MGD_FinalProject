using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject target;
    public int damage = 10;
    public float attackRate = 1f;
    public float idleRate = 1f;
    private bool isAttacking = false;
    
    [SerializeField] private float attackRange = 2f;  
    [SerializeField] private Animator animator;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Boss");
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > attackRange  && !isAttacking)
            {
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(target.transform.position);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == target && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(Attack());
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject == target)
    //     {
    //         StopCoroutine(Attack());
    //         isAttacking = false;
    //     }
    // }

    IEnumerator Attack()
    {
        while (isAttacking)
        {
            animator.SetBool("isAttacking", true);
            target.GetComponent<TargetHealthController>()?.TakeDamage(damage);
            yield return new WaitForSeconds(attackRate);
            animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(idleRate);
        }
    }
}