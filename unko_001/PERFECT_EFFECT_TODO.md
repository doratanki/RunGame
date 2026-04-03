# パーフェクトエフェクト Unity 残作業

## 1. PerfectEffectManager をアタッチ
- `TowerGameManager` と同じ GameObject に `PerfectEffectManager` コンポーネントを追加

---

## 2. 画面フラッシュ用 Image を作成
1. `Canvas/SafeArea` 直下に空の GameObject を作成 → 名前: `FlashImage`
2. `Image` コンポーネントを追加
3. RectTransform を全画面ストレッチに設定（Anchor: stretch/stretch、offset全て0）
4. Color を白 `(1, 1, 0.6, 0.6)` に設定
5. GameObject を **非アクティブ** にしておく
6. `PerfectEffectManager` の `Flash Image` フィールドにアサイン

---

## 3. ビネット用 Image を作成
1. `Canvas/SafeArea` 直下に空の GameObject を作成 → 名前: `VignetteImage`
2. `Image` コンポーネントを追加
3. RectTransform を全画面ストレッチに設定
4. ラジアルグラデーションの Sprite をアサイン（外側が不透明、中央が透明）
5. GameObject を **非アクティブ** にしておく
6. `PerfectEffectManager` の `Vignette Image` フィールドにアサイン

---

## 4. カメラシェイク設定
- `PerfectEffectManager` の `Camera Follow` フィールドに `CameraFollow` コンポーネントをアサイン

---

## 5. パーティクル作成（任意）
1. 空の GameObject を作成 → `ParticleSystem` コンポーネントを追加
2. キラキラ・火花など好みに設定
3. `PerfectEffectManager` の `Perfect Particle` フィールドにアサイン
4. 未設定でも他のエフェクトは動作する

---

## 6. 描画順の確認
- `FlashImage` と `VignetteImage` は他の UI より前面に来るよう Canvas の一番下に配置
