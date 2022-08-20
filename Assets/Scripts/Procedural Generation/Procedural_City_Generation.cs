using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//
//NOTES:
//
//Ok so this algorithm is decently awkward
//There is another option that I think is reasonable if you guys want to do it
//We would generate a bunch of vertical and horizontal straight streets and then just take where they intersect and create an intersection
//We could lock the generation so that each of the roads have to be x width apart, which I think could work very well tbh
//What do you guys think?
//The current one makes more natural roads while the other one would be a good bit easier to implement (not like an incredible difference in dificulty but enough to mention)
//
//We could also use the L-system (https://www.youtube.com/watch?v=75-yKBDO9yo&t=9s)
//
//Other than these algorithms, we could use a noise based on like perlin noise but I don't think that would work very well for our purposes and I think its better to just stick with this
//




public class Procedural_City_Generation : MonoBehaviour
{

    //
    //
    //CONVERT ALL OF THIS TO A GRID BASED FORMAT 
    //
    //


    //array of all of the cityElements (models) that can be set in the editor
    public GameObject[] cityElements = new GameObject[19];


    //set quaternions that will be useful when instantiating specific gameObjects
    public Quaternion defaultQuaternion; //default 0, 0, 0 quaternion
    public Quaternion buildingWallQuaternion; //these for some reason need their own rotation so here it is
    public Quaternion defaultStreetQuaternion; //default for regular street objects
    public Quaternion leftStreetQuaternion; //streets that are on the far left (we only have one model for street side)
    public Quaternion rightStreetQuaternion; //streets that are on the far right (we only have one model for street side)
    public Quaternion defaultStreetQuaternionSideways; //default for regular street objects (Sideways)
    public Quaternion leftStreetQuaternionSideways; //streets that are on the far left (we only have one model for street side) (Sideways)
    public Quaternion rightStreetQuaternionSideways; //streets that are on the far right (we only have one model for street side) (Sideways)
    /*public Quaternion leftTurnStreetBaseQuaternion; //the start of the turn
    public Quaternion leftTurnStreetTurnQuaternion; //the
    public Quaternion leftTurnStreetExitQuaternion; */


    //stats (cannot be tweaked in the editor must be tweaked in script)
    public static uint roadWidth {get;} = 3; //width of the road in tiles 
    public static float tileSize {get;} = 2; //the size of tiles in the gameWorld
    public static int maxPropogation {get;} = 50; //the maximum number of times that one thread can propogate (determines the size of the map)
    public static int propogationLock {get;} = 30; //after an intersection is created, this is the number of propogations before the algorithm is able to roll for a new intersection
    public static int fourWayIntersectionGenerationProbability {get;} = 30; //this is the number that determines the probability of a 4 way intersection every tiling (higher number lower probability)
    public static int defaultYPos {get;} = 3;
    static Dictionary <Vector2, GameObject> tileDictionary; //an map representing the positions of everything along the x and z axes

    static System.Random random = new System.Random(); //instantiate a random class (idk why c# does this either)
        

    //the only reason this exists is to act as a reference to which index the city elements are in
    public enum cityElementsNames { 

        bricks = 0,
        bricks_window = 1,
        building_base = 2,
        building_wall = 3,
        ceiling_corner = 4,
        ceiling_oneside = 5,
        crosswalk = 6,
        grass_tile = 7,
        sidewalk_bothsides = 8,
        sidewalk_corner = 9,
        sidewalk_oneside = 10,
        street = 11,
        street_corner = 12,
        street_dotted = 13,
        street_dotted_side = 14,
        street_dotted_side_yellow = 15,
        streetside = 16,
        streetside_yellow = 17,
        tree = 18
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

        float tileSize; //how large the 3d model is in world space 

        GameObject usedTile; //a reference to the tile prefab that is currently being used to generate the building

        Quaternion usedQuaternion; //a reference to the rotation quaternion that is currently being used to generate the thing

        GameObject[ , , ] buildingArray; //WTHHHHHHHHH IS THIS SYNTAX I HATE IT HERE but this represents the full building as a 3d array of gameObjects


        public building (float xPos_, float yPos_, float zPos_,    uint width_, uint height_, uint length_,    float tileSize_,    Procedural_City_Generation thisClass) : this(){
            
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
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.building_base];
                    else
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.building_wall];


                    for (uint zIndex = 0; zIndex < length; zIndex++){
                        
                        buildingArray[xIndex, yIndex, zIndex] = tileDictionary[new Vector2 (xIndex, zIndex)] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos + (yIndex * tileSize), zPos + (zIndex * tileSize)), thisClass.gameObject.GetComponent<Procedural_City_Generation>().buildingWallQuaternion); //instantiate the 3d array with tile objects in the correct position (depending on its position in the 3d array and its size)
                    }
                }
            }
        }   
    }


    //this represents a straight strip of street without any turns 
    struct straightStreet {

        //xyz coord
        float xPos;
        float yPos;
        float zPos;

        //length and width just cause they are useful things to have as a reference
        uint width;
        uint length;

        float tileSize; //how large the 3d model is in world space 

        GameObject usedTile; //a reference to the tile prefab that is currently being used to generate the street

        Quaternion usedQuaternion; //a reference to the rotation quaternion that is currently being used to generate the thing

        GameObject [ , ] streetArray; //represents a street in a 2d array made out of (mostly) 2d models


        public straightStreet (float xPos_, float yPos_, float zPos_,    uint width_, uint length_,    int xDir, int zDir,   float tileSize_,    Procedural_City_Generation thisClass) : this(){

            //set the variables of the building's xyz, width, length, and height (and also tilesize) based on what was passed
            xPos = xPos_;
            yPos = yPos_;
            zPos = zPos_;
            width = width_;
            length = length_;
            tileSize = tileSize_;

            streetArray = new GameObject[width, length + 1]; //represents the street made of (mostly) 2d tile GameObjects

        
            //loop through the whole array and instantiate accordingly
            for (uint xIndex = 0; xIndex < width; xIndex++){

                //zIndex = 1 is a case specific thing for length = 1 (fix later)
                for (uint zIndex = 0; zIndex < length; zIndex++){

                    //check if this tile is on the far left or the far right of the street. If so, use the appropriate model when constructing this
                    if (xIndex == 0) {
                        
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.streetside];
                        
                        if (zDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().leftStreetQuaternion;
                        else if (xDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().leftStreetQuaternionSideways;
                    } 

                    else if (xIndex == (width - 1)){
                        
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.streetside];
                        
                        if (zDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().rightStreetQuaternion;
                        else if (xDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().rightStreetQuaternionSideways;
                    }

                    else {

                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.street_dotted];
                        
                        if (zDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().defaultStreetQuaternion;
                        else if (xDir != 0)
                            usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().defaultStreetQuaternionSideways;
                    }

                    
                    //INSTANTIATE
                    //streetArray[xIndex, zIndex] = tileDictionary[new Vector2 (xIndex, zIndex)] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos, zPos + (zIndex * tileSize)), usedQuaternion); //instantiate the 2d array with tile objects in the correct position (depending on its position in the 2d array and its size))
                    if (xDir != 0)
                        streetArray[xIndex, zIndex] = tileDictionary[new Vector2 (xIndex, zIndex)] = Instantiate (usedTile, new Vector3 (xPos * tileSize, yPos, zPos + (xIndex * tileSize)), usedQuaternion); //instantiate the 2d array with tile objects in the correct position (depending on its position in the 2d array and its size))
                    else if (zDir != 0)
                        streetArray[xIndex, zIndex] = tileDictionary[new Vector2 (xIndex, zIndex)] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos, zPos * tileSize), usedQuaternion); //instantiate the 2d array with tile objects in the correct position (depending on its position in the 2d array and its size))
                }
            }
        }
    }

    
    struct turnStreet {

        //xyz coord
        float xPos;
        float yPos;
        float zPos;

        float tileSize; //how large the 3d model is in world space 
        
        GameObject usedTile; //a reference to the tile prefab that is currently being used to generate the street


        public turnStreet(float xPos_, float yPos_, float zPos_,    uint width,    uint tileSize_,    Procedural_City_Generation thisClass) : this() {

            xPos = xPos_;
            yPos = yPos_;
            zPos = zPos_;

            usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.building_base];


            for (int xIndex = 0; xIndex < width; xIndex++) {
                for(int zIndex = 0; zIndex < width; zIndex++) {

                    Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos, zPos + (zIndex * tileSize)), Quaternion.identity); //instantiate the 2d array with tile objects in the correct position (depending on its position in the 2d array and its size))
                }
            }

            
        }
    }


    struct leftThreeWayIntersection{

    }



    struct rightThreeWayIntersection{

    }



    struct fourWayIntersection {

    }


    //for this point on the grid, is there a road tile (check the tile dictionary)
    public static bool streetExists (int xPos, int zPos) {
        
        //if the key actually exists (lul) and there is a gameObject there, then there will probably be a street there (at least for our purposes and algorithm)
        return (tileDictionary.ContainsKey(new Vector2 (xPos, zPos)) && tileDictionary [new Vector2 (xPos, zPos)] != null);
    }


    //propogate tiles and split off into different branches until a specified limit or the branch hits an already built road
    //initially propogate in the z = -1 and z = 1 directions
    //Once an intersection is created, make sure that it can't create another intersection until int propogationLock
    //Use recursion (WAIT HOLD UP ur telling me that AP CS was ACTUALLY useful... im not buying it)
    //if you hit an already built road and it is a normal road, tranform it into a turn
    //if you hit an already built road and it is a turn, transform it into a 3 way intersection and so on
    public static void generateStreets(Procedural_City_Generation thisClass) {

        branchStreet (0, 0,   0, -1,   propogationLock,   0,   thisClass);
        branchStreet (0, 1,   0, 1,   propogationLock,   0,   thisClass);
    }

    //rotates the Quaternion that is in use so that the streets are facing the correct direction
    public static void rotateStreetQuaternion(int xDir, int zDir, Procedural_City_Generation thisClass) {

        thisClass.defaultStreetQuaternion = Quaternion.Euler(-90, (Math.Abs(xDir) * 90), 0);
        thisClass.leftStreetQuaternion = Quaternion.Euler(-90, (Math.Abs(xDir) * 90), 180);
        thisClass.rightStreetQuaternion = Quaternion.Euler(-90, (Math.Abs(xDir) * 90), 0);
    } 


    //recursive method that is called whenever a new branch
    //xDir and zDir can only be set to -1, 0, and 1 to denote how to propogate from its local starting point might change this to a vector later
    public static void branchStreet(int xPos_, int zPos_,   int xDir, int zDir,   int lockCountDown_,   int propogationCount_, Procedural_City_Generation thisClass) {

        bool isLocked = false;
        int lockCountDown = lockCountDown_; //is this necesary?
        int propogationCount = propogationCount_;

        int xPos = xPos_;
        int zPos = zPos_;


        //lock the intersection generation
        if (lockCountDown <= 0) 
            isLocked = false;
        else
            isLocked = true;
        

        //end branch if you run into another road
        if (streetExists(xPos, zPos)) {

            //Debug.Log("This Street Already Exists Stupid");
            //create a turn or whatever at that position 
            //SOMETHING SOMETHING CODE TODO (Just make to make it look nice)

            //end this branch
            return;
        }

        //make sure the algorithm actually ends lmao
        else if (propogationCount >= maxPropogation) {

            //Debug.Log("Reached Max Propogation Count");
            return;
        }


        //
        //ACTUALLY DOING SOMETHING
        //

        //create a four way intersection
        else if (random.Next(0, fourWayIntersectionGenerationProbability) == 0 && !isLocked) {
           
            //Debug.Log("Four Way Intersection Street Generation");

            //make this line into a new 4 way intersection (3 x 3 square instead of a 3 x 1 rect)
            new straightStreet (xPos, defaultYPos, zPos,   roadWidth, (uint) 1,   xDir, zDir,   tileSize,   thisClass);

            branchStreet (xPos + 1/*(int)Math.Round(((double)roadWidth)/2)*/, zPos, 1, 0, lockCountDown, ++propogationCount, thisClass);
            branchStreet (xPos - 1/*(int)Math.Round(((double)roadWidth)/2)*/, zPos, -1, 0, lockCountDown, ++propogationCount, thisClass);
            branchStreet (xPos, zPos + 1/*(int)Math.Round(((double)roadWidth)/2)*/, 0, 1, lockCountDown, ++propogationCount, thisClass);
            branchStreet (xPos, zPos - 1/*(int)Math.Round(((double)roadWidth)/2)*/, 0, -1, lockCountDown, ++propogationCount, thisClass);
        }

        //continue straight
        else {

            //Debug.Log("Continuing Straight Street Generation");

            //create the tile object at the correct position
            new straightStreet (xPos, defaultYPos, zPos,   roadWidth, (uint) 1,   xDir, zDir,   tileSize,   thisClass);

            branchStreet (xPos + xDir, zPos + zDir, xDir, zDir, --lockCountDown, ++propogationCount, thisClass);
        }

    }


    //call this either after every street is already made to just go through all of the streets and place sidewalk tiles on their sides
    public static void generateSideWalk(){

    }


    //call this after after every street (and sidewalk) is already made to just go through all of the empty (untouched space) and place a building there
    public static void generateBuildings(){

    }



    void Awake(){
        
        tileDictionary = new Dictionary<Vector2, GameObject>();

        generateStreets (this);
    }


}
