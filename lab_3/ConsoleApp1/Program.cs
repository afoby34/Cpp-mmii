using System;
using System.Collections.Generic;

public class FixedStringArray
{
    private readonly string[] _data;
    public int Length => _data.Length;
    public int FixedLength { get; }

    public FixedStringArray(int size, int fixedLength)
    {
        if (size <= 0) throw new ArgumentException("Размер массива должен быть > 0");
        if (fixedLength <= 0) throw new ArgumentException("Длина строки должна быть > 0");

        FixedLength = fixedLength;
        _data = new string[size];
    }

    public string this[int index]
    {
        get
        {
            CheckIndex(index);
            return _data[index];
        }
        set
        {
            CheckIndex(index);
            string val = value ?? string.Empty;
            _data[index] = val.Length > FixedLength 
                ? val.Substring(0, FixedLength) 
                : val.PadRight(FixedLength);
        }
    }

    private void CheckIndex(int index)
    {
        if (index < 0 || index >= _data.Length)
            throw new IndexOutOfRangeException($"Индекс {index} вне диапазона [0..{_data.Length - 1}]");
    }

    public FixedStringArray Concat(FixedStringArray other)
    {
        int newLen = Math.Max(this.FixedLength, other.FixedLength);
        var res = new FixedStringArray(_data.Length + other._data.Length, newLen);
        int idx = 0;
        foreach (var s in _data) res[idx++] = s;
        foreach (var s in other._data) res[idx++] = s;
        return res;
    }

    public FixedStringArray Merge(FixedStringArray other)
    {
        int newLen = Math.Max(this.FixedLength, other.FixedLength);
        var unique = new List<string>();

        bool IsDuplicate(string val) => unique.Exists(u => u.Trim() == val.Trim());

        foreach (var s in _data)
            if (!IsDuplicate(s)) unique.Add(s);

        foreach (var s in other._data)
            if (!IsDuplicate(s)) unique.Add(s);

        var res = new FixedStringArray(unique.Count, newLen);
        for (int i = 0; i < unique.Count; i++)
            res[i] = unique[i];
        return res;
    }

    public void PrintElement(int index) =>
        Console.WriteLine($"  arr[{index}] = \"{this[index]}\"");

    public void PrintAll()
    {
        Console.WriteLine($"[Размер: {Length}] [Фикс. длина строки: {FixedLength}]");
        for (int i = 0; i < Length; i++)
            Console.WriteLine($"  [{i}] = \"{_data[i]}\"");
        Console.WriteLine();
    }
}

class Program
{
    static readonly List<FixedStringArray> Arrays = new();

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Меню операций ===");
            
            if (Arrays.Count == 0)
                Console.WriteLine("Массивы ещё не созданы.");
            else
                for (int i = 0; i < Arrays.Count; i++)
                    Console.WriteLine($"[{i + 1}] Массив #{i + 1} (размер: {Arrays[i].Length})");

            Console.WriteLine("[C] Создать новый массив");
            Console.WriteLine("[D] Удалить массив");
            Console.WriteLine("[1] Сцепить два массива");
            Console.WriteLine("[2] Слить два массива (без дублей)");
            Console.WriteLine("[3] Вывести массив / элемент");
            Console.WriteLine("[0] Выход");
            Console.Write("Выбор: ");

            string choice = Console.ReadLine()?.Trim().ToUpper();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "C": CreateArray(); break;
                    case "D": DeleteArray(); break;
                    case "1": CombineArrays(false); break;
                    case "2": CombineArrays(true); break;
                    case "3": PrintArray(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] {ex.Message}");
            }

            Console.WriteLine("Нажмите Enter для возврата в меню...");
            Console.ReadLine();
        }
    }

    static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine(), out int val))
            throw new Exception("Ожидается целое число");
        return val;
    }

    static int SelectArray()
    {
        if (Arrays.Count == 0) throw new Exception("Нет созданных массивов");
        int idx = ReadInt("Номер массива: ");
        if (idx < 1 || idx > Arrays.Count) throw new Exception("Такого массива нет");
        return idx - 1;
    }

    static void CreateArray()
    {
        int size = ReadInt("Количество элементов: ");
        int len = ReadInt("Фиксированная длина строки: ");

        var arr = new FixedStringArray(size, len);
        Console.WriteLine($"Заполните {size} элементов (пустая строка = пробелы):");
        for (int i = 0; i < size; i++)
        {
            Console.Write($"  [{i}] = ");
            arr[i] = Console.ReadLine() ?? string.Empty;
        }

        Arrays.Add(arr);
        Console.WriteLine("Массив создан.");
    }

    static void DeleteArray()
    {
        if (Arrays.Count == 0) throw new Exception("Нет массивов для удаления");
        Console.WriteLine("Выберите массив для удаления:");
        int idx = SelectArray();
        Arrays.RemoveAt(idx);
        Console.WriteLine("Массив успешно удалён.");
    }

    static void CombineArrays(bool isMerge)
    {
        if (Arrays.Count < 2) throw new Exception("Нужно минимум 2 массива");
        Console.WriteLine("Выберите два массива для операции:");
        int i1 = SelectArray();
        int i2 = SelectArray();

        FixedStringArray res = isMerge 
            ? Arrays[i1].Merge(Arrays[i2]) 
            : Arrays[i1].Concat(Arrays[i2]);

        res.PrintAll();
        Console.Write("Сохранить результат как новый массив? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y") Arrays.Add(res);
    }

    static void PrintArray()
    {
        int idx = SelectArray();
        Console.Write("Вывести весь массив? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
            Arrays[idx].PrintAll();
        else
            Arrays[idx].PrintElement(ReadInt("Индекс элемента: "));
    }
}