using UnityEngine;

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PromptAdvance();
        }
    }

    //menjalankan fungsi userprompt yang nantinya memengaruhi kecepatan dialogue
    public void PromptAdvance()
    {
        DialogueSystem.instance.OnUserPrompt_Next();
    }
}
}
