using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GestureRecognizer
{
    [System.Serializable]
    public struct Gesture
    {
        public string name;
        public List<Vector3> fingerDatas;
        public UnityEvent onRecognized;
        public UnityEvent onRecognitionEnded;
        public bool active;
    }
}