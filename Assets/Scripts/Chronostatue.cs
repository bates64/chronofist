using System;
using Health;
using Npc;
using UnityEngine;
using World;

public class Chronostatue : MonoBehaviour
{
    private Animator _animator;
    private int index = 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        // Disable player time loss
        index = 0;
        var h = LevelManager.GetPlayer().GetComponent<HealthTimeDepletion>();
        InputManager.SetMode(InputManager.Mode.None);
        h.depletionRate = 0f;
        GameObject go = GameObject.FindGameObjectWithTag("LevelMusic");
        go.GetComponent<AudioSource>().mute = true;
        // Say some stuff
    }

    private void Update()
    {
        switch (index)
        {
            case 0:
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    index++;
                    Debug.Log("IMRUNNINGBITHC");
                    //GetComponent<SpeechTrigger>().TriggerSpeech();
                }
                break;
            case 1:
                break;
        }

    }
}
