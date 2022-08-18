// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Procedural_City_Generation : MonoBehaviour
{
    //array of all of the cityElements (models)
    public static GameObject[] cityElements = new GameObject[19];

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
    struct building{

        //length width and height just cause they are useful things to have as a reference
        int width;
        int height;
        int length;

        int tileSize; //how large the 3d is (just in case)

        GameObject usedTile; //a reference to the tile prefab that is currently being used to generate the building

        GameObject[ , , ] buildingArray; //WTHHHHHHHHH IS THIS SYNTAX I HATE IT HERE but this represents the full building as a 3d array of gameObjects


        public building (int width_, int height_, int length_, int tileSize_) : this(){
            
            //set the variables of the building's width, length, and height (and also tilesize) based on what was passed
            width = width_;
            height = height_;
            length = length_;
            tileSize = tileSize_;

            buildingArray = new GameObject[width, height, length]; //represents the building made of 3d tile GameObjects

            //loop through the whole array and instantiate 3d tiles accordingly
            for(int xIndex = 0; xIndex < width; xIndex++){

                for (int yIndex = 0; yIndex < height; yIndex++){

                    //for each y index above 0, the tile that you procedurally generate the building with should be windows
                    //there is probably a better way to do this but whatever 
                    if (xIndex == 0)
                        usedTile = cityElements[(int)cityElementsNames.building_wall];
                    else
                        usedTile = cityElements[(int)cityElementsNames.bricks_window];


                    for (int zIndex = 0; zIndex < length; zIndex++){
                        
                        buildingArray[xIndex, yIndex, zIndex] = Instantiate (usedTile, new Vector3 (xIndex * tileSize, yIndex * tileSize, zIndex * tileSize), Quaternion.identity); //instantiate the 3d array with tile objects in the correct position (depending on its position in the 3d array and its size)
                    }
                }
            }
        }   
    }



    



}
