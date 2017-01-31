﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;
using EA4S.Tutorial;
using EA4S.UI;
using TMPro;
using DG.Tweening;

namespace EA4S.Minigames.Maze
{
    public class MazeGameManager : MiniGame
    {
        public static MazeGameManager instance;

        private const int MAX_NUM_ROUNDS = 6;

        public GameObject characterPrefab;
        public GameObject arrowTargetPrefab;

        public MazeCharacter currentCharacter;
        public HandTutorial currentTutorial;

        public List<GameObject> prefabs;

        public Canvas endGameCanvas;




        public float idleTime = 7;
        public TextMeshProUGUI roundNumberText;


        private int roundNumber;
        public GameObject currentPrefab;
        public int health = 4;
        public GameObject cracks;
        List<GameObject> _cracks;
        //List<GameObject> lines;
        public List<Vector3> pointsList;

        public List<LineRenderer> lines;

        int correctLetters = 0;
        int wrongLetters = 0;

        [HideInInspector]
        public float gameTime = 0;
        public float maxGameTime = 120;
        public MazeTimer timer;
        public GameObject antura;
        public GameObject fleePositionObject;

        private List<Vector3> fleePositions;

        public bool isTutorialMode;
        //for letters:
        public Dictionary<string, int> allLetters;

        private MazeLetter currentMazeLetter;
        private IInputManager inputManager;

        public Color drawingColor;
        public Color incorrectLineColor;
        public float durationToTweenLineColors;

        void setupIndices()
        {
            allLetters = new Dictionary<string, int>();
            for (int i = 0; i < prefabs.Count; ++i)
            {
                allLetters.Add(prefabs[i].name, i);
            }
        }

        private void OnPointerDown()
        {
            if (currentMazeLetter != null)
            {
                currentMazeLetter.OnPointerDown();
            }
        }

        private void OnPointerUp()
        {
            if (currentMazeLetter != null)
            {
                currentMazeLetter.OnPointerUp();
            }
        }

        public Vector2 GetLastPointerPosition()
        {
            return inputManager.LastPointerPosition;
        }

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        public void ColorCurrentLinesAsIncorrect()
        {
            /*foreach (var line in lines)
            {
                line.material.DOColor(incorrectLineColor, durationToTweenLineColors);
            }*/

            lines[lines.Count - 1].material.DOColor(incorrectLineColor, durationToTweenLineColors);
        }

        public void startGame()
        {
            isTutorialMode = true;
            setupIndices();

            fleePositions = new List<Vector3>();
            foreach (Transform child in fleePositionObject.transform)
            {
                fleePositions.Add(child.position);
            }

            antura.AddComponent<MazeAntura>();
            //cracks to display:
            _cracks = new List<GameObject>();
            cracks.SetActive(true);
            foreach (Transform child in cracks.transform)
            {
                child.gameObject.SetActive(false);
                _cracks.Add(child.gameObject);
            }
            //lines = new List<GameObject>();

            lines = new List<LineRenderer>();

            roundNumber = 0;
            roundNumberText.text = "#" + (roundNumber + 1);

            gameTime = maxGameTime / (1 + MazeConfiguration.Instance.Difficulty);

            //init first letter
            MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Maze_Title, () =>
            {
                initCurrentLetter();
            });

            Context.GetAudioManager().PlayMusic(Music.Theme8);
        }

        public void initUI()
        {
            //ui:
            MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);

            timer.initTimer();
        }

        public void addLine()
        {

            pointsList = new List<Vector3>();
            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0, -0.2f);
            go.transform.Rotate(new Vector3(90, 0, 0));
            LineRenderer line = go.AddComponent<LineRenderer>();
            //line.material = new Material (Shader.Find ("Particles/Additive"));
            line.numPositions = 0;
            line.startWidth = 0.6f;
            line.endWidth = 0.6f;
            //line.SetColors (Color.green, Color.green);
            //line.useWorldSpace = true;    

            line.material = new Material(Shader.Find("Antura/Transparent"));
            line.material.color = drawingColor;

            lines.Add(line);

        }

        /*protected override void ReadyForGameplay()
		{
			base.ReadyForGameplay();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnMinigameQuit()
		{
			base.OnMinigameQuit();
		}*/

        public bool tutorialForLetterisComplete()
        {
            return currentTutorial.isCurrentTutorialDone();
        }

        public bool isCurrentLetterComplete()
        {
            return currentTutorial.isComplete();
        }

        public void showAllCracks()
        {
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;
            if (health == 0)
                return;

            for (int i = 0; i < _cracks.Count; ++i)
                _cracks[i].SetActive(true);
            //StartCoroutine (shakeCamera (0.5f, 0.5f));

        }
        public void wasHit()
        {

            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;
            _cracks[_cracks.Count - health].SetActive(true);
            health--;

            //StartCoroutine (shakeCamera (0.5f, 0.5f));

        }

        IEnumerator waitAndPerformCallback(float seconds, VoidDelegate init, VoidDelegate callback)
        {
            init();

            yield return new WaitForSeconds(seconds);

            callback();
        }


        public void moveToNext(bool won = false)
        {
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            isShowingAntura = false;
            //check if current letter is complete:
            if (currentCharacter.isComplete())
            {



                if (!isTutorialMode)
                {
                    correctLetters++;
                    roundNumber++;
                }
                //show message:
                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);

                //TutorialUI.MarkYes(currentCharacter.transform.position + new Vector3(2, 2, 2), TutorialUI.MarkSize.Huge);
                currentCharacter.Celebrate(() =>
                {
                    if (roundNumber == MAX_NUM_ROUNDS)
                    {
                        endGame();
                        return;
                    }
                    else
                    {
                        if (isTutorialMode)
                        {
                            isTutorialMode = false;
                            initUI();
                        }


                        roundNumberText.text = "#" + (roundNumber + 1);
                        restartCurrentLetter(won);
                    }
                });


                //print ("Prefab nbr: " + currentLetterIndex + " / " + prefabs.Count);

            }
            else
            {
                addLine();
                currentCharacter.nextPath();
                currentTutorial.moveToNextPath();
            }
        }

        public void lostCurrentLetter()
        {
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            if (isTutorialMode)
            {
                hideCracks();

                //remove last line
                if (lines.Count > 0)
                {
                    lines[lines.Count - 1].numPositions = 0;
                    lines.RemoveAt(lines.Count - 1);
                }

                pointsList.RemoveRange(0, pointsList.Count);

                //removeLines();

                TutorialUI.Clear(false);
                addLine();

                currentCharacter.resetToCurrent();
                showCurrentTutorial();
                return;
            }

            wrongLetters++;
            roundNumber++;
            if (roundNumber == MAX_NUM_ROUNDS)
            {
                endGame();
                return;
            }
            else
            {
                roundNumberText.text = "#" + (roundNumber + 1);

                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                restartCurrentLetter();
            }

        }

        public void restartCurrentLetter(bool won = false)
        {

            //Destroy (currentPrefab);
            int numberOfStars = 0;
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
            {
                numberOfStars = 1;
            }
            else
            {
                numberOfStars = 0;
            }

            if (numberOfStars > 0)
            {
                MinigamesUI.Starbar.GotoStar(numberOfStars - 1);
            }




            currentPrefab.SendMessage("moveOut", won);

            hideCracks();
            removeLines();

            initCurrentLetter();





        }

        void removeLines()
        {
            foreach (LineRenderer line in lines)
                line.numPositions = 0;
            lines = new List<LineRenderer>();
            pointsList.RemoveRange(0, pointsList.Count);

        }

        void hideCracks()
        {
            health = 4;
            //hide cracks:
            foreach (Transform child in cracks.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        public LL_LetterData currentLL = null;
        void initCurrentLetter()
        {
            currentCharacter = null;
            currentTutorial = null;

            TutorialUI.Clear(false);
            addLine();


            //get a new letter:
            IQuestionPack newQuestionPack = MazeConfiguration.Instance.Questions.GetNextQuestion();
            List<ILivingLetterData> ldList = (List<ILivingLetterData>)newQuestionPack.GetCorrectAnswers();
            LL_LetterData ld = (LL_LetterData)ldList[0];
            int index = -1;

            if (allLetters.ContainsKey(ld.Id))
                index = allLetters[ld.Id];
            if (index == -1)
            {
                Debug.Log("Letter got from Teacher is: " + ld.Id + " - does not match 11 models we have, we will play sound of the returned data");
                index = UnityEngine.Random.Range(0, prefabs.Count);
            }

            currentLL = ld;
            currentPrefab = (GameObject)Instantiate(prefabs[index]);

            /*int index = allLetters.IndexOf(ld.Id);

            int found = -1;
            for(int i =0; i < prefabs.Count; ++i)
            {
                if(prefabs[i].GetComponent<MazeLetterBuilder>().letterDataIndex == index)
                {
                    found = i;
                    
                    break;
                }
            }
            
            */


            //currentPrefab.GetComponent<MazeLetterBuilder>().letterData = ld;
            currentPrefab.GetComponent<MazeLetterBuilder>().build(() =>
            {

                if (!isTutorialMode)
                    MazeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(ld);



                foreach (Transform child in currentPrefab.transform)
                {
                    if (child.name == "Mazecharacter")
                        currentCharacter = child.GetComponent<MazeCharacter>();
                    else if (child.name == "HandTutorial")
                        currentTutorial = child.GetComponent<HandTutorial>();
                }

                currentCharacter.gameObject.SetActive(false);

                currentMazeLetter = currentPrefab.GetComponentInChildren<MazeLetter>();
            });
        }

        public void showCharacterMovingIn()
        {
            if (isTutorialMode)
            {
                MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Maze_Intro,
                        () =>
                        {
                            MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Maze_Tuto, () =>
                            {
                                MazeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(currentLL);
                            });
                            currentCharacter.initialPosition = currentCharacter.transform.position;
                            currentCharacter.initialRotation = currentCharacter.transform.rotation;
                            //currentCharacter.transform.position = new Vector3(0, 0, 15);
                            currentCharacter.gameObject.SetActive(true);
                            currentCharacter.Appear();
                        }
                        );
                return;
            }
            currentCharacter.initialPosition = currentCharacter.transform.position;
            currentCharacter.initialRotation = currentCharacter.transform.rotation;
            //currentCharacter.transform.position = new Vector3(0, 0, 15);
            currentCharacter.gameObject.SetActive(true);
            currentCharacter.Appear();
        }

        public void showCurrentTutorial()
        {
            isShowingAntura = false;

            if (currentTutorial != null)
            {
                currentTutorial.showCurrentTutorial();

            }

            if (currentCharacter != null)
            {
                currentCharacter.initialize();
            }
        }

        IEnumerator shakeCamera(float duration, float magnitude)
        {

            float elapsed = 0.0f;

            Vector3 originalCamPos = Camera.main.transform.position;

            while (elapsed < duration)
            {

                elapsed += Time.deltaTime;

                float percentComplete = elapsed / duration;
                float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

                // map value to [-1, 1]
                float x = UnityEngine.Random.value * 2.0f - 1.0f;
                float y = UnityEngine.Random.value * 2.0f - 1.0f;
                x *= magnitude * damper;
                y *= magnitude * damper;

                Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

                yield return null;
            }

            Camera.main.transform.position = originalCamPos;
        }

        public void appendToLine(Vector3 mousePos)
        {
            if (!pointsList.Contains(mousePos))
            {
                //mousePos.z = -0.1071415f;
                pointsList.Add(mousePos);
                lines[lines.Count - 1].numPositions = pointsList.Count;
                lines[lines.Count - 1].SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
            }
        }

        bool gameEnded = false;
        private void endGame()
        {
            if (gameEnded)
                return;

            gameEnded = true;

            MinigamesUI.Timer.Pause();
            TutorialUI.Clear(false);

            int numberOfStars = 0;
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
            {
                numberOfStars = 1;
            }
            else
            {
                numberOfStars = 0;
            }

            if (numberOfStars > 0)
            {
                MinigamesUI.Starbar.GotoStar(numberOfStars - 1);
            }

            EndGame(numberOfStars, correctLetters);
            //StartCoroutine(EndGame_Coroutine());
        }



        private IEnumerator EndGame_Coroutine()
        {
            yield return new WaitForSeconds(1f);
            int numberOfStars = 0;
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
            {
                numberOfStars = 1;
            }
            else
            {
                numberOfStars = 0;
            }
            EndGame(numberOfStars, correctLetters);

        }


        public void onTimeUp()
        {
            endGame();
        }

        public bool isShowingAntura = false;
        public void onIdleTime()
        {
            if (isShowingAntura) return;
            isShowingAntura = true;

            timer.StopTimer();

            antura.SetActive(true);
            antura.GetComponent<MazeAntura>().SetAnturaTime(true, currentCharacter.transform.position);

            int randIndex = UnityEngine.Random.Range(0, fleePositions.Count);
            currentCharacter.fleeTo(fleePositions[randIndex]);
        }

        public Vector3 getRandFleePosition()
        {
            int randIndex = UnityEngine.Random.Range(0, fleePositions.Count);
            return (fleePositions[randIndex]);
        }

        //states
        public MazeIntroState IntroductionState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return MazeConfiguration.Instance;
        }

        protected override IState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new MazeIntroState(this);

            inputManager = context.GetInputManager();
            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerUp += OnPointerUp;
        }
    }
}