// iOS ハプティクスプラグイン
// UIImpactFeedbackGenerator (light) を使用 — 最も短く軽いバイブ
#import <UIKit/UIKit.h>

extern "C" {
    void _TriggerLightHaptic() {
        if (@available(iOS 10.0, *)) {
            UIImpactFeedbackGenerator *generator =
                [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
            [generator prepare];
            [generator impactOccurred];
        }
    }
}
