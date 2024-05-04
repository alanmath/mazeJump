using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector2 movementDirection;
    private Vector2 touchStart;
    private Rigidbody2D rb;
    private bool canMove = true;  // Permite movimento inicial

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        if (canMove && movementDirection != Vector2.zero)
        {
            Move();
        }
    }

    void HandleInput()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipe = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - touchStart;
            if (canMove)
            {
                DetectSwipeDirection(swipe);
            }
        }
        #elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipe = (Vector2)Camera.main.ScreenToWorldPoint(touch.position) - touchStart;
                if (canMove)
                {
                    DetectSwipeDirection(swipe);
                }
            }
        }
        #endif
    }

    void DetectSwipeDirection(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            movementDirection = swipe.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            movementDirection = swipe.y > 0 ? Vector2.up : Vector2.down;
        }
        canMove = false;  // Impede novos movimentos após o início do primeiro
    }

    void Move()
    {
        rb.velocity = movementDirection * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            float dotProduct = Vector2.Dot(normal, -movementDirection);

            if (dotProduct > 0.5f)  // Colisão frontal
            {
                rb.velocity = Vector2.zero;
                movementDirection = Vector2.zero;
                canMove = true;  // Permite um novo movimento após a colisão
            }
        }
    }
}
