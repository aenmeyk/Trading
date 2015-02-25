using System.Linq;

namespace NeuralNet
{
    public class Network
    {
        private int _recordCount;

        public Core Core { get; private set; }
        public double LearningRate { get; set; }

        public double[][] InputValues { get; private set; }
        public double[] OutputValues { get; private set; }

        public double[][] HiddenDelta { get; private set; }
        public double[] OutputDelta { get; private set; }

        public double[][] HiddenOutput { get; private set; }
        public double[] OutputOutput { get; private set; }

        public Network(Core core, double[][] inputValues, double[] outputValues = null)
        {
            Core = core;
            _recordCount = inputValues.Count();

            InputValues = inputValues;
            OutputValues = outputValues;

            InitializeHiddenDeltaArray();
            InitializeOutputDeltaArray();

            InitializeHiddenOutputArray();
            InitializeOutputOutputArray();
        }

        private void InitializeHiddenDeltaArray()
        {
            HiddenDelta = new double[_recordCount][];

            for (int j = 0; j < _recordCount; j++)
            {
                HiddenDelta[j] = new double[NetworkSettings.HiddenNeuronCount];
            }
        }

        private void InitializeOutputDeltaArray()
        {
            OutputDelta = new double[_recordCount];
        }

        private void InitializeHiddenOutputArray()
        {
            HiddenOutput = new double[_recordCount][];

            for (int j = 0; j < _recordCount; j++)
            {
                HiddenOutput[j] = new double[NetworkSettings.HiddenNeuronCount];
            }
        }

        private void InitializeOutputOutputArray()
        {
            OutputOutput = new double[_recordCount];
        }
    }
}
