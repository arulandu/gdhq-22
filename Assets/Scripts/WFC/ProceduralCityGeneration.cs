using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProceduralCityGeneration : MonoBehaviour {

    //stats (cannot be tweaked in the editor must be tweaked in script)
    public float tileSize {get;} = 6; //the size of tiles in the gameWorld
    public int defaultYPos {get;} = 9;
    public int mapWidth; //width of the map lol
    public int mapHeight; //height of the map lol
    public Color streetColor; //color of the texture that the algorithm will read as a street 
    public int textureResolution {get;} = 2;
    public Texture2D pattern; //input pattern to the WFC algorithm

    public float streetXOffsetX;
    public float streetXOffsetZ;
    public float streetZOffsetX;
    public float streetZOffsetZ;
    public float grassOffsetX;
    public float grassOffsetZ;
    public float houseUpOffsetX;
    public float houseUpOffsetZ;
    public float houseDownOffsetX;
    public float houseDownOffsetZ;
    public float houseLeftOffsetX;
    public float houseLeftOffsetZ;
    public float houseRightOffsetX;
    public float houseRightOffsetZ;

    public int houseCount;
    public bool isSchool = false;
    static System.Random random = new System.Random(); //instantiate a random class (idk why c# does this either)

    static Dictionary <Vector2, GameObject> tileDictionary;
    static Texture cityTexture; //proc gen texture to build the city off of 
    static int [ , ] cityMap; // thing that store the location of every street and everything so that we can make a map of it later


    public GameObject[] cityElements = new GameObject[12]; //array of all of the cityElements (models) that can be set in the editor

    //the only reason this exists is to act as a reference to which index the city elements are in
    public enum cityElementsNames { 

        buildingBase = 1,
        buildingWindow = 2,
        houseUp = 3,
        houseDown = 4,
        houseLeft = 5,
        houseRight = 6,
        school = 7,
        streetX = 8,
        streetZ = 9,
        streetIntersection = 10,
        grass = 11
    } 

    //a building represents a 3 dimensional array of building objects each of which are cubical
    struct building {

        //xyz coord
        float xPos;
        float yPos;
        float zPos;

        //length width and height just cause they are useful things to have as a reference
        uint width;
        uint height;
        uint length;

        uint tileSize; //how large the 3d model is in world space 

        GameObject usedTile; //a reference to the tile prefab that is currently being used to generate the building

        Quaternion usedQuaternion; //a reference to the rotation quaternion that is currently being used to generate the thing

        GameObject[ , , ] buildingArray; //WTHHHHHHHHH IS THIS SYNTAX I HATE IT HERE but this represents the full building as a 3d array of gameObjects


        public building (float xPos_, float yPos_, float zPos_,    uint width_, uint height_, uint length_,    uint tileSize_,    Procedural_City_Generation thisClass) : this(){
            
            //set the variables of the building's xyz, width, length, and height (and also tilesize) based on what was passed
            xPos = xPos_;
            yPos = yPos_;
            zPos = zPos_;
            width = width_;
            height = height_;
            length = length_;
            tileSize = tileSize_;

            buildingArray = new GameObject[width, height, length]; //represents the building made of 3d tile GameObjects

            //loop through the whole array and instantiate 3d tiles accordingly
            for(uint xIndex = 0; xIndex < width; xIndex++){

                for (uint yIndex = 0; yIndex < height; yIndex++){

                    //for each y index above 0, the tile that you procedurally generate the building with should be windows
                    //there is probably a better way to do this but whatever 
                    if (yIndex == 0)
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.buildingBase];
                    else
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.buildingWindow];


                    for (uint zIndex = 0; zIndex < length; zIndex++){
                        
                        buildingArray[xIndex, yIndex, zIndex] = tileDictionary[new Vector2 (xIndex, zIndex)] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos + (yIndex * tileSize), zPos + (zIndex * tileSize)), thisClass.gameObject.GetComponent<Procedural_City_Generation>().buildingWallQuaternion); //instantiate the 3d array with tile objects in the correct position (depending on its position in the 3d array and its size)
                    }
                }
            }
        }   
    }



    static void WFCToStreet(ProceduralCityGeneration thisClass) {

        int width = thisClass.mapWidth;
        int height = thisClass.mapHeight;

        cityMap = new int[height, width];

        Texture2D WFCTexture = WFC.Generate(thisClass.pattern, 3, width, height, false, true, false, 8, (int)1e9, 1);

        
        //loop through the WFC texture and generate a street if the pixel is the right color
        for (int xTextureIndex = 0; xTextureIndex < width; xTextureIndex += thisClass.textureResolution) {
            for (int yTextureIndex = 0; yTextureIndex < height; yTextureIndex+= thisClass.textureResolution) { 

                Debug.Log(xTextureIndex + " " + yTextureIndex);
                
                Color pixelColor = WFCTexture.GetPixel(xTextureIndex, yTextureIndex);

                if (pixelColor == thisClass.streetColor) 
                    generateStreet(xTextureIndex, yTextureIndex,    WFCTexture,    thisClass);

            }   
        }

        generateGrass(thisClass);
        generateHouses(thisClass);
    }

    //generate a street at the specified point in space
    static void generateStreet (int xIndex, int zIndex,   Texture2D WFCTexture,   ProceduralCityGeneration thisClass) {

        if (IsVerticalStreet(xIndex, zIndex,   WFCTexture,   thisClass) && IsHorizontalStreet(xIndex, zIndex,   WFCTexture,   thisClass)) {  // (if its a part of an intersection)
            Instantiate (thisClass.cityElements[(int)cityElementsNames.streetIntersection],   new Vector3 ((xIndex * thisClass.tileSize) / thisClass.textureResolution, thisClass.defaultYPos, (zIndex * thisClass.tileSize) / thisClass.textureResolution),   Quaternion.identity);
            cityMap [xIndex / thisClass.textureResolution, zIndex / thisClass.textureResolution] = (int)cityElementsNames.streetIntersection;
        }

        else if (IsVerticalStreet(xIndex, zIndex,   WFCTexture,   thisClass)) { // (if its a street thats going in the z dir)
            Instantiate (thisClass.cityElements[(int)cityElementsNames.streetZ] ,   new Vector3 (thisClass.streetZOffsetX + (xIndex * thisClass.tileSize) / thisClass.textureResolution, thisClass.defaultYPos, thisClass.streetZOffsetZ + (zIndex * thisClass.tileSize) / thisClass.textureResolution),   Quaternion.identity);
            cityMap [xIndex / thisClass.textureResolution, zIndex / thisClass.textureResolution] = (int)cityElementsNames.streetZ;
        }

        else if (IsHorizontalStreet(xIndex, zIndex,   WFCTexture,   thisClass)) { // (if its a street thats going in the x dir)
            Instantiate (thisClass.cityElements[(int)cityElementsNames.streetX],   new Vector3 (thisClass.streetXOffsetX + (xIndex * thisClass.tileSize) / thisClass.textureResolution, thisClass.defaultYPos, thisClass.streetXOffsetZ + (zIndex * thisClass.tileSize) / thisClass.textureResolution),   Quaternion.identity);
            cityMap [xIndex / thisClass.textureResolution, zIndex / thisClass.textureResolution] = (int)cityElementsNames.streetX; 
        }
    } 


    //Check if there are streets above or below the current street
    static bool IsVerticalStreet(int xIndex, int zIndex,   Texture2D WFCTexture,   ProceduralCityGeneration thisClass) {

        if (WFCTexture.GetPixel(xIndex, zIndex + thisClass.textureResolution) == thisClass.streetColor || WFCTexture.GetPixel(xIndex, zIndex - thisClass.textureResolution) == thisClass.streetColor)
            return true;
        return false;
    }

    //Check if there are streets to the sides of the current street
    static bool IsHorizontalStreet(int xIndex, int zIndex,   Texture2D WFCTexture,   ProceduralCityGeneration thisClass) {

        if (WFCTexture.GetPixel(xIndex + thisClass.textureResolution, zIndex) == thisClass.streetColor || WFCTexture.GetPixel(xIndex - thisClass.textureResolution, zIndex) == thisClass.streetColor)
            return true;
        return false;
    }

    static void generateGrass (ProceduralCityGeneration thisClass) {

        for (int xIndex = 0; xIndex < thisClass.mapWidth; xIndex++) {
            for (int zIndex = 0; zIndex < thisClass.mapHeight; zIndex++) {
                
                if (cityMap [xIndex, zIndex] == 0) {
                    Instantiate (thisClass.cityElements[(int)cityElementsNames.grass], new Vector3 (thisClass.grassOffsetX + (xIndex * thisClass.tileSize), thisClass.defaultYPos, thisClass.grassOffsetZ + (zIndex * thisClass.tileSize)), Quaternion.identity);
                    cityMap [xIndex, zIndex] = (int)cityElementsNames.grass;
                }
                    
            }
        }
    }


    static void generateHouses (ProceduralCityGeneration thisClass) {

        for (int xIndex = 0; xIndex < thisClass.mapWidth; xIndex++) {
            for (int zIndex = 0; zIndex < thisClass.mapHeight; zIndex++) {

                if (cityMap [xIndex, zIndex] == (int)cityElementsNames.grass) {
                    generateIfNearStreet (xIndex, zIndex,   cityMap,   thisClass);
                }
            }
        }
    }

    static void generateIfNearStreet (int xIndex, int zIndex,   int[ , ] cityMap,   ProceduralCityGeneration thisClass) {

        if (indexExists (xIndex + 1, zIndex, cityMap) &&
            (cityMap [xIndex + 1, zIndex] == (int)cityElementsNames.streetIntersection || 
             cityMap [xIndex + 1, zIndex] == (int)cityElementsNames.streetX || 
             cityMap [xIndex + 1, zIndex] == (int)cityElementsNames.streetZ)) { //street to the right
            
            Instantiate (thisClass.cityElements[(int)cityElementsNames.houseRight], new Vector3 (thisClass.houseRightOffsetX + (xIndex * thisClass.tileSize), thisClass.defaultYPos, thisClass.houseRightOffsetZ + (zIndex * thisClass.tileSize)), Quaternion.identity);
            cityMap [xIndex, zIndex] = (int)cityElementsNames.houseRight;
        }

        else if (indexExists (xIndex - 1, zIndex, cityMap) &&
            (cityMap [xIndex - 1, zIndex] == (int)cityElementsNames.streetIntersection || 
            cityMap [xIndex - 1, zIndex] == (int)cityElementsNames.streetX || 
            cityMap [xIndex - 1, zIndex] == (int)cityElementsNames.streetZ)) { //street to the left

            Instantiate (thisClass.cityElements[(int)cityElementsNames.houseLeft], new Vector3 (thisClass.houseLeftOffsetX + (xIndex * thisClass.tileSize), thisClass.defaultYPos, thisClass.houseLeftOffsetZ + (zIndex * thisClass.tileSize)), Quaternion.identity);
            cityMap [xIndex, zIndex] = (int)cityElementsNames.houseLeft;
        }

        else if (indexExists (xIndex, zIndex + 1, cityMap) &&
            (cityMap [xIndex, zIndex + 1] == (int)cityElementsNames.streetIntersection || 
            cityMap [xIndex, zIndex + 1] == (int)cityElementsNames.streetX || 
            cityMap [xIndex, zIndex + 1] == (int)cityElementsNames.streetZ)) { //street above

            Instantiate (thisClass.cityElements[(int)cityElementsNames.houseUp], new Vector3 (thisClass.houseUpOffsetX + (xIndex * thisClass.tileSize), thisClass.defaultYPos, thisClass.houseUpOffsetZ + (zIndex * thisClass.tileSize)), Quaternion.identity);
            cityMap [xIndex, zIndex] = (int)cityElementsNames.houseUp;
        }

        else if (indexExists (xIndex, zIndex - 1, cityMap) &&
            (cityMap [xIndex, zIndex - 1] == (int)cityElementsNames.streetIntersection || 
            cityMap [xIndex, zIndex - 1] == (int)cityElementsNames.streetX || 
            cityMap [xIndex, zIndex - 1] == (int)cityElementsNames.streetZ)) {//street below

            Instantiate (thisClass.cityElements[(int)cityElementsNames.houseDown], new Vector3 (thisClass.houseDownOffsetX + (xIndex * thisClass.tileSize), thisClass.defaultYPos, thisClass.houseDownOffsetZ + (zIndex * thisClass.tileSize)), Quaternion.identity);
            cityMap [xIndex, zIndex] = (int)cityElementsNames.houseDown;
        }
    }

    static bool indexExists (int xIndex, int zIndex, int [ , ] matrix) {

        if (xIndex >= 0 && zIndex >= 0 && xIndex < matrix.GetLength(1) && zIndex < matrix.GetLength(0))
            return true;
        return false;
    }

    void Awake(){
        WFCToStreet (this);
    }


}
