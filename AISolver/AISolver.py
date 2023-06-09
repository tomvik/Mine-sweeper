from time import sleep
import keyboard
import numpy as np

width = 0
height = 0
totalBombs = 0
hiddenTilesLeft = 1000

BOMB = -1
HIDDEN = -2
FLAGGED = -3


def ReadBoard():
    global width
    global height
    global totalBombs
    global hiddenTilesLeft

    file = open("data/board.txt", "r")
    header = file.readline()
    body = file.readline()

    header = [int(element) for element in header.split(",")]
    body = [int(element) for element in body.split(",")[:-1]]

    width = header[0]
    height = header[1]
    totalBombs = header[2]
    hiddenTilesLeft = header[3]

    body2d = []
    for i in range(height):
        body2d.append(body[(width*i):(width*(i+1))])

    return np.rot90(np.array(body2d))

def IsNumberTile(tile):
    return tile > 0

def IsValidPos(col, row):
    return col >= 0 and col < width and row >= 0 and row < height

def GetOnlyHiddenPos(board, col, row):
    global HIDDEN

    for deltaX in range(-1,2):
        for deltaY in range(-1,2):
            x = col + deltaX
            y = row + deltaY
            if IsValidPos(x, y) and board[x, y] == HIDDEN:
                return x, y

def WriteProb(probabilities, col, row, prob):
    if probabilities[col, row] < prob and probabilities[col, row] != 0:
        probabilities[col, row] = prob

def GetSameNumberOfHiddenProb(board, col, row, probabilities):
    global HIDDEN

    adjacentHiddenTilesCount = 0
    adjacentFlaggedTilesCount = 0
    for deltaX in range(-1,2):
        for deltaY in range(-1,2):
            x = col + deltaX
            y = row + deltaY
            if IsValidPos(x, y):
                if board[x, y] == HIDDEN:
                    adjacentHiddenTilesCount += 1
                if board[x, y] == FLAGGED:
                    adjacentFlaggedTilesCount += 1

    if board[col, row] == adjacentFlaggedTilesCount:
        for deltaX in range(-1,2):
            for deltaY in range(-1,2):
                x = col + deltaX
                y = row + deltaY
                if IsValidPos(x, y):
                    if board[x, y] == HIDDEN:
                        probabilities[x, y] = 0
        return

    if board[col, row] - adjacentFlaggedTilesCount == adjacentHiddenTilesCount:
        hiddenX, hiddenY = GetOnlyHiddenPos(board, col, row)
        WriteProb(probabilities, hiddenX, hiddenY, 100)


def GetProbabilites(board):
    global width
    global height

    probabilities = np.ones((width, height))

    for col in range(width):
        for row in range(height):
            if IsNumberTile(board[col, row]):
                GetSameNumberOfHiddenProb(board, col, row, probabilities)
    return probabilities

def GetBestTileToFlag(probabilities):
    global width
    global height

    bestProb = 99
    bestX = -1
    bestY = -1
    for col in range(width):
        for row in range(height):
            if probabilities[col, row] > bestProb:
                bestProb = probabilities[col, row]
                bestX = col
                bestY = row

    return bestX, bestY

def GetBestTileToClick(probabilities):
    global width
    global height

    bestProb = 1
    bestX = -1
    bestY = -1
    for col in range(width):
        for row in range(height):
            if probabilities[col, row] < bestProb:
                bestProb = probabilities[col, row]
                bestX = col
                bestY = row

    return bestX, bestY

def ClickTile(x, y):
    print(x, y)
    pass

def SolveMineSweeper():
    while (hiddenTilesLeft != totalBombs and not (keyboard.read_key() == "q")):
        sleep(0.1)
        try:
            board = ReadBoard()
            # print(board)
        except:
            print("no file")
            continue

        print(board)

        probabilities = GetProbabilites(board)
        print(probabilities)

        x, y = GetBestTileToFlag(probabilities)
        print("Best tile to flag:", x, y)

        x, y = GetBestTileToClick(probabilities)
        print("Best tile to click:", x, y)

if __name__ == "__main__":
    SolveMineSweeper()