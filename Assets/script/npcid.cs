using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcid : MonoBehaviour
{
    public string id;

    public void OnPlayerInteract()
    {
        if (DialogueLoader.Instance != null && !string.IsNullOrEmpty(id))
        {
            // DialogueLoader의 StartDialogue 함수를 호출하여 이 NPC의 대화를 시작하도록 요청합니다.
            // 이때 이 NPC의 고유 ID(npcId)를 넘겨주어 DialogueLoader가 올바른 대사를 찾게 합니다.
            DialogueLoader.Instance.StartDialog(id);
        }

    }
}
