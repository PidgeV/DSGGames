using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] Text dialogueText;

    Queue<DialogueClass> dialogueQueue = new Queue<DialogueClass>();

    public List<DialogueClass> Dialogue;

    // Editor
    public DialogueClass NewDialogue;
    public Texture2D SoundIcon;
    public bool ShowDefaultEditor;

    private int textPos = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    [ContextMenu("Play Index Zero")]
    public void PlayIndexZero()
    {
        AddDialogue(0);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (!audioSource) audioSource = GameObject.FindGameObjectWithTag("Dialogue").GetComponent<AudioSource>();

            if (audioSource && !audioSource.isPlaying && dialogueQueue.Count > 0)
            {
                audioSource.clip = dialogueQueue.Peek().SoundClip;
                audioSource.Play();

                if (dialogueText) dialogueText.text = dialogueQueue.Peek().Text[textPos];

                if (dialogueQueue.Peek().Text.Length == 1) dialogueQueue.Dequeue();
                else if (textPos == dialogueQueue.Peek().Text.Length)
                {
                    textPos = 0;
                    dialogueQueue.Dequeue();
                }
            }
            else if (!audioSource.isPlaying && dialogueQueue.Count == 0)
            {
                dialogueText.text = "";
            }
            else if (!audioSource)
            {
                dialogueText.text = "";
                Debug.LogWarning("No AudioSource found for Dialogue");
            }
        }
    }

    IEnumerator TrackTextDisplay()
    {
        while (true)
        {
            yield return null;

            DialogueClass diag = dialogueQueue.Peek();

            if (diag.Text.Length > 0)
            {
                yield return new WaitForSecondsRealtime(diag.displayTime[textPos]);

                textPos++;
            }

        }
    }

    public void AddDialogue(DialogueClass dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }

    public DialogueClass AddDialogue(int listIndex)
    {
        if (listIndex < Dialogue.Count)
        {
            foreach (DialogueClass dialogue in Dialogue)
            {
                if (dialogue.Index == listIndex)
                {
                    DialogueClass pickedDialogue = dialogue;
                    if (dialogue.alternatives != null && dialogue.alternatives.Length > 0)
                    {
                        int randomIndex = Random.Range(0, dialogue.alternatives.Length + 1);

                        if (randomIndex > 0)
                            pickedDialogue = Dialogue[dialogue.alternatives[randomIndex - 1]];
                    }

                    dialogueQueue.Enqueue(pickedDialogue);
                    return pickedDialogue;
                }
            }
        }
        else
        {
            Debug.LogError("Index out of range.");
        }

        return null;
    }

    public void PlayQuickClip(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void GenerateNewDialogue()
    {
        NewDialogue.Index = Dialogue.Count;
        Dialogue.Add(NewDialogue);
        NewDialogue = new DialogueClass();
    }
}
