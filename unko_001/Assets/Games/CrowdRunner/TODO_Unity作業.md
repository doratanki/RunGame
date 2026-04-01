# Crowd Runner — Unity 作業 TODO

## 1. シーン作成

- [ ] `File > New Scene` で新規シーンを作成
- [ ] `Assets/Games/CrowdRunner/Scenes/CrowdRunner.unity` として保存

---

## 2. Hierarchy 構成

```
CrowdRunner (シーンルート)
├── Main Camera
├── Directional Light
├── GameManager (Empty GameObject)
├── Player (Capsule)
├── Ground (Cube)              ← 地面（ScaleX=10, Y=0.1, Z=200 ぐらい）
├── Crowd_1〜N (Capsule)        ← コース上に手動配置
├── Obstacle_1〜N (Cube)        ← コース上に手動配置
├── EnemyGate (Cube)           ← ゴール手前に配置
└── Canvas
    ├── StartPanel
    │   └── StartButton
    ├── GamePanel
    │   └── MemberCountText (TextMeshProUGUI)
    └── ResultPanel
        ├── ResultText    (TextMeshProUGUI) ← "WIN!" / "LOSE..."
        ├── ScoreText     (TextMeshProUGUI)
        ├── BestText      (TextMeshProUGUI)
        └── RestartButton
```

---

## 3. コンポーネントのアタッチ

### Main Camera
- [ ] `RunnerCamera` をアタッチ
- [ ] `Offset` → `(0, 8, -10)`
- [ ] `Smooth Speed` → `6`
- [ ] `Target` は実行時に CrowdGameManager が自動設定するため空でOK

### GameManager (Empty)
- [ ] `CrowdGameManager` をアタッチ
- [ ] `CrowdRunnerUI` をアタッチ

### Player (Capsule)
- [ ] `RunnerPlayer` をアタッチ
- [ ] Capsule Collider の `Is Trigger` → **オン**
- [ ] 色を付けたい場合は Inspector でマテリアル設定

### Crowd_1〜N (Capsule)
- [ ] `CrowdMember` をアタッチ
- [ ] Capsule Collider の `Is Trigger` → **オン**
- [ ] `Member Color` を Inspector で設定（緑系推奨）

### Obstacle_1〜N (Cube)
- [ ] `Obstacle` をアタッチ
- [ ] Box Collider の `Is Trigger` → **オン**
- [ ] `Loss Count` を Inspector で設定（例: 3〜5）

### EnemyGate (Cube)
- [ ] `EnemyGate` をアタッチ
- [ ] Box Collider の `Is Trigger` → **オン**
- [ ] `Enemy Count` を Inspector で設定（例: 10〜20）
- [ ] Scale を幅広に（ScaleX=10, Y=3, Z=1 ぐらい）

---

## 4. Inspector のアサイン

### CrowdGameManager
| フィールド | アサイン先 |
|---|---|
| Player | Player オブジェクト |
| Runner Camera | Main Camera |
| Crowd Runner UI | Canvas オブジェクト |

### CrowdRunnerUI（Canvas にアタッチ）
| フィールド | アサイン先 |
|---|---|
| Start Panel | StartPanel |
| Game Panel | GamePanel |
| Result Panel | ResultPanel |
| Member Count Text | GamePanel > MemberCountText |
| Result Text | ResultPanel > ResultText |
| Result Score Text | ResultPanel > ScoreText |
| Result Best Text | ResultPanel > BestText |

---

## 5. ボタンの OnClick 設定

| ボタン | OnClick メソッド |
|---|---|
| StartButton | `CrowdRunnerUI.OnStartButton()` |
| RestartButton | `CrowdRunnerUI.OnRestartButton()` |

---

## 6. Canvas 設定

- [ ] `Render Mode` → `Screen Space - Overlay`
- [ ] `Canvas Scaler` → `Scale With Screen Size`、Reference `1080 x 1920`

---

## 7. 動作確認

- [ ] Play → StartPanel が表示される
- [ ] StartButton を押す → プレイヤーが前進する
- [ ] ドラッグで左右に動く
- [ ] Capsule に触れると仲間が追従し、カウントが増える
- [ ] Obstacle に触れると仲間が飛んでカウントが減る
- [ ] EnemyGate に到達すると停止し ResultPanel が表示される
- [ ] 仲間数 > 敵数 → "WIN!"、以下 → "LOSE..."
- [ ] ベストスコアが PlayerPrefs に保存される

---

## 8. 今後（複数ゲーム対応）

- [ ] `Assets/GameSelect/Scenes/` にゲーム選択シーンを作成
- [ ] Stack Tower / Crowd Runner の選択ボタンを配置
- [ ] Build Settings にシーンを登録（GameSelect → StackTower → CrowdRunner）
