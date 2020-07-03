using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Bullet bullet;
    public Transform shellEjectionPoint;
    public Shell shell;
    [Range(0.1f, 1f)]
    public float timeBetweenShots;
    public GameObject flash;
    public float flashTime;
    [NonSerialized]
    public bool fireTriggerReleased = false;
    public AudioClip shootSound;

    public abstract void Shoot();
}
