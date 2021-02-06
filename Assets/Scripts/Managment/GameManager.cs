using GGJ21.Gameplay.Objects;
using GGJ21.Gameplay.Words;
using GGJ21.General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GGJ21.Game.Core
{
    /// <summary>
    /// GameManager - 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Definitions

        public enum ApplicationState
        {
            Preload,
            MainMenu,
            Game
        }

        public enum GamePhase
        {
            Intro,
            Map,
            Words,
            Pause,
            Restart,
            GameOver
        }

        public enum MatchResult
        {
            None,
            Win,
            Loose
        }

        #endregion

        #region Settings/Variables

        // --- References ---
        public bool ingameRepresentation = false;
        public bool debug = false;
        public bool winWithOne = false;
        public ResourceManager resourceManager;
        public AssetReference[] gameScenes;

        // --- Variables ---
        // References
        private UIManager uiManager;
        private InputManager inputManager;
        private CameraManager cameraManager;
        private SceneSettings sceneSettings;
        private Localisation.LocalisationManager localisationManager;

        private PathManager pathManager;
        private ObjectGenerator objectGenerator;
        private WordManager wordManager;
        private UIWordPuzzleManager uiWordManager;

        private PathMovement characterMovement;
        private InteractObjectComponent characterInteractor;
        
        private bool tutorialVisited = false;

        // States
        private ApplicationState applicationState;
        private GamePhase gamePhase;
        private GamePhase lastGamePhase;
        private GamePhase beforeIntroPhase;

        private AssetReference currentGameScene;

        // Gameplay
        private int selectedLevelIndex = -1;
        private MatchConditionsProfile matchConditionsProfile;
        private MatchWinCondition winCondition;
        private MatchResult matchResult;

        private bool boardManagerListenerSet = false;

        private int totalMoves = 0;
        private int _remainingMoves = 0;
        private int _score = 0;

        private ObjectTileComponent[] puzzleTiles;
        private Vector2Int goalTile;
        private TreasureCheast treasureCheast;

        private Vector2Int markedTile;
        private ObjectComponent markedObject;
        private bool foundMarkedObject;

        private LayerMask groundMask;
        private LayerMask objectMask;

        public bool helpAvailable = false;

        private float helpDurtaiton = 5f;
        private float helpTimer = 5f;
        private bool helpTimerActive = false;

        // --- Properties ---
        public int RemainingMoves
        {
            get { return _remainingMoves; }

            private set
            {
                _remainingMoves = value;
                OutputMoves();
            }
        }

        public int Score
        {
            get { return _score; }

            private set
            {
                _score = value;
                OutputScore();
            }
        }

        public bool OnMap => applicationState == ApplicationState.Game && gamePhase == GamePhase.Map;

        public bool HelpAvailable => helpAvailable;

        public Vector3 PlayerPosition { get; private set; }

        public Vector3 TargetPosition { get; private set; }

        #endregion

        private void Awake()
        {
            groundMask = LayerMask.GetMask("Ground");
            objectMask = LayerMask.GetMask("Interactable");

            // --- Clean Methods for GameScene Setup ----

            // GameController
            GameObject[] gameControllerInstances = GameObject.FindGameObjectsWithTag("GameController");
            
            if(gameControllerInstances.Length > 1)
                Destroy(gameObject);
            else
                resourceManager.InitializePreload();

            // Cameras
            GameObject[] mainCameraInstances = GameObject.FindGameObjectsWithTag("MainCamera");

            if(mainCameraInstances.Length > 1)
                Destroy(mainCameraInstances[1]);

            GameObject gameCameraInstance = GameObject.FindGameObjectWithTag("GameCamera");

            if(gameCameraInstance != null)
                Destroy(gameCameraInstance);
        }

        private void Start()
        {
            StartPreload();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            
            switch(applicationState)
            {
                case ApplicationState.Preload:
                    ProcessPreloader(dt);
                    break;
                case ApplicationState.MainMenu:
                    ProcessMenu(dt);
                    break;
                case ApplicationState.Game:
                    ProcessGame(dt);
                    break;
                default:
                    break;
            }
        }

        #region MenuState

        public void ChangeApplicationState(ApplicationState newState)
        {
            // --- Exit Old State ---
            ApplicationState oldState = applicationState;

            switch(oldState)
            {
                case ApplicationState.Preload:
                    break;
                case ApplicationState.MainMenu:
                    HideMenu();
                    break;
                case ApplicationState.Game:
                    EndGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldState), oldState, null);
            }

            // --- Enter New State ---
            applicationState = newState;

            switch(newState)
            {
                case ApplicationState.Preload:
                    EndPreload();
                    break;
                case ApplicationState.MainMenu:
                    ShowMenu();
                    break;
                case ApplicationState.Game:
                    if(oldState == ApplicationState.MainMenu)
                        InitializeGame();
                    else
                        InitializeGameComplete();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        #endregion

        #region GamePhase

        public void ChangeGamePhase(GamePhase newPhase)
        {
            // --- Exit Old State ---
            GamePhase oldPhase = gamePhase;

            switch(oldPhase)
            {
                case GamePhase.Intro:
                    break;
                case GamePhase.Map:
                    break;
                case GamePhase.Words:
                    break;
                case GamePhase.Pause:
                    CallUnpause();
                    break;
                case GamePhase.Restart:
                    break;
                case GamePhase.GameOver:
                    CallUnpause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldPhase), oldPhase, null);
            }

            // --- Enter New State ---
            gamePhase = newPhase;

            switch(newPhase)
            {
                case GamePhase.Intro:
                    CallIntro();
                    break;
                case GamePhase.Map:
                    CallMapPanel();
                    break;
                case GamePhase.Words:
                    CallWordPuzzle();
                    break;
                case GamePhase.Pause:
                    CallPause();
                    break;
                case GamePhase.Restart:
                    CallRestart();
                    break;
                case GamePhase.GameOver:
                    StartGameOver();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newPhase), newPhase, null);
            }
        }

        private bool CheckMatchConditions(out MatchResult matchResult)
        {
            if(matchConditionsProfile.ValidateWinCondition(Score, RemainingMoves))
            {
                // Check Win Condition
                matchResult = MatchResult.Win;
                return true;
            }
            else if(matchConditionsProfile.ValidateLooseCondition(RemainingMoves))
            {
                // Check loose Condition
                matchResult = MatchResult.Loose;
                return true;
            }

            matchResult = MatchResult.None;
            return false;
        }

        private void StartGameOver()
        {
            inputManager.SetInputActive(false);

            if(matchResult == MatchResult.Win)
                uiManager.ChangeUIPanel("GameOver_Win", 0f, 0.5f);
            else
                uiManager.ChangeUIPanel("GameOver_Loose", 0f, 0.5f);

            OutputScore();
            OutputMoves();
        }

        #endregion

        #region Preload State

        private void StartPreload()
        {
            resourceManager.LoadPreloadAssets(PreloadDone);
        }

        private void ProcessPreloader(float dt)
        {

        }

        private void EndPreload()
        {

        }

        private void PreloadDone()
        {
            uiManager = FindObjectOfType<UIManager>();

            // --- Button Registration ----
            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("MainMenu",
                new UIManager.UIButtonRegistationAction[]
                {
                    new UIManager.UIButtonRegistationAction("Start", CallStart),
                    new UIManager.UIButtonRegistationAction("Exit", QuitGame),
                }));

            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("Intro",
                new UIManager.UIButtonRegistationAction[]
                {
                    new UIManager.UIButtonRegistationAction("Continue", OnIntroContinue),
                }));

            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("Game",
                new UIManager.UIButtonRegistationAction[]
                {
                    new UIManager.UIButtonRegistationAction("Map", OnButtonMapClick),
                    new UIManager.UIButtonRegistationAction("Words", OnButtonWordsClick),
                    new UIManager.UIButtonRegistationAction("Menu", OnButtonPauseMenuClick),
                    new UIManager.UIButtonRegistationAction("Help", OnButtonHelpClick),
                }));

            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("WordPuzzle",
                new UIManager.UIButtonRegistationAction[]
                {
                    new UIManager.UIButtonRegistationAction("Map", OnButtonMapClick),
                    new UIManager.UIButtonRegistationAction("Words", OnButtonWordsClick),
                    new UIManager.UIButtonRegistationAction("Menu", OnButtonPauseMenuClick),
                    new UIManager.UIButtonRegistationAction("Help", OnButtonHelpClick),
                }));

            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("Pause",
                new UIManager.UIButtonRegistationAction[]
                {
                    new UIManager.UIButtonRegistationAction("Resume", OnButtonPauseMenuResumeClick),
                    new UIManager.UIButtonRegistationAction("Restart", OnButtonPauseMenuRestartClick),
                    new UIManager.UIButtonRegistationAction("Exit", OnButtonPauseMenuExitClick),
                }));
        
            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("GameOver_Win",
               new UIManager.UIButtonRegistationAction[]
               {
                    new UIManager.UIButtonRegistationAction("Restart", OnButtonPauseMenuRestartClick),
                    new UIManager.UIButtonRegistationAction("Exit", OnButtonPauseMenuExitClick),
               }));

            uiManager.RegisterButtonActionsOnPanel(new UIManager.UIPanelButtonsRegistation("GameOver_Loose",
               new UIManager.UIButtonRegistationAction[]
               {
                    new UIManager.UIButtonRegistationAction("Restart", OnButtonPauseMenuRestartClick),
                    new UIManager.UIButtonRegistationAction("Exit", OnButtonPauseMenuExitClick),
               }));

            // --- Show UI ---
            uiManager.Show(UIManager.CANVAS_FADEIN_DURATION);
            localisationManager = GetComponent<Localisation.LocalisationManager>();

            // --- Initialize BoardMananger & InputManager ---
            inputManager = GetComponent<InputManager>();
            inputManager.cameraReference = Camera.main;
            inputManager.OnMouseDown.AddListener(OnTileClick);

            cameraManager = Camera.main.GetComponent<CameraManager>();

            objectGenerator = GetComponent<ObjectGenerator>();
            wordManager = GetComponent<WordManager>();
            uiWordManager = uiManager.GetComponent<UIWordPuzzleManager>();

            uiWordManager.OnPuzzleSolved.AddListener(OnPuzzleSolved);

            // --- Move to MainMenu ---
            if(ingameRepresentation)
                ChangeApplicationState(ApplicationState.Game);
            else
                ChangeApplicationState(ApplicationState.MainMenu);
        }
        
        #endregion

        #region Menu State

        // --- Panel Functions ---
        private void ShowMenu()
        {
            uiManager.ChangeUIPanel("MainMenu");
        }

        private void ProcessMenu(float dt)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                QuitGame();
        }

        private void HideMenu()
        {

        }

        // --- Button Actions ---
        private void CallStart()
        {
            selectedLevelIndex = 0;
            ChangeApplicationState(ApplicationState.Game);
        }

        #endregion

        #region Game State

        // --- Panel Functions ---
        private void InitializeGame()
        {
            currentGameScene = gameScenes[selectedLevelIndex];
            resourceManager.LoadScene(currentGameScene, InitializeGameComplete);
        }

        private void InitializeGameComplete()
        {
            // --- Get SceneSettings ---
            GameObject sceneSettingsObject = GameObject.FindGameObjectWithTag("SceneSettings");

            if(sceneSettingsObject != null && sceneSettingsObject.TryGetComponent(out SceneSettings sceneSettings))
            {
                this.sceneSettings = sceneSettings;
                this.characterMovement = sceneSettings.character;
                this.characterInteractor = sceneSettings.character.GetComponent<InteractObjectComponent>();

                inputManager.InitializeBoardInput(sceneSettings.boardOrigin);
                cameraManager.Initialize(this.characterMovement.transform);

                matchConditionsProfile = sceneSettings.matchConditions;
                winCondition = new MatchWinCondition(this.matchConditionsProfile.winCondtion);

                pathManager = GameObject.FindGameObjectWithTag("PathManager")?.GetComponent<PathManager>();
                objectGenerator = GetComponent<ObjectGenerator>();

                int winConditionValue = matchConditionsProfile.winCondtion.puzzleCount;
                int wordCount = matchConditionsProfile.winCondtion.wordCount;

                (goalTile, puzzleTiles) = objectGenerator.Initialize(pathManager, sceneSettings.objectProfile, winConditionValue);
                wordManager.CreateWordPuzzles(puzzleTiles, wordCount, localisationManager.Language);

                if(!boardManagerListenerSet)
                    boardManagerListenerSet = true;

                treasureCheast = sceneSettings.treasureCheast;

                helpDurtaiton = sceneSettings.matchConditions.helpDuration;

                StartGame();
            }
        }

        private void StartGame()
        {
            treasureCheast.Hide();
            matchResult = MatchResult.None;
            CallIntro();

            Score = 0;
            tutorialVisited = false;

            SetPlayerPosition();
            inputManager.SetInputActive(true);
        }

        private void RestartGame()
        {
            treasureCheast.Hide();
            matchResult = MatchResult.None;

            SetPlayerPosition();
            inputManager.SetInputActive(true);

            int winConditionValue = matchConditionsProfile.winCondtion.puzzleCount;
            int wordCount = matchConditionsProfile.winCondtion.wordCount;

            objectGenerator.Deinitialize();
            (goalTile,puzzleTiles) = objectGenerator.Initialize(pathManager, sceneSettings.objectProfile, winConditionValue);
           
            wordManager.CreateWordPuzzles(puzzleTiles, wordCount, localisationManager.Language);
            uiWordManager.HideWordList();

            ShowGame();

            Score = 0;
            RemainingMoves = totalMoves;

            ChangeGamePhase(GamePhase.Map);
        }

        private void SetPlayerPosition()
        {
            cameraManager.SetCameraEnabled(false);
            pathManager.SetPositionOfPathMovementComponent(characterMovement);
            StartCoroutine(ResetCamera());
        }

        private IEnumerator ResetCamera()
        {
            yield return null;
            cameraManager.SetCameraEnabled(true);
        }

        private void CallIntro()
        {
            beforeIntroPhase = gamePhase;
            inputManager.SetInputActive(false);
            uiManager.ChangeUIPanel("Intro");
        }

        private void ShowGame()
        {
            OutputScore();
            uiManager.ChangeUIPanel("Game");
        }

        private void ProcessGame(float dt)
        {
            // --- Navigation Input ---
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(gamePhase == GamePhase.Pause || gamePhase == GamePhase.GameOver)
                    OnButtonPauseMenuExitClick();
                else if(gamePhase != GamePhase.Pause)
                    OnButtonPauseMenuClick();
            }
            else if(Application.isEditor && Input.GetKeyDown(KeyCode.R))
            {
                CallRestart();
            }
            else if(Application.isEditor && Input.GetKeyDown(KeyCode.Return))
            {
                OnPuzzleSolved();
            }

            // Update Player Position
            if(gamePhase == GamePhase.Map)
            {
                PlayerPosition = characterMovement.transform.position;

                if(helpTimerActive && helpTimer > 0f)
                {
                    helpTimer -= dt;

                    if(helpTimer < 0f)
                        helpAvailable = true;
                }
            }
        }

        private void EndGame()
        {
            Score = RemainingMoves = 0;
            resourceManager.UnloadScene(currentGameScene, EndGameComplete);
        }

        private void EndGameComplete()
        {
            
        }

        // --- Words ---

        private void CallMapPanel()
        {
            inputManager.SetInputActive(true);
            uiManager.ChangeUIPanel("Game"); 
            OutputScore();
        }

        private void CallWordPuzzle()
        {
            inputManager.SetInputActive(false);
            uiWordManager.InitializePuzzle(wordManager.CurrentWordPuzzle);
            uiManager.ChangeUIPanel("WordPuzzle");
            OutputScore();
        }  

        private void CallNewWordPuzzle()
        {
            #if UNITY_EDITOR
            Vector2Int coordinates = wordManager.CurrentWordPuzzle.coordinate;
            objectGenerator.ObjectTiles[coordinates.x, coordinates.y].MarkGoalObject(false);
            #endif

            wordManager.NextWordPuzzle();
            uiWordManager.ClearWordList();

            CallWordPuzzle();

            helpAvailable = false;
            helpTimerActive = false;
        }

        private void OnPuzzleSolved()
        {
            ++Score;
            ChangeGamePhase(GamePhase.Map);

            Vector2Int coordinates = wordManager.CurrentWordPuzzle.coordinate;
            ObjectTileComponent objectTileComponent = objectGenerator.ObjectTiles[coordinates.x, coordinates.y];
            TargetPosition = objectTileComponent.GetComponent<PathComponent>().center.position;

            #if UNITY_EDITOR
            if(debug)
                objectTileComponent.MarkGoalObject(true);
            #endif

            helpTimer = helpDurtaiton;
            helpTimerActive = true;

            // --- Show Treasure Cheast ---
            if(Score == winCondition.puzzleCount)
            {
                GameObject anchor = objectTileComponent.GetRandomAnchor();

                treasureCheast.transform.position = anchor.transform.position;
                treasureCheast.Show();
            }
        }

        // --- Button Actions ---

        private void OnButtonHelpClick()
        {
            ChangeGamePhase(GamePhase.Intro);
        }

        private void OnIntroContinue()
        {
            if(tutorialVisited)
                ChangeGamePhase(GamePhase.Map);
            else
                ChangeGamePhase(GamePhase.Words);

            tutorialVisited = true;
        }

        private void OnButtonMapClick()
        {
            if(gamePhase == GamePhase.Map)
                return;

            ChangeGamePhase(GamePhase.Map);
        }

        private void OnButtonWordsClick()
        {
            if(gamePhase == GamePhase.Words)
                return;

            ChangeGamePhase(GamePhase.Words);
        }

        private void OnButtonPauseMenuClick()
        {
            if(gamePhase != GamePhase.Pause)
                lastGamePhase = gamePhase;

            ChangeGamePhase(GamePhase.Pause);
        }

        private void OnButtonPauseMenuResumeClick()
        {
            ChangeGamePhase(lastGamePhase);
        }

        private void OnButtonPauseMenuRestartClick()
        {
            ChangeGamePhase(GamePhase.Restart);
        }

        private void OnButtonPauseMenuExitClick()
        {
            if(ingameRepresentation)
                QuitGame();
            else
                ChangeApplicationState(ApplicationState.MainMenu);
        }

        #endregion

        #region Pause

        public void CallPause()
        {
            uiManager.ChangeUIPanel("Pause");
            inputManager.SetInputActive(false);
        }

        private void CallUnpause()
        {
            inputManager.SetInputActive(true);
            ShowGame();
        }

        #endregion

        #region Restart

        private void CallRestart()
        {
            RestartGame();
        }

        #endregion

        #region UI Events

        private void OutputScore()
        {
            Debug.Log(string.Format("{0}/{1}", _score.ToString(), winCondition.puzzleCount));
            uiManager.RaiseTextOutput("Score", string.Format("{0}/{1}", _score.ToString(), winCondition.puzzleCount));
        }

        private void OutputMoves()
        {
            if(matchResult != MatchResult.None)
            {
                uiManager.RaiseTextOutput("Moves", string.Format("{0}/{1}", (totalMoves-_remainingMoves), totalMoves));
            }
            else
            {
                if(winCondition.condition == WinCondition.Endless)
                    uiManager.RaiseTextOutput("Moves", Mathf.Abs(_remainingMoves).ToString());
                else
                    uiManager.RaiseTextOutput("Moves", string.Format("{0}/{1}", _remainingMoves, totalMoves));
            }
        }

        public WordPuzzleCollection GetCurrentWordPuzzle()
        {
            return wordManager.CurrentWordPuzzle;
        }

        #endregion

        private void OnTileClick(Vector3 position)
        {
            float length = 0f;
            Ray ray = new Ray(position + new Vector3(0f, 0f, -5f), Vector3.forward);

            #if UNITY_EDITOR
            Debug.DrawRay(position, Vector3.up, Color.white, 10f); 
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 10f);
            #endif

            // --- Select Object ---
            foundMarkedObject = false;
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, objectMask);

            if(hit.collider != null && hit.transform.TryGetComponent(out markedObject))
                foundMarkedObject = true;

            // --- MoveTo Tile ---
            (bool status, Vector2Int tile) = characterMovement.MoveTo(position, MoveToComplete);

            markedTile = tile;

            if(!status && foundMarkedObject)
                InteractWithObject();
        }

        private void MoveToComplete()
        {
            if(winWithOne && markedTile == wordManager.CurrentWordPuzzle.coordinate)
            {
                matchResult = MatchResult.Win;
                PlayFinishAnimation(matchResult);
            }
            else if(markedTile == wordManager.CurrentWordPuzzle.coordinate && CheckMatchConditions(out MatchResult matchResult) && matchResult == MatchResult.Win)
            {
                PlayFinishAnimation(matchResult);
            }
            else if(foundMarkedObject)
            {
                InteractWithObject();
            }
        }

        private void InteractWithObject()
        {
            if(characterInteractor.IsInteracting)
                return;

            inputManager.SetInputActive(false);
            characterInteractor.Interact(markedObject, InteractWithObjectComplete);
        }

        private void InteractWithObjectComplete()
        {
            markedObject = null;
            inputManager.SetInputActive(true);

            if(wordManager.CurrentWordPuzzle.coordinate == markedTile && Score == wordManager.CurrentWordPuzzleIndex+1)
                CallNewWordPuzzle();
        }

        private void PlayFinishAnimation(MatchResult matchResult)
        {
            this.matchResult = matchResult;
            inputManager.SetInputActive(false);

            characterInteractor.PlayFinish(PlayFinishAnimationComplete);
        }

        private void PlayFinishAnimationComplete()
        {
            ChangeGamePhase(GamePhase.GameOver);
        }

        #region Quit

        public void QuitGame()
        {
            // save any game data here
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        #endregion
    }
}

