using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct SpreadMap : IJobParallelFor {
    public int Columns;
    public int Rows; 
    
    [ReadOnly] public NativeArray<float> CurrentMap;
    public NativeArray<float> NextMap;
    
    public void Execute(int index) {
        var myTemp = CurrentMap[index];
    
        var column = index / Rows;
        var row = index % Rows;

        var totalHeatExchange = 0f;

        for (var x = -1; x <= 1; x++) {
            for (var y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                var xCol = column + x;
                var yRow = row + y;

                if (xCol >= 0 && xCol < Columns && yRow >= 0 && yRow < Rows) {
                    var neighborTemp = CurrentMap[xCol * Rows + yRow];
                    var diff = neighborTemp - myTemp;
                    totalHeatExchange += diff * .125f; 
                }
            }
        }

        var afterCooling = Mathf.Clamp01(myTemp + totalHeatExchange);
        afterCooling = afterCooling < .5f ? afterCooling * .75f : afterCooling; // Speed up cooling to 25%
        NextMap[index] = afterCooling;
    }

}