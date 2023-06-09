using UnityEngine;

public class Player : MonoBehaviour
{
    private float[,] probabilities; 
    private int undiscoveredMines;
    public void Solve(Cell[,] state, int width, int height, int mines)
    {
        probabilities = new float[width, height];
        undiscoveredMines = mines;

        ComputeProbabilites(state, width, height);
        for (int col = 0; col < width; ++col)
        {
            string rowString = "";
            for (int row = 0; row < height; ++row)
            {
                switch (state[col,row].status)
                {
                    case Cell.Status.Hidden:
                        rowString += "H";
                        break;
                    case Cell.Status.Revealed:
                        switch (state[col,row].type)
                        {
                            case Cell.Type.Empty:
                                rowString += " ";
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                Debug.Log(state.ToString());
            }
        }
    }

    private void ComputeProbabilites(Cell[,] state, int width, int height)
    {
        for (int col = 0; col < width; ++col)
        {
            for (int row = 0; row < height; ++row)
            {
                Debug.Log("something");
            }
        }

    }
}
