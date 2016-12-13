using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGeneration;

public class MazeObject : MonoBehaviour {
    
    Maze maze;

    const int mazeWidth = 30;
    const int mazeHeight = 20;

    // Use this for initialization
    void Start () {
        maze = new Maze(mazeWidth, mazeHeight);

        GetComponent<Renderer>().material.mainTexture = RenderMaze(maze);
        InstantiateMaze(maze).transform.parent = transform;
    }

    static Texture2D RenderMaze(Maze maze)
    {
        Texture2D texture = new Texture2D(3 * maze.width, 3 * maze.height);

        CellState[,] cs = new CellState[3, 3]
        {
                { CellState.Top | CellState.Left, CellState.Top, CellState.Top | CellState.Right },
                { CellState.Left, CellState.Visited, CellState.Right },
                { CellState.Bottom | CellState.Left, CellState.Bottom, CellState.Bottom | CellState.Right }
        };

        for (int y = 0; y < maze.height; y++)
        {
            for (int x = 0; x < maze.width; x++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        texture.SetPixel(x * 3 + j, y * 3 + i, maze[x, y].HasFlags(cs[i, j]) ? Color.white : Color.black);
                    }
                }
            }
        }
        texture.Apply();
        return texture;
    }

    static GameObject InstantiateMaze(Maze maze)
    {
        Vector3 scale = new Vector3(10, 10, 10);
        Vector3 move = new Vector3(-maze.width / 2.0f, 0, -maze.height / 2.0f);

        GameObject anchor = new GameObject();

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.SetParent(anchor.transform);
        floor.transform.localScale = new Vector3(maze.width, 1, maze.height);

        for (int y = 0; y < maze.height; y++)
        {
            for (int x = 0; x < maze.width; x++)
            {
                if (maze[x, y].HasFlags(CellState.Top))
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    go.transform.SetParent(anchor.transform);
                    go.transform.position = Vector3.Scale(scale, move + new Vector3(x, 0.5f, y - 0.5f));
                    go.transform.Rotate(-90, 0, 0);
                }
                if (maze[x, y].HasFlags(CellState.Left))
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    go.transform.SetParent(anchor.transform);
                    go.transform.position = Vector3.Scale(scale, move + new Vector3(x - 0.5f, 0.5f, y));
                    go.transform.Rotate(0, 0, -90);
                }
                if (maze[x, y].HasFlags(CellState.Right))
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    go.transform.SetParent(anchor.transform);
                    go.transform.position = Vector3.Scale(scale, move + new Vector3(x + 0.5f, 0.5f, y));
                    go.transform.Rotate(0, 0, 90);
                }
                if (maze[x, y].HasFlags(CellState.Bottom))
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    go.transform.SetParent(anchor.transform);
                    go.transform.position = Vector3.Scale(scale, move + new Vector3(x, 0.5f, y + 0.5f));
                    go.transform.Rotate(90, 0, 0);
                }
            }
        }
        return anchor;
    }
	
    // Update is called once per frame
    void Update () {
    	
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 100, 100),
            GetComponent<Renderer>().material.mainTexture,
            ScaleMode.ScaleToFit, true, 1.0f * mazeWidth/mazeHeight);
    }
}
