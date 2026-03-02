using UnityEngine;

public class ZombieChasePlayer : MonoBehaviour
{
    public Transform target;
    public float speed = 1.5f;
    public float stopDistance = 1.5f;
    public float rotateSpeed = 5f;

    void Start()
    {
        if (!target && Camera.main) target = Camera.main.transform;
    }

    void Update()
    {
        if (!target) return;
        var dir = target.position - transform.position;
        dir.y = 0f;
        var dist = dir.magnitude;
        if (dist <= stopDistance) return;
        var desiredRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
