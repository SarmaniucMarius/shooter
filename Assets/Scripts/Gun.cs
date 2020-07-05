using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool fireTriggerReleased = true;
    public AudioClip shootSound;
    public float reloadTime;
    [NonSerialized]
    public float currentReloadTime;
    [NonSerialized]
    public bool isReloading = false;
    public int totalBulletsInMagazine;
    [NonSerialized]
    public int currentBulletsInMagazine;

    public abstract void Shoot();
}
