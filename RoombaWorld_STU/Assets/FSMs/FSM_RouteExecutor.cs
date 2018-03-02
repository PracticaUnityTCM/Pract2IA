using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    [RequireComponent(typeof(PathFollowing))]
    [RequireComponent(typeof(Seeker))]
    // route executor is a complex pathFeeder...
    // ... well, a pathFeeder with a TERMINATED state...
    public class FSM_RouteExecutor : FiniteStateMachine
    {

        public enum State { INITIAL, GENERATING, FOLLOWING, TERMINATED };
        public State currentState = State.INITIAL;

        public GameObject target; // the place I'm going
        private Seeker seeker; // the path generator
        private Path path; // the path calculated by the seeker
        private PathFollowing pathFollowing; // the steering behaviour
                
        void Awake()
        {
            seeker = GetComponent<Seeker>();
            pathFollowing = GetComponent<PathFollowing>();
            pathFollowing.enabled = false;
            //pathFollowing.wayPointReachedRadius = 3f;
        }

        public override void Exit()
        {
            pathFollowing.enabled = false;
            target = null;
            path = null;
            base.Exit();
        }

        public override void ReEnter()
        {
            base.ReEnter();
            currentState = State.INITIAL;
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.GENERATING);
                    break;

                case State.GENERATING:
                    if (path != null)
                    { // just wait until the path has been generated
                        ChangeState(State.FOLLOWING);
                        break;
                    }
                    // in this state just wait until the path has been calculated
                    break;

                case State.FOLLOWING:
                    if (pathFollowing.currentWaypointIndex == path.vectorPath.Count)
                    { // end of path reached
                        ChangeState(State.TERMINATED);
                        break;
                    }
                    // do nothing in particular while in this state
                    break;

                case State.TERMINATED:
                    // this is a PIT state
                    break;

            } // end of switch
        }



        private void ChangeState(State newState)
        {

            // EXIT STATE LOGIC. Depends on current state
            switch (currentState)
            {

                case State.GENERATING:
                    // do nothing in particular when leaving this state
                    break;

                case State.FOLLOWING:
                    // the path has been completed. stop moving
                    pathFollowing.enabled = false;
                    break;

                    // State.Terminated cannot be exited so no entry here

            } // end exit switch

            // ENTER STATE LOGIC. Depends on newState
            switch (newState)
            {

                case State.GENERATING:
                    // just ask the seeker for fresh path
                    path = null;
                    seeker.StartPath(gameObject.transform.position, target.transform.position, OnPathComplete);
                    break;

                case State.FOLLOWING:
                    // a new path has been calculated. Feed that path to the pathfollowinf steering	
                    pathFollowing.path = path;
                    pathFollowing.currentWaypointIndex = 0;
                    pathFollowing.enabled = true;
                    break;

                    // State.TERMINATED is just a pit, so do nothing here 

            } // end of enter switch


            currentState = newState;

        } // end of method ChangeState


        // calback method for seeker.StartPath
        private void OnPathComplete(Path p)
        {
            path = p;
        }

    }
}