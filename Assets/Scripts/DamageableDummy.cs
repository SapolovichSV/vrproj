using UnityEngine;
using VR.Weapons;

public class DamageableDummy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public int pointsOnKill = 10; // Points awarded when destroyed

    public void ApplyHit(HitInfo info)
    {
        health -= info.damage;
        Debug.Log($"Hit {name} for {info.damage}. Health now {health}");
        if (health <= 0f)
        {
            // If the GameManager exists, add score
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(pointsOnKill);
            }

            Debug.Log($"{name} destroyed");
            Destroy(gameObject);
        }
    }
}
