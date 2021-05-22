using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class UpdateUIScore : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text text;
    // Start is called before the first frame update
    int score = 0;
    void Start()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PipeMoveSystemV2>().OnEventTrigger += (obj) =>
        {
            score++;
            text.text = $"Score:{score}";
        };
    }
}
