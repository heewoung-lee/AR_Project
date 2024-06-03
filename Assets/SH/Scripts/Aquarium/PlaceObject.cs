using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class PlaceObject : MonoBehaviour
{
    public List<GameObject> _prefabs = new List<GameObject>();
    public ARRaycastManager _arRaycastManager;
    static List<ARRaycastHit> _arRayHits = new List<ARRaycastHit>();
    public Transform _objectPool;
    public Button placeButton; // ��ġ ��ư
    public Button doneButton; // �Ϸ� ��ư
    Vector2 _centerVec;
    GameObject _nowobject;
    PlaneClassification _nowTypeTag;
   // List<GameObject> _placedObjects = new List<GameObject>();

    GameObject _placedObject = null; // ����Ʈ �Ⱦ����� �ϴ°�

    private void Start()
    {
        _centerVec = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        placeButton.onClick.AddListener(OnPlaceButtonClicked);
        doneButton.onClick.AddListener(OnDoneButtonClicked);
    }

    private void Update()
    {
        if (_nowobject != null)
        {
            if (_arRaycastManager.Raycast(_centerVec, _arRayHits, TrackableType.PlaneWithinPolygon))
            {
                ARPlane tPlane = _arRayHits[0].trackable.GetComponent<ARPlane>();
                if (_nowTypeTag == tPlane.classification)
                {
                    _nowobject.transform.position = _arRayHits[0].pose.position;
                    _nowobject.transform.rotation = _arRayHits[0].pose.rotation;
                    _nowobject.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    _nowobject.transform.localScale = new Vector3(0, 0, 0);
                }
            }
            else
            {
                _nowobject.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }

    public void SetObject(int type)
    {
        // ������ ��ġ�� ������Ʈ�� ������ ����
        //foreach (var placedObject in _placedObjects)
        //{
        //    Destroy(placedObject);
        //}
        //_placedObjects.Clear();

        Destroy(_placedObject);

        if (_nowobject != null)
        {
            Destroy(_nowobject);
            _nowobject = null;
        }

        GameObject gameObject = null;

        switch (type)
        {
            case 0:
                gameObject = _prefabs[0];
                _nowTypeTag = PlaneClassification.None;
                break;
                // �ٸ� Ÿ�� �߰� ����
        }

        _nowobject = Instantiate(gameObject);
        _nowobject.transform.SetParent(_objectPool);
        _nowobject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetObjectDone()
    {
        if (_nowobject != null)
        {
            //_placedObjects.Add(_nowobject);
            _placedObject = _nowobject;
            _nowobject = null;
        }
    }

    private void OnPlaceButtonClicked()
    {
        SetObject(0);
    }

    private void OnDoneButtonClicked()
    {
        SetObjectDone();
    }
}
