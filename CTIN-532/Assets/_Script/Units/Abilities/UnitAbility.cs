using UnityEngine;

public abstract class UnitAbility : MonoBehaviour
{
    [SerializeField]
    protected float AbilityCooldown = 0.5f;

    [SerializeField]
    protected float CooldownRemainingInSeconds;

    private void Awake()
    {
        ResetCooldownTimer();
    }

    void FixedUpdate()
    {
        CooldownRemainingInSeconds -= Time.fixedDeltaTime;
        if (CooldownRemainingInSeconds <= 0.0f)
        {
            ResetCooldownTimer();
            UseAbility();
        }
    }

    protected abstract void UseAbility();

    protected virtual void ResetCooldownTimer()
    {
        CooldownRemainingInSeconds = 1.0f / AbilityCooldown;
    }
}
