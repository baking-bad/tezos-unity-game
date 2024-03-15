using System;
using UnityEngine;

namespace UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        private Transform _target;

        private void Start()
        {
            _target = Camera.main.gameObject.transform;
        }

        private void Update()
        {
            transform.LookAt(_target);
        }
    }
}
