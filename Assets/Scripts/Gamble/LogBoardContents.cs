using UnityEngine;

// 상황판에 대한 처리를 수행하는 클래스
public class LogBoardContents : MonoBehaviour
{
    // 로그 메시지에 대한 프리팹 참조 변수
    public GameObject logMessage;

    // 로그 메시지를 추가하는 메서드
    public void LogMessage(string message)
    {
        // 로그 메시지 프리팹을 복제하여 새로운 메시지 오브젝트 생성
        GameObject messageObject = Instantiate(logMessage);

        // 메시지 텍스트 설정
        messageObject.GetComponent<LogMessageInfo>().SetTextUI(message);

        // 부모로 현재 오브젝트 설정 및 월드 좌표 기준에서 로컬 좌표로 변환하지 않도록 설정
        messageObject.transform.SetParent(transform, false);
    }
}
