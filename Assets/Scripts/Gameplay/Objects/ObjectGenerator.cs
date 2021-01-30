using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    public class ObjectGenerator : MonoBehaviour
    {
        private ObjectTileComponent[,] objectTiles;
        private List<ObjectComponent> objectComponents = new List<ObjectComponent>();

        private Vector2Int gridDimensions;
        private Vector2Int goalCoordinate;

        public int[] goalObjectCombination;
        public const int goalUniqueValue = 3;

        private void Awake()
        {
            
        }

        public (Vector2Int, ObjectTileComponent[]) Initialize(PathManager pathManager, ObjectProfile objectProfile, int puzzleCount)
        {
            // --- Collect Tiles ---
            int xLength = pathManager.GridDimensions.x;
            int yLength = pathManager.GridDimensions.y;

            objectTiles = new ObjectTileComponent[xLength, yLength];
            gridDimensions = new Vector2Int(xLength, yLength);

            for(int y = 0; y < yLength; y++)
            {
                for(int x = 0; x < xLength; x++)
                {
                    objectTiles[x, y] = pathManager.PathGrid[x, y].GetComponent<ObjectTileComponent>();
                }
            }

            // --- Generate Unique Combination ---
            int objectCount = objectProfile.objects.Length;

            int xArea = Random.Range(1, pathManager.GridDimensions.x - 1);
            int yArea = Random.Range(1, pathManager.GridDimensions.y - 1);
            goalCoordinate = new Vector2Int(xArea, yArea);

            var rand = new System.Random();
            goalObjectCombination = GetArrayOfUniqueNumbers(objectCount);

            // --- Puzzles ---
            ObjectTileComponent[] puzzleObjectTiles = new ObjectTileComponent[puzzleCount];
            int[] randomList = GetArrayOfUniqueNumbers(puzzleCount);

            for(int i = 0; i < puzzleCount - 1; i++)
            {
                int xAreaPuz = Random.Range(1, pathManager.GridDimensions.x - 1);
                int yAreaPuz = Random.Range(1, pathManager.GridDimensions.y - 1);
                puzzleObjectTiles[i] = objectTiles[xAreaPuz, yAreaPuz];
            }

            puzzleObjectTiles[puzzleCount-1] = objectTiles[goalCoordinate.x, goalCoordinate.y];

            // --- Fill Tiles ---
            for(int y = 0; y < yLength; y++)
            {
                for(int x = 0; x < xLength; x++)
                {
                    objectTiles[x, y].Initialize();

                    if(x == goalCoordinate.x && y == goalCoordinate.y) // Place Goal Tile
                        SetObjectTileComponent(objectProfile, objectTiles[x, y], goalObjectCombination);
                    else // Place Random Tile
                        SetObjectTileComponent(objectProfile, objectTiles[x, y]);
                }
            }

            // --- Mark Goal ---
            #if UNITY_EDITOR
            objectTiles[goalCoordinate.x, goalCoordinate.y].MarkGoalObject(true);
            #endif

            return (goalCoordinate, puzzleObjectTiles);
        }

        public void Deinitialize()
        {
            for(int i = 0; i < objectComponents.Count; i++)
                Destroy(objectComponents[i].gameObject);

            objectComponents.Clear();
        }

        #region Instantiate

        private void SetObjectTileComponent(ObjectProfile objectProfile, ObjectTileComponent objectTileComponent, params int[] uniqueIDs)
        {
            bool uniqueIDsSet = uniqueIDs != null && uniqueIDs.Length > 0;
            int anchorCount = objectTileComponent.ObjectAnchorCount;
            int objectCount = objectProfile.objects.Length;

            // Set ID Pool
            int[] randomIds = null;

            if(uniqueIDsSet)
            {
                randomIds = uniqueIDs;
            }
            else
            {
                bool uniqueCombi = true;
                randomIds = GetArrayOfUniqueNumbers(objectCount);

                if(anchorCount > goalUniqueValue)
                {
                    while(uniqueCombi)
                    {
                        int equalCount = 0;

                        for(int i = 0; i < anchorCount; i++)
                        {
                            for(int x = 0; x < anchorCount; x++)
                            {
                                if(randomIds[i] == goalObjectCombination[x])
                                {
                                    ++equalCount;
                                    break;
                                }
                            }
                        }

                        if(equalCount >= goalUniqueValue)
                            randomIds = GetArrayOfUniqueNumbers(objectCount);
                        else
                            uniqueCombi = false;
                    }
                }
            }

            for(int i = 0; i < anchorCount; i++)
            {
                GameObject anchor = objectTileComponent.ObjectAnchors[i];
                ObjectComponent instance = InstantiateObject(objectProfile, anchor.transform, randomIds[i]);
                objectComponents.Add(instance);

                objectTileComponent.AddObjectToAnchor(i, instance);
            }
        }

        private ObjectComponent InstantiateObject(ObjectProfile objectProfile, Transform anchor, int index)
        {
            ObjectComponent objectComponentPrefab = objectProfile.objects[index];
            ObjectComponent objectComponentInstance = Instantiate(objectComponentPrefab, anchor);

            return objectComponentInstance;
        }
    
        private int[] GetArrayOfUniqueNumbers(int maxValue)
        {
            int[] nums = Enumerable.Range(0, maxValue).ToArray();
            System.Random rnd = new System.Random();

            // Shuffle the array
            for(int i = 0; i < nums.Length; ++i)
            {
                int randomIndex = rnd.Next(nums.Length);
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            return nums;
        }

        #endregion
    }
}