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

        public ObjectTileComponent[,] ObjectTiles => objectTiles;

        private void Awake()
        {
            
        }

        public void Initialize(PathManager pathManager, ObjectProfile objectProfile)
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
            
            // --- Fill Tiles ---
            for(int y = 0; y < yLength; y++)
            {
                for(int x = 0; x < xLength; x++)
                {
                    objectTiles[x, y].Initialize(x,y);
                    SetObjectTileComponent(objectProfile, objectTiles[x, y], y);
                }
            }
        }

        public (Vector2Int, ObjectTileComponent[]) GeneratePuzzleTiles(PathManager pathManager, ObjectProfile objectProfile, int puzzleCount)
        {
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

            puzzleObjectTiles[puzzleCount - 1] = objectTiles[goalCoordinate.x, goalCoordinate.y];

            return (goalCoordinate, puzzleObjectTiles);
        }

        public void Deinitialize()
        {
            for(int i = 0; i < objectComponents.Count; i++)
                Destroy(objectComponents[i].gameObject);

            objectComponents.Clear();
        }

        #region Instantiate

        private void SetObjectTileComponent(ObjectProfile objectProfile, ObjectTileComponent objectTileComponent, int row, params int[] uniqueIDs)
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

                //if(anchorCount > goalUniqueValue)
                //{
                //    while(uniqueCombi)
                //    {
                //        int equalCount = 0;

                //        for(int i = 0; i < anchorCount; i++)
                //        {
                //            for(int x = 0; x < anchorCount; x++)
                //            {
                //                if(randomIds[i] == goalObjectCombination[x])
                //                {
                //                    ++equalCount;
                //                    break;
                //                }
                //            }
                //        }

                //        if(equalCount >= goalUniqueValue)
                //            randomIds = GetArrayOfUniqueNumbers(objectCount);
                //        else
                //            uniqueCombi = false;
                //    }
                //}
            }

            for(int i = 0; i < anchorCount; i++)
            {
                GameObject anchor = objectTileComponent.ObjectAnchors[i];
                ObjectPlacement anchorPlacement = objectTileComponent.objectAnchorsInfo[i];
                ObjectComponent objectComponentPrefab = objectProfile.objects[randomIds[i]];

                if(!anchorPlacement.IsLabelActive(objectComponentPrefab.name))
                    randomIds[i] = anchorPlacement.GetRandomFreeIndex();

                ObjectComponent instance = InstantiateObject(objectProfile, anchor.transform, row, randomIds[i]);

                objectComponents.Add(instance);
                objectTileComponent.AddObjectToAnchor(i, instance);
            }
        }

        private ObjectComponent InstantiateObject(ObjectProfile objectProfile, Transform anchor, int row, int index)
        {
            ObjectComponent objectComponentPrefab = objectProfile.objects[index];
            ObjectComponent objectComponentInstance = Instantiate(objectComponentPrefab, anchor);

            int spriteOrderAdd = (row*10) + index;
            objectComponentInstance.IncreaseSpriteOrder(spriteOrderAdd);

            return objectComponentInstance;
        }
    
        private int[] GetArrayOfUniqueNumbers(int maxValue)
        {
            int[] nums = Enumerable.Range(0, maxValue).ToArray();
            //System.Random rnd = new System.Random();

            // Shuffle the array
            for(int i = 0; i < nums.Length; ++i)
            {
                int randomIndex = Random.Range(0, maxValue);
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            return nums;
        }

        #endregion
    }
}