using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeffTalking : MonoBehaviour
{
    public string[] dialogueLines;
    public string whoIsTalking = "Jeff";
    private List<int> dialogueUsed = new List<int>();

    private ZoomDialogue dialogueManager;

    private void Start()
    {
        dialogueManager = FindObjectOfType<ZoomDialogue>();
    }

    // Call this to play a specific line by index
    public void SayDialogue(int index, bool oneTime, float duration = 10f)
    {
        if (index < 0 || index >= dialogueLines.Length)
        {
            Debug.LogWarning("Dialogue index out of range!");
            return;
        }
        // If oneTime is true and we've already said this line, exit
        if (oneTime && dialogueUsed.Contains(index))
        {
            Debug.Log($"Dialogue index {index} has already been used.");
            return;
        }

        dialogueManager.ShowDialogue(dialogueLines[index], whoIsTalking);
        Invoke(nameof(HideDialogue), duration);

        // If oneTime, record this index as used
        if (oneTime)
        {
            dialogueUsed.Add(index);
        }
    }

    private void HideDialogue()
    {
        dialogueManager.HideDialogue();
    }
}
