using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Level Queue")]
public class LevelQueue : ScriptableObject
{
    [System.Serializable]
    public class LevelQueueField : object
    {
        public LevelQueueField(UnityEngine.Object level, bool inQueue = true)
        {
            this.level = level;
            this.inQueue = inQueue;
        }
        public UnityEngine.Object level;
        public bool inQueue;
    }
    [SerializeField]
    private LevelQueueField[] settings;
    public Levels levels
    {
        get { return new Levels(settings); }
    }

    public class Levels : IEnumerable
    {
        private LevelQueueField[] levels;
        public Levels(LevelQueueField[] levels)
        {
            this.levels = new LevelQueueField[levels.Length];
            for(int i = 0; i < levels.Length; ++i)
            {
                this.levels[i] = levels[i];
            }
        }

        public Levels shuffle()
        {
            List<LevelQueueField> tmp = new List<LevelQueueField>();
            foreach (UnityEngine.Object l in this) tmp.Add(new LevelQueueField(l));
            if (tmp.Count == 0) return this;

            int index = 0;
            foreach (int i in new UniqueRandomSample(1, tmp.Count))
            {
                while (index < levels.Length && !levels[index].inQueue) ++index;
                levels[index] = new LevelQueueField(tmp[i].level);
            }
            return this;
        }

        public int Length
        {
            get
            {
                int count = 0;
                Enumerator e = GetEnumerator();
                while (e.MoveNext()) ++count;
                return count;
            }
        }

        public UnityEngine.Object this[int key]
        {
            get
            {
                Enumerator e = GetEnumerator();
                for(int i = 0; i <= key; ++i)
                {
                    if (!e.MoveNext()) throw new IndexOutOfRangeException();
                }
                return e.Current;
            }
        }

        public UnityEngine.Object this[string name]
        {
            get
            {
                foreach(LevelQueueField l in levels)
                {
                    if(name == l.level.name)
                    {
                        if (!l.inQueue) throw new KeyNotFoundException();
                        return l.level; 
                    }
                }
                throw new KeyNotFoundException();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(levels);
        }

        public class Enumerator : IEnumerator
        {
            private LevelQueueField[] levels;
            private int position = -1;

            public Enumerator(LevelQueueField[] levels) { this.levels = levels; }

            public bool MoveNext()
            {
                do
                {
                    ++position;
                    if (position >= levels.Length) return false;
                } while (!levels[position].inQueue);
                return true;
            }

            public void Reset() { position = -1; }

            object IEnumerator.Current { get { return Current; } }
            public UnityEngine.Object Current
            {
                get
                {
                    try { return levels[position].level; }
                    catch (IndexOutOfRangeException) { throw new InvalidOperationException(); }
                }
            }
        }
    }
}
