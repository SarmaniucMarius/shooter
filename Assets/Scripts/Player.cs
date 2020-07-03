using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HealthBar
{
    public Slider slider;
    public Gradient color;
    public Image fill;

    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = color.Evaluate(health / slider.maxValue);
    }

    public void setHealthValue(int health)
    {
        slider.value = health;
        fill.color = color.Evaluate(health / slider.maxValue);
    }
}

[RequireComponent (typeof (Rigidbody))]
public class Player : MonoBehaviour
{
    public Transform hand;
    public Gun gun;
    public float moveSpeed = 5;
    public Crosshair crosshair;
    public HealthBar healthBar;
    public LayerMask collisionMask;
    public ParticleSystem dashEffectPrefab;

    Game game;
    Camera viewCamera;
    Rigidbody rigidBody;
    Vector3 velocity;
    int health = 5;
    bool isDashButtonDown = false;

    void Start()
    {
        game = FindObjectOfType<Game>();
        rigidBody = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
        gun = Instantiate(gun, hand.position, hand.rotation);
        gun.transform.parent = hand.transform;
        healthBar.setMaxHealth(health);
    }

    void Update()
    {
        if(transform.position.y < -50f)
        {
            Destroy(gameObject);
            game.GameOver();
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity = new Vector3(horizontal, 0, vertical).normalized;

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, hand.position);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 correctedLookat = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(correctedLookat);
            crosshair.transform.position = point;
            crosshair.CheckIfEnemyDetected(ray);
        }

        if(Input.GetMouseButton(0))
        {
            gun.Shoot();
            gun.fireTriggerReleased = false;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            gun.fireTriggerReleased = true;
        }
        if(Input.GetMouseButtonUp(1))
        {
            isDashButtonDown = true;
        }
    }

    void FixedUpdate()
    {
        if(isDashButtonDown)
        {
            ParticleSystem deshEffect = Instantiate(dashEffectPrefab, rigidBody.position, Quaternion.identity);
            Destroy(deshEffect, deshEffect.main.duration);
            print(deshEffect.main.duration);

            float dashAmount = 50f;
            float maxDistance = dashAmount * Time.fixedDeltaTime;
            Vector3 newPosition = rigidBody.position + velocity * maxDistance;
            
            RaycastHit raycastHit;
            if(Physics.Raycast(rigidBody.position, velocity, out raycastHit, maxDistance, collisionMask))
            {
                newPosition = raycastHit.point;
            }
            
            rigidBody.MovePosition(newPosition);
            isDashButtonDown = false;
        }
        else
        {
            rigidBody.MovePosition(rigidBody.position + velocity * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void TakeDamage()
    {
        if(--health <= 0)
        {
            GameObject.Destroy(gameObject);
            game.GameOver();
        }
        healthBar.setHealthValue(health);
    }
}
