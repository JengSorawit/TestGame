using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    private int y;

    private MoveablePiece moveableComponent;
    public MoveablePiece MoveableCompponent
    {
        get { return moveableComponent; }
    }
    private ColorPiece colorComponent;
    public ColorPiece ColorComponent
    {
        get { return colorComponent; }
    }
    private Grid grid;
    public Grid GridRef
    {
        get { return grid; }
    }
    public int X
    {
        get { return x; }
        set
        {
            if (IsMoveable())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get { return y; }
        set
        {
            if (IsMoveable())
            {
                y = value;
            }
        }
    }
    private Grid.PieceType pieceType;

    public Grid.PieceType Type
    {
        get { return pieceType; }
    }
    private void OnMouseDown()
    {
        if (grid != null)
        {
           if (Type == Grid.PieceType.BOMB)
            {
                grid.ClearRowAndColumn(X, Y);
            }
            else if (Type == Grid.PieceType.DISCO)
            {
                grid.ClearColor(ColorComponent.Color);
            }
            else
            {
                grid.ClearConnectedPieces(this);
            }
        }
    }



    public void Init(int someX, int someY, Grid someGrid, Grid.PieceType someType)
    {
        x = someX;
        y = someY;
        grid = someGrid;
        pieceType = someType;
    }


    private void Awake()
    {
        moveableComponent = GetComponent<MoveablePiece>();
        colorComponent = GetComponent<ColorPiece>();
    }
    public bool IsMoveable()
    {
        return moveableComponent != null;
    }
    public bool IsColored()
    {
        return colorComponent != null;
    }
}
