using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private float _timeToMove;

        [SerializeField] private Transform _pointOne;
        [SerializeField] private Transform _pointTwo;

        private Camera _camera;
        private Transform _nextPoint;

        private Transform CameraTransform => _camera.transform;

        private void Awake()
        {
            _camera = Camera.main;
            CameraTransform.position = _pointOne.position;
            CameraTransform.rotation = _pointOne.rotation;
            _nextPoint = _pointTwo;
        }

        public IEnumerator Move()
        {
            while (Vector3.Distance(CameraTransform.position, _nextPoint.position) >= 0.01f)
            {
                CameraTransform.position = Vector3.Lerp(CameraTransform.position, _nextPoint.position,
                    _timeToMove * Time.deltaTime);
                CameraTransform.rotation = Quaternion.Lerp(CameraTransform.rotation, _nextPoint.rotation,
                    _timeToMove * Time.deltaTime);

                yield return null;
            }
            
            _nextPoint = _nextPoint == _pointOne ? _pointTwo : _pointOne;
        }
    }
}