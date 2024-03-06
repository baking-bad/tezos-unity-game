using System;
using UnityEngine;

namespace UI
{
    public class UiHero : MonoBehaviour
    {
        [SerializeField] private bool isMainHero;
        [SerializeField] private Transform targetPosition;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;

        private UiMenuManager _uiMenuManager;
        private Animator _animator;
        private bool isAnimated;
        private Camera Cam => Camera.main;
        private Quaternion _targetRotation;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiMenuManager = GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<UiMenuManager>();
            _uiMenuManager.LoadingScreenShowed += AnimateHero;
        }

        private void Start()
        {
            if (Camera.main == null) return;
            
            _targetRotation = Cam == null
                ? Quaternion.identity
                : Quaternion.Euler(
                    Cam.transform.eulerAngles.x,
                    Cam.transform.eulerAngles.y - 180f,
                    Cam.transform.eulerAngles.z);
        }

        private void Update()
        {
            if (!isAnimated) return;
            
            AnimateHero();
        }

        private void AnimateHero()
        {
            if (isMainHero)
            {
                _animator.SetTrigger("isReady");
            }
            else
            {
                isAnimated = true;

                if (targetPosition.position - transform.position == Vector3.zero)
                {
                    _animator.SetBool("isMoving", false);
                    transform.rotation = Quaternion
                        .Lerp(transform.rotation, _targetRotation, Time.deltaTime * rotateSpeed);

                    if (_targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y == 0)
                    {
                        _animator.SetTrigger("isReady");
                        isAnimated = false;
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPosition.position,
                        moveSpeed * Time.deltaTime);

                    _animator.SetBool("isMoving", true);
                }
            }
        }

        private void OnDisable()
        {
            _uiMenuManager.LoadingScreenShowed -= AnimateHero;
        }
    }
}
