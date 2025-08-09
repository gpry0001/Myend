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
            // DialogueLoader�� StartDialogue �Լ��� ȣ���Ͽ� �� NPC�� ��ȭ�� �����ϵ��� ��û�մϴ�.
            // �̶� �� NPC�� ���� ID(npcId)�� �Ѱ��־� DialogueLoader�� �ùٸ� ��縦 ã�� �մϴ�.
            DialogueLoader.Instance.StartDialog(id);
        }

    }
}
