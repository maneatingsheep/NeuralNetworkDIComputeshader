
using System.Threading.Tasks;

public interface IVerifier {
    void Init();

    int GetInputSize { get; }
    int GetOtputSize { get; }

    Task<float> EvaluateModel();
    void SetNewVerefication(bool isTraining);

    void SetFitness(Model[] models);
    Task VisualizeSample();
    void VisualizeOutputReference();
    float[] CalculateWantedRes(float[] inputs);
}
