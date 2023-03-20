using UnityEngine;

public abstract class UnitAbility : MonoBehaviour
{
    [SerializeField]
    protected float RateOfFire = 0.5f;

    [SerializeField]
    protected float SecondsUntilNextFire;

    private void Awake()
    {
        ResetSecondsUntilNextShot();
    }

    void FixedUpdate()
    {
        SecondsUntilNextFire -= Time.fixedDeltaTime;
        if (SecondsUntilNextFire <= 0.0f)
        {
            ResetSecondsUntilNextShot();
            UseAbility();
        }
    }

    protected abstract void UseAbility();

    protected virtual void ResetSecondsUntilNextShot()
    {
        SecondsUntilNextFire = 1.0f / RateOfFire;
    }
}
