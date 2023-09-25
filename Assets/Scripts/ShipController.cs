namespace NMX.SpaceShooter.Controllers
{
    using Data;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class ShipController : MonoBehaviour
    {
        public enum ShipType { Player, EnemyDrone, }
        public ShipType shipType;
        [SerializeField]
        private Sprite idleImgA, idleImgB;
        [SerializeField]
        private BulletController bulletPrefab;
        public static event System.Action<ShipController> OnDestroyed, OnBulletFired;
        private BulletController bullet;
        private RectTransform rectTransform;
        private Image image;
        private bool allowHorizontalMovement, allowVerticalMovement;
        private int coordinateLimitX, coordinateLimitY;
        private float movementDelay, idleEffectsDelay;
        private float movementTimer, idleEffectsTimer;
        private enum AutoMovementDirection { Left, Right }
        private AutoMovementDirection autoMovementDirection;
        private bool autoMovementLeftDone, autoMovementRightDone;


        private void Start()
        {
            SetDefaultValues();
            Initialize();
        }
        private void Update()
        {
            if (!GameStats.running) return;
            CheckActions();
            CheckMovement();
            CheckEffects();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            //commom
            if (other.gameObject == bulletPrefab.gameObject) return;
            ShipController otherShip = other.gameObject.GetComponent<ShipController>();
            BulletController otherBullet = other.gameObject.GetComponent<BulletController>();
            //specific
            switch (shipType)
            {
                case ShipType.Player:
                    if (otherShip != null && otherShip.shipType != ShipType.Player) Destroy();
                    else if (otherBullet != null && otherBullet.bulletType != BulletController.BulletType.PlayerStandard) Destroy();
                    break;
                case ShipType.EnemyDrone:
                    if (otherShip != null && otherShip.shipType == ShipType.Player) Destroy();
                    else if (otherBullet != null && otherBullet.bulletType == BulletController.BulletType.PlayerStandard) Destroy();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void SetDefaultValues()
        {
            //common
            coordinateLimitX = 50;
            allowHorizontalMovement = true;
            allowVerticalMovement = true;
            idleEffectsDelay = 0.1f;
            //specifics
            switch (shipType)
            {
                case ShipType.Player:
                    coordinateLimitY = 60;
                    movementDelay = 0.008f;
                    break;
                case ShipType.EnemyDrone:
                    coordinateLimitY = 80;
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
            image.sprite = idleImgA;
            //specifics
            switch (shipType)
            {
                case ShipType.Player:
                    rectTransform.anchoredPosition = new Vector2(0, -coordinateLimitY);
                    if (bullet == null) SpawnBullet();
                    break;
                case ShipType.EnemyDrone:
                    rectTransform.anchoredPosition = new Vector2(Random.Range(-coordinateLimitX, coordinateLimitX + 1), coordinateLimitY);
                    autoMovementDirection = (AutoMovementDirection)Random.Range(0, 2);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public void Destroy()
        {
            //common
            if (rectTransform == null) return;
            if (rectTransform.TryGetComponent<Collider2D>(out var collider)) collider.enabled = false;
            OnDestroyed?.Invoke(this);
            Destroy(rectTransform.gameObject);
            rectTransform = null;
        }

        private void MoveLeft()
        {
            if (rectTransform.anchoredPosition.x <= -coordinateLimitX) return;
            rectTransform.anchoredPosition += (int)(movementTimer/movementDelay) * Vector2.left;
        }
        private void MoveRight()
        {
            if (rectTransform.anchoredPosition.x >= coordinateLimitX) return;
            rectTransform.anchoredPosition += (int)(movementTimer / movementDelay) * Vector2.right;
        }
        private void MoveUp()
        {
            if (rectTransform.anchoredPosition.y >= coordinateLimitY) return;
            rectTransform.anchoredPosition += (int)(movementTimer / movementDelay) * Vector2.up;
        }
        private void MoveDown()
        {
            if (rectTransform.anchoredPosition.y <= -coordinateLimitY) return;
            rectTransform.anchoredPosition += (int)(movementTimer / movementDelay) * Vector2.down;
        }
        private void CheckMovement()
        {
            //common
            if (rectTransform == null) return;
            if (movementTimer < movementDelay) { movementTimer += Time.deltaTime; return; }
            //specifics
            switch (shipType)
            {
                case ShipType.Player:
                    if (allowHorizontalMovement && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))) MoveLeft();
                    if (allowHorizontalMovement && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))) MoveRight();
                    if (allowVerticalMovement && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) MoveUp();
                    if (allowVerticalMovement && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))) MoveDown();
                    break;
                case ShipType.EnemyDrone:
                    if (rectTransform.anchoredPosition.y <= -coordinateLimitY || rectTransform.anchoredPosition.y > coordinateLimitY) { Destroy(); return; }
                    if (rectTransform.anchoredPosition.x <= -coordinateLimitX) autoMovementLeftDone = true;
                    if (rectTransform.anchoredPosition.x >= coordinateLimitX) autoMovementRightDone = true;
                    if (autoMovementLeftDone && autoMovementRightDone || (Random.Range(0, 9) == 0)) 
                    { MoveDown(); autoMovementLeftDone = autoMovementRightDone = false; }
                    if (autoMovementLeftDone && !autoMovementRightDone) autoMovementDirection = AutoMovementDirection.Right;
                    if (!autoMovementLeftDone && autoMovementRightDone) autoMovementDirection = AutoMovementDirection.Left;
                    if (!autoMovementLeftDone && autoMovementDirection == AutoMovementDirection.Left) MoveLeft();
                    if (!autoMovementRightDone && autoMovementDirection == AutoMovementDirection.Right) MoveRight();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            //common
            movementTimer = 0;
        }

        private void SpawnBullet()
        {
            bullet = Instantiate(bulletPrefab, rectTransform);
            if (bullet.TryGetComponent(out Collider2D collider)) collider.enabled = false;
            if (collider != null) collider.enabled = false;
            bullet.moving = false;
        }
        private void ShootBullet()
        {
            if (bullet == null) return;
            if (bullet.TryGetComponent(out Collider2D collider)) collider.enabled = true;
            OnBulletFired?.Invoke(this);
            bullet.transform.SetParent(rectTransform.parent, true);
            bullet.moving = true;
        }
        private void CheckActions()
        {
            //specifics
            switch (shipType)
            {
                case ShipType.Player:
                    if (bullet == null) SpawnBullet();
                    if (!bullet.moving && Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Space)) ShootBullet();
                    break;
                case ShipType.EnemyDrone:
                    if (bullet == null) SpawnBullet();
                    if (!bullet.moving) ShootBullet();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
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