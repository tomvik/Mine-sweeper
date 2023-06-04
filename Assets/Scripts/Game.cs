using UnityEngine;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int numberOfMines = 16;

    private Board board;
    private int[,] adjacentDeltas;
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
        GenerateAdjacentDeltas();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        Camera.main.orthographicSize = (float)(Mathf.Max(width, height)) * 0.6f;

        GenerateBlankCells();

        // We need to draw it first to utilize the tilemap method HasTile.
        board.Draw(state);
        GenerateMineCells();
        GenerateNumberCells();

        board.Draw(state);
    }

    private void GenerateAdjacentDeltas()
    {
        adjacentDeltas = new int [8, 2];
        // upper left
        adjacentDeltas[0, 0] = -1;
        adjacentDeltas[0, 1] = 1;
        // upper mid
        adjacentDeltas[1, 0] = 0;
        adjacentDeltas[1, 1] = 1;
        // upper right
        adjacentDeltas[2, 0] = 1;
        adjacentDeltas[2, 1] = 1;
        // right mid
        adjacentDeltas[3, 0] = 1;
        adjacentDeltas[3, 1] = 0;
        // right down
        adjacentDeltas[4, 0] = 1;
        adjacentDeltas[4, 1] = -1;
        // down mid
        adjacentDeltas[5, 0] = 0;
        adjacentDeltas[5, 1] = -1;
        // down left
        adjacentDeltas[6, 0] = -1;
        adjacentDeltas[6, 1] = -1;
        // left mid
        adjacentDeltas[7, 0] = -1;
        adjacentDeltas[7, 1] = 0;

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

    private void GenerateMineCells()
    {
        numberOfMines = Mathf.Min(numberOfMines, height * width - 1);
        for (int index = 0; index < numberOfMines; ++index)
        {
            int linearPosition = Random.Range(0, height * width);
            int col = linearPosition / width;
            int row = linearPosition % height;
            Debug.Log("LinearPosition: " + linearPosition.ToString());
            Debug.Log("Col: " + col.ToString() + " Row: " + row.ToString());

            if (state[col, row].type != Cell.Type.Empty)
            {
                --index;
                continue;
            }
            state[col, row].type = Cell.Type.Mine;
        }
    }

    private void GenerateNumberCells()
    {
        for (int col = 0; col < width; ++col)
        {
            for (int row = 0; row < height; ++row)
            {
                if (state[col, row].type == Cell.Type.Empty)
                {
                    int adjacentMines = GetAdjacentMines(col, row);
                    if (adjacentMines > 0)
                    {
                        state[col, row].type = Cell.Type.Number;
                        state[col, row].number = adjacentMines;
                    }
                }
            }
        }
    }

    private int GetAdjacentMines(int col, int row)
    {
        int minesCount = 0;

        for (int index = 0; index < 8; ++index)
        {
            int x = col + adjacentDeltas[index, 0];
            int y = row + adjacentDeltas[index, 1];
            Debug.Log("Checking for within board");
            Debug.Log("Col: " + x.ToString() + " Row: " + y.ToString());

            if (board.CoordIsWithinBoard(x, y))
            {
                Debug.Log("Within board");
                Debug.Log("Col: " + x.ToString() + " Row: " + y.ToString());
                if (state[x, y].type == Cell.Type.Mine)
                {
                    ++minesCount;
                }
            }
        }

        return minesCount;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click");
            FlagCell();
            board.Draw(state);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click");
            RevealCell();
            board.Draw(state);
        }
    }

    private void FlagCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);

        if (board.CoordIsWithinBoard(cellPosition.x, cellPosition.y))
        {
            switch (state[cellPosition.x, cellPosition.y].status)
            {
                case Cell.Status.Flagged:
                    state[cellPosition.x, cellPosition.y].status = Cell.Status.Hidden;
                    break;
                case Cell.Status.Hidden:
                    state[cellPosition.x, cellPosition.y].status = Cell.Status.Flagged;
                    break;
                case Cell.Status.Exploded:
                case Cell.Status.Revealed:
                    break;
                default:
                    break;
            }
        }
    }

    private void RevealCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);

        RevealCellWithCoordinates(cellPosition.x, cellPosition.y);
        CheckIfExploded(cellPosition.x, cellPosition.y);
    }

    private void RevealCellWithCoordinates(int col, int row)
    {
        if (board.CoordIsWithinBoard(col, row))
        {
            switch (state[col, row].status)
            {
                case Cell.Status.Hidden:
                    state[col, row].status = Cell.Status.Revealed;
                    if (state[col, row].type == Cell.Type.Empty)
                    {
                        
                        RevealCellWithCoordinates(col-1, row);
                        RevealCellWithCoordinates(col, row+1);
                        RevealCellWithCoordinates(col+1, row);
                        RevealCellWithCoordinates(col, row-1);
                    }
                    break;
                case Cell.Status.Flagged:
                case Cell.Status.Exploded:
                case Cell.Status.Revealed:
                    break;
                default:
                    break;
            }
        }
    }

    private void CheckIfExploded(int col, int row)
    {
        if (board.CoordIsWithinBoard(col, row))
        {
            if (state[col, row].type == Cell.Type.Mine)
            {
                state[col, row].status = Cell.Status.Exploded;
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        RevealCellWithCoordinates(x, y);
                    }
                }
            }
        }
    }
}
