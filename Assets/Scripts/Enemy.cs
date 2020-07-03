using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public ParticleSystem deathEffectPrefab;
    public Spawner mySpawner;

    int health = 5;
    Transform target;
    bool hasTarget;
    float enemyRadius;
    float playerRadius;
    UnityEngine.AI.NavMeshAgent pathfinder;
    Material material;
    Color originalColor;
    void Start()   
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            target = player.transform;
            hasTarget = true;
            playerRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        enemyRadius = GetComponent<CapsuleCollider>().radius;
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
        StartCoroutine("UpdatePath");
    }

    float nextAttackTime = 0.0f;
    float attackDistance = 0.5f;
    void Update()
    {
        float timeBetweenAttacks = 1.5f;
        if((Time.time > nextAttackTime)  && target)
        {
            float distanceSq = (target.position - transform.position).sqrMagnitude;
            float attackDistanceSq = Mathf.Pow(attackDistance + enemyRadius + playerRadius, 2);
            if (distanceSq <= attackDistanceSq)
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Attack()
    {
        material.color = Color.red;

        Vector3 enemyPos = transform.position;
        Vector3 movingDirection = (target.position - transform.position).normalized;
        Vector3 playerPos = target.position - movingDirection * playerRadius;

        float attack_speed = 5;
        float precent = 0.0f;
        bool hasAppliedDamage = false;
        while (precent <= 1)
        {
            float interpolation = (-precent*precent + precent)*4;
            transform.position = Vector3.Lerp(enemyPos, playerPos, interpolation);
            precent += Time.deltaTime * attack_speed;
            if((precent >= 0.5f) && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                if(target)
                {
                    Player player = target.GetComponent<Player>();
                    player.TakeDamage();
                }
            }

            yield return null;
        }

        material.color = originalColor;
    }

#pragma warning disable 0618 // ignore warning that ParticleSystem.startLifeTime is obsolete
    public void TakeDamage(Vector3 hit_point)
    {
        if(--health == 0)
        {
            ParticleSystem deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            // this particle system doesn't use a curve to describe it's lifetime, 
            // so I guess the obsolete proprety can be used...
            Destroy(deathEffect.gameObject, deathEffect.startLifetime);
            mySpawner.killedEnemies++;
            hasTarget = false;
            target = null;
            GameObject.Destroy(gameObject);
        }
    }

    IEnumerator UpdatePath()
    {
        float updateRate = .25f;

        while(target != null)
        {
            if(hasTarget)
            {
                Vector3 movingDirection = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - movingDirection * (enemyRadius + playerRadius + attackDistance / 2.0f);
                pathfinder.SetDestination(targetPosition);
                yield return new WaitForSeconds(updateRate);
            }
        }
    }
}
