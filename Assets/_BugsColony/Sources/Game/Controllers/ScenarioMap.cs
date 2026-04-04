using System.Linq;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Cell {
    
    public int Column { get; }
    public int Row { get; }
    
    public float Heat { get; set; }

    public Vector3 WorldPosition { get; }
    
    public Cell(int column, int row, Vector3 position) {
        Column = column;
        Row = row;
        Heat = 0;
        
        WorldPosition = position;
    }
}

public interface IHeatMap {
    Vector3[] RequestPosition(float heat, int positionCount);
    Vector3 RequestWarmThan(float heat);
    Vector3 RequestCoolerThan(float heat);
    Cell GetCellAtWorldPosition(Vector3 position);
}

public class ScenarioMap : MonoBehaviour, IHeatMap {
    [Header("Properties")]
    [SerializeField] private float _cellSize;
    [SerializeField] private float _timeToUpdate = 1f;
    
    [Header("Map Build")]
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private bool _showRunTimePreview = false;
    [SerializeField] private bool _solidPreview = false;
    
    [Header("Debug")]
    [SerializeField, NaughtyAttributes.ReadOnly] private int _columns;
    [SerializeField, NaughtyAttributes.ReadOnly] private int _rows;
    
    private Cell[] _cells;
    private float _timer;

    private void OnValidate() => BuildCells();

    private void Awake() => BuildCells();

    public void ResetMap() => BuildCells();

    private void BuildCells() {
        if (_renderer == null) return; 
        
        var bounds = _renderer.bounds;
        
        _columns = (int)(bounds.size.x / _cellSize);
        _rows = (int)(bounds.size.z / _cellSize);
        
        _cells = new Cell[_columns * _rows];

        for (var column = 0; column < _columns; column++)
            for (var row = 0; row < _rows; row++) {
               var cell = new Cell(column, row, GetWorldPositionAt(column, row));
               
               _cells[column * _columns + row] = cell;
            }
    }

    private void Update() {
        if ((_timer -= Time.deltaTime) <= 0) {
            _timer = _timeToUpdate;
            
            CoolingMap();
        }
    }

    private void CoolingMap() {
        
        var heatMap = _cells.Select(c => c.Heat).ToNativeArray(Allocator.TempJob);
        var resultMap = new NativeArray<float>(heatMap.Length, Allocator.TempJob);
        
        var job = new SpreadMap() {
            Columns = _columns,
            Rows = _rows,
            
            CurrentMap = heatMap,
            NextMap = resultMap
        };
        
        var task  = job.Schedule(_cells.Length, _rows);
        task.Complete();

        for (var n = 0; n < _cells.Length; n++) {
            //_cells[n].Heat = Mathf.Clamp01(job.HeatMap[n]);
            _cells[n].Heat = Mathf.Clamp01(job.NextMap[n]);
        }
        resultMap.Dispose();
        heatMap.Dispose();
    }
    
    public void Heat(Vector3[] map, float heat) {
        map
            .Select(GetCellAtWorldPosition)
            .EachNonAlloc(c => {
                c.Heat = Mathf.Clamp01(c.Heat + heat);
            });
    }

    public Vector3[] RequestPosition(float heat, int positionCount) {
        var order = _cells
            .Where(cell => heat > 0 ? cell.Heat > heat : cell.Heat < Mathf.Abs(heat));
        
        var positions = order.Count() >= positionCount?
                order: 
                _cells;
        
        return positions
            .Shuffle(2) // Randomize collection
            .Take(positionCount)
            .Select(pos => pos.WorldPosition)
            .ToArray();
    }
    
    public Vector3 RequestWarmThan(float heat) {
        var order = _cells
            .Where(cell => cell.Heat >= heat);
        
        order = order.Count() > 0 ? 
            order : 
            _cells;
        
        return order
            //.OrderBy(cell => cell.Heat)
            .Shuffle(2)
             // Randomize collection
            .Select(pos => pos.WorldPosition)
            .FirstOrDefault();
    }
    
    public Vector3 RequestCoolerThan(float heat) {
        var order = _cells
            .Where(cell => cell.Heat <= heat);
        order = order.Count() > 0 ? 
            order : 
            _cells;
        
        return order
            //.OrderByDescending(cell => cell.Heat)
            .Shuffle(2)
            // Randomize collection
            .Select(pos => pos.WorldPosition)
            .FirstOrDefault();
    }

    private Cell At(int column, int row) {
        column = Mathf.Clamp(column, 0, _columns - 1);
        row = Mathf.Clamp(row, 0, _rows - 1);
        return _cells[column * _columns + row];
    }

    public Cell GetCellAtWorldPosition(Vector3 position) {
        var offset = new Vector3(_cellSize * _columns, .25f, _cellSize * _rows) * .5f;
    
        var localPos = position + offset;

        var column = Mathf.FloorToInt(localPos.x / _cellSize);
        var row = Mathf.FloorToInt(localPos.z / _cellSize);

        return At(column, row);
    }

    private Vector3 GetWorldPositionAt(int column, int row) {
        var offset = new Vector3(_cellSize * _columns, .25f, _cellSize * _rows) * .5f;
        
        return new Vector3(column * _cellSize + _cellSize * .5f, 0, row * _cellSize + _cellSize * .5f) - offset;
    }

    private void OnDrawGizmos() {
        if (!_showRunTimePreview || _renderer == null || _cells.Length == 0) return;
        
        foreach (var cell in _cells) {
            Gizmos.color = Color.Lerp(Color.green, Color.red, cell.Heat);
            if(!_solidPreview) 
                Gizmos.DrawWireCube(cell.WorldPosition + Vector3.up * 2f, DebugCell(_cellSize - .25f));
            else
                Gizmos.DrawCube(cell.WorldPosition + Vector3.up * 2f, DebugCell(_cellSize - .1f));
        }
    }

    private Vector3 DebugCell(float size) => new (size, .1f, size);

    [Button]
    private void Refresh() => BuildCells();

    [Button]
    private void ExamineCells() => RequestWarmThan(1);
    
    [Button]
    private void Reset() => _cells.EachNonAlloc(cell => cell.Heat = 0);

    
}