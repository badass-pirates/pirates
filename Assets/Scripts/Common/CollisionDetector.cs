using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollisionDetector : MonoBehaviour
{
    Collider parent;
    public TextMeshProUGUI target;
    // Update is called once per frame
    void Update()
    {
        target.text = parent.attachedRigidbody.gameObject.name;
    }

    void Start()
    {
        
        parent = transform.GetComponentInParent<Collider>();
    }
}
