using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 2;
    public int damage = 50;
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
            DisableObject();
            Destroy(gameObject);
        }
    }

    private void DisableObject()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bullet hit the player (assuming the player has the tag "Player")
        if (collision.CompareTag("Player"))
        {
            // Try to get the Character component (or your specific player script)
            Character playerCharacter = collision.GetComponent<Character>();
            if (playerCharacter != null)
            {
                playerCharacter.TakeDamage(damage);
            }
            // Destroy the bullet after it hits
            DisableObject();
            Destroy(gameObject);
        }
        
        if (collision.CompareTag("Destructible"))
        {
            ObstacleHealth health = collision.GetComponent<ObstacleHealth>();
            if (health != null)
            {
                health.TakeDamage(1); // or more, depending on your bullet
            }

            Destroy(gameObject); // destroy the bullet
        }

        if (collision.CompareTag("Indestructible"))
        {
            Destroy(gameObject);
        }
    }
    
    
}
