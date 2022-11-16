using General;
using UnityEngine;

namespace UI {
    public class UiManager : Singleton<UiManager> {
        [SerializeField] private DebugUi debugUi;

        #region Properties

        public static DebugUi DebugUi => Instance.debugUi;

        #endregion

        protected override void init() {
        }
    }
}
