using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.XR.Interaction.Toolkit;

namespace VR.Weapons
{
    [DisallowMultipleComponent]
    public class GunShooter : MonoBehaviour
    {
        [Header("Setup")] public Transform muzzle; // tip of barrel
        public LayerMask hitMask = ~0;
        public float maxDistance = 100f;
        [Tooltip("Seconds between shots")] public float fireInterval = 0.15f;
        public float damage = 10f;
        public XRBaseController controller; // optional for haptics
        public AudioSource audioSource; // optional
        public AudioClip fireClip; // optional
        public ParticleSystem muzzleFlash; // optional
        public bool automatic = false; // hold trigger

        [Header("Spread")]
        public float spreadAngle = 0f; // degrees full cone

        [Header("Hit VFX")]
        public GameObject hitVfxPrefab; // optional pooled
        public int hitVfxPoolSize = 8;

        public event Action<ShotInfo> OnFired;
        public event Action<HitInfo> OnHit;

        float _nextFireTime;
        readonly System.Collections.Generic.Queue<GameObject> _vfxPool = new();
        bool _triggerHeld;

#if ENABLE_INPUT_SYSTEM
        [Header("Input Action (Optional)")] public InputActionProperty triggerAction; // Value (float)
#endif
        void Awake()
        {
            if (!muzzle) muzzle = transform;
            InitPool();
#if ENABLE_INPUT_SYSTEM
            if (triggerAction.action != null)
            {
                triggerAction.action.started += OnTriggerStarted;
                triggerAction.action.canceled += OnTriggerCanceled;
            }
#endif
        }

        void OnDestroy()
        {
#if ENABLE_INPUT_SYSTEM
            if (triggerAction.action != null)
            {
                triggerAction.action.started -= OnTriggerStarted;
                triggerAction.action.canceled -= OnTriggerCanceled;
            }
#endif
        }
        void InitPool()
        {
            if (!hitVfxPrefab) return;
            for (int i = 0; i < hitVfxPoolSize; i++)
            {
                var go = Instantiate(hitVfxPrefab);
                go.SetActive(false);
                _vfxPool.Enqueue(go);
            }
        }

#if ENABLE_INPUT_SYSTEM
        void OnTriggerStarted(InputAction.CallbackContext ctx)
        {
            _triggerHeld = true;
            TryFire();
        }
        void OnTriggerCanceled(InputAction.CallbackContext ctx) => _triggerHeld = false;
#endif

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (automatic && _triggerHeld) TryFire();
#else
            if (automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) TryFire();
#endif
        }

        public void FireOnceExternal() => TryFire(); // for XR Interaction events

        void TryFire()
        {
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + fireInterval;

            var shot = new ShotInfo
            {
                origin = muzzle.position,
                direction = GetSpreadDirection(muzzle.forward),
                time = Time.time
            };
            PerformRaycast(shot);
            PlayFireFeedback();
            OnFired?.Invoke(shot);
        }

        Vector3 GetSpreadDirection(Vector3 forward)
        {
            if (spreadAngle <= 0f) return forward;
            var halfAngle = spreadAngle * 0.5f;
            var random = UnityEngine.Random.insideUnitCircle;
            var yaw = random.x * halfAngle;
            var pitch = random.y * halfAngle;
            return Quaternion.Euler(pitch, yaw, 0f) * forward;
        }

        void PerformRaycast(ShotInfo shot)
        {
            if (Physics.Raycast(shot.origin, shot.direction, out var hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
            {
                var info = new HitInfo
                {
                    point = hit.point,
                    normal = hit.normal,
                    collider = hit.collider,
                    damage = damage,
                    shot = shot
                };
                if (hit.collider.TryGetComponent<IDamageable>(out var dmg))
                    dmg.ApplyHit(info);
                else
                {
                    var parentDmg = hit.collider.GetComponentInParent<IDamageable>();
                    parentDmg?.ApplyHit(info);
                }
                SpawnHitVfx(info);
                OnHit?.Invoke(info);
            }
        }

        void SpawnHitVfx(HitInfo info)
        {
            if (_vfxPool.Count == 0) return;
            var go = _vfxPool.Dequeue();
            go.transform.SetPositionAndRotation(info.point, Quaternion.LookRotation(info.normal));
            go.SetActive(true);
            var ps = go.GetComponent<ParticleSystem>();
            if (ps) ps.Play();
            StartCoroutine(DisableLater(go, 2f));
        }

        System.Collections.IEnumerator DisableLater(GameObject go, float t)
        {
            yield return new WaitForSeconds(t);
            go.SetActive(false);
            _vfxPool.Enqueue(go);
        }

        void PlayFireFeedback()
        {
            if (muzzleFlash) muzzleFlash.Play();
            if (audioSource && fireClip) audioSource.PlayOneShot(fireClip);
            if (controller != null)
            {
                controller.SendHapticImpulse(0.6f, 0.05f);
            }
        }
    }

    public struct ShotInfo
    {
        public Vector3 origin;
        public Vector3 direction;
        public float time;
    }

    public struct HitInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public Collider collider;
        public float damage;
        public ShotInfo shot;
    }

    public interface IDamageable
    {
        void ApplyHit(HitInfo info);
    }
}
