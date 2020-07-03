using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{
    float nextShotTime;

    public override void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            Instantiate(bullet, muzzle.position, muzzle.rotation);
            AudioManager.Instance.PlaySound(shootSound, muzzle.position);
            flash.SetActive(true);
            Invoke("DisableFlash", flashTime);
            Instantiate(shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    private void DisableFlash()
    {
        flash.SetActive(false);
    }
}
