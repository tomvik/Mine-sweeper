
def ReadBoard():
    pass

def Print2D(matrix):
    pass

def GetProbabilites(board):
    pass

def GetBestTile(board):
    pass

def ClickTile(board):
    pass

def SolveMineSweeper():
    board = ReadBoard()

    Print2D(board)

    probabilities = GetProbabilites(board)

    bestTile = GetBestTile(probabilities)

    ClickTile(bestTile)

if __name__ == "__main__":
    SolveMineSweeper()