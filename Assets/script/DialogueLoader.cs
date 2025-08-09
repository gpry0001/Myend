using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.VirtualTexturing;
// JSON�� �ֻ��� ������ ��Ÿ���ϴ�: { "npcs": [...] }
[System.Serializable] // ����Ƽ�� �� Ŭ������ ����ȭ�Ͽ� JSON�� ������ �� �ְ� �մϴ�.
public class AllDialogues
{
    public List<NPCDialogueEntry> npcs; // JSON�� "npcs" �迭�� �̸��� ��Ȯ�� ��ġ�ؾ� �մϴ�.
}

// 'npcs' �迭 ���� �� NPC ��Ʈ�� ������ ��Ÿ���ϴ�: { "id": "...", "dialogue": [...] }
[System.Serializable]
public class NPCDialogueEntry
{
    public string id; // JSON�� "id" �ʵ�� �̸��� ��Ȯ�� ��ġ�ؾ� �մϴ�.
    public List<DialogLine> dialogue; // JSON�� "dialogue" �迭�� �̸��� ��Ȯ�� ��ġ�ؾ� �մϴ�.
}

// 'dialogue' �迭 ���� �� ��� �� ���� ������ ��Ÿ���ϴ�: { "Name": "...", "Text": "..." }
[System.Serializable]
public class DialogLine
{
    public string Name;   // ����ϴ� ĳ������ �̸� (JSON�� "Name"�� ��ġ)
    public string Text;   // ���� ��� ���� (JSON�� "Text"�� ��ġ)
}

// Dictionary�� Value Ÿ������ ����ϱ� ���� ���� Ŭ�����Դϴ�.
// List<DialogLine>�� �ٷ� Dictionary�� ������ �� �� ���� ������ �� �� �����ݴϴ�.
[System.Serializable]
public class DialogueContainer
{
    public List<DialogLine> dialogue;
}

public class DialogueLoader : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

   // JSON���� �ε��� ��� NPC ��縦 ID(string)�� Ű�� �����ϴ� ��ųʸ�
    private Dictionary<string, DialogueContainer> allNpcDialogues;
    private List<DialogLine> currentDialogueLines; // ���� ǥ�� ���� NPC�� ��� ���
    private int currentLineIndex;                  // ���� ��� ���� �ε���
    
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public static DialogueLoader Instance { get; private set; }

    public event System.Action OnDialogueFinished;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDialog();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void LoadDialog()
    {
        TextAsset jsontextAsset = Resources.Load<TextAsset>("Dialogues/Dialog");
        if (jsontextAsset == null)
        {
            Debug.LogError("����: 'Dialog.json' ������ 'Assets/Resources' �������� ã�� �� �����ϴ�!");
            return;
        }
        AllDialogues loadedData = JsonUtility.FromJson<AllDialogues>(jsontextAsset.text);
        

        allNpcDialogues = new Dictionary<string, DialogueContainer>();

        foreach (NPCDialogueEntry entry in loadedData.npcs)
        {
            allNpcDialogues.Add(entry.id, new DialogueContainer { dialogue = entry.dialogue });
        }
    }
    public void StartDialog(string id)
    {
        currentDialogueLines = allNpcDialogues[id].dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }
    public void AdvanceDialogue()
    {
        if (!dialoguePanel.activeSelf) return; 

        if (isTyping) 
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            if (currentDialogueLines != null && currentLineIndex > 0 && currentLineIndex - 1 < currentDialogueLines.Count)
            {
                dialogueText.text = currentDialogueLines[currentLineIndex - 1].Text;
            }
            isTyping = false;
            
        }
        else 
        {
            DisplayNextLine();
        }
    }
    void DisplayNextLine()
    {
        if (currentDialogueLines == null || currentLineIndex >= currentDialogueLines.Count)
        {
            EndDialogue();
            return;
        }
        DialogLine line = currentDialogueLines[currentLineIndex];
        nameText.text = line.Name;
        if(typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        typingCoroutine = StartCoroutine(Typing(line.Text));
        currentLineIndex++;
    }
    IEnumerator Typing(string text)
    {
        isTyping = true;
        dialogueText.text = ""; // �ؽ�Ʈ �ʵ带 ���� ����

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i]; // �� ���ھ� �߰�
            yield return new WaitForSeconds(0.05f); // Ÿ���� �ӵ� ���� (0.05�ʸ���)
        }
        isTyping = false;
        typingCoroutine = null; // �ڷ�ƾ �Ϸ� �� ���� ����
    }
    void EndDialogue()
    {
        dialoguePanel.SetActive(false); 
        currentDialogueLines = null;     
        currentLineIndex = 0; 
        isTyping = false; 
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        OnDialogueFinished?.Invoke();
    }


}
