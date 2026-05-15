using System;
using System.Collections.Generic;

namespace VehicleConsoleApp
{
    public abstract class Vehicle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public decimal Price { get; set; }
        public double Speed { get; set; }
        public int YearOfManufacture { get; set; }

        protected Vehicle(double x, double y, decimal price, double speed, int year)
        {
            X = x; Y = y; Price = price; Speed = speed; YearOfManufacture = year;
        }

        public abstract void Move(double newX, double newY);
        public abstract string GetDynamicState();
    }

    public class Plane : Vehicle
    {
        public double Altitude { get; set; }
        public int PassengerCapacity { get; set; }

        public Plane(double x, double y, decimal price, double speed, int year, double altitude, int passengers)
            : base(x, y, price, speed, year)
        {
            Altitude = altitude;
            PassengerCapacity = passengers;
        }

        public override void Move(double newX, double newY) => (X, Y) = (newX, newY);
        public override string GetDynamicState() =>
            $"Самолет | Скорость: {Speed} км/ч | Высота: {Altitude} м | Вместимость: {PassengerCapacity}";
    }

    public class Car : Vehicle
    {
        public Car(double x, double y, decimal price, double speed, int year)
            : base(x, y, price, speed, year) { }

        public override void Move(double newX, double newY) => (X, Y) = (newX, newY);
        public override string GetDynamicState() =>
            $"🚗 Автомобиль | Скорость: {Speed} км/ч | Год выпуска: {YearOfManufacture}";
    }

    public class Ship : Vehicle
    {
        public int PassengerCapacity { get; set; }
        public string HomePort { get; set; }

        public Ship(double x, double y, decimal price, double speed, int year, int passengers, string port)
            : base(x, y, price, speed, year)
        {
            PassengerCapacity = passengers;
            HomePort = port;
        }

        public override void Move(double newX, double newY) => (X, Y) = (newX, newY);
        public override string GetDynamicState() =>
            $"🚢 Корабль | Скорость: {Speed} км/ч | Вместимость: {PassengerCapacity} | Порт: {HomePort}";
    }

    class Program
    {
        static readonly List<Vehicle> Fleet = new List<Vehicle>();

        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ ТРАНСПОРТОМ ===");
                Console.WriteLine("1. Создать транспорт");
                Console.WriteLine("2. Переместить транспорт");
                Console.WriteLine("3. Показать все транспорты");
                Console.WriteLine("4. Выход");
                Console.Write("Ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out int choice)) { Wait("Ошибка ввода. Нажмите Enter..."); continue; }

                switch (choice)
                {
                    case 1: CreateVehicle(); break;
                    case 2: MoveVehicle(); break;
                    case 3: ShowAll(); break;
                    case 4: Console.WriteLine("Выход..."); return;
                    default: Wait("Неверный пункт. Нажмите Enter..."); break;
                }
            }
        }

        static void CreateVehicle()
        {
            Console.WriteLine("\n--- Создание транспорта ---");
            Console.WriteLine("1. Самолет\n2. Автомобиль\n3. Корабль");
            int type = ReadInt("Выберите тип (1-3): ");
            if (type < 1 || type > 3) { Wait("Неверный тип."); return; }

            double x = ReadDouble("Координата X: ");
            double y = ReadDouble("Координата Y: ");
            decimal price = ReadDecimal("Цена: ");
            double speed = ReadDouble("Скорость (км/ч): ");
            int year = ReadInt("Год выпуска: ");

            Vehicle v = type switch
            {
                1 => new Plane(x, y, price, speed, year, ReadDouble("Высота полета (м): "), ReadInt("Вместимость (пасс.): ")),
                2 => new Car(x, y, price, speed, year),
                3 => new Ship(x, y, price, speed, year, ReadInt("Вместимость (пасс.): "), ReadString("Порт приписки: ")),
                _ => null
            };

            Fleet.Add(v);
            Wait("Транспорт успешно добавлен! Нажмите Enter...");
        }

        static void MoveVehicle()
        {
            if (Fleet.Count == 0) { Wait("Список пуст. Создайте транспорт сначала."); return; }

            Console.WriteLine("\n--- Выберите транспорт для перемещения ---");
            for (int i = 0; i < Fleet.Count; i++)
                Console.WriteLine($"{i + 1}. [{Fleet[i].GetType().Name}] X:{Fleet[i].X:F1} Y:{Fleet[i].Y:F1}");

            int idx = ReadInt("Номер транспорта: ") - 1;
            if (idx < 0 || idx >= Fleet.Count) { Wait("Неверный номер."); return; }

            double newX = ReadDouble("Новая координата X: ");
            double newY = ReadDouble("Новая координата Y: ");

            Fleet[idx].Move(newX, newY);
            Console.WriteLine($"Перемещено! Текущее положение: ({newX:F1}, {newY:F1})");
            Wait("Нажмите Enter...");
        }

        static void ShowAll()
        {
            Console.WriteLine("\n--- Текущий список ---");
            if (Fleet.Count == 0) { Console.WriteLine("Нет созданных транспортов."); }
            else
            {
                for (int i = 0; i < Fleet.Count; i++)
                    Console.WriteLine($"{i + 1}. {Fleet[i].GetDynamicState()} | Коорд: ({Fleet[i].X:F1}, {Fleet[i].Y:F1})");
            }
            Wait("Нажмите Enter...");
        }

        // Вспомогательные методы ввода
        static int ReadInt(string p) { while (true) { Console.Write(p); if (int.TryParse(Console.ReadLine(), out int r)) return r; Console.WriteLine("Введите целое число."); } }
        static double ReadDouble(string p) { while (true) { Console.Write(p); if (double.TryParse(Console.ReadLine(), out double r)) return r; Console.WriteLine("Введите число."); } }
        static decimal ReadDecimal(string p) { while (true) { Console.Write(p); if (decimal.TryParse(Console.ReadLine(), out decimal r)) return r; Console.WriteLine("Введите число."); } }
        static string ReadString(string p) { Console.Write(p); return Console.ReadLine()?.Trim() ?? ""; }
        static void Wait(string msg) { Console.WriteLine(msg); Console.ReadLine(); }
    }
}