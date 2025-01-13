using System.Text;
using MDA.ListMethods;

namespace MDA;

public class MdaList : IMdaCallable
{
    private readonly List<object> _elements = [];
    private readonly Dictionary<string, IMdaCallable> methods = new();

    public MdaList()
    {
        methods["push"] = new MdaListPush(this);
        methods["pop"] = new MdaListPop(this);
        methods["reverse"] = new MdaListReverse(this);
        methods["length"] = new MdaListLength(this);
        methods["insertAt"] = new MdaListInsertAt(this);
        methods["removeAt"] = new MdaListRemoveAt(this);
        methods["sort"] = new MdaListSort(this);
        methods["sorted"] = new MdaListSorted(this);
        methods["contains"] = new MdaListContains(this);
        methods["indexOf"] = new MdaListIndexOf(this);
        methods["lastIndexOf"] = new MdaListLastIndexOf(this);
        methods["remove"] = new MdaListRemove(this);
        methods["removeAll"] = new MdaListRemoveAll(this);
        methods["filter"] = new MdaListFilter(this);
        methods["filtered"] = new MdaListFiltered(this);
        methods["customSort"] = new MdaListCustomSort(this);
    }

    /*
     * Get the element at the given index.
     */
    public object Get(int index)
    {
        if (index < 0 || index >= _elements.Count)
        {
            throw new RuntimeError(null, "Array index out of bounds.");
        }

        return _elements[index];
    }

    /*
     * Set the element at the given index to the given value.
     */
    public void Set(int index, object value)
    {
        if (index < 0)
        {
            throw new RuntimeError(null, "Array index cannot be negative.");
        }

        // Allow expanding the array by setting elements beyond current size
        while (_elements.Count <= index)
        {
            _elements.Add(null);
        }

        _elements[index] = value;
    }

    /*
     * Add the given value to the end of the array.
     */
    public void Push(object value)
    {
        _elements.Add(value);
    }

    /*
     * Remove and return the last element of the array.
     */
    public object Pop()
    {
        if (_elements.Count == 0)
        {
            throw new RuntimeError(null, "Cannot pop from an empty array.");
        }

        object value = _elements[_elements.Count - 1];
        _elements.RemoveAt(_elements.Count - 1);
        return value;
    }
    
    /*
     * Reverse the array.
     */
    public void Reverse()
    {
        _elements.Reverse();
    }
    
    /*
     * Insert the given value at the given index.
     */
    public void InsertAt(int index, object value)
    {
        if (index < 0 || index > _elements.Count)
        {
            throw new RuntimeError(null, "Array index out of bounds.");
        }

        _elements.Insert(index, value);
    }
    
    /*
     * Remove the element at the given index.
     */
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _elements.Count)
        {
            throw new RuntimeError(null, "Array index out of bounds.");
        }

        _elements.RemoveAt(index);
    }
    
    /*
     * Check if the array contains the given value.
     */
    public bool Contains(object value)
    {
        return _elements.Contains(value);
    }
    
    /*
     * Returns the index of the given value in the array.
     * Returns -1 if the value is not in the array.
     */
    public int IndexOf(object value)
    {
        return _elements.IndexOf(value);
    }
    
    /*
     * Sort in place the array.
     */
    public void Sort()
    {
        // TODO: Implement a more robust sorting algorithm.
        // This is a naive implementation that will throw an exception if not all elements are comparable.
        // A more robust implementation would check if all elements are comparable before sorting.
        
        _elements.Sort((a, b) => Utils.Compare(a, b));
    }
    
    /*
     * Returns a sorted copy of the array.
     */
    public MdaList Sorted()
    {
        MdaList list = new();
        list._elements.AddRange(_elements);
        list.Sort();
        return list;
    }

    /*
     * Remove the first occurrence of the given value from the array.
     *
     * Returns true if the value was removed, false otherwise.
     */
    public bool Remove(object value)
    {
        return _elements.Remove(value);
    }
    
    /*
     * Remove all occurrences of the given value from the array.
     *
     * Returns the number of elements removed.
     */
    public object RemoveAll(object value)
    {
        return _elements.RemoveAll(v => v.Equals(value));
    }

    /*
     * Get the last index of an elemet in the array
     */
    public object LastIndexOf(object value)
    {
        return _elements.LastIndexOf(value);
    }
    
    /*
     * Create a new list filtered by the given function.
     *
     * Returns the filtered list.
     */
    public object Filtered(Interpreter interpreter, MdaFunction function)
    {
        MdaList list = new();
        foreach (var element in _elements)
        {
            if (Utils.IsTruthy(function.Call(interpreter, new List<object>{ element })))
            {
                list.Push(element);
            }
        }
        return list;
    }    
    
    /*
     * Filter the array by the given function.
     *
     * Returns the array.
     */
    public object Filter(Interpreter interpreter, MdaFunction function)
    {
        _elements.RemoveAll((el) =>
        {
            return !Utils.IsTruthy(function.Call(interpreter, new List<object> { el }));
        });

        return this;
    }
    
    public object CustomSort(Interpreter interpreter, MdaFunction function)
    {
        object? res = null;
        _elements.Sort((a, b) =>
        {
            res = function.Call(interpreter, new List<object> { a, b });
            if (res is double d)
            {
                return (int)d;
            }

            return 0;
        });
        return this;
    }
    
    /*
     * Get the length of the array.
     */
    public int Length()
    {
        return _elements.Count;
    }
    
    public IMdaCallable? FindMethod(string name)
    {
        if (methods.ContainsKey(name))
        {
            return methods[name];
        }

        return null;
    }

    public int Arity()
    {
        return 0;
    }

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return new MdaList();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("[");
        for (int i = 0; i < _elements.Count; i++)
        {
            stringBuilder.Append(Utils.Stringify(_elements[i]));
            if (i < _elements.Count - 1)
            {
                stringBuilder.Append(", ");
            }
        }

        stringBuilder.Append("]");
        return stringBuilder.ToString();
    }
}