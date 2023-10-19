using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 일반적인 객체 직렬화 클래스
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    private List<T> target;

    // 생성자: 리스트를 받아 초기화
    public Serialization(List<T> target)
    {
        this.target = target;
    }

    // 리스트 반환 메서드
    public List<T> ToList()
    {
        return target;
    }
}

// 각종 유틸리티 함수를 제공하는 클래스
public class Utils
{
    #region Singleton
    // 싱글톤 패턴을 구현한 Utils 클래스의 인스턴스
    public static Utils U = new Utils();
    #endregion

    // 씬 변경 및 대기하는 코루틴 함수
    public IEnumerator ChangeScene(string scene, float waitSeconds = 0)
    {
        // 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.IsMasterClient) yield break;

        // 대기 시간만큼 대기
        yield return new WaitForSeconds(waitSeconds);

        // 현재 씬에 있는 모든 Photon 객체 파괴
        PhotonNetwork.DestroyAll();

        // 주어진 씬으로 전환
        PhotonNetwork.LoadLevel(scene);
    }
}
