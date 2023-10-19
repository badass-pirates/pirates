using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// XR 디바이스에 연결된 손 모델의 표현을 제어하는 스크립트
public class HandPresence : MonoBehaviour
{
    // XR 디바이스의 특성을 지정하는 변수
    public InputDeviceCharacteristics controllerCharacteristics;

    // 손 모델을 나타내는 프리팹
    public GameObject handModelPrefab;

    // 현재 연결된 XR 디바이스
    private InputDevice targetDevice;

    // 생성된 손 모델의 참조
    private GameObject spawnedHandModel;

    // 손 모델에 적용된 애니메이터
    private Animator handAnimator;

    void Start()
    {
        // 초기화 시도
        TryInitialize();
    }

    // XR 디바이스 초기화 시도
    void TryInitialize()
    {
        // XR 디바이스 목록을 가져옴
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        // 연결된 XR 디바이스가 없으면 종료
        if (devices.Count == 0)
        {
            return;
        }

        // 첫 번째로 찾은 XR 디바이스를 대상 디바이스로 설정
        targetDevice = devices[0];

        // 손 모델 프리팹을 생성하여 손 모델을 표시
        spawnedHandModel = Instantiate(handModelPrefab, transform);

        // 생성된 손 모델에서 애니메이터 참조
        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }

    // 손 모델의 애니메이터 업데이트
    void UpdateHandAnimator()
    {
        // 트리거 값을 읽어 애니메이터에 전달
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        // 그립(손잡이) 값을 읽어 애니메이터에 전달
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void Update()
    {
        // XR 디바이스가 유효하지 않으면 초기화 시도
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            // XR 디바이스가 유효하면 애니메이터 업데이트
            UpdateHandAnimator();
        }
    }
}
