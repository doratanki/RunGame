using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ボールの前方にタイルを動的生成し、後方のタイルを自動削除する。
/// </summary>
public class TileSpawner : MonoBehaviour
{
    [Header("タイルサイズ")]
    public float tileWidth  = 2f;
    public float tileHeight = 0.3f;
    public float tileDepth  = 2f;

    [Header("生成設定")]
    public float initialGapX    = 2.5f;   // 初期タイル間隔
    public float gapIncrement   = 0.05f;  // スコアごとの間隔増加量
    public float maxGapX        = 5f;     // 間隔の上限
    public float yVariance      = 1.5f;   // Y 軸のばらつき幅（±）
    public float spawnAheadDist = 20f;    // ボール前方何ユニット先まで生成するか
    public float despawnBehind  = 15f;    // ボール後方何ユニット後ろで削除するか
    public int   initialCount   = 8;      // ゲーム開始時に敷く初期タイル数

    [Header("カラーパレット")]
    public Color[] tileColors = new Color[]
    {
        new Color(0.3f, 0.7f, 1.0f),
        new Color(0.4f, 1.0f, 0.5f),
        new Color(1.0f, 0.8f, 0.2f),
        new Color(1.0f, 0.4f, 0.6f),
        new Color(0.7f, 0.4f, 1.0f),
    };

    private Transform _ball;
    private float _nextTileX;
    private float _lastTileY;
    private int _colorIndex = 0;
    private bool _isSpawning = false;

    private readonly List<GameObject> _tiles = new List<GameObject>();

    public void StartSpawning(Transform ball)
    {
        _ball = ball;
        _isSpawning = true;
        _nextTileX = ball.position.x;
        _lastTileY = ball.position.y - 1f;
        _colorIndex = 0;

        // 初期タイルを敷く
        for (int i = 0; i < initialCount; i++)
            SpawnTile();
    }

    void Update()
    {
        if (!_isSpawning || _ball == null) return;

        // 前方に不足があれば生成
        while (_nextTileX < _ball.position.x + spawnAheadDist)
            SpawnTile();

        // 後方タイルを削除
        _tiles.RemoveAll(t =>
        {
            if (t == null) return true;
            if (t.transform.position.x < _ball.position.x - despawnBehind)
            {
                Destroy(t);
                return true;
            }
            return false;
        });
    }

    void SpawnTile()
    {
        int score = BallGameManager.Instance != null ? BallGameManager.Instance.Score : 0;
        float gap = Mathf.Min(initialGapX + score * gapIncrement, maxGapX);

        // Y 軸をランダムにばらつかせる（急激な変化を抑えるため前タイルから ±yVariance）
        float newY = _lastTileY + Random.Range(-yVariance, yVariance);
        newY = Mathf.Clamp(newY, -2f, 3f);  // 上下限

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Tile";
        go.transform.position = new Vector3(_nextTileX, newY, 0f);
        go.transform.localScale = new Vector3(tileWidth, tileHeight, tileDepth);

        Color color = tileColors[_colorIndex % tileColors.Length];
        _colorIndex++;

        BallTile tile = go.AddComponent<BallTile>();
        tile.Initialize(color);

        _tiles.Add(go);

        _lastTileY = newY;
        _nextTileX += tileWidth + gap;
    }
}
