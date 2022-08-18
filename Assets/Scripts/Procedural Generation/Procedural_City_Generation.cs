// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

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
    public Quaternion leftTurnStreetBaseQuaternion; //the start of the turn
    public Quaternion leftTurnStreetTurnQuaternion; //the actua
    public Quaternion leftTurnStreetExitQuaternion; //

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

    public const int roadWidth = 3; //width of the road


    //a building represents a 3 dimensional array of building objects each of which are cubical
    struct building{

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
                        
                        buildingArray[xIndex, yIndex, zIndex] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos + (yIndex * tileSize), zPos + (zIndex * tileSize)), thisClass.gameObject.GetComponent<Procedural_City_Generation>().buildingWallQuaternion); //instantiate the 3d array with tile objects in the correct position (depending on its position in the 3d array and its size)
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


        public straightStreet (float xPos_, float yPos_, float zPos_,    uint width_, uint length_,    float tileSize_,    Procedural_City_Generation thisClass) : this(){

            //set the variables of the building's xyz, width, length, and height (and also tilesize) based on what was passed
            xPos = xPos_;
            yPos = yPos_;
            zPos = zPos_;
            width = width_;
            length = length_;
            tileSize = tileSize_;

            streetArray = new GameObject[width, length]; //represents the street made of (mostly) 2d tile GameObjects


            //loop through the whole array and instantiate accordingly
            for (uint xIndex = 0; xIndex < width; xIndex++){

                for (uint zIndex = 0; zIndex < length; zIndex++){

                    //check if this tile is on the far left or the far right of the street. If so, use the appropriate model when constructing this
                    if (xIndex == 0) {

                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.streetside];
                        usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().leftStreetQuaternion;
                    } 

                    else if (xIndex == (width - 1)){
                        
                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.streetside];
                        usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().rightStreetQuaternion;
                    }

                    else {

                        usedTile = thisClass.gameObject.GetComponent<Procedural_City_Generation>().cityElements[(int)cityElementsNames.street_dotted];
                        usedQuaternion = thisClass.gameObject.GetComponent<Procedural_City_Generation>().defaultStreetQuaternion;
                    }

                    //INSTANTIATE
                    streetArray[xIndex, zIndex] = Instantiate (usedTile, new Vector3 (xPos + (xIndex * tileSize), yPos, zPos + (zIndex * tileSize)), usedQuaternion); //instantiate the 2d array with tile objects in the correct position (depending on its position in the 2d array and its size))
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


        public turnStreet(float xPos, float yPos, float zPos,    uint width,    uint tileSize_,    Procedural_City_Generation thisClass) : this() {

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



    void Awake(){
            
            building build = new building(15f, 2f, 10f,   10, 10, 10,   2,   this);
            building build1 = new building(-15f, 2f, 10f,   10, 10, 10,   2,   this);
            straightStreet street1 = new straightStreet(0f, 2f, -10f,   3, 10,   2,   this);
    }


}
