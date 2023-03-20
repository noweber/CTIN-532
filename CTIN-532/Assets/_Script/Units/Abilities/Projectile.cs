using UnityEngine;

public class Projectile : HurtBox
{
    [SerializeField]
    private float speed = 4;

    [SerializeField]
    private Vector3 direction;

    public void SetTarget(Vector3 targetPosition)
    {
        direction = targetPosition - this.transform.position;
    }

    void FixedUpdate()
    {
        if (direction == Vector3.zero)
        {
            Debug.LogError("Projectile's direction was not set.");
            Destroy(gameObject);
        }

        this.transform.position += speed * direction * Time.fixedDeltaTime;
    }
}
