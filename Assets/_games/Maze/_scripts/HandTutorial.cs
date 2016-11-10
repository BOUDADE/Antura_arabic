﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	
	public class HandTutorial : MonoBehaviour {

		public float handSpeed = 2.0f;

		//support mutliple paths:
		public List<GameObject> pathsToFollow;
		public List<GameObject> linesToShow;
		public List<GameObject> numbersToShow;

		private List<Vector3> wayPoints = new List<Vector3> ();

		private int currentPath = 0;
		private int currentWayPoint = 0;


		private bool isMovingOnPath = false;
		public bool isStopped = false;

		private Vector3 startingPosition;
		// Use this for initialization
		void Start () {

			startingPosition = new Vector3(-1,-1,-1);

			//hide the path
			foreach(GameObject pathToFollow in pathsToFollow)
				pathToFollow.SetActive(false);

			foreach(GameObject lineToShow in linesToShow)
				lineToShow.SetActive(false);

			foreach(GameObject numberToShow in numbersToShow)
				numberToShow.SetActive(false);

			gameObject.SetActive (false);

            
        }
		
		// Update is called once per frame
		void Update () {
            return;
			//no way points to follow:
			if (wayPoints.Count == 0)
				return;


			//otherwise move to next way point:
			transform.position = Vector3.MoveTowards (transform.position, wayPoints[currentWayPoint], Time.deltaTime*handSpeed);
           
           
            if (transform.position == wayPoints[currentWayPoint])
            {//(transform.position - wayPoints [currentWayPoint]).magnitude <= float.Epsilon) {
				currentWayPoint++;

				//arrived:
				if (currentWayPoint == wayPoints.Count) {
					currentWayPoint = wayPoints.Count - 1;
					gameObject.SetActive (false);

					//set tutorial done:
					isMovingOnPath = false;

					MazeGameManager.Instance.timer.StartTimer ();

				}
			}


		}

		public void showCurrentTutorial()
		{
			if(startingPosition.x != -1 && startingPosition.y != -1 && startingPosition.z != -1)
				gameObject.transform.position = startingPosition;
			gameObject.SetActive (true);
			isMovingOnPath = true;
			setWayPoints ();
			
		}

		public void stopCurrentTutorial()
		{
            TutorialUI.Clear(false);

            wayPoints.Clear ();
			gameObject.SetActive (false);

			//set tutorial done:
			isMovingOnPath = false;
			isStopped = true;
		}

		void setWayPoints()
		{
			
			gameObject.SetActive (true);
           
			wayPoints = new List<Vector3> ();
			//construct the path waypoints:
			pathsToFollow[currentPath].SetActive(true);
			numbersToShow [currentPath].SetActive (true);
			linesToShow [currentPath].SetActive (true);
			foreach (Transform child in pathsToFollow[currentPath].transform) {
				wayPoints.Add (child.transform.position);
			}
			startingPosition = wayPoints[0];
			currentWayPoint = 0;

            

            TutorialUI.DrawLine(wayPoints.ToArray(), TutorialUI.DrawLineMode.FingerAndArrow, true, true);
        }

		public bool isComplete()
		{
			return currentPath == pathsToFollow.Count - 1;
		}


		public void moveToNextPath()
		{
			if (currentPath < pathsToFollow.Count - 1) {
				isStopped = false;
				pathsToFollow [currentPath].SetActive (false);
				numbersToShow [currentPath].SetActive (false);
				linesToShow [currentPath].SetActive (false);
				currentPath++;
				setWayPoints ();
			}
		}

		public bool isCurrentTutorialDone()
		{
			return !isMovingOnPath;
		}


	}

}
