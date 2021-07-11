using NBench;
using OVRSharp.Graphics.DirectX;

namespace OVRSharp.Benchmarks.Graphics
{
    public class CompositorBenchmarks
    {
        [PerfSetup]
        public void Setup()
        {
            var app = new Application(Application.ApplicationType.Background);
        }

        [PerfBenchmark(
            RunMode = RunMode.Iterations,
            NumberOfIterations = 100,
            TestMode = TestMode.Measurement
        )]
        [TimingMeasurement()]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void GetMirrorImageBenchmark()
        {
            DirectXCompositor.Instance.GetMirrorImage();
        }
    }
}
