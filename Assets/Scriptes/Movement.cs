using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed; // 角色移动速度
    [SerializeField] private float firstJumpForce; // 第一次跳跃的力度
    [SerializeField] private float glideForce = 0.5f; // 滑翔时的额外上升力度
    [SerializeField] private float glideDuration = 1.0f; // 滑翔持续时间
    [SerializeField] private float fallMultiplier = 2.5f; // 下降时的重力倍数
    [SerializeField] private float lowJumpMultiplier = 2f; // 低跳时的重力倍数
    private Rigidbody2D body; // 角色的Rigidbody2D组件
    private bool isGrounded = true; // 角色是否站在地面上
    private bool isGliding = false; // 角色是否正在滑翔
    private float glideTimeLeft; // 剩余的滑翔时间

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 处理水平移动
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // 处理跳跃逻辑
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, firstJumpForce);
            isGrounded = false;
            isGliding = false; // 重置滑翔状态
        }

        // 检查是否达到跳跃顶点并开始滑翔
        if (!isGrounded && !isGliding && body.velocity.y <= 0)
        {
            if (Input.GetButton("Jump"))
            {
                isGliding = true;
                glideTimeLeft = glideDuration;
            }
        }

        // 处理滑翔逻辑
        if (isGliding && glideTimeLeft > 0)
        {
            // 应用一个小的上升力来模拟滑翔，并逐渐减少以模拟下落趋势
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -glideForce));
            glideTimeLeft -= Time.deltaTime;
        }

        // 当角色在下降时，增加额外的重力效果
        if (!isGliding && body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 当角色与地面碰撞时，重置跳跃和滑翔状态
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isGliding = false;
        }
    }
}
