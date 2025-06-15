using System;

public class MyMap<K, V>
{
    private MyList<K> keys = new MyList<K>();
    private MyList<V> values = new MyList<V>();

    public void Add(K key, V value)
    {
        if (ContainsKey(key))
            throw new Exception("La clave ya existe: " + key);

        keys.Add(key);
        values.Add(value);
    }

    public void Set(K key, V value)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(key))
            {
                values[i] = value;
                return;
            }
        }

        keys.Add(key);
        values.Add(value);
    }

    public V Get(K key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(key))
                return values[i];
        }

        throw new Exception("Clave no encontrada: " + key);
    }

    public bool ContainsKey(K key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(key))
                return true;
        }

        return false;
    }

    public V this[K key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public void Remove(K key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(key))
            {
                keys.RemoveAt(i);
                values.RemoveAt(i);
                return;
            }
        }
    }

    public int Count => keys.Count;

    public MyList<K> Keys => keys;
}
