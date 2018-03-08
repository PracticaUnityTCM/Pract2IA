using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    [RequireComponent(typeof(FSM_Mouse))]
    public class FSM_MouseFlee : MonoBehaviour
    {
        public enum State { IINITAL, NORAMAL, FLEE }
        public State currentState;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}