using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Sprite shipIdleA, shipIdleB;
    private bool allowHorizontalMovement = true, allowVerticalMovement = true;
    private const int coordinateLimitX = 54, coordinateLimitY = 60;
    private const int bulletInitialOffsetY = -4;
    private const float movementDelay = 0.008f, idleEffectsDelay = 0.1f, bulletEffectsDelay = 0.002f;
    private float movementTimer, idleEffectsTimer, bulletEffectsTimer;
    private RectTransform player;
    private Image playerImg;
    [SerializeField]
    private RectTransform bulletPrefab;
    private RectTransform bullet;
    private bool bulletShot = false;


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
        if (player.anchoredPosition.x <= -coordinateLimitX) return;
        player.anchoredPosition += Vector2.left;
    }
    private void MoveRight()
    {
        if (player.anchoredPosition.x >= coordinateLimitX) return;
        player.anchoredPosition += Vector2.right;
    }
    private void MoveUp()
    {
        if (player.anchoredPosition.y >= coordinateLimitY) return;
        player.anchoredPosition += Vector2.up;
    }
    private void MoveDown()
    {
        if (player.anchoredPosition.y <= -coordinateLimitY) return;
        player.anchoredPosition += Vector2.down;
    }
    private void CheckMovement()
    {
        if (movementTimer < movementDelay) { movementTimer += Time.deltaTime; return; }
        movementTimer = 0;
        if (allowHorizontalMovement && Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
        if (allowHorizontalMovement && Input.GetKey(KeyCode.RightArrow)) MoveRight();
        if (allowVerticalMovement && Input.GetKey(KeyCode.UpArrow)) MoveUp();
        if (allowVerticalMovement && Input.GetKey(KeyCode.DownArrow)) MoveDown();
        
    }

    private void ShootBullet()
    {
        if (bullet == null) return;
        bulletShot = true;
        bullet.SetParent(player.transform.parent, true);
    }
    private void CheckActions()
    {
        if (!bulletShot && Input.GetKeyDown(KeyCode.RightControl)) ShootBullet();
    }

    private void IdleEffects()
    {
        if (idleEffectsTimer < idleEffectsDelay) { idleEffectsTimer += Time.deltaTime; return; }
        idleEffectsTimer = 0;
        playerImg.sprite = playerImg.sprite != shipIdleA ? shipIdleA : shipIdleB;
    }
    private void BulletEffects()
    {
        if (!(bulletShot && bullet != null)) return;
        if (bulletEffectsTimer < bulletEffectsDelay) { bulletEffectsTimer += Time.deltaTime; return; }
        bulletEffectsTimer = 0;
        bullet.anchoredPosition += Vector2.up;
        if (bullet.anchoredPosition.y >= 0) DestroyBullet();
    }
    private void CheckEffects()
    {
        IdleEffects();
        BulletEffects();
    }
    private void SpawnBullet()
    {
        if (bullet != null) return;
        bulletShot = false;
        bullet = Instantiate(bulletPrefab, player);
        bullet.anchoredPosition = new Vector2(0, bulletInitialOffsetY);
    }
    private void DestroyBullet()
    {
        if (bullet == null) return;
        Destroy(bullet.gameObject);
        bullet = null;
        bulletShot = false;
        SpawnBullet();
    }

    private void Initialize()
    {
        movementTimer = 0;
        player = GetComponent<RectTransform>();
        player.anchoredPosition = new Vector2(0, -coordinateLimitY);
        playerImg = GetComponent<Image>();
        playerImg.sprite = shipIdleA;
        SpawnBullet();
    }
}
