using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRidigBody;
    public float minForce;
    public float maxForce;
    public float lifeTime;
    public float fadeTime;
    
    void Start()
    {
        float force = Random.Range(minForce, maxForce);
        myRidigBody.AddForce(transform.right * force);
        myRidigBody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        Material material = GetComponent<Renderer>().material;
        Color initialColor = material.color;
        float fadeSpeed = 1 / fadeTime;

        float precent = 0;
        while(precent <= 1f)
        {
            precent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(initialColor, Color.clear, precent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
