using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;  // 存储要播放的所有帧图片
    public float frameRate = 10f;  // 每秒播放多少帧

    private SpriteRenderer spriteRenderer;  // 用于显示Sprite的组件
    private int currentFrame = 0;  // 当前播放的帧
    private float frameTimer = 0f;  // 计时器，用于控制帧的播放速度

    void Start()
    {
        // 获取SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 如果没有挂载SpriteRenderer组件，输出警告
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer component found!");
        }
    }

    void Update()
    {
        if (sprites.Length == 0 || spriteRenderer == null)
        {
            return;  // 没有图片或者没有SpriteRenderer时不做任何处理
        }

        // 每帧增加计时器
        frameTimer += Time.deltaTime;

        // 如果计时器超过了每帧需要的时间
        if (frameTimer >= 1f / frameRate)
        {
            // 重置计时器
            frameTimer = 0f;

            // 切换到下一帧
            currentFrame++;

            // 如果当前帧超过了动画帧的总数，则重新开始
            if (currentFrame >= sprites.Length)
            {
                currentFrame = 0;
            }

            // 设置SpriteRenderer的sprite为当前帧
            spriteRenderer.sprite = sprites[currentFrame];
        }
    }
}