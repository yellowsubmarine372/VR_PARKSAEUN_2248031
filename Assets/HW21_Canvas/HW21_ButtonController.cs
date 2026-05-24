using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 버튼 클릭 시 텍스트를 변경하는 컨트롤러.
/// TMP_Text 또는 legacy Text 모두 지원.
/// </summary>
public class HW21_ButtonController : MonoBehaviour
{
    [Header("Target Text (TMP)")]
    public TMP_Text tmpText;

    [Header("Target Text (Legacy - 필요 시)")]
    public Text legacyText;

    [Header("Messages")]
    public string defaultMessage = "버튼을 클릭하세요!";
    public string clickedMessage = "버튼이 클릭되었습니다!";

    private bool _isClicked = false;

    void Start()
    {
        SetText(defaultMessage);
    }

    public void OnButtonClicked()
    {
        _isClicked = !_isClicked;
        SetText(_isClicked ? clickedMessage : defaultMessage);
    }

    private void SetText(string msg)
    {
        if (tmpText != null) tmpText.text = msg;
        if (legacyText != null) legacyText.text = msg;
    }
}
