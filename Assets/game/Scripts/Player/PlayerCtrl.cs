using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCtrl : MonoBehaviour
{
    private Animator anim;
    //
    public float movSpeed = 5f;

    private float speedX, speedY;
    private Rigidbody2D rb;

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("isAttacking", false);
    }

    //
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        bool isMoving = moveX != 0 || moveY != 0;

        anim.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isAttacking"))
        {
            anim.SetBool("isAttacking", true);
            StartCoroutine(ResetAttack());
        }

        Vector3 scale = transform.localScale;

        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movSpeed;

        if (moveX > 0)
            scale.x = Mathf.Abs(scale.x);
        else if (moveX < 0)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(speedX, speedY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Zderzenie z przeciwnikiem!");
        }
    }
}
