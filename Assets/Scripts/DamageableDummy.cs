using UnityEngine;
using VR.Weapons;

public class DamageableDummy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public void ApplyHit(HitInfo info)
    {
        health -= info.damage;
        Debug.Log($"Hit {name} for {info.damage}. Health now {health}");
        if (health <= 0f)
        {
            Debug.Log($"{name} destroyed");
            Destroy(gameObject);
        }
    }
}
