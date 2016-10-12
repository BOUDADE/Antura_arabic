﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace EA4S.Maze
{
	public class MazeShowPrefab : MonoBehaviour {


		bool movingIn = false;
		bool movingOut = false;

		public int letterIndex = 0;

		// Use this for initialization
		void Start () {
			movingIn = true;
			movingOut = false;


			AudioManager.I.PlayLetter(AppManager.Instance.Letters[letterIndex].Key);

			transform.position = new Vector3 (40, 0, 0);
		}


		
		// Update is called once per frame
		void Update () {


			if (!movingIn && !movingOut)
				return;

			if (movingIn) {
				
				transform.position = Vector3.MoveTowards (transform.position, Vector3.zero, Time.deltaTime * 20);
				if (transform.position.x == 0) {
					movingIn = false;
					MazeGameManager.Instance.showCurrentTutorial ();
				}
				return;
			}

			if (movingOut) {

				transform.position = Vector3.MoveTowards (transform.position, new Vector3(-50,0,0), Time.deltaTime * 20);
				if (transform.position.x == -50) {
					movingOut = false;
					Destroy(gameObject);
				}
				return;
			}
		}


		public void moveOut(bool win = false)
		{
			movingIn = false;
			movingOut = true;

			if(win)
				AudioManager.I.PlayLetter(AppManager.Instance.Letters[letterIndex].Key);
			
		}
	}
}
