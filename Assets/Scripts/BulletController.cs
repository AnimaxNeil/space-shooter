namespace NMX.SpaceShooter.Controllers
{
    using Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class BulletController : MonoBehaviour
    {
        public enum BulletType { PlayerStandard, EnemyDroneStandard, }
        public BulletType bulletType;
        [SerializeField]
        private Sprite idleImgA, idleImgB;
        [HideInInspector]
        public bool moving;
        public static event System.Action<BulletController> OnDestroyed;
        private RectTransform rectTransform;
        private Image image;
        private float movementDelay, idleEffectsDelay;
        private float movementTimer, idleEffectsTimer;


        private void Start()
        {
            SetDefaultValues();
            Initialize();
        }

        private void Update()
        {
            if (!GameStats.running) return;
            CheckMovement();
            CheckEffects();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            //common
            //if (!moving) return;
            ShipController otherShip = other.gameObject.GetComponent<ShipController>();
            BulletController otherBullet = other.gameObject.GetComponent<BulletController>();
            //specific
            switch (bulletType)
            {
                case BulletType.PlayerStandard:
                    if (otherShip != null && otherShip.shipType != ShipController.ShipType.Player) Destroy();
                    else if (otherBullet != null && otherBullet.bulletType != BulletType.PlayerStandard) Destroy();
                    break;
                case BulletType.EnemyDroneStandard:
                    if (otherShip != null && otherShip.shipType == ShipController.ShipType.Player) Destroy();
                    else if (otherBullet != null && otherBullet.bulletType == BulletType.PlayerStandard) Destroy();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void SetDefaultValues()
        {
            idleEffectsDelay = 0.1f;
            //specifics
            switch (bulletType)
            {
                case BulletType.PlayerStandard:
                    movementDelay = 0.008f;
                    break;
                case BulletType.EnemyDroneStandard:
                    movementDelay = 0.016f;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
        private void Initialize()
        {
            //common
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            if (rectTransform == null || image == null) return;
            image.sprite = Random.Range(0, 2) == 0 ? idleImgA : idleImgB;
        }

        public void Destroy()
        {
            if (rectTransform == null) return;
            if (rectTransform.TryGetComponent<Collider2D>(out var collider)) collider.enabled = false;
            OnDestroyed?.Invoke(this);
            Destroy(rectTransform.gameObject);
            rectTransform = null;
            moving = false;
        }
        private void CheckMovement()
        {
            //common
            if (!(moving && rectTransform != null)) return;
            if (movementTimer < movementDelay) { movementTimer += Time.deltaTime; return; }
            //specifics
            switch (bulletType)
            {
                case BulletType.PlayerStandard:
                    rectTransform.anchoredPosition += (int)(movementTimer / movementDelay) * 2 * Vector2.up;
                    if (rectTransform.anchoredPosition.y >= 0) Destroy();
                    break;
                case BulletType.EnemyDroneStandard:
                    rectTransform.anchoredPosition += (int)(movementTimer / movementDelay) * 2 * Vector2.down;
                    if (rectTransform.anchoredPosition.y <= 0) Destroy();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            //common
            movementTimer = 0;
        }

        private void IdleEffects()
        {
            if (image == null) return;
            if (idleEffectsTimer < idleEffectsDelay) { idleEffectsTimer += Time.deltaTime; return; }
            idleEffectsTimer = 0;
            image.sprite = image.sprite != idleImgA ? idleImgA : idleImgB;
        }
        private void CheckEffects()
        {
            IdleEffects();
        }

    }
}