using UnityEngine;
using UnityEngine.InputSystem;
using DIALOGUE;

namespace TESTING
{
public class Testing : MonoBehaviour
{
    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;
    DialogueSystem ds;
    TextArchitect architect;
    string[] lines = new string[5]
        {
            "Do you know? i'm always use this train every morning",
            "Even i can go to school with bicycle",
            "This is because this train is meaningfull for me",
            "This train make me meet the people who i love",
            "Then, it is you..",
        };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ds = DialogueSystem.instance;
        architect = new TextArchitect(ds.dialogueContainer.dialogueText);
        architect.buildMethod = TextArchitect.BuildMethod.fade;
        architect.speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(bm != architect.buildMethod)
        {
            architect.buildMethod = bm;
            architect.Stop();
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            architect.Stop();
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            if(architect.isBuilding)
            {
                if(!architect.hurryUp){
                    architect.hurryUp = true;
                }else{
                    architect.ForceComplete();
                }
            }else{
                architect.Build(lines[Random.Range(0, lines.Length)]);
            }
        }else if(Input.GetKeyDown(KeyCode.A))
        {
            architect.Append(lines[Random.Range(0, lines.Length)]);
        }
    }
}
}