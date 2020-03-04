using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Text dialogueText;
	
	Queue<DialogueClass> dialogueQueue = new Queue<DialogueClass>();

	public List<DialogueClass> Dialogue;

	// Editor
	public DialogueClass NewDialogue;
	public Texture2D SoundIcon;
	public bool ShowDefaultEditor;

	[ContextMenu("Play Index Zero")]
	public void PlayIndexZero()
	{
		AddDialogue(0);
	}

	// Start is called before the first frame update
	IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if (!audioSource) audioSource = GameObject.FindGameObjectWithTag("Dialogue").GetComponent<AudioSource>();

            if (audioSource && !audioSource.isPlaying && dialogueQueue.Count > 0)
            {
                audioSource.clip = dialogueQueue.Peek().SoundClip;
                audioSource.Play();

                if (dialogueText) dialogueText.text = dialogueQueue.Peek().Text;

                dialogueQueue.Dequeue();
            }
            else if(!audioSource.isPlaying && dialogueQueue.Count == 0)
            {
                dialogueText.text = "";
            }
            else if(!audioSource)
            {
                dialogueText.text = "";
                Debug.LogWarning("No AudioSource found for Dialogue");
            }
        }
    }

    public void AddDialogue(DialogueClass dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }

	public void AddDialogue(int listIndex)
	{
		if (listIndex < Dialogue.Count)
		{
			foreach (DialogueClass dialogue in Dialogue) {
				if (dialogue.Index == listIndex)
				{
					dialogueQueue.Enqueue(dialogue);
					return;
				}
			}
		}
		else
		{
			Debug.LogError("Index out of range.");
		}
	}

	public void PlayQuickClip(AudioClip clip)
	{
		if (audioSource != null) {
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
