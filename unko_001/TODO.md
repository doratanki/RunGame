# 手動対応 残タスク

## 1. AdsManager — Inspector設定

`AdsManager` コンポーネントのInspectorを開いて以下を設定する。

| フィールド | 設定値 |
|-----------|--------|
| Game Id Ios | Unity Dashboard の本番 iOS ゲームID |
| Game Id Android | Unity Dashboard の本番 Android ゲームID |
| Interstitial Ad Unit Id Ios | 本番 Interstitial AdUnit ID (iOS) |
| Interstitial Ad Unit Id Android | 本番 Interstitial AdUnit ID (Android) |
| Rewarded Ad Unit Id Ios | 本番 Rewarded AdUnit ID (iOS) |
| Rewarded Ad Unit Id Android | 本番 Rewarded AdUnit ID (Android) |
| Test Mode | リリース前に **false** に変更 |

---

## 2. GameConfig ScriptableObject — 作成 & アサイン

1. Projectウィンドウで右クリック →  `Create > StackTower > GameConfig`
2. 生成されたアセットを `TowerGameManager` の **Game Config** フィールドにドラッグ&ドロップ
3. 必要に応じて各パラメータを調整する

| パラメータ | デフォルト値 | 説明 |
|-----------|------------|------|
| Perfect Threshold | 0.1 | これ未満のズレはPerfect判定 |
| Good Ratio | 0.35 | トリム量がブロック幅の35%未満はGood判定 |
| Foundation Hit Multiplier | 1.7 | 基礎直上ブロックのヒット範囲倍率 |
| Max Combo Cap | 5 | Perfect時のコンボボーナス上限 |
| Continue Chance | 0.3 | ゲームオーバー時にコンティニューダイアログが出る確率 |
