using System;
using System.Collections;

public class UniqueRandomSample : IEnumerable
{
    public UniqueRandomSample(int low, int high)
    {
        sample = new int[Math.Max(high, low) - Math.Min(high, low)];
        for (int i = 0; i < sample.Length; ++i) sample[i] = i + Math.Min(high, low);
        for (int i = 0; i < sample.Length - 1; ++i)
        {
            int j = UnityEngine.Random.Range(i, sample.Length);
            int tmp = sample[i];
            sample[i] = sample[j];
            sample[j] = tmp;
        }
    }
    public UniqueRandomSample(int[] sample)
    {
        this.sample = new int[sample.Length];
        for (int i = 0; i < sample.Length - 1; ++i)
        {
            int j = UnityEngine.Random.Range(i, sample.Length);
            int tmp = sample[i];
            sample[i] = sample[j];
            sample[j] = tmp;
        }
        for (int i = 0; i < sample.Length; ++i) this.sample[i] = sample[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator) GetEnumerator();
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(sample);
    }

    private int[] sample;

    public class Enumerator : IEnumerator
    {
        private int[] sample;
        private int position = -1;

        public Enumerator(int[] sample) { this.sample = sample; }

        public bool MoveNext()
        {
            ++position;
            return (position < sample.Length);
        }

        public void Reset() { position = -1; }

        object IEnumerator.Current { get { return Current; } }
        public int Current
        {
            get
            {
                try { return sample[position]; }
                catch (IndexOutOfRangeException) { throw new InvalidOperationException(); }
            }
        }
    }
}

public static class RandomUniqueExtension
{
    public static UniqueRandomSample UniqueSample(this UnityEngine.Random rng, int[] values)
    {
        return new UniqueRandomSample(values);
    }

    public static UniqueRandomSample UniqueRange(this UnityEngine.Random rng, int low, int high)
    {
        return new UniqueRandomSample(low, high);
    }
}
