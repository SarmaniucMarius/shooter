using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask;

    float speed = 10.0f;
    int lifeTime = 3;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            Enemy enemy = initialCollisions[0].GetComponent<Enemy>();
            enemy.TakeDamage(transform.position);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            Enemy enemy = rayHit.collider.GetComponent<Enemy>();
            enemy.TakeDamage(rayHit.point);
            Destroy(gameObject);
        }
        transform.Translate(Vector3.forward * moveDistance);
    }
}
