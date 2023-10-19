using UnityEngine;

// 마이크로부터 받은 음량에 따라 스케일을 변경하는 클래스
public class MicrophoneListener : MonoBehaviour
{
    // 민감도 및 현재 음량 변수
    public float sensitivity = 100;
    public float loudness = 0;

    // 연결할 오브젝트와 스케일 변수
    public GameObject component;
    public float scale = 1f;
    public float minumScale = 1f;
    public float maximumScale = 3f;

    // 피벗 음량과 함께 음량이 컴포넌트 스케일에 영향을 미치는 값을 조절하는 변수
    public float pivotLoudness;

    // AudioSource 컴포넌트 
    private AudioSource _audio;

    void Start()
    {
        // AudioSource 컴포넌트 초기화 및 마이크로부터 오디오 데이터를 받기 위한 설정
        _audio = GetComponent<AudioSource>();
        _audio.clip = Microphone.Start(null, true, 10, 44100);
        _audio.loop = true;

        // 마이크로부터의 입력이 시작될 때까지 대기
        while (!(Microphone.GetPosition(null) > 0)) { }

        // AudioSource 재생 시작
        _audio.Play();
    }

    void Update()
    {
        // 현재 음량 업데이트 및 컴포넌트 스케일 변경 메서드 호출
        loudness = GetAveragedVolume() * sensitivity;
        ChangeComponentScale();
    }

    // 컴포넌트 스케일을 변경하는 메서드
    void ChangeComponentScale()
    {
        // 음량이 피벗 음량보다 크면 스케일 증가, 그렇지 않으면 감소
        scale += (loudness > pivotLoudness) ? Time.deltaTime : -Time.deltaTime;

        // 스케일이 최솟값 미만으로 내려가지 않도록 조정
        if (scale < minumScale)
        {
            scale = minumScale;
        }

        // 스케일이 최댓값을 초과하지 않도록 조정
        if (scale > maximumScale)
        {
            scale = maximumScale;
        }

        // 컴포넌트의 로컬 스케일을 업데이트
        component.transform.localScale = new Vector3(scale, scale, scale);
    }

    // 평균 음량을 계산하는 메서드
    float GetAveragedVolume()
    {
        // 오디오 데이터를 저장할 배열 및 초기화
        float[] data = new float[256];
        float a = 0;

        // AudioSource로부터 오디오 데이터 가져오기
        _audio.GetOutputData(data, 0);

        // 데이터 배열의 각 요소의 절댓값을 더하기
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }

        // 평균 음량 반환
        return a / 256;
    }
}
