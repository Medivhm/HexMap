using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;  // �洢Ҫ���ŵ�����֡ͼƬ
    public float frameRate = 10f;  // ÿ�벥�Ŷ���֡

    private SpriteRenderer spriteRenderer;  // ������ʾSprite�����
    private int currentFrame = 0;  // ��ǰ���ŵ�֡
    private float frameTimer = 0f;  // ��ʱ�������ڿ���֡�Ĳ����ٶ�

    void Start()
    {
        // ��ȡSpriteRenderer���
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ���û�й���SpriteRenderer������������
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer component found!");
        }
    }

    void Update()
    {
        if (sprites.Length == 0 || spriteRenderer == null)
        {
            return;  // û��ͼƬ����û��SpriteRendererʱ�����κδ���
        }

        // ÿ֡���Ӽ�ʱ��
        frameTimer += Time.deltaTime;

        // �����ʱ��������ÿ֡��Ҫ��ʱ��
        if (frameTimer >= 1f / frameRate)
        {
            // ���ü�ʱ��
            frameTimer = 0f;

            // �л�����һ֡
            currentFrame++;

            // �����ǰ֡�����˶���֡�������������¿�ʼ
            if (currentFrame >= sprites.Length)
            {
                currentFrame = 0;
            }

            // ����SpriteRenderer��spriteΪ��ǰ֡
            spriteRenderer.sprite = sprites[currentFrame];
        }
    }
}