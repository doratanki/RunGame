# Unity 残作業リスト

## 1. タイトル変更
- [ ] Player Settings → `productName` を `"Texas Meat Tower"` に
- [ ] Bundle ID → `com.aoyagi.texasmeattower` に変更
- [ ] StartPanel のタイトルテキストを `"TEXAS MEAT TOWER"` に

## 2. Good/Bad ポップアップ UI を作成
- [ ] GamePanel 配下に `GoodPopup` → TextMeshPro「GOOD!」（緑、fontSize=48）
- [ ] GamePanel 配下に `BadPopup` → TextMeshPro「BAD...」（赤、fontSize=48）
- [ ] それぞれに `ComboUIAnimator` コンポーネントをアタッチ
- [ ] `TowerUI` の `Good Animator` / `Bad Animator` にアサイン

## 3. Unity Ads パッケージ確認
- [ ] Package Manager で `com.unity.ads` が入っているか確認
- [ ] 入っていなければ再インストール

## 4. AdsManager をシーンに追加
- [ ] `TowerGameManager` の GameObject に `AdsManager` コンポーネントをアタッチ
- [ ] Inspector で `Game Id Ios` / `Game Id Android` を設定（Unity Dashboard から取得）
- [ ] リリース時は `Test Mode` を false に
