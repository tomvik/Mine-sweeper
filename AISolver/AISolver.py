from time import sleep
import keyboard
import numpy as np
import pyautogui

width = 16
height = 16
totalBombs = 0
hiddenTilesLeft = 1000

min_pixel_x = 0
min_pixel_y = 0
max_pixel_x = 0
max_pixel_y = 0
block_delta = 0

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

def ClickTile(x, y, left):
    global min_pixel_x
    global min_pixel_y
    global max_pixel_x
    global max_pixel_y
    global block_delta

    if (x == -1):
        return

    if (left):
        pyautogui.leftClick(min_pixel_x + (x * block_delta) + (.2 * block_delta), min_pixel_y + (y * block_delta) + (.2 * block_delta))
    else:
        pyautogui.rightClick(min_pixel_x + (x * block_delta)+ (.2 * block_delta), min_pixel_y + (y * block_delta)+ (.2 * block_delta))
    # print(x, y)

def GetRandomTileToClick(board):
    for col in range(width):
        for row in range(height):
            if board[col, row] == HIDDEN:
                return col, row
    return -1, -1


def SolveMineSweeper():
    while (hiddenTilesLeft != totalBombs): # and not (keyboard.read_key() == "q")):
        sleep(0.3)
        try:
            board = ReadBoard()
            # print(board)
        except:
            # print("no file")
            continue

        # print(board)

        probabilities = GetProbabilites(board)
        # print(probabilities)

        # file = open("data/moves.txt", "w")

        flag_x, flag_y = GetBestTileToFlag(probabilities)
        ClickTile(flag_y, flag_x, False)
        # print("Best tile to flag:", flag_x, flag_y)
        # file.write("{},{}\n".format(x, y))

        click_x, click_y = GetBestTileToClick(probabilities)
        ClickTile(click_y, click_x, True)
        # print("Best tile to click:", click_x, click_y)
        # file.write("{},{}".format(x, y))

        if (flag_x == -1 and click_y == -1):
            click_x, click_y = GetRandomTileToClick(board)
            ClickTile(click_y, click_x, True)
            # print("Random tile to click:", click_x, click_y)

        # file.close()

def GetBoardCoordinates():
    global min_pixel_x
    global min_pixel_y
    global max_pixel_x
    global max_pixel_y
    global block_delta
    global width

    print("Locate the mouse in the first block and click x when ready")
    while True:
        # Check if x key is pressed
        sleep(0.1)

        if keyboard.is_pressed('x'):
            print('x key pressed...')
            break

    x, y = pyautogui.position()
    print(f'spam at position: {x}, {y}')
    min_pixel_x = x
    min_pixel_y = y

    sleep(1)

    
    print("Locate the mouse in the lasy block and click x when ready")
    while True:
        # Check if x key is pressed
        sleep(0.1)

        if keyboard.is_pressed('x'):
            print('x key pressed...')
            break

    x, y = pyautogui.position()
    print(f'spam at position: {x}, {y}')
    max_pixel_x = x
    max_pixel_y = y

    block_delta = ((max_pixel_x - min_pixel_x) + (max_pixel_y - min_pixel_y)) / 2 / (width-1)

if __name__ == "__main__":
    GetBoardCoordinates()
    SolveMineSweeper()