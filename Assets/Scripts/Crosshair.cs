using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask enemy;
    public SpriteRenderer dotRenderer;
    public Color newDotColor;
    Color oldDotColor;

    Vector3 initialScale;
    Vector3 attackScale;

    private void Start()
    {
        Cursor.visible = false;
        oldDotColor = dotRenderer.color;
        initialScale = transform.localScale;
        attackScale = initialScale * 0.8f;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    public void CheckIfEnemyDetected(Ray ray)
    {
        if(Physics.Raycast(ray, 100, enemy))
        {
            transform.localScale = attackScale;
            dotRenderer.color = newDotColor;
        }
        else
        {
            transform.localScale = initialScale;
            dotRenderer.color = oldDotColor;
        }
    }
}
