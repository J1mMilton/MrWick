using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 2;
    public int damage = 5;
    public float maxDistance = 10;
     
    
    private Vector2 startPosition;
    private float conquaredDistance;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 targetPosition)
    {
        startPosition = transform.position;

        Vector2 direction = (targetPosition - startPosition).normalized;
        rb2d.velocity = transform.right * speed;

        // Rotate to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        conquaredDistance = Vector2.Distance(transform.position, startPosition);
        if (conquaredDistance >= maxDistance)
        {
            
        }
    }

    private void DisableObject()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided" + collision.name);
        DisableObject();
    }
}
