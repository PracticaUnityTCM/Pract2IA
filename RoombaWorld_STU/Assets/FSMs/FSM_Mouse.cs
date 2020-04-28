using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    [RequireComponent(typeof(MOUSE_Blackboard))]
    public class FSM_Mouse : FiniteStateMachine
    {

        public enum State { INITIAL, ENTER, MAKE_POO, EXIT,DESTROY };
        public State currentState = State.INITIAL;
        private FSM_RouteExecutor RouterExecutor;
        private MOUSE_Blackboard MouseBB;
        private GameObject GOTarjet;
        private Vector3 target;
        public override void Exit()
        {
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
            RouterExecutor = GetComponent<FSM_RouteExecutor>();
            MouseBB = GetComponent<MOUSE_Blackboard>();
            RouterExecutor.enabled = false;
            GOTarjet = new GameObject("MouseTarget");
        }

        // Update is called once per frame
        void Update()
        {
            switch(currentState)
            {
                case State.INITIAL:
                    target = RandomLocationGenerator.RandomPatrolLocation();

                    GOTarjet.transform.position = target;
                    ChangeState(State.ENTER);
                    break;
                case State.ENTER:
                    if (RouterExecutor.currentState == FSM_RouteExecutor.State.TERMINATED)
                        ChangeState(State.MAKE_POO);
                    break;
                case State.MAKE_POO:
                    Instantiate(MouseBB.pooPrefab, transform.position, Quaternion.identity);
                    GOTarjet= MouseBB.RandomExitPoint();
                    ChangeState(State.EXIT);
                    break;
                case State.EXIT:
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
            switch (currentState)
            {
                case State.INITIAL:
                    break;
                case State.ENTER:
                    RouterExecutor.Exit();
                    RouterExecutor.target = null;
                    RouterExecutor.enabled = false;
                    break;
                case State.MAKE_POO:
                    break;
                case State.EXIT:
                    RouterExecutor.Exit();
                    RouterExecutor.target = null;
                    RouterExecutor.enabled = false;
                    break;
            }
            switch (newState)
            {
                case State.INITIAL:
                    break;
                case State.ENTER:
                    RouterExecutor.ReEnter();
                    RouterExecutor.target = GOTarjet;
                    RouterExecutor.enabled = true;
                    break;
                case State.MAKE_POO:
                    break;
                case State.EXIT:
                    RouterExecutor.ReEnter();
                    RouterExecutor.target = GOTarjet;
                    RouterExecutor.enabled = true;
                    break;
            }
            currentState = newState;
        }
    }
}