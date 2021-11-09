using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _speed;

    [Header("BroadCast Channel")]
    public GameObjectEventChannelSO ServerRopeHitWallEvent;
    public GameObjectEventChannelSO ServerRopeHitOtherPlayerEvent;

    [HideInInspector]
    public GameObject OwnerCharacter;
    public Vector3 EndPoint => _endPoint;
    private GameObject _ropeHead;
    private LineRenderer _lineRenderer;
    private Transform _shootPoint;
    private Vector3 _endPoint;
    private Vector3 _direction;
    private bool _haveInit = false;
    private bool _stopMoving = false;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        _ropeHead = transform.GetChild(0).gameObject;
    }

    public void Init(Transform shootPoint, Vector3 direction, GameObject ownerCharacter){
        _haveInit = true;
        OwnerCharacter = ownerCharacter;
        _ropeHead.GetComponent<RopeHeadBehaviour>().OwnerCharacter = OwnerCharacter;
        _ropeHead.GetComponent<RopeHeadBehaviour>().RopeBehaviour = this;
        _ropeHead.GetComponent<SphereCollider>().enabled = true;
        _shootPoint = shootPoint;
        _endPoint = _shootPoint.position;
        _direction = direction;
        _lineRenderer.SetPosition(0, _shootPoint.position);
        _lineRenderer.SetPosition(1, _shootPoint.position);
    }

    private void Update() {
        if(_haveInit == false) return;

        // update endpoint
        _lineRenderer.SetPosition(0, _shootPoint.position);
        if(_stopMoving == false){
            _endPoint += _direction * _speed * Time.deltaTime;
            _lineRenderer.SetPosition(1, _endPoint);
            _ropeHead.transform.position = _endPoint;
        }
    }

    public void OnHitWall(){
        _stopMoving = true;
        ServerRopeHitWallEvent.RaiseEvent(this.gameObject);
    }

    public void OnHitPlayer(){

    }
    
}
