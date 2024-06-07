using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAquarium : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !EventSystem.current.currentSelectedGameObject)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitinfo;

                if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, 1 << 6)) // �ش� ������Ʈ ���̾��� ����
                {
                    Destroy(hitinfo.collider.gameObject);
                }
            }
        }
    }
}
