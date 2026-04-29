using UnityEngine;

[CreateAssetMenu(fileName = "QtsData", menuName = "Quiz/QtsData")]
public class QtsData : ScriptableObject
{
    [System.Serializable]
    public struct Question
    {
        public string questionText;
        public string[] replies;
        public int correctReplyIndex;
        public string explanation;
    }

    public Question[] questions;
}