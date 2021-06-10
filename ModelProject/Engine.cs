using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelProject
{
    class Engine
    {
        #region Поля класса
        /// <summary>
        /// Наибольшее модельное время в секундах. Если при моделировании достигается это значение, считается, что двигатель не перегревается.
        /// Устанавливается в статическом конструкторе, по умолчанию = 10 суток.
        /// </summary>
        public static int MAX_TIME;
        /// <summary>
        /// Момент инерции, кг*м2.
        /// </summary>
        public double I { get; private set; }
        /// <summary>
        /// Коэффициент зависимости нагрева от крутящего момента.
        /// </summary>
        public double Hm { get; private set; }
        /// <summary>
        /// Коэффициент зависимости нагрева от скорости вращения.
        /// </summary>
        public double Hv { get; private set; }
        /// <summary>
        /// Коэффициент зависимости охлаждения от температуры двигателя и окр.среды.
        /// </summary>
        public double C { get; private set; }
        /// <summary>
        /// Температура перегрева двигателя.
        /// </summary>
        public double TOverheat { get; private set; }
        /// <summary>
        /// Зависимость крутящего момента M от скорости вращения V.
        /// </summary>
        public Dictionary<double, double> MVPairs { get; private set; }
        #endregion
        #region Публичные сеттеры для полей
        public void SetI(double val)    {   I = val;    }
        public void SetHm(double val)   {   Hm = val;   }
        public void SetHv(double val)   {   Hv = val;   }
        public void SetC(double val)    {   C = val;    }
        public void SetTOverheat(double val)    {   TOverheat = val;    }
        public void SetMV(Dictionary<double, double> keyValuePairs)
        {
            MVPairs = keyValuePairs.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
        }
        #endregion
        #region Конструкторы
        static Engine()
        {
            MAX_TIME = 3600 * 24 * 10;
        }
        /// <summary>
        /// Конструктор двигателя из txt-файла.
        /// </summary>
        /// <param name="filename">Полный адрес файла формата txt.</param>
        public Engine(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    I = double.Parse(sr.ReadLine());
                    TOverheat = double.Parse(sr.ReadLine());
                    Hm = double.Parse(sr.ReadLine());
                    Hv = double.Parse(sr.ReadLine());
                    C = double.Parse(sr.ReadLine());
                    MVPairs = new Dictionary<double, double>();
                    uint count = uint.TryParse(sr.ReadLine(), out uint ct) ? ct : 0;
                    if (count > 0)
                    {
                        for (uint j = 0; j < count; j++)
                        {
                            string[] elems = sr.ReadLine().Split(' ');
                            double m = double.Parse(elems[0]);
                            double v = double.Parse(elems[1]);
                            MVPairs.Add(v, m);
                        }
                    }
                    MVPairs = MVPairs.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
                }
            }
            catch
            {
                Console.WriteLine($"Произошла ошибка при чтении данных из файла {filename}.\nФайл не существует, поврежден, имеет неверный формат или содержит некорректные данные.");
                I = double.MinValue;    //  маркер того, что чтение данных завершилось с ошибкой
                return;
            }
        }
        /// <summary>
        /// Конструктор двигателя с явным указанием параметров.
        /// </summary>
        /// <param name="i">Момент инерции.</param>
        /// <param name="to">Температура перегрева.</param>
        /// <param name="hm">Коэффициент зависимости нагрева от крутящего момента.</param>
        /// <param name="hv">Коэффициент зависимости нагрева от скорости вращения.</param>
        /// <param name="c">Коэффициент зависимости охлаждения от температуры двигателя и окр.среды.</param>
        /// <param name="keyValuePairs">Зависимость крутящего момента M от скорости вращения V.</param>
        public Engine(double i, double to, double hm, double hv, double c, Dictionary<double, double> keyValuePairs)
        {
            I = i;
            Hm = hm;
            Hv = hv;
            C = c;
            TOverheat = to;
            MVPairs = keyValuePairs.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
        }
        #endregion
        #region Методы класса
        /// <summary>
        /// Чтение входных данных из текстового файла или с клавиатуры.
        /// </summary>
        /// <param name="engine">Выходной параметр - экземпляр класса Engine.</param>
        /// <param name="source">Источник чтения данных (файл формата txt), по умолчанию - с клавиатуры.</param>
        /// <returns></returns>
        public static bool TryReadEngine(out Engine engine, string source = "console")
        {
            //  чтение данных с клавиатуры
            if (string.Equals(source, "console"))
            {
                try
                {
                    Console.Write("Момент инерции, кг*м2 = ");  var i = double.TryParse(Console.ReadLine(), out double val) ? val : double.MinValue;
                    if (i == double.MinValue)
                        throw new IOException("Недопустимое значение момента инерции");
                    Console.Write("Температура перегрева = "); var to = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                    if (to == double.MinValue)
                        throw new IOException("Недопустимое значение температуры перегрева");
                    Console.Write("Коэффициент зависимости нагрева от крутящего момента = "); var hm = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                    if (hm == double.MinValue)
                        throw new IOException("Недопустимое значение коэффициента зависимости нагрева Hm");
                    Console.Write("Коэффициент зависимости нагрева от скорости вращения = "); var hv = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                    if (hv == double.MinValue)
                        throw new IOException("Недопустимое значение коэффициента зависимости нагрева Hv");
                    Console.Write("Коэффициент зависимости охлаждения от температуры двигателя и окр.среды = "); var c = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                    if (c == double.MinValue)
                        throw new IOException("Недопустимое значение коэффициента зависимости охлаждения");
                    Console.Write("Кусочно-линейная зависимость крутящего момента M от скорости вращения V. Кол-во отрезков = "); uint count = uint.TryParse(Console.ReadLine(), out uint ct) ? ct : 0;
                    if (count == 0)
                        throw new IOException("Недопустимое количество отрезков кусочно-линейной зависимости M от V");
                    var keyValuePairs = new Dictionary<double, double>();
                    for (uint j = 0; j < count; j++)
                    {
                        Console.Write("M = "); var m = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                        Console.Write("V = "); var v = double.TryParse(Console.ReadLine(), out val) ? val : double.MinValue;
                        if (m != double.MinValue && v != double.MinValue)
                            keyValuePairs.Add(v, m);
                        else throw new IOException("Недопустимое значение крутящего момента или скорости на одном из отрезков");
                    }
                    engine = new Engine(i, to, hm, hv, c, keyValuePairs.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value));
                    return true;
                }
                catch (IOException exc)
                {
                    Console.WriteLine($"Ошибка при вводе данных: {exc.Message}\nПоказать полный стек возникшей ошибки (y/n)?");
                    if (Console.ReadKey().KeyChar == 'y' || Console.ReadKey().KeyChar == 'Y')
                        Console.WriteLine(exc.StackTrace);
                    engine = null;
                    return false;
                }
            }
            //  чтение данных из текстового файла
            engine = new Engine(source);
            return (engine != null) && (engine.I != double.MinValue);
        }
        /// <summary>
        /// Моделирование работы двигателя и пошаговый вывод результатов.
        /// </summary>
        /// <param name="target">Полный адрес файла в формате txt, в который производится запись. По умолчанию - вывод на экран.</param>
        /// <param name="outerTemperature">Температура окружающей среды.</param>
        /// <returns>Кортеж из двух элементов: время в секундах и достигнутая температура.</returns>
        public Tuple<int, double> CalculatePrintModel(double outerTemperature, string target = "console")
        {
            double velocity = 0.0, temperature = outerTemperature;
            int time = 0;
            //  консольный вывод
            if (string.Equals(target, "console"))
            {
                Console.WriteLine("Моделирование работы двигателя с параметрами:\n");
                Console.WriteLine(this.ToString());
                Console.WriteLine("t\t|v\t|T");
                Console.WriteLine("---------------------------");
                Console.WriteLine($"{time}\t|0\t|{temperature.ToString("#.####")}");
                while (temperature < TOverheat && time < MAX_TIME)
                {
                    velocity += Acceleration(velocity) * 1.0;
                    temperature += ComputeVHeat(velocity);
                    temperature += ComputeVCold(outerTemperature, temperature);
                    time++;
                    Console.WriteLine($"{time}\t|{velocity.ToString("#.##")}\t|{temperature.ToString("#.####")}");
                }
            }
            //  вывод в текстовый файл
            else
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(target))
                    {
                        sw.WriteLine("Моделирование работы двигателя с параметрами:\n");
                        sw.WriteLine(this.ToString());
                        sw.WriteLine("t\t|v\t|T");
                        sw.WriteLine("---------------------------");
                        sw.WriteLine($"{time}\t|0\t|{temperature.ToString("#.####")}");
                        while (temperature < TOverheat && time < MAX_TIME)
                        {
                            velocity += Acceleration(velocity) * 1.0;
                            temperature += ComputeVHeat(velocity);
                            temperature -= ComputeVCold(outerTemperature, temperature);
                            time++;
                            sw.WriteLine($"{time}\t|{velocity.ToString("#.##")}\t|{temperature.ToString("#.####")}");
                        }
                    }
                    Console.WriteLine($"Пошаговые результаты моделирования успешно записаны в файл {target}");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Ошибка при записи в файл {target}:\n{exc.Message}\nВывод результатов моделирования на экран.");
                    Console.WriteLine("Моделирование работы двигателя с параметрами:\n");
                    Console.WriteLine(this.ToString());
                    Console.WriteLine("t\t|v\t|T");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine($"{time}\t|0\t|{temperature.ToString("#.####")}");
                    while (temperature < TOverheat && time < MAX_TIME)
                    {
                        velocity += Acceleration(velocity) * 1.0;
                        temperature += ComputeVHeat(velocity);
                        temperature -= ComputeVCold(outerTemperature, temperature);
                        time++;
                        Console.WriteLine($"{time}\t|{velocity.ToString("#.##")}\t|{temperature.ToString("#.####")}");
                    }
                }
            }
            return new Tuple<int, double>(time, temperature);
        }
        /// <summary>
        /// Вычисление скорости нагрева двигателя.
        /// </summary>
        /// <param name="velocity">Скорость вращения коленвала.</param>
        /// <returns></returns>
        public double ComputeVHeat(double velocity)
        {
            double m = 0.0;
            foreach (var v in MVPairs)
            {
                if (v.Key > velocity)
                    break;
                else
                    m = v.Value;
            }
            return m * Hm + velocity * velocity * Hv;
        }
        /// <summary>
        /// Вычисление ускорения.
        /// </summary>
        /// <param name="velocity">Скорость вращения коленвала.</param>
        /// <returns></returns>
        public double Acceleration(double velocity)
        {
            double m = 0.0;
            foreach (var v in MVPairs)
            {
                if (v.Key > velocity)
                    break;
                else
                    m = v.Value;
            }
            return m / I;
        }
        /// <summary>
        /// Вычисление скорости охлаждения двигателя.
        /// </summary>
        /// <param name="outerTemp">Температура окружающей среды.</param>
        /// <param name="engineTemp">Температура двигателя.</param>
        /// <returns></returns>
        public double ComputeVCold(double outerTemp, double engineTemp)
        {
            return C * (outerTemp - engineTemp);
        }
        public override string ToString()
        {
            string engine = string.Format("Момент инерации = {0} кг*м2\nТемпература перегрева = {1}\n", I, TOverheat)   +
                            string.Format("Коэфф.зависимости нагрева от крутящего момента = {0}\n", Hm)                 +
                            string.Format("Коэфф.зависимости нагрева от скорости вращения = {0}\n", Hv)                 +
                            string.Format("Коэфф.зависимости охлаждения = {0}\n", C);
            return engine;
        }
        #endregion
    }
}