using UnityEngine;
using TMPro;

public class CollisionDetector : MonoBehaviour
{
    // 부모 오브젝트의 Collider를 저장하는 변수
    private Collider parent;

    // 충돌한 오브젝트의 이름을 표시할 TextMeshProUGUI 컴포넌트
    public TextMeshProUGUI target;

    void Start()
    {
        // 부모 오브젝트의 Collider를 찾아서 변수에 할당
        parent = transform.GetComponentInParent<Collider>();
    }

    void Update()
    {
        // TextMeshProUGUI 컴포넌트에 부모 오브젝트의 Collider가 연결된 Rigidbody의 이름을 표시
        target.text = parent.attachedRigidbody.gameObject.name;
    }
}
