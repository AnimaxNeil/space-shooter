using UnityEngine;
using UnityEngine.UI;

public sealed class ShipController : MonoBehaviour
{
    private enum ControllerType { Player, Enemy, }
    [SerializeField]
    private ControllerType controllerType;
    [SerializeField]
    private Sprite shipIdleA, shipIdleB;
    private bool allowHorizontalMovement, allowVerticalMovement;
    private int coordinateLimitX, coordinateLimitY;
    private float shipMovementDelay, idleEffectsDelay, bulletMovementDelay, bulletEffectsDelay;
    private float shipMovementTimer, idleEffectsTimer, bulletMovementTimer, bulletEffectsTimer;
    private float autoBulletFireDelay;
    private float autoBulletFireTimer;
    private RectTransform ship;
    private Image shipImg;
    [SerializeField]
    private RectTransform bulletPrefab;
    [SerializeField]
    private Sprite bulletMovingA, bulletMovingB;
    private RectTransform bullet;
    private Image bulletImg;
    private bool bulletShot;


    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        CheckActions();
        CheckMovement();
        CheckEffects();
    }

    private void MoveLeft()
    {
        if (ship.anchoredPosition.x <= -coordinateLimitX) return;
        ship.anchoredPosition += Vector2.left;
    }
    private void MoveRight()
    {
        if (ship.anchoredPosition.x >= coordinateLimitX) return;
        ship.anchoredPosition += Vector2.right;
    }
    private void MoveUp()
    {
        if (ship.anchoredPosition.y >= coordinateLimitY) return;
        ship.anchoredPosition += Vector2.up;
    }
    private void MoveDown()
    {
        if (ship.anchoredPosition.y <= -coordinateLimitY) return;
        ship.anchoredPosition += Vector2.down;
    }
    private void CheckMovement()
    {
        //common
        if (shipMovementTimer < shipMovementDelay) { shipMovementTimer += Time.deltaTime; return; }
        shipMovementTimer = 0;
        //specifics
        switch (controllerType)
        {
            case ControllerType.Player:
                if (allowHorizontalMovement && Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
                if (allowHorizontalMovement && Input.GetKey(KeyCode.RightArrow)) MoveRight();
                if (allowVerticalMovement && Input.GetKey(KeyCode.UpArrow)) MoveUp();
                if (allowVerticalMovement && Input.GetKey(KeyCode.DownArrow)) MoveDown();
                break;
            case ControllerType.Enemy:
                //MoveDown();
                if (Random.Range(0, 2) == 0 && ship.anchoredPosition.x > -coordinateLimitX) MoveLeft(); else MoveRight();
                break;
        }
    }

    private void SpawnBullet()
    {
        if (bullet != null) return;
        bulletShot = false;
        bullet = Instantiate(bulletPrefab, ship);
        bulletImg = bullet.GetComponent<Image>();
        bulletImg.sprite = Random.Range(0, 2) == 0 ? bulletMovingA : bulletMovingB;
    }
    private void DestroyBullet()
    {
        if (bullet == null) return;
        Destroy(bullet.gameObject);
        bullet = null;
        bulletShot = false;
        SpawnBullet();
    }
    private void ShootBullet()
    {
        if (bullet == null) return;
        bulletShot = true;
        bullet.SetParent(ship.transform.parent, true);
    }
    private void MoveBullet()
    {
        //common
        if (!(bulletShot && bullet != null)) return;
        if (bulletMovementTimer < bulletMovementDelay) { bulletMovementTimer += Time.deltaTime; return; }
        bulletMovementTimer = 0;
        //specifics
        switch (controllerType)
        {
            case ControllerType.Player:
                bullet.anchoredPosition += Vector2.up;
                if (bullet.anchoredPosition.y >= 0) DestroyBullet();
                break;
            case ControllerType.Enemy:
                bullet.anchoredPosition += Vector2.down;
                if (bullet.anchoredPosition.y <= 0) DestroyBullet();
                break;
        }
    }
    private void AutoBulletFire()
    {
        if (autoBulletFireDelay < 0) return;
        if (autoBulletFireTimer < autoBulletFireDelay) { autoBulletFireTimer += Time.deltaTime; return; }
        autoBulletFireTimer = 0;
        SpawnBullet();
        ShootBullet();
    }
    private void CheckActions()
    {
        //common
        AutoBulletFire();
        MoveBullet();
        //specifics
        switch (controllerType)
        {
            case ControllerType.Player:
                if (!bulletShot && Input.GetKeyDown(KeyCode.RightControl)) ShootBullet();
                break;
            case ControllerType.Enemy:
                break;
        }
    }

    private void IdleEffects()
    {
        if (idleEffectsTimer < idleEffectsDelay) { idleEffectsTimer += Time.deltaTime; return; }
        idleEffectsTimer = 0;
        shipImg.sprite = shipImg.sprite != shipIdleA ? shipIdleA : shipIdleB;
    }
    private void BulletEffects()
    {
        if (!(bulletShot && bullet != null)) return;
        if (bulletEffectsTimer < bulletEffectsDelay) { bulletEffectsTimer += Time.deltaTime; return; }
        bulletEffectsTimer = 0;
        bulletImg.sprite = bulletImg.sprite != bulletMovingA ? bulletMovingA : bulletMovingB;
    }
    private void CheckEffects()
    {
        IdleEffects();
        BulletEffects();
    }

    private void SetDefaultValues(ControllerType controllerType)
    {
        //common
        allowHorizontalMovement = true;
        allowVerticalMovement = true;
        coordinateLimitX = 54;
        coordinateLimitY = 60;
        idleEffectsDelay = 0.1f;
        bulletEffectsDelay = 0.1f;
        //specifics
        switch (controllerType)
        {
            case ControllerType.Player:
                shipMovementDelay = 0.008f;
                bulletMovementDelay = 0.004f;
                autoBulletFireDelay = -1f;
                break;
            case ControllerType.Enemy:
                shipMovementDelay = 0.05f;
                bulletMovementDelay = 0.01f;
                autoBulletFireDelay = 1f;
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    private void Initialize()
    {
        //common
        SetDefaultValues(controllerType);
        ship = GetComponent<RectTransform>();
        shipImg = GetComponent<Image>();
        shipImg.sprite = shipIdleA;
        //specifics
        switch (controllerType)
        {
            case ControllerType.Player:
                ship.anchoredPosition = new Vector2(0, -coordinateLimitY);
                SpawnBullet();
                break;
            case ControllerType.Enemy:
                ship.anchoredPosition = new Vector2(0, coordinateLimitY);
                break;
        }
    }
}
