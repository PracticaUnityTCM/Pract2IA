using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    [RequireComponent(typeof(FSM_Mouse))]
    [RequireComponent(typeof(MOUSE_Blackboard))]
    public class FSM_MouseFlee : FiniteStateMachine
    {
        public enum State { INITIAL, NORAMAL, FLEE ,DESTROY}
        public State currentState=State.INITIAL;
        public FSM_Mouse FSMMOuse;
        public FSM_RouteExecutor RouterExecutor;
        private MOUSE_Blackboard MouseBB;
        private GameObject OBTarget;
      
        public override void Exit()
        {
            MouseBB = GetComponent<MOUSE_Blackboard>();
            RouterExecutor.target = null;
            RouterExecutor.enabled = false;
            base.Exit();
        }

        public override void ReEnter()
        {
            base.ReEnter();
            currentState = State.INITIAL;
        }
        // Use this for initialization
        void Start()
        {
            FSMMOuse = GetComponent<FSM_Mouse>();
            RouterExecutor = GetComponent<FSM_RouteExecutor>();
            MouseBB = GetComponent<MOUSE_Blackboard>();
            FSMMOuse.enabled = false;
            RouterExecutor.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.NORAMAL);
                    break;
                case State.NORAMAL:

                    float Distance = SensingUtils.DistanceToTarget(gameObject,MouseBB.Roomba);
                    if(Distance< MouseBB.roombaDetectionRadius)
                    {
                        OBTarget = MouseBB.NearestExitPoint();
                        ChangeState(State.FLEE);
                    }
                    break;
                case State.FLEE:
                    if (RouterExecutor.currentState == FSM_RouteExecutor.State.TERMINATED)
                        ChangeState(State.DESTROY);
                    break;
                case State.DESTROY:
                    Destroy(gameObject);
                    break;
            }
        }
        void ChangeState(State newState)
        {
            //Exit
            switch (currentState)
            {
                case State.INITIAL:
                    
                    break;
                case State.NORAMAL:
                    FSMMOuse.Exit();
                    FSMMOuse.enabled = false;
                    break;
                case State.FLEE:
                    RouterExecutor.Exit();
                    RouterExecutor.target = null;
                    RouterExecutor.enabled = false;
                    break;
            }
            //enter
            switch (newState)
            {
                case State.INITIAL:
                    
                    break;
                case State.NORAMAL:
                    FSMMOuse.ReEnter();
                    FSMMOuse.enabled = true;
                    break;
                case State.FLEE:
                    RouterExecutor.ReEnter();
                    RouterExecutor.target = OBTarget;
                    RouterExecutor.enabled = true;
                   
                    break;
            }
            currentState = newState;
        }
    }
}