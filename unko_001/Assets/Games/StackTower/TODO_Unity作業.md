# Stack Tower — Unity 作業 TODO

## 1. シーン作成

- [ ] `File > New Scene` で新規シーンを作成
- [ ] `Assets/Games/StackTower/Scenes/StackTower.unity` として保存

---

## 2. Hierarchy 構成

以下の GameObject を作成する。

```
StackTower (シーンルート)
├── Main Camera
├── Directional Light
├── GameManager (Empty GameObject)
└── Canvas
    ├── StartPanel
    │   └── StartButton (Button)
    ├── GamePanel
    │   └── ScoreText (TextMeshProUGUI)
    └── GameOverPanel
        ├── ScoreLabel (TextMeshProUGUI) ← "SCORE  0"
        ├── BestLabel  (TextMeshProUGUI) ← "BEST   0"
        └── RestartButton (Button)
```

---

## 3. コンポーネントのアタッチ

### Main Camera
- [ ] `CameraFollow` スクリプトをアタッチ
- [ ] `Offset` → `(5, 5, -10)`
- [ ] `Smooth Speed` → `4`
- [ ] `Target` は実行時に TowerGameManager が自動設定するため空でOK

### GameManager
- [ ] `TowerGameManager` スクリプトをアタッチ
- [ ] `BlockSpawner` スクリプトをアタッチ

---

## 4. Inspector のアサイン

### TowerGameManager
| フィールド | アサイン先 |
|---|---|
| Block Spawner | GameManager オブジェクト |
| Tower UI | Canvas オブジェクト |
| Camera Follow | Main Camera |

### BlockSpawner
| フィールド | 値 |
|---|---|
| Block Width | 3 |
| Block Height | 0.4 |
| Block Depth | 3 |
| Move Range | 3 |
| Base Speed | 2.5 |
| Speed Increment | 0.1 |
| Block Colors | 任意の5色（デフォルト値あり） |

### TowerUI（Canvas にアタッチ）
| フィールド | アサイン先 |
|---|---|
| Start Panel | StartPanel |
| Game Panel | GamePanel |
| Game Over Panel | GameOverPanel |
| Score Text | GamePanel > ScoreText |
| Game Over Score Text | GameOverPanel > ScoreLabel |
| Game Over Best Text | GameOverPanel > BestLabel |

---

## 5. ボタンの OnClick 設定

| ボタン | OnClick メソッド |
|---|---|
| StartButton | `TowerUI.OnStartButton()` |
| RestartButton | `TowerUI.OnRestartButton()` |

---

## 6. Canvas 設定

- [ ] Canvas の `Render Mode` → `Screen Space - Overlay`
- [ ] `Canvas Scaler` → `Scale With Screen Size`、Reference Resolution `1080 x 1920`（縦持ち想定）

---

## 7. 動作確認

- [ ] Play ボタンで起動 → StartPanel が表示される
- [ ] StartButton を押す → ブロックが生成されて左右に動く
- [ ] Space / クリック / タップ → ブロックが止まって Slice される
- [ ] ブロックが積まれるたびにスコアが増える
- [ ] カメラが上方向に追従する
- [ ] 完全に外れたら GameOverPanel が表示される
- [ ] PlayerPrefs にベストスコアが保存される（再起動後も残る）
- [ ] RestartButton でシーンがリロードされる

---

## 8. 今後（複数ゲーム対応）

- [ ] `Assets/GameSelect/Scenes/` にゲーム選択シーンを作成
- [ ] ゲーム選択 → Stack Tower シーンへの遷移を実装
- [ ] Build Settings にシーンを登録（GameSelect → StackTower の順）
