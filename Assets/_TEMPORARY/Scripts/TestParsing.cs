using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace TESTING
{
public class TestParsing : MonoBehaviour
{
    [SerializeField] private TextAsset file;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SendFileToParse();
    }

    void SendFileToParse()
    {
        List<string> lines = FIleManager.ReadTextAsset(file);
        foreach(string line in lines)
        {
            if(line == string.Empty)
            {
                continue;
            }
            DIALOGUE_LINE dl = DialogueParser.Parse(line);
        }
    }

    // Update is called once per frame

}
}

