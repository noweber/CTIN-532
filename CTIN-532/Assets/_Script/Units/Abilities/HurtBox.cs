using UnityEngine;
using static MapNodeController;

public class HurtBox : MonoBehaviour
{
    public Player Owner;

    [SerializeField]
    public float Damage = 1;

    [SerializeField]
    private float rateOfFire = 1;

    [SerializeField]
    private float secondRemainingUntilNextHurt;

    [SerializeField]
    private bool destroyOnContact = false;

    [SerializeField]
    private GameObject particlesPrefab;

    public void SetRateOfFire(float value)
    {
        rateOfFire = value;
    }

    void Awake()
    {
        ResetHurtTimer();
    }

    protected virtual void DamageHitBox(HitBox hitBox)
    {
        hitBox.ReceiveDamage(Damage);

        if (particlesPrefab != null)
        {
            Instantiate(particlesPrefab, transform);
        }
    }

    protected virtual void ResetHurtTimer()
    {
        if (rateOfFire == 0)
        {
            secondRemainingUntilNextHurt = 0;
        }
        else
        {
            secondRemainingUntilNextHurt = 1.0f / rateOfFire;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (secondRemainingUntilNextHurt > 0.0f)
        {
            secondRemainingUntilNextHurt -= Time.fixedDeltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        HandleTriggerCollision(other);
    }

    private void HandleTriggerCollision(Collider other)
    {
        if (secondRemainingUntilNextHurt > 0.0f)
        {
            return;
        }

        if (other.TryGetComponent<HitBox>(out var hitBox))
        {
            if (hitBox.Owner != this.Owner)
            {
                DamageHitBox(hitBox);


                if (destroyOnContact)
                {
                    Destroy(gameObject);
                }
                else
                {
                    ResetHurtTimer();
                }
            }
        }
    }
}
