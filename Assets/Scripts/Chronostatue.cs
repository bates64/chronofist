using Health;
using Npc;
using UnityEngine;
using World;

public class Chronostatue : MonoBehaviour {
    private void Awake() {
        // Disable player time loss
        var h = LevelManager.GetPlayer().GetComponent<HealthTimeDepletion>();
        h.depletionRate = 0f;

        // Say some stuff
        GetComponent<SpeechTrigger>().TriggerSpeech();
    }
}
