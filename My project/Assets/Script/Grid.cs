using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public int xDimension;
    public int yDimension;
    public float fillTime;

    public enum PieceType
    {
        EMPTY, NORMAL, BUBBLE, BOMB, DISCO, COUNT
    }

    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    public GameObject backgroundPrefabs;

    public PiecePrefab[] piecePrefab;
    private Dictionary<PieceType, GameObject> piecePrefabDict;

    private GamePiece[,] pieces;

    private bool inverse = false;

    private void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefab.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefab[i].type))
            {
                piecePrefabDict.Add(piecePrefab[i].type, piecePrefab[i].prefab);
            }
        }
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                GameObject background = Instantiate(backgroundPrefabs, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDimension, yDimension];
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }

        StartCoroutine(Fill());
    }

    private void Update()
    {
    }

    public IEnumerator Fill()
    {
        while (FillStep())
        {
            inverse = !inverse;
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for (int y = yDimension - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDimension; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDimension - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];

                if (piece != null && piece.IsMoveable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow == null || pieceBelow.Type == PieceType.EMPTY)
                    {
                        if (pieceBelow != null)
                        {
                            Destroy(pieceBelow.gameObject);
                        }
                        piece.MoveableCompponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDimension)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece == null || diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove != null && pieceAbove.IsMoveable())
                                            {
                                                break;
                                            }
                                            else if (pieceAbove != null && !pieceAbove.IsMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove)
                                        {
                                            if (diagonalPiece != null)
                                            {
                                                Destroy(diagonalPiece.gameObject);
                                            }
                                            piece.MoveableCompponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // สำหรับการเติมชิ้นส่วนใหม่ที่ยอดกระดาน
        for (int x = 0; x < xDimension; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow == null || pieceBelow.Type == PieceType.EMPTY)
            {
                if (pieceBelow != null)
                {
                    Destroy(pieceBelow.gameObject);
                }
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MoveableCompponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }
        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDimension / 2.0f + x, transform.position.y + yDimension / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        if (piecePrefabDict.ContainsKey(type))
        {
            GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
            newPiece.transform.parent = transform;
            pieces[x, y] = newPiece.GetComponent<GamePiece>();
            if (pieces[x, y] != null)
            {
                pieces[x, y].Init(x, y, this, type);
            }
            return pieces[x, y];
        }
        return null;
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void ClearConnectedPieces(GamePiece piece)
    {
        List<GamePiece> connectedPieces = GetConnectedPieces(piece);
        int numConnected = connectedPieces.Count;

        if (numConnected >= 2) // At least two pieces must be connected
        {
            foreach (GamePiece p in connectedPieces)
            {
                Destroy(p.gameObject);
                pieces[p.X, p.Y] = null;
            }

            if (numConnected >= 10)
            {
                // Create Disco Piece
                SpawnSpecialPiece(piece.X, piece.Y, PieceType.DISCO, piece.ColorComponent.Color);
            }
            else if (numConnected >= 6)
            {
                // Create Bomb Piece
                SpawnSpecialPiece(piece.X, piece.Y, PieceType.BOMB, piece.ColorComponent.Color);
            }

            StartCoroutine(Fill()); // Refill the grid after clearing pieces
        }
    }

    private void SpawnSpecialPiece(int x, int y, PieceType type, ColorPiece.ColorType color)
    {
        if (piecePrefabDict.ContainsKey(type))
        {
            GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
            newPiece.transform.parent = transform;
            GamePiece gamePiece = newPiece.GetComponent<GamePiece>();
            if (gamePiece != null)
            {
                gamePiece.Init(x, y, this, type);
                gamePiece.ColorComponent.SetColor(color);
                pieces[x, y] = gamePiece;
            }
        }
    }

    private List<GamePiece> GetConnectedPieces(GamePiece piece)
    {
        List<GamePiece> connectedPieces = new List<GamePiece>();
        Queue<GamePiece> queue = new Queue<GamePiece>();
        queue.Enqueue(piece);
        HashSet<GamePiece> visited = new HashSet<GamePiece>();

        while (queue.Count > 0)
        {
            GamePiece current = queue.Dequeue();
            if (current == null || visited.Contains(current))
            {
                continue;
            }

            visited.Add(current);
            connectedPieces.Add(current);

            List<GamePiece> neighbors = GetNeighbors(current);
            foreach (GamePiece neighbor in neighbors)
            {
                if (neighbor != null && neighbor.ColorComponent != null && neighbor.ColorComponent.Color == piece.ColorComponent.Color && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return connectedPieces;
    }

    private List<GamePiece> GetNeighbors(GamePiece piece)
    {
        List<GamePiece> neighbors = new List<GamePiece>();

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = piece.X + dx[i];
            int newY = piece.Y + dy[i];

            if (newX >= 0 && newX < xDimension && newY >= 0 && newY < yDimension)
            {
                GamePiece neighbor = pieces[newX, newY];
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    public void ClearColor(ColorPiece.ColorType color)
    {
        Queue<GamePiece> piecesToClear = new Queue<GamePiece>();

        // Add all pieces with the same color to the queue
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                if (pieces[x, y] != null && pieces[x, y].ColorComponent.Color == color)
                {
                    piecesToClear.Enqueue(pieces[x, y]);
                }
            }
        }

        // Clear all the pieces in the queue
        while (piecesToClear.Count > 0)
        {
            GamePiece piece = piecesToClear.Dequeue();
            if (piece.Type == PieceType.BOMB)
            {
                Queue<GamePiece> additionalPiecesToClear = GetPiecesInRowAndColumn(piece.X, piece.Y);
                while (additionalPiecesToClear.Count > 0)
                {
                    piecesToClear.Enqueue(additionalPiecesToClear.Dequeue());
                }
            }
            else if (piece.Type == PieceType.DISCO)
            {
                Queue<GamePiece> additionalPiecesToClear = GetPiecesWithColor(piece.ColorComponent.Color);
                while (additionalPiecesToClear.Count > 0)
                {
                    piecesToClear.Enqueue(additionalPiecesToClear.Dequeue());
                }
            }
            Destroy(piece.gameObject);
            pieces[piece.X, piece.Y] = null;
        }

        StartCoroutine(Fill());
    }

    private Queue<GamePiece> GetPiecesWithColor(ColorPiece.ColorType color)
    {
        Queue<GamePiece> pieces = new Queue<GamePiece>();
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                if (this.pieces[x, y] != null && this.pieces[x, y].ColorComponent.Color == color)
                {
                    pieces.Enqueue(this.pieces[x, y]);
                }
            }
        }
        return pieces;
    }

    private Queue<GamePiece> GetPiecesInRowAndColumn(int x, int y)
    {
        Queue<GamePiece> pieces = new Queue<GamePiece>();
        for (int i = 0; i < xDimension; i++)
        {
            if (this.pieces[i, y] != null)
            {
                pieces.Enqueue(this.pieces[i, y]);
            }
        }

        for (int j = 0; j < yDimension; j++)
        {
            if (this.pieces[x, j] != null)
            {
                pieces.Enqueue(this.pieces[x, j]);
            }
        }
        return pieces;
    }
    public void ClearRowAndColumn(int x, int y)
    {
        Queue<GamePiece> piecesToClear = new Queue<GamePiece>();

        // Add all pieces in the row and column to the queue
        for (int i = 0; i < xDimension; i++)
        {
            if (pieces[i, y] != null)
            {
                piecesToClear.Enqueue(pieces[i, y]);
            }
        }

        for (int j = 0; j < yDimension; j++)
        {
            if (pieces[x, j] != null)
            {
                piecesToClear.Enqueue(pieces[x, j]);
            }
        }

        // Clear all the pieces in the queue
        while (piecesToClear.Count > 0)
        {
            GamePiece piece = piecesToClear.Dequeue();
            if (piece.Type == PieceType.BOMB)
            {
                Queue<GamePiece> additionalPiecesToClear = GetPiecesInRowAndColumn(piece.X, piece.Y);
                while (additionalPiecesToClear.Count > 0)
                {
                    piecesToClear.Enqueue(additionalPiecesToClear.Dequeue());
                }
            }
            else if (piece.Type == PieceType.DISCO)
            {
                Queue<GamePiece> additionalPiecesToClear = GetPiecesWithColor(piece.ColorComponent.Color);
                while (additionalPiecesToClear.Count > 0)
                {
                    piecesToClear.Enqueue(additionalPiecesToClear.Dequeue());
                }
            }
            Destroy(piece.gameObject);
            pieces[piece.X, piece.Y] = null;
        }

        StartCoroutine(Fill());
    }
}