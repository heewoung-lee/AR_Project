using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// 선택 안되는 버튼은 회색으로 만들고.
// 해당 버튼 하단에 이미지, 텍스트 가 있을수 잇는데 
// 해당 버튼이 선택이 안되었을때 하위 오브젝트를 모두 색을 바꾸는 자동화 코드도  
//

/// <summary>
/// 추후에 이벤트 연결 다하면 됨
/// </summary>
public class ButtonEventComponent : MonoBehaviour 
{
    // MonoBehavior 제외 -> 따로 버튼을 연결해줘야 하는데 
    // 상관은 딱히 없다.
    // 우리가 사용할 버튼에다 컴포넌트로 붙여주고 
    // 그쪽 클래스에서 Getcomponent 해서 사용 하는 건데 
    // 이렇게 사용 하는 이유는 여러가지를 사용할때 
    // 해당 트리거 사용으로 할거면 좀 유용함. 

    private EventTrigger eventTrigger;

    private void Awake()
    {
        eventTrigger = gameObject.AddComponent<EventTrigger>();
        SetEvent();
    }

    public UnityAction testAction;

    public void ButtonAction(UnityAction action)
    {

        testAction = action;
    }

    // SetEvent쪽에 눌렀을때 땟을때 함수를 정의
    // Button을 그냥 사용하지 않고 EventTrigger를 사용 해서 사용
    private void SetEvent()
    {
        // 타입을 정하고 해당 타입의 행동에 어떤 함수가 실행이 될건지 정해 놓는 것
        AddEventTrigger(OnPointerDown, EventTriggerType.PointerDown);
        AddEventTrigger(OnPointerUp, EventTriggerType.PointerUp);
    }

    private void OnPointerDown(BaseEventData eventData)
    {
         // 눌렀을때 
    }

    private void OnPointerUp(BaseEventData eventData) 
    {
        // 땟을때 
        testAction?.Invoke();   
    }

    private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType triggerType)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry(); 
        entry.eventID = triggerType;
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
    }

    
}
