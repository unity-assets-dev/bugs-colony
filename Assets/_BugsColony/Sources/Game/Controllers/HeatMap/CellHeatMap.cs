using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct CellHeatMap : IJobParallelFor {
    
    [ReadOnly] public float CoolingSpeed;
    
    public NativeArray<float> HeatMap;
    
    public void Execute(int index) {
        HeatMap[index] -= CoolingSpeed;
    }

}