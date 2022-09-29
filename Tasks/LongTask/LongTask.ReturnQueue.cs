using OpenCvSharp;

namespace ClipHunta2;

public partial class LongTaskWithReturn<T, TR>
{
    int _finished = 0;
    int averageMilliSeconds = 0;
    int maxMilliSeconds = 0;
    int maxBackPressure = 0;
    int fastestMilliSeconds = int.MaxValue;
    public override int Finished()
    {
        return _finished;
    }

    public override int AverageMilliSeconds()
    {
        return averageMilliSeconds;
    }

    public override int MaxMilliSeconds()
    {
        return maxMilliSeconds;
    }

    public override int MaxBackPressure()
    {
        return maxBackPressure;
    }

    public override int FastestMilliSecond()
    {
        return fastestMilliSeconds;
    }

 

    public class ReturnQueue
    {
        private CancellationTokenSource _source = new CancellationTokenSource();
        private TR? _value;

        public void SetValue(TR? value)
        {
 
            _value = value;
            _source.Cancel();
        }

        public TR? GetReturn()
        {
            try
            {
                Task.Delay(-1, _source.Token).Wait();
            }
            catch (Exception e)
            {
            }

         

            return _value;
        }
    }
}