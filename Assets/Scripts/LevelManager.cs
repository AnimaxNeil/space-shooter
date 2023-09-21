using UnityEngine;

public sealed class LevelManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform gameArea;
    [SerializeField]
    private ShipController playerPrefab, enemyPrefab;


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerPrefab = Instantiate(playerPrefab, gameArea);
        enemyPrefab = Instantiate(enemyPrefab, gameArea);
    }
}
