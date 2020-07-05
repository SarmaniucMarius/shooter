using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    float nextShotTime;

    private void Start()
    {
        currentBulletsInMagazine = totalBulletsInMagazine;
    }

    private void Update()
    {
        if(isReloading)
        {
            if(Time.time >= currentReloadTime)
            {
                isReloading = false;
                currentBulletsInMagazine = totalBulletsInMagazine;
            }
        }
    }

    public override void Shoot()
    {
        if ((Time.time > nextShotTime) && fireTriggerReleased)
        {
            if (currentBulletsInMagazine > 0)
            {
                Instantiate(bullet, muzzle.position, muzzle.rotation);
                AudioManager.Instance.PlaySound(shootSound, muzzle.position);
                flash.SetActive(true);
                Invoke("DisableFlash", flashTime);
                Instantiate(shell, shellEjectionPoint.position, shellEjectionPoint.rotation);

                currentBulletsInMagazine--;
                if(currentBulletsInMagazine == 0)
                {
                    currentReloadTime = Time.time + reloadTime;
                    isReloading = true;
                }

                nextShotTime = Time.time + timeBetweenShots;
            }
        }
    }

    private void DisableFlash()
    {
        flash.SetActive(false);
    }
}
