using UnityEngine;
using UnityEngine.UI;

public class AnimateTextureTiling : MonoBehaviour
{
    private RawImage rawImage;
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private float speed = 0.25f;

    private Vector2 offset;

    private void Awake()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        if (rawImage == null)
        {
            Debug.LogWarning("AnimateTextureTiling requires a RawImage component.", this);
        }
    }

    private void Update()
    {
        if (rawImage == null)
        {
            return;
        }

        Vector2 moveDirection = new Vector2(direction.x, direction.y);

        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        moveDirection = moveDirection.normalized;
        offset += moveDirection * speed * Time.deltaTime;

        // Keep UV values wrapped inside [0,1) so the texture never keeps increasing forever.
        float x = Mathf.Repeat(offset.x, 1f);
        float y = Mathf.Repeat(offset.y, 1f);

        Rect uv = rawImage.uvRect;
        rawImage.uvRect = new Rect(x, y, uv.width, uv.height);
    }
}
