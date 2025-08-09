using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.VirtualTexturing;
// JSON의 최상위 구조를 나타냅니다: { "npcs": [...] }
[System.Serializable] // 유니티가 이 클래스를 직렬화하여 JSON과 매핑할 수 있게 합니다.
public class AllDialogues
{
    public List<NPCDialogueEntry> npcs; // JSON의 "npcs" 배열과 이름이 정확히 일치해야 합니다.
}

// 'npcs' 배열 안의 각 NPC 엔트리 구조를 나타냅니다: { "id": "...", "dialogue": [...] }
[System.Serializable]
public class NPCDialogueEntry
{
    public string id; // JSON의 "id" 필드와 이름이 정확히 일치해야 합니다.
    public List<DialogLine> dialogue; // JSON의 "dialogue" 배열과 이름이 정확히 일치해야 합니다.
}

// 'dialogue' 배열 안의 각 대사 한 줄의 구조를 나타냅니다: { "Name": "...", "Text": "..." }
[System.Serializable]
public class DialogLine
{
    public string Name;   // 대사하는 캐릭터의 이름 (JSON의 "Name"과 일치)
    public string Text;   // 실제 대사 내용 (JSON의 "Text"와 일치)
}

// Dictionary의 Value 타입으로 사용하기 위한 헬퍼 클래스입니다.
// List<DialogLine>을 바로 Dictionary의 값으로 쓸 수 없기 때문에 한 번 감싸줍니다.
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

   // JSON에서 로드한 모든 NPC 대사를 ID(string)를 키로 저장하는 딕셔너리
    private Dictionary<string, DialogueContainer> allNpcDialogues;
    private List<DialogLine> currentDialogueLines; // 현재 표시 중인 NPC의 대사 목록
    private int currentLineIndex;                  // 현재 대사 줄의 인덱스
    
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
            Debug.LogError("오류: 'Dialog.json' 파일을 'Assets/Resources' 폴더에서 찾을 수 없습니다!");
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
        dialogueText.text = ""; // 텍스트 필드를 비우고 시작

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i]; // 한 글자씩 추가
            yield return new WaitForSeconds(0.05f); // 타이핑 속도 조절 (0.05초마다)
        }
        isTyping = false;
        typingCoroutine = null; // 코루틴 완료 후 참조 해제
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
