using TMPro;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]  // Edit 모드에서도 실행
[RequireComponent(typeof(TMP_Text))]
public class TMPToSpriteNumber : MonoBehaviour, ITextPreprocessor
{
    TMP_Text target;

    void Awake()
    {
        target = GetComponent<TMP_Text>();
        target.textPreprocessor = this;
    }

    public string PreprocessText(string text)
    {
        string result = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                result += $"<sprite={c}/>";
            else
                result += c;
        }
        return result;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // 에디터에서도 TMP 강제 갱신
        if (!Application.isPlaying && target != null)
        {
            target.ForceMeshUpdate();
            target.SetText(PreprocessText(target.text));
        }
    }
#endif
}
