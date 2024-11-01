using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float verticalFollowSpeed = 2f;
    public float xOffset = 0f;
    public float yOffset = -1f;
    public float maxVerticalOffset = 5f;

    public float minX = -10f;
    public float maxX = 1000f;
    public float minY = -50f;
    public float maxY = 15f;

    private Rigidbody2D targetRb;
    private float idleTime = 0f;
    private float idleThreshold = 2f;
    private bool isIdle = false;

    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Check if Mario is moving (by checking Rigidbody2D velocity)
        if (targetRb.linearVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            idleTime = 0f;
            isIdle = false;
        }
        else
        {
            idleTime += Time.deltaTime;
            if (idleTime >= idleThreshold)
            {
                isIdle = true;
            }
        }

        // Calculate target position for X-axis
        float targetXPosition = Mathf.Clamp(target.position.x + xOffset, minX, maxX);

        // Calculate target Y position based on idle state
        float targetYPosition;
        if (isIdle)
        {
            // Keep the camera's current Y position if Mario is idle
            targetYPosition = transform.position.y;
        }
        else
        {
            // Follow Mario's Y position with vertical smoothing
            targetYPosition = Mathf.Clamp(target.position.y + yOffset, minY, maxY);
            targetYPosition = Mathf.Lerp(transform.position.y, targetYPosition, verticalFollowSpeed * Time.deltaTime);
        }

        // Combine X and Y for final target position
        Vector3 targetPosition = new Vector3(targetXPosition, targetYPosition, -10f);

        // Smoothly interpolate to the target position for overall movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
