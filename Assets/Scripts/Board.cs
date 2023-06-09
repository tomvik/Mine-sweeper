using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileHidden;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileFlagged;
    public Tile tileExploded;
    public Tile tileNumber1;
    public Tile tileNumber2;
    public Tile tileNumber3;
    public Tile tileNumber4;
    public Tile tileNumber5;
    public Tile tileNumber6;
    public Tile tileNumber7;
    public Tile tileNumber8;


    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell[,] state, string textHeader)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        string boardStr = "";

        for (int col = 0; col < width; ++col)
        {
            for (int row = 0; row < height; ++row)
            {
                Cell cell = state[col, row];
                tilemap.SetTile(cell.position, GetTile(cell));
                
                if (cell.status == Cell.Status.Hidden) {
                    boardStr += "-2";
                } else if (cell.status == Cell.Status.Flagged) {
                    boardStr += "-3";
                } else {
                    boardStr += cell.number.ToString();
                }
                boardStr += ",";
            }
        }
        WriteToFile(boardStr, textHeader);
        Debug.Log(boardStr);
    }

    private void WriteToFile(string text, string textHeader)
    {
        string path = "AISolver/data/board.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(textHeader);
        writer.WriteLine(text);
        writer.Close();
    }

    private Tile GetTile(Cell cell)
    {
        switch (cell.status)
        {
            case Cell.Status.Exploded:
                return tileExploded;
            case Cell.Status.Flagged:
                return tileFlagged;
            case Cell.Status.Hidden:
                return tileHidden;
            case Cell.Status.Revealed:
                return GetRevealedTile(cell);
            default:
                return null;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty:
                return tileEmpty;
            case Cell.Type.Mine:
                return tileMine;
            case Cell.Type.Number:
                return GetNumberTile(cell);
            default:
                return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1:
                return tileNumber1;
            case 2:
                return tileNumber2;
            case 3:
                return tileNumber3;
            case 4:
                return tileNumber4;
            case 5:
                return tileNumber5;
            case 6:
                return tileNumber6;
            case 7:
                return tileNumber7;
            case 8:
                return tileNumber8;
            default:
                return null;
        }
    }

    public bool CoordIsWithinBoard(int col, int row)
    {
        return tilemap.HasTile(new Vector3Int(col, row, 0));
    }
}
