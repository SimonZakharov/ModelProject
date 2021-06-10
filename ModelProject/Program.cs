using System;

namespace ModelProject
{
    class Program
    {
        private const string TempErrorMessage = "Введено недопустимое значение температуры окр. среды. Повторите ввод.";
        private const string ArgWarnMessage0 = "Введено слишком много аргументов. Попытка чтения из файла ";
        private const string ArgWarnMessage1 = " и вывода в файл ";
        static int Main(string[] args)
        {
            string source, target;
            double outerTemperature;
            switch (args.Length)
            {
                case 0: //  ввод с клавиатуры, вывод на экран
                    source = target = "console";
                    break;
                case 1: //  ввод из файла, вывод на экран
                    source = args[0]; target = "console";
                    break;
                case 2: //  ввод из файла, вывод в файл
                    source = args[0]; target = args[1];
                    break;
                default://  при большем кол-ве аргументов выводится предупреждение об этом; ввод из файла, вывод в файл
                    Console.WriteLine($"{ArgWarnMessage0}{args[0]}{ArgWarnMessage1}{args[1]}");
                    source = args[0]; target = args[1];
                    break;
            }
            //  чтение входных данных
            Console.Write("Температура окружающей среды = "); outerTemperature = double.TryParse(Console.ReadLine(), out double t) ? t : double.MinValue;
            while (outerTemperature < -273)
            {
                Console.WriteLine(TempErrorMessage);
                Console.Write("Температура окружающей среды = "); outerTemperature = double.TryParse(Console.ReadLine(), out t) ? t : double.MinValue;
            }
            Engine engine = Engine.TryReadEngine(out Engine e, source) ? e : null;
            if (engine == null)
                return 1;
            //  моделирование и вывод промежуточных результатов
            var timeTemp = engine.CalculatePrintModel(outerTemperature, target);
            string respond = timeTemp.Item1 == Engine.MAX_TIME ? $"Двигатель не перегревается.\nМаксимальная достигнутая температура = {timeTemp.Item2} градусов Цельсия." 
                                                               : $"Время работы двигателя = {timeTemp.Item1} c.\nДостигнутая температура = {timeTemp.Item2} градусов Цельсия.";
            Console.WriteLine(respond);
            return 0;
        }
    }
}
