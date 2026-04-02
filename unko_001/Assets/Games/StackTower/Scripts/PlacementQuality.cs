/// <summary>
/// ブロック設置の精度判定。
/// </summary>
public enum PlacementQuality
{
    Perfect,  // ほぼ中心（幅変化なし）
    Good,     // 少しはみ出た（35%未満）
    Bad,      // 大きくはみ出た（35%以上）
}
