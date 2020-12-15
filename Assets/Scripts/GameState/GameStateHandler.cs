using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Hosting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameStateHandler
{
    public static readonly string SaveFolder =
        Application.dataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;

    private GameState latestGameState;

    public GameStateHandler()
    {
        //Create save directory if not exists
        if (!Directory.Exists(SaveFolder))
        {
            Directory.CreateDirectory(SaveFolder);
        }
    }

    private string GenerateSaveFileName()
    {
        return GetFullSaveFileName(DateTime.Now.ToString("yyyyMMddHHmmssffff"));
    }

    private string GetFullSaveFileName(string fileName)
    {
        return fileName + ".json";
    }

    public bool SaveGameState()
    {
        var objects = Object.FindObjectsOfType<GameStateComponent>();
        
        Debug.Log("Found " + objects.Length);

        //Turn game state components to GameObjectState List
        var states = objects.Select(g => new GameObjectState(g)).ToList();

        Debug.Log("Found states" + states.Count);

        latestGameState = new GameState(states);
        Debug.Log(JsonUtility.ToJson(latestGameState));
        //Write json file
        //File.WriteAllText(SaveFolder + GenerateSaveFileName(), JsonUtility.ToJson(states));
        return true;
    }
    
    public bool QuickLoadGameState()
    {
        if (latestGameState != null)
        {
            return latestGameState.RestoreState();
        }
        
        return false;
    }

    private string ReadGameState(string saveFile)
    {
        var objects = Object.FindObjectsOfType<GameStateComponent>();

        var states = objects.Select(g => new GameObjectState(g)).ToList();
        
        if (File.Exists(SaveFolder + GetFullSaveFileName(saveFile)))
        {
            return File.ReadAllText(SaveFolder + GenerateSaveFileName());
        }

        //Return null if save is unknown
        return null;
    }

    private GameState CreateGameState(string saveFile)
    {
        return JsonUtility.FromJson<GameState>(saveFile);
    }

    [Serializable]
    public class GameState
    {
        [SerializeField] public List<GameObjectState> gameObjectStates;

        public GameState(List<GameObjectState> gameObjectStates)
        {
            this.gameObjectStates = gameObjectStates;
        }


        public bool RestoreState()
        {
            var success = true;
            
            //Fetch all relevant components here so we don't call it once per game object
            var objects = Object.FindObjectsOfType<GameStateComponent>();

            foreach (var gameObjectState in gameObjectStates)
            {
                //Restore each game object. If it returns false we failed to restore everything
                if (!gameObjectState.RestoreState(objects))
                {
                    success = false;
                }
            }

            return success;
        }
    }
    
    [Serializable]
    public class GameObjectState
    {
        private GameObject _gameObject;

        [SerializeField] public string guid;
        
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Vector3 position;

        [SerializeField] private Quaternion localRotation;
        [SerializeField] private Quaternion rotation;

        public GameObjectState(GameStateComponent gameStateComponent)
        {
            _gameObject = gameStateComponent.gameObject;
            guid = gameStateComponent.guid;
            localPosition = _gameObject.transform.localPosition;
            position = _gameObject.transform.position;
            localRotation = _gameObject.transform.localRotation;
            rotation = _gameObject.transform.rotation;
        }

        public bool RestoreState(GameStateComponent[] objects)
        {
            foreach (var gameStateComponent in objects)
            {
                if (!gameStateComponent.guid.Equals(guid))
                {
                    continue;
                }

                var gameObject = gameStateComponent.gameObject;
                gameObject.transform.localPosition = localPosition;
                gameObject.transform.position = position;

                gameObject.transform.localRotation = localRotation;
                gameObject.transform.rotation = rotation;
                return true;
            }

            return false;
        }
    }
}