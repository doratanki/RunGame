# Stack Tower — Unity 作業 TODO

## 1. シーン作成

- [ ] `File > New Scene` で新規シーンを作成
- [ ] `Assets/Games/StackTower/Scenes/StackTower.unity` として保存

---

## 2. Hierarchy 構成

```
StackTower (シーンルート)
├── Main Camera
├── Directional Light
├── GameManager (Empty GameObject)
└── Canvas
    ├── StartPanel
    │   └── StartButton (Button)
    ├── GamePanel
    │   ├── ScoreText (TextMeshProUGUI)
    │   └── ComboText (TextMeshProUGUI)
    ├── ResultPanel
    │   ├── ScoreText      (TextMeshProUGUI)
    │   ├── BestText       (TextMeshProUGUI)
    │   ├── PerfectText    (TextMeshProUGUI)
    │   ├── MaxComboText   (TextMeshProUGUI)
    │   ├── NewBestLabel   (TextMeshProUGUI)   ← NEW BEST 表示
    │   ├── RankDisplay    (Empty GameObject)  ← RankDisplayUI をアタッチ
    │   │   ├── RankLabel  (TextMeshProUGUI)
    │   │   └── RankImage  (Image)
    │   ├── RestartButton     (Button)
    │   └── BackToTitleButton (Button)
    ├── ContinuePanel                          ← ゲームオーバー時に一度だけ表示
    │   ├── ScoreText      (TextMeshProUGUI)   ← 現在スコア表示
    │   ├── CountdownText  (TextMeshProUGUI)   ← カウントダウン表示
    │   ├── ContinueButton (Button)
    │   └── GiveUpButton   (Button)
    ├── CardLotteryPanel                       ← ランク演出終了後に表示
    │   ├── CardArtwork  (Image)
    │   ├── CardNameText (TextMeshProUGUI)
    │   ├── RarityText   (TextMeshProUGUI)     ← レアリティ名（色付き）
    │   └── NewLabel     (TextMeshProUGUI)     ← "NEW!" 新規取得時のみ表示
    ├── GalleryPanel                           ← タイトル画面のギャラリーボタンで開く
    │   ├── OwnedCountText (TextMeshProUGUI)   ← "12 / 30" など所持数
    │   ├── CardTabButton  (Button)
    │   ├── RankTabButton  (Button)
    │   ├── CloseButton    (Button)
    │   ├── CardTabContent (Empty GameObject)
    │   │   └── CardScrollView > Viewport > Content  ← GridLayoutGroup 推奨
    │   └── RankTabContent (Empty GameObject)
    │       └── RankScrollView > Viewport > Content  ← VerticalLayoutGroup 推奨
    ├── CardDetailPopup                        ← カードタップで表示するモーダル
    │   ├── Backdrop       (Image, 半透明黒)   ← タップで閉じる
    │   ├── CardArtwork    (Image)             ← 拡大表示
    │   ├── CardNameText   (TextMeshProUGUI)
    │   ├── RarityText     (TextMeshProUGUI)
    │   ├── UnownedMessage (TextMeshProUGUI)   ← 未所持時のみ表示
    │   └── CloseButton    (Button)
    └── RemoveAdsDialog                        ← 広告削除購入ダイアログ
        ├── UnpurchasedView (Empty GameObject) ← 未購入時に表示
        │   ├── DescriptionText (TextMeshProUGUI) ← 説明文
        │   ├── PurchaseButton  (Button)
        │   └── RestoreButton   (Button)       ← iOS 審査要件
        ├── PurchasedView   (Empty GameObject) ← 購入済み時に表示
        │   └── PurchasedText (TextMeshProUGUI) ← "広告削除済み" など
        ├── StatusText      (TextMeshProUGUI)  ← 処理中・エラー表示
        └── CloseButton     (Button)
```

---

## 3. コンポーネントのアタッチ

### Main Camera
- [ ] `CameraFollow` をアタッチ
- [ ] `Offset` → `(5, 5, -10)`、`Smooth Speed` → `4`

### GameManager
- [ ] `TowerGameManager` をアタッチ
- [ ] `BlockSpawner` をアタッチ

### Canvas（または専用の空 GameObject）
- [ ] `TowerUI` をアタッチ

### ResultPanel
- [ ] `ResultScreenUI` をアタッチ
- [ ] `ResultAnimator` をアタッチ

### RankDisplay（Empty GameObject）
- [ ] `RankDisplayUI` をアタッチ

### ContinuePanel
- [ ] `ContinueDialog` をアタッチ
- [ ] 初期状態は非アクティブにしておく
- [ ] `Countdown Seconds` → `5`（調整可）

### CardLotteryPanel
- [ ] `CardLotteryUI` をアタッチ

### GalleryPanel
- [ ] `GalleryUI` をアタッチ

### CardDetailPopup
- [ ] `CardDetailPopup` をアタッチ
- [ ] 初期状態は非アクティブにしておく

### RemoveAdsDialog
- [ ] `RemoveAdsDialog` をアタッチ
- [ ] 初期状態は非アクティブにしておく

### GameManager
- [ ] `IAPManager` をアタッチ（TowerGameManager / AdsManager と同じ GameObject 推奨）

### GalleryCardCell プレハブの作成
- [ ] `Assets/Games/StackTower/Prefab/` に `GalleryCardCell.prefab` を作成
- [ ] `GalleryCardCell` スクリプトをアタッチし各フィールドをアサイン
  - Artwork (Image) / CardNameText (TextMeshProUGUI) / RarityText (TextMeshProUGUI)
- [ ] セル全体に `Button` コンポーネントをアタッチして `button` フィールドにアサイン

### GalleryRankCell プレハブの作成
- [ ] `Assets/Games/StackTower/Prefab/` に `GalleryRankCell.prefab` を作成
- [ ] `GalleryRankCell` スクリプトをアタッチし各フィールドをアサイン
  - RankImage (Image) / RankLabel (TextMeshProUGUI) / MinScoreText (TextMeshProUGUI)

---

## 4. ランク画像の準備

- [ ] 各ランク用 Sprite 画像（8枚）を `Assets/Games/StackTower/IMage/Rank/` に配置
- [ ] Texture Type を `Sprite (2D and UI)` に変更

| ランク | 画像例         |
|--------|----------------|
| E      | rank_e.png     |
| D      | rank_d.png     |
| C      | rank_c.png     |
| B      | rank_b.png     |
| A      | rank_a.png     |
| S      | rank_s.png     |
| SS     | rank_ss.png    |
| SSS    | rank_sss.png   |

---

## 5. カード画像の準備

- [ ] カード枚数分のイラストを `Assets/Games/StackTower/IMage/Cards/` に配置
- [ ] Texture Type を `Sprite (2D and UI)` に変更

---

## 6. ScriptableObject アセットの作成

### RankTable
- [ ] `Assets/Games/StackTower/` で右クリック → `Create > StackTower > RankTable`
- [ ] 各エントリーに minScore・色・Sprite をアサイン

| ランク | minScore（デフォルト） |
|--------|----------------------|
| E      | 0                    |
| D      | 30                   |
| C      | 60                   |
| B      | 100                  |
| A      | 150                  |
| S      | 220                  |
| SS     | 300                  |
| SSS    | 400                  |

### CardData（カード1枚ごとに作成）
- [ ] `Create > StackTower > CardData` でカード分作成
- [ ] `cardId`（一意な文字列）・`cardName`・`rarity`・`artwork` をセット

### CardPool
- [ ] `Create > StackTower > CardPool` で1つ作成
- [ ] `cards` リストに全 CardData をアサイン

### CardLotteryTable
- [ ] `Create > StackTower > CardLotteryTable` で1つ作成
- [ ] ランク別レアリティ重みを調整（デフォルト値あり）

| ランク | デフォルト排出率                          |
|--------|-----------------------------------------|
| E      | Common 97% / Rare 3%                    |
| D      | Common 90% / Rare 10%                   |
| C      | Common 75% / Rare 23% / Epic 2%         |
| B      | Common 60% / Rare 33% / Epic 7%         |
| A      | Common 40% / Rare 40% / Epic 18% / Legendary 2% |
| S      | Rare 50% / Epic 40% / Legendary 10%     |
| SS     | Rare 25% / Epic 50% / Legendary 25%     |
| SSS    | Epic 30% / Legendary 70%               |

---

## 7. Inspector のアサイン

### TowerUI
| フィールド        | アサイン先              |
|-------------------|------------------------|
| Start Panel       | StartPanel             |
| Game Panel        | GamePanel              |
| Score Text        | GamePanel > ScoreText  |
| Combo Text        | GamePanel > ComboText  |
| Continue Dialog   | ContinuePanel          |
| Result Screen     | ResultPanel            |
| Gallery UI        | GalleryPanel           |
| Remove Ads Dialog | RemoveAdsDialog        |

### ResultScreenUI（ResultPanel にアタッチ）
| フィールド       | アサイン先                    |
|------------------|------------------------------|
| Panel            | ResultPanel 自身             |
| Score Text       | ResultPanel > ScoreText      |
| Best Text        | ResultPanel > BestText       |
| Perfect Text     | ResultPanel > PerfectText    |
| Max Combo Text   | ResultPanel > MaxComboText   |
| New Best Label   | ResultPanel > NewBestLabel   |
| Rank Display     | ResultPanel > RankDisplay    |
| Rank Table       | RankTable.asset              |
| Result Animator  | ResultPanel（同 GameObject） |
| Lottery Table    | CardLotteryTable.asset       |
| Card Pool        | CardPool.asset               |
| Card Lottery UI  | CardLotteryPanel             |

### ResultAnimator（ResultPanel にアタッチ）
| フィールド             | 値（調整可） |
|------------------------|-------------|
| Count Duration         | 2.0         |
| Rank Up Pause Duration | 0.3         |

### RankDisplayUI（RankDisplay にアタッチ）
| フィールド | アサイン先                           |
|------------|-------------------------------------|
| Rank Label | RankDisplay > RankLabel             |
| Rank Image | RankDisplay > RankImage             |
| Animator   | ランクアップ演出用 Animator（省略可）|

### CardLotteryUI（CardLotteryPanel にアタッチ）
| フィールド     | アサイン先                         |
|----------------|-----------------------------------|
| Panel          | CardLotteryPanel 自身             |
| Card Artwork   | CardLotteryPanel > CardArtwork    |
| Card Name Text | CardLotteryPanel > CardNameText   |
| Rarity Text    | CardLotteryPanel > RarityText     |
| New Label      | CardLotteryPanel > NewLabel       |

### GalleryUI（GalleryPanel にアタッチ）
| フィールド           | アサイン先                                           |
|----------------------|-----------------------------------------------------|
| Panel                | GalleryPanel 自身                                   |
| Card Tab Button      | GalleryPanel > CardTabButton                        |
| Rank Tab Button      | GalleryPanel > RankTabButton                        |
| Card Tab Content     | GalleryPanel > CardTabContent                       |
| Rank Tab Content     | GalleryPanel > RankTabContent                       |
| Card Grid            | CardTabContent > CardScrollView > Viewport > Content |
| Rank Grid            | RankTabContent > RankScrollView > Viewport > Content |
| Card Cell Prefab     | GalleryCardCell.prefab                              |
| Rank Cell Prefab     | GalleryRankCell.prefab                              |
| Card Pool            | CardPool.asset                                      |
| Rank Table           | RankTable.asset                                     |
| Owned Count Text     | GalleryPanel > OwnedCountText（省略可）             |
| Card Detail Popup    | CardDetailPopup                                     |

### RemoveAdsDialog（RemoveAdsDialog にアタッチ）
| フィールド        | アサイン先                              |
|-------------------|-----------------------------------------|
| Panel             | RemoveAdsDialog 自身                   |
| Unpurchased View  | RemoveAdsDialog > UnpurchasedView      |
| Purchased View    | RemoveAdsDialog > PurchasedView        |
| Purchase Button   | UnpurchasedView > PurchaseButton       |
| Restore Button    | UnpurchasedView > RestoreButton        |
| Close Button      | RemoveAdsDialog > CloseButton          |
| Status Text       | RemoveAdsDialog > StatusText（省略可） |

### CardDetailPopup（CardDetailPopup にアタッチ）
| フィールド      | アサイン先                              |
|-----------------|----------------------------------------|
| Panel           | CardDetailPopup 自身                   |
| Backdrop        | CardDetailPopup > Backdrop             |
| Artwork         | CardDetailPopup > CardArtwork          |
| Card Name Text  | CardDetailPopup > CardNameText         |
| Rarity Text     | CardDetailPopup > RarityText           |
| Unowned Message | CardDetailPopup > UnownedMessage       |

### TowerGameManager
| フィールド    | アサイン先   |
|---------------|-------------|
| Block Spawner | GameManager |
| Tower UI      | Canvas      |
| Camera Follow | Main Camera |

### BlockSpawner
| フィールド      | 値        |
|-----------------|----------|
| Block Width     | 3        |
| Block Height    | 0.4      |
| Block Depth     | 3        |
| Move Range      | 3        |
| Base Speed      | 2.5      |
| Speed Increment | 0.1      |
| Block Colors    | 任意の5色 |

---

## 8. ボタンの OnClick 設定

| ボタン            | OnClick メソッド                    |
|-------------------|-------------------------------------|
| StartButton       | `TowerUI.OnStartButton()`           |
| RestartButton     | `TowerUI.OnRestartButton()`         |
| BackToTitleButton | `TowerUI.OnBackToTitleButton()`     |
| ContinueButton    | `ContinueDialog.OnContinueButton()` |
| GiveUpButton      | `ContinueDialog.OnGiveUpButton()`   |
| GalleryButton     | `TowerUI.OnGalleryButton()`         |
| CloseButton（ギャラリー） | `TowerUI.OnGalleryCloseButton()` |
| CardTabButton     | `GalleryUI.ShowCardTab()`                    |
| RankTabButton     | `GalleryUI.ShowRankTab()`                    |
| Backdrop（詳細）        | `CardDetailPopup.OnCloseButton()`      |
| CloseButton（詳細）     | `CardDetailPopup.OnCloseButton()`      |
| RemoveAdsButton         | `TowerUI.OnRemoveAdsButton()`          |
| PurchaseButton          | `RemoveAdsDialog.OnPurchaseButton()`   |
| RestoreButton           | `RemoveAdsDialog.OnRestoreButton()`    |
| CloseButton（広告削除） | `RemoveAdsDialog.OnCloseButton()`      |

---

## 9. Canvas 設定

- [ ] `Render Mode` → `Screen Space - Overlay`
- [ ] `Canvas Scaler` → `Scale With Screen Size`、Reference Resolution `1080 x 1920`

---

## 10. Unity IAP セットアップ

### パッケージのインストール
- [ ] `Window > Package Manager > Unity Registry` で `In App Purchasing` を Install

### Product ID の設定
- [ ] [IAPManager.cs](Scripts/IAPManager.cs) の `ProductIdRemoveAds` を自分のバンドルIDに合わせて変更
  - 例: `"com.yourstudio.stacktower.removeads"` → `"com.実際のスタジオ名.stacktower.removeads"`

### iOS（App Store Connect）
- [ ] App Store Connect でアプリ内課金を追加
  - 種類: **非消耗型**
  - 製品ID: `ProductIdRemoveAds` と一致させる
- [ ] 価格・説明を設定して「審査待ち」状態にする

### Android（Google Play Console）
- [ ] Google Play Console でアプリ内アイテムを追加
  - 種類: **管理対象プロダクト（一度のみ購入）**
  - アイテムID: `ProductIdRemoveAds` と一致させる
- [ ] 価格を設定して有効化する

### テスト
- [ ] iOS: Sandbox テスターアカウントで購入テスト
- [ ] Android: ライセンステスターアカウントで購入テスト
- [ ] `IAPManager.cs` の初期化ログ `[IAP] Initialized.` が出ることを確認
- [ ] 購入後に広告が表示されないことを確認
- [ ] アンインストール → 再インストール後に復元できることを確認

---

## 11. ランクアップ Animator（任意・後回し可）

- [ ] RankDisplay に Animator コンポーネントをアタッチ
- [ ] AnimatorController を作成し各ランク名のトリガーを追加
  - トリガー名: `RankE` / `RankD` / `RankC` / `RankB` / `RankA` / `RankS` / `RankSS` / `RankSSS`
- [ ] 各トリガーにパンチスケール・フラッシュ等のアニメーションを設定
- [ ] RankDisplayUI の `Animator` フィールドにアサイン

---

## 12. 動作確認

- [ ] Play → StartPanel が表示される
- [ ] StartButton → ブロックが生成されて左右に動く
- [ ] タップ → ブロックが止まって Slice される
- [ ] スコアが増える / カメラが上に追従する
- [ ] 完全に外れたら ContinuePanel が表示される（初回のみ）
- [ ] ContinueButton → ブロックが初期サイズで再開される
- [ ] カウントダウンが 0 になると自動的にあきらめる扱いになる
- [ ] 2回目のゲームオーバーは直接 ResultPanel が表示される
- [ ] スコアが 0 からカウントアップされる
- [ ] ランク閾値を超えるたびに RankLabel・RankImage が更新される
- [ ] ランク演出完了後に CardLotteryPanel が表示される
- [ ] ランクに応じたレアリティでカードが抽選される
- [ ] 未所持カードなら "NEW!" が表示され PlayerPrefs に保存される
- [ ] 所持済みカードが当選した場合は "NEW!" が表示されない（重複当選）
- [ ] NEW BEST 時に NewBestLabel が表示される
- [ ] PlayerPrefs にベストスコア・所持カードが保存される（再起動後も残る）
- [ ] RestartButton / BackToTitleButton が正常に動作する
- [ ] タイトルの「広告削除」ボタンで RemoveAdsDialog が開く
- [ ] 未購入時は UnpurchasedView が表示される
- [ ] 購入ボタンを押すとストアの購入フローが走る
- [ ] 購入成功後に PurchasedView に切り替わる
- [ ] 購入後ゲームオーバーで広告が表示されなくなる
- [ ] 復元ボタンで購入履歴が復元される
- [ ] タイトルのギャラリーボタンで GalleryPanel が開く
- [ ] カードタブ：全カードがグリッド表示され、未所持は暗転・名前が "???"
- [ ] ランクタブ：全ランクの画像・スコア範囲が表示される
- [ ] 所持数テキストが正しく表示される（例: 3 / 20）
- [ ] カード取得後にギャラリーを開くと所持状況が更新されている
- [ ] カードをタップすると CardDetailPopup が開く
- [ ] 所持済みカードはアートワーク・名前・レアリティがフル表示される
- [ ] 未所持カードは暗転＋"???"＋UnownedMessage が表示される
- [ ] Backdrop またはCloseButton タップでポップアップが閉じる

---

## 13. 今後（複数ゲーム対応）

- [ ] `Assets/GameSelect/Scenes/` にゲーム選択シーンを作成
- [ ] ゲーム選択 → Stack Tower シーンへの遷移を実装
- [ ] Build Settings にシーンを登録（GameSelect → StackTower の順）
