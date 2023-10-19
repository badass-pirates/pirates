using UnityEngine;
using TMPro;

// 상황판 메세지를 Set 해주는 클래스
public class LogMessageInfo : MonoBehaviour
{
    // 메시지를 표시할 TextMeshProUGUI 오브젝트에 대한 참조 변수
    public TextMeshProUGUI messageUI;

    // TextMeshProUGUI에 텍스트를 설정하는 메서드
    public void SetTextUI(string message)
    {
        // 주어진 메시지로 TextMeshProUGUI의 텍스트 설정
        messageUI.text = message;
    }
}