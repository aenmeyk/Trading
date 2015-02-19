using System.Linq;

namespace NeuralNet
{
    public class Network
    {
        private int _recordCount;

        public double CurrentLearningRate { get; set; }

        public double[][] InputValues;
        public double[] OutputValues;

        public double[][] HiddenDelta;
        public double[] OutputDelta;

        public double[][] HiddenOutput;
        public double[] OutputOutput;

        public Network(double[][] inputValues, double[] outputValues = null)
        {
            _recordCount = inputValues.Count();

            this.InputValues = inputValues;
            this.OutputValues = outputValues;

            this.initializeHiddenDeltaArray();
            this.initializeOutputDeltaArray();

            this.initializeHiddenOutputArray();
            this.initializeOutputOutputArray();
        }

        private void initializeHiddenDeltaArray()
        {
            this.HiddenDelta = new double[this._recordCount][];

            for (int j = 0; j < this._recordCount; j++)
            {
                this.HiddenDelta[j] = new double[NetworkSettings.HiddenNeurons];
            }
        }

        private void initializeOutputDeltaArray()
        {
            this.OutputDelta = new double[this._recordCount];
        }

        private void initializeHiddenOutputArray()
        {
            this.HiddenOutput = new double[this._recordCount][];

            for (int j = 0; j < this._recordCount; j++)
            {
                this.HiddenOutput[j] = new double[NetworkSettings.HiddenNeurons];
            }
        }

        private void initializeOutputOutputArray()
        {
            this.OutputOutput = new double[this._recordCount];
        }

        //private void initializeInputValuesArray()
        //{
        //    this.InputValues = new double[this._recordCount][];

        //    for (int i = 0; i < this._recordCount; i++)
        //    {
        //        this.InputValues[i] = new double[NetworkSettings.InputNeurons];
        //    }
        //}

        //private void initializeOutputValuesArray()
        //{
        //    this.OutputValues = new double[this._recordCount];
        //}

        //private void populateData(bool testNetwork)
        //{
        //    if (!testNetwork)
        //    {
        //        this.InputValues[0] = new[] { 1.0, 3.0, 3.0 };
        //        this.InputValues[1] = new[] { 4.0, 9.0, 4.0 };
        //        this.InputValues[2] = new[] { 5.0, 4.0, 7.0 };
        //        this.InputValues[3] = new[] { 3.0, 1.0, 1.0 };
        //        this.InputValues[4] = new[] { 1.0, 2.0, 2.0 };
        //        this.InputValues[5] = new[] { 3.0, 8.0, 1.0 };
        //        this.InputValues[6] = new[] { 5.0, 7.0, 4.0 };
        //        this.InputValues[7] = new[] { 2.0, 3.0, 1.0 };
        //        this.InputValues[8] = new[] { 5.0, 2.0, 8.0 };
        //        this.InputValues[9] = new[] { 2.0, 9.0, 9.0 };

        //        this.OutputValues[0] = 0;
        //        this.OutputValues[1] = 1;
        //        this.OutputValues[2] = 1;
        //        this.OutputValues[3] = 0;
        //        this.OutputValues[4] = 0;
        //        this.OutputValues[5] = 1;
        //        this.OutputValues[6] = 1;
        //        this.OutputValues[7] = 0;
        //        this.OutputValues[8] = 1;
        //        this.OutputValues[9] = 1;
        //    }
        //    else
        //    {
        //        this.InputValues[0] = new[] { 7.0, 4.0, 2.0 };
        //        this.InputValues[1] = new[] { 3.0, 1.0, 1.0 };
        //        this.InputValues[2] = new[] { 4.0, 2.0, 2.0 };
        //        this.InputValues[3] = new[] { 5.0, 9.0, 5.0 };
        //        this.InputValues[4] = new[] { 9.0, 9.0, 9.0 };
        //        this.InputValues[5] = new[] { 3.0, 8.0, 1.0 };
        //        this.InputValues[6] = new[] { 5.0, 7.0, 4.0 };
        //        this.InputValues[7] = new[] { 2.0, 3.0, 1.0 };
        //        this.InputValues[8] = new[] { 5.0, 2.0, 8.0 };
        //        this.InputValues[9] = new[] { 2.0, 9.0, 9.0 };
        //    }
        //}

        //private void populateData(bool testNetwork)
        //{
        //    if (!testNetwork)
        //    {
        //        this.InputValues[0] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[1] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[2] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[3] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[4] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[5] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[6] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[7] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[8] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[9] = new[] { 1.0, 1.0, 1.0 };

        //        this.OutputValues[0] = 1;
        //        this.OutputValues[1] = 0;
        //        this.OutputValues[2] = 0;
        //        this.OutputValues[3] = 0;
        //        this.OutputValues[4] = 1;
        //        this.OutputValues[5] = 1;
        //        this.OutputValues[6] = 0;
        //        this.OutputValues[7] = 1;
        //        this.OutputValues[8] = 0;
        //        this.OutputValues[9] = 1;
        //    }
        //    else
        //    {
        //        this.InputValues[0] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[1] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[2] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[3] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[4] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[5] = new[] { 1.0, 1.0, 1.0 };
        //        this.InputValues[6] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[7] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[8] = new[] { 0.0, 0.0, 0.0 };
        //        this.InputValues[9] = new[] { 1.0, 1.0, 1.0 };
        //    }
        //}
    }
}
