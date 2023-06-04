using UnityEngine;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;

    private Board board;
    private Cell[,] state;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();   
    }

    private void NewGame()
    {
        state = new Cell[width, height];

        GenerateBlankCells();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        Camera.main.orthographicSize = (float)(Mathf.Max(width, height)) * 0.6f;

        board.Draw(state);
    }

    private void GenerateBlankCells()
    {
        for (int col = 0; col < width; ++col)
        {
            for (int row = 0; row < height; ++row)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(col, row, 0);
                cell.type = Cell.Type.Empty;
                state[col, row] = cell;
            }
        }
    }
}
