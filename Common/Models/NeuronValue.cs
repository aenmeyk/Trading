namespace Common.Models
{
    public class NeuronValue
    {
        public string Symbol { get; set; }
        public int HiddenNeuronIndex { get; set; }
        public int InputNeuronIndex { get; set; }
        public double Value { get; set; }
    }
}
