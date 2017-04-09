using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Level Queue")]
public class LevelQueue : ScriptableObject
{
    [System.Serializable]
    public class LevelQueueField : object
    {
        public LevelQueueField(LevelQueueField other)
            : this(other.scene, other.inQueue)
        {
        }
        public LevelQueueField(SceneAsset scene, bool inQueue = true)
        {
            this.scene = scene;
            this.inQueue = inQueue;
        }

        [SerializeField]
        public SceneAsset scene;
        public string level { get { return scene.name; } }
        public bool inQueue;
    }

    [SerializeField]
    private LevelQueueField[] settings;

    public class Levels : IEnumerable
    {
        public Levels(LevelQueue outer)
        {
            List<string> lst = new List<string>();
            foreach (LevelQueueField f in outer.settings)
            {
                Debug.Log(f.level);
                if (f.inQueue) lst.Add(f.level);
            }
            levels = lst.ToArray();
        }

        public Levels shuffle()
        {
            List<string> lst = new List<string>();
            foreach (int i in new UniqueRandomSample(0, levels.Length)) lst.Add(levels[i]);
            for (int i = 0; i < levels.Length; ++i) levels[i] = lst[i];
            return this;
        }

        public int Length { get { return levels.Length; } }

        public string this[int key]
        {
            get { return levels[key]; }
        }

        private string[] levels;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        public class Enumerator : IEnumerator
        {
            public Enumerator(Levels outer)
            {
                this.outer = outer;
                Reset();
            }

            object IEnumerator.Current { get { return Current; } }
            public string Current
            {
                get
                {
                    try { return outer.levels[position]; }
                    catch (IndexOutOfRangeException) { throw new InvalidOperationException(); }
                }
            }
            bool IEnumerator.MoveNext() { return MoveNext(); }
            public bool MoveNext()
            {
                ++position;
                return position < outer.levels.Length;
            }

            void IEnumerator.Reset() { Reset(); }
            public void Reset()
            {
                position = -1;
            }

            private int position;
            private Levels outer;
        }
    }

    public Levels levels
    {
        get { return new Levels(this); }
    }
}
