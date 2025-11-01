using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    // Lưu hướng cuối cùng để biết khi dừng sẽ idle hướng nào
    private Vector2 lastMoveDir = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Chuẩn hóa để không nhanh hơn khi đi chéo
        if (moveInput.magnitude > 1)
            moveInput.Normalize();

        bool isMoving = moveInput.magnitude > 0;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetBool("IsMoving", isMoving);

        // Nếu đang di chuyển, lưu lại hướng cuối cùng
        if (isMoving)
        {
            lastMoveDir = moveInput;
        }

        // Lật nhân vật sang trái/phải
        if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // trái
        }
        else if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // phải
        }

        // Cập nhật hướng idle (nếu animator dùng)
        animator.SetFloat("LastMoveX", lastMoveDir.x);
        animator.SetFloat("LastMoveY", lastMoveDir.y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
