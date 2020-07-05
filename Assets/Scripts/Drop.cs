using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public Sprite droppedGunIcon;
    public Gun droppedGun;

    private void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.ChangeGun(droppedGun);
            Destroy(gameObject);
        }
    }
}
