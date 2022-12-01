using General;
using UnityEngine;

namespace Ui {
    public class CreditsController : MonoBehaviour {
        public SpriteRenderer logoA;
        public SpriteRenderer logoB;
        public LogoMoveInOut logoC;
        public CreditsRoll credits;
        public SpriteRenderer creditsLogoCopy;

        public void Start() {
            SetCreditsActive(false);
            credits.gameObject.SetActive(false);
        }

        public void SetCreditsActive(bool active) {
            logoA.enabled = !active;
            logoB.enabled = !active;
            logoC.gameObject.SetActive(!active);
            credits.enabled = active;
            creditsLogoCopy.enabled = active;
            credits.ResetCredits();
        }

        private void Update() {
            if (logoA.transform.position.x >= -Util.PIXEL)
                credits.gameObject.SetActive(true);

            if (credits.IsDone)
                SetCreditsActive(false);
        }
    }
}
