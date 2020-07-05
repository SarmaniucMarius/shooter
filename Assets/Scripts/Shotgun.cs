using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [Range(0, 5)]
    public int burstCount;
    float nextShotTime;

    private void Start()
    {
        currentBulletsInMagazine = totalBulletsInMagazine;
    }

    private void Update()
    {
        if (isReloading)
        {
            if (Time.time >= currentReloadTime)
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
                for (int i = 0; i < burstCount; i++)
                {
                    int x = Random.Range(-20, 20);
                    int y = Random.Range(-10, 10);
                    Vector3 bulletAngle = muzzle.eulerAngles + new Vector3(x, y, 0);
                    Instantiate(bullet, muzzle.position, Quaternion.Euler(bulletAngle));
                }
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
